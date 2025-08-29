using System.Collections;
using Infrastructure;
using Point = Infrastructure.Point;

namespace Runner;

public class PairFinder
{
    public static ScenarioAnswer Calculate(CoolerScenario scenario)
    {
        var cells = scenario.Cells;
        var width = scenario.Width;
        var height = scenario.Height;

        var starts = new List<Point>();
        var ends = new List<Point>();

        var drivablePairs = new HashSet<(char, char)>();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (cells[x, y].Type == CellType.InterestingStart)
                {
                    starts.Add(new Point(x, y));
                }

                if (cells[x, y].Type == CellType.InterestingFinish)
                {
                    ends.Add(new Point(x, y));
                }
            }
        }

        foreach (var start in starts)
        {
            var visited = new HashSet<Point>();

            var stack = new Stack<(Point currCell, Point? prevCell)>();
            stack.Push((start, null));

            while (stack.Count > 0)
            {
                var (currCell, prevCell) = stack.Pop();

                visited.Add(currCell);

                if (cells[currCell.X, currCell.Y].Type == CellType.InterestingFinish)
                {
                    var s = cells[start.X, start.Y].Value;
                    var f = cells[currCell.X, currCell.Y].Value;
                    drivablePairs.Add((s, f));
                }

                foreach (var dest in GetAdjacentSafe(currCell, prevCell))
                {
                    if (!visited.Contains(dest))
                    {
                        stack.Push((dest, currCell));
                    }
                }
            }
        }

        IEnumerable<Point> GetAdjacentSafe(Point point, Point? prevPoint)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || (i != 0 && j != 0))
                        continue;

                    var newX = point.X + i;
                    var newY = point.Y + j;

                    if (!IsValid(newX, newY)) continue;

                    var wasHoriz = point.Y == prevPoint?.Y;
                    var isInit = prevPoint is null;

                    if (newY == point.Y) //horizontal move
                    {
                        if (isInit)
                        {
                            yield return new Point(newX, newY);
                        }
                        else
                        {
                            if (!wasHoriz) //changed direction
                            {
                                if (IsSafeHorizontal(point, (Point) prevPoint))
                                    yield return new Point(newX, newY);
                            }
                            else
                            {
                                yield return new Point(newX, newY);
                            }
                        }
                    }
                    else //vertical move
                    {
                        if (isInit)
                        {
                            yield return new Point(newX, newY);
                        }
                        else
                        {
                            if (wasHoriz) //changed direction
                            {
                                if (IsSafeVertical(point, (Point) prevPoint))
                                    yield return new Point(newX, newY);
                            }
                            else
                            {
                                yield return new Point(newX, newY);
                            }
                        }
                    }
                }
            }
        }

        bool IsValid(int x, int y)
        {
            if (x < 0 || y < 0) return false;
            if (x >= width || y >= height) return false;
            if (cells[x, y].Type == CellType.Wall) return false;
            if (cells[x, y].Type == CellType.Hazard) return false;
            return true;
        }

        bool IsSafeVertical(Point point, Point prevPoint)
        {
            if (prevPoint.Y > point.Y)
            {
                for (var y = point.Y; y >= 0; y--)
                {
                    if (cells[point.X, y].Type == CellType.Hazard)
                    {
                        return false;
                    }

                    if (cells[point.X, y].Type == CellType.Wall)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (var y = point.Y; y < height; y++)
                {
                    if (cells[point.X, y].Type == CellType.Hazard)
                    {
                        return false;
                    }

                    if (cells[point.X, y].Type == CellType.Wall)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        bool IsSafeHorizontal(Point point, Point prevPoint)
        {
            if (prevPoint.X > point.X)
            {
                for (var x = point.X; x >= 0; x--)
                {
                    if (cells[x, point.Y].Type == CellType.Hazard)
                    {
                        return false;
                    }

                    if (cells[point.X, point.Y].Type == CellType.Wall)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (var x = point.X; x < width; x++)
                {
                    if (cells[x, point.Y].Type == CellType.Hazard)
                    {
                        return false;
                    }

                    if (cells[x, point.Y].Type == CellType.Wall)
                    {
                        break;
                    }
                }
            }
            return true;
        }

        var answer = string.Join(" ", drivablePairs
            .Select(p => new string([ p.Item1, p.Item2 ]))
            .OrderBy(s => s)
        );
        answer = answer == "" ? "NONE" : answer;

        return new ScenarioAnswer(answer);
    }
}