using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Infrastructure
{
    public class Navigator
    {
        private readonly Cell[,] _cells;
        private readonly TravelPoint[,] _travelPoints;
        private readonly long _width;
        private readonly long _height;

        public Navigator(Cell[,] cells, long width, long height)
        {
            _cells = cells;
            _travelPoints = new TravelPoint[width, height];
            _width = width;
            _height = height;
            InitTravelPoints();
            ScanCells();
        }

        public string FindPairs()
        {
            var startingCells = Iterate().Where((x) => _cells[x.Item1, x.Item2].Type == CellType.InterestingStart);
            var pairs = startingCells.SelectMany((start) => 
                FindInterestingFinishesFromCell(start.Item1, start.Item2).Select((finish) => _cells[start.Item1, start.Item2].Value + finish.ToString())
            ).Distinct();
            return pairs.Any() ? string.Join(" ", pairs.Order()) : "NONE";
        }
        
        private IEnumerable<char> FindInterestingFinishesFromCell(int column, int row)
        {
            return FindInterestingFinishesFromCell(column, row, []);
        }

        private IEnumerable<char> FindInterestingFinishesFromCell(int column, int row, IEnumerable<(int, int)> explored)
        {
            var cell = _cells[column, row];
            if (cell.Type == CellType.InterestingFinish) yield return cell.Value;

            var travelPoint = _travelPoints[column, row];
            
            if (travelPoint.CanTravelHorizontal)
            {
                foreach (var c in IterateHorizontal(column, row)
                    .Where((x) => !explored.Contains(x))
                    .SelectMany((x) => FindInterestingFinishesFromCell(x.Item1, x.Item2, [.. explored, x])))
                {
                    yield return c;
                }
            }

            if (travelPoint.CanTravelVertical)
            {
                foreach (var c in IterateVertical(row, column)
                    .Where((x) => !explored.Contains(x))
                    .SelectMany((x) => FindInterestingFinishesFromCell(x.Item1, x.Item2, [.. explored, x])))
                {
                    yield return c;
                }
            }
        }

        private IEnumerable<(int, int)> IterateHorizontal(int startColumn, int axis)
        {
            var hasHitUpBlocker = false;
            var hasHitDownBlocker = false;
            var rangeTraveled = 0;
            while (!hasHitUpBlocker || !hasHitDownBlocker)
            {
                rangeTraveled++;

                var columnToLeft = startColumn - rangeTraveled;
                hasHitUpBlocker = hasHitUpBlocker || columnToLeft < 0 || IsBlocker(_cells[columnToLeft, axis]);
                if (!hasHitUpBlocker) yield return (columnToLeft, axis);

                var columnToRight = startColumn + rangeTraveled;
                hasHitDownBlocker = hasHitDownBlocker || columnToRight >= _width || IsBlocker(_cells[columnToRight, axis]);
                if (!hasHitDownBlocker) yield return (columnToRight, axis);
            }
        }

        private IEnumerable<(int, int)> IterateVertical(int startRow, int axis)
        {
            var hasHitUpBlocker = false;
            var hasHitDownBlocker = false;
            var rangeTraveled = 0;
            while (!hasHitUpBlocker || !hasHitDownBlocker)
            {
                rangeTraveled++;

                var aboveRow = startRow - rangeTraveled;
                hasHitUpBlocker = hasHitUpBlocker || aboveRow < 0 || IsBlocker(_cells[axis, aboveRow]);
                if (!hasHitUpBlocker) yield return (axis, aboveRow);

                var belowRow = startRow + rangeTraveled;
                hasHitDownBlocker = hasHitDownBlocker || belowRow >= _height || IsBlocker(_cells[axis, belowRow]);
                if (!hasHitDownBlocker) yield return (axis, belowRow);
            }
        }

        private static bool IsBlocker(Cell cell)
        {
            return cell.Type == CellType.Wall || cell.Type == CellType.Hazard;
        }

        private void InitTravelPoints()
        {
            foreach (var (column, row) in Iterate())
            {
                _travelPoints[column, row] = new OpenTravelPoint();
            }
        }

        private void ScanCells()
        {
            foreach (var (column, row) in Iterate())
            {
                if (_cells[column, row].Type == CellType.Hazard)
                {
                    if (column - 1 >= 0) _travelPoints[column - 1, row] = _travelPoints[column - 1, row].WithHorizontalHazard();
                    if (column + 1 < _width) _travelPoints[column + 1, row] = _travelPoints[column + 1, row].WithHorizontalHazard();
                    if (row - 1 >= 0) _travelPoints[column, row - 1] = _travelPoints[column, row - 1].WithVerticalHazard();
                    if (row + 1 < _height) _travelPoints[column, row + 1] = _travelPoints[column, row + 1].WithVerticalHazard();
                }
            }
        }

        private IEnumerable<(int, int)> Iterate()
        {
            for (int column = 0; column < _width; column++)
            {
                for (int row = 0; row < _height; row++)
                {
                    yield return (column, row);
                }
            }
        }
    }
}
