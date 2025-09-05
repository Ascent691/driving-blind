using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var tasks = startingCells.Select(start => Task.Run(
                    () => FindInterestingFinishesFromCell(start)
                        .Select((finish) => _cells[start.Item1, start.Item2].Value + finish.ToString())
                        )).ToArray();
            Task.WaitAll(tasks);
            var pairs = tasks.SelectMany((t) => t.Result).Distinct().Order();
            return pairs.Any() ? string.Join(" ", pairs) : "NONE";
        }
        
        private IEnumerable<char> FindInterestingFinishesFromCell((int, int) startingCell)
        {
            var explored = new List<(int, int)>();
            var queue = new List<(int, int)>() { startingCell };

            while (queue.Count > 0)
            {
                foreach (var cell in new List<(int, int)>(queue))
                {
                    queue.Remove(cell);
                    explored.Add(cell);
                    if (_cells[cell.Item1, cell.Item2].Type == CellType.InterestingFinish) yield return _cells[cell.Item1, cell.Item2].Value;


                    var travelPoint = _travelPoints[cell.Item1, cell.Item2];
                    var explorable = new List<(int, int)>();

                    if (travelPoint.CanTravelVertical) explorable.AddRange(IterateVertical(cell));
                    if (travelPoint.CanTravelHorizontal) explorable.AddRange(IterateHorizontal(cell));

                    queue.AddRange(explorable.Where((c) => !explored.Contains(c)));
                }
            }
        }

        private IEnumerable<(int, int)> IterateHorizontal((int, int) startingPoint)
        {
            var line = IterateLine(
                startingPoint, 
                (point, range) => (point.Item1 + range, point.Item2), 
                (point) => point.Item1 < 0 || point.Item1 >= _width
            );
            
            if (line.Count(x => !_travelPoints[x.Item1, x.Item2].CanTravelHorizontal) >= 2) return [];

            return line;
        }

        private IEnumerable<(int, int)> IterateVertical((int, int) startingPoint)
        {
            var line = IterateLine(
                startingPoint, 
                (point, range) => (point.Item1, point.Item2 + range), 
                (point) => point.Item2 < 0 || point.Item2 >= _height
            );

            if (line.Count((x) => !_travelPoints[x.Item1, x.Item2].CanTravelVertical) >= 2) return [];

            return line;
        }

        private IEnumerable<(int, int)> IterateLine(
            (int, int) startingPoint, 
            Func<(int, int), int, (int, int)> iterator, 
            Func<(int, int), bool> hasHitLimit
        )
        {
            var hasHitPositiveBlocker = false;
            var hasHitNegativeBlocker = false;
            var range = 0;
            while (!hasHitPositiveBlocker || !hasHitNegativeBlocker)
            {
                range++;

                var negativePoint = iterator(startingPoint, -range);
                hasHitNegativeBlocker |= hasHitLimit(negativePoint) || IsBlocker(_cells[negativePoint.Item1, negativePoint.Item2]);

                if (!hasHitNegativeBlocker) yield return negativePoint;

                var positivePoint = iterator(startingPoint, range);
                hasHitPositiveBlocker |= hasHitLimit(positivePoint) || IsBlocker(_cells[positivePoint.Item1, positivePoint.Item2]);

                if (!hasHitPositiveBlocker) yield return positivePoint;
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
