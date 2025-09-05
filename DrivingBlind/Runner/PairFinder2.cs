using System.Collections;
using System.Drawing;
using System.Xml.Linq;
using Infrastructure;
using Point = Infrastructure.Point;

namespace Runner;

//public record struct Pair(char Start, char Finish);

public class PairFinder2
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

        foreach (var end in ends)
        {
            var safeCells = new bool[width, height];
            safeCells[end.X, end.Y] = true;
            
            var toCheck = GetValidAdjacent(end).ToHashSet();

            while (toCheck.Count > 0)
            {
                foreach (var point in toCheck.ToList())
                {
                    var adjacent = GetValidAdjacent(point);
                    var safeMove = false;
                    var unknowns = new List<Point>();

                    foreach (var adj in adjacent)
                    {
                        if (safeCells[adj.X, adj.Y])
                        {
                            if (IsSafeBehind(point, adj, safeCells))
                            {
                                safeMove = true;
                                safeCells[point.X, point.Y] = true;
                            }
                        }
                        else
                        {
                            unknowns.Add(adj);
                        }
                    }

                    if (safeMove && unknowns.Count > 0)
                    {
                        toCheck.UnionWith(unknowns);
                    }
                    
                    toCheck.Remove(point);
                }
            }

            foreach (var start in starts)
            {
                if (safeCells[start.X, start.Y])
                {
                    drivablePairs.Add((cells[start.X, start.Y].Value, cells[end.X, end.Y].Value));
                }
            }

            if (cells[end.X, end.Y].Value == 'D')
            {
                Console.WriteLine(cells[end.X, end.Y].Value);
                PrintMatrix(safeCells, end);
                Console.WriteLine("---");
            }
            
        }

        foreach (var start in starts)
        {
            var safeCells = new bool[width, height];
            safeCells[start.X, start.Y] = true;

            var toCheck = GetValidAdjacent(start).ToHashSet();

            while (toCheck.Count > 0)
            {
                foreach (var point in toCheck.ToList())
                {
                    var adjacent = GetValidAdjacent(point);
                    var safeMove = false;
                    var unknowns = new List<Point>();

                    foreach (var adj in adjacent)
                    {
                        if (safeCells[adj.X, adj.Y])
                        {
                            if (IsSafeBehind(point, adj, safeCells))
                            {
                                safeMove = true;
                                safeCells[point.X, point.Y] = true;
                            }
                        }
                        else
                        {
                            unknowns.Add(adj);
                        }
                    }

                    if (safeMove && unknowns.Count > 0)
                    {
                        toCheck.UnionWith(unknowns);
                    }

                    toCheck.Remove(point);
                }
            }

            foreach (var end in ends)
            {
                if (safeCells[end.X, end.Y])
                {
                    drivablePairs.Add((cells[start.X, start.Y].Value, cells[end.X, end.Y].Value));
                }
            }

            if (cells[start.X, start.Y].Value == 'a')
            {
                Console.WriteLine(cells[start.X, start.Y].Value);
                PrintMatrix(safeCells, start);
                Console.WriteLine("---");
            }

        }

        IEnumerable<Point> GetValidAdjacent(Point point)
        {
            for (var i = 0; i <= 1; i++)
            {
                for (var j = 0; j <= 1; j++)
                {
                    if ((i == 0 && j == 0) || (i != 0 && j != 0))
                        continue;

                    var forward = new Point(point.X + i, point.Y + j);
                    var backward = new Point(point.X - i, point.Y - j);

                    //if (IsHazard(forward) || IsHazard(backward)) continue;
                    if (IsValid(forward)) yield return forward;
                    if (IsValid(backward)) yield return backward;
                }
            }
        }

        bool IsValid(Point point)
        {
            var (x, y) = point;
            if (x < 0 || y < 0) return false;
            if (x >= width || y >= height) return false;
            if (cells[x, y].Type == CellType.Wall) return false;
            if (cells[x, y].Type == CellType.Hazard) return false;
            return true;
        }

        bool IsHazard(Point point)
        {
            var (x, y) = point;
            if (x < 0 || y < 0) return false;
            if (x >= width || y >= height) return false;
            return cells[x, y].Type == CellType.Hazard;
        }

        bool IsSafeBehind(Point origin, Point target, bool[,] safeCells)
        {
            if (origin.X < target.X) //check left
            {
                for (var i = origin.X - 1; i >= 0; i--)
                {
                    var newPoint = origin with { X = i };
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Hazard) return false;
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Wall) return true;
                    if (safeCells[newPoint.X, newPoint.Y]) return true;
                }
            }
            else if (origin.X > target.X) //check right
            {
                for (var i = origin.X + 1; i < width; i++)
                {
                    var newPoint = origin with { X = i };
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Hazard) return false;
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Wall) return true;
                    if (safeCells[newPoint.X, newPoint.Y]) return true;
                }
            }
            else if (origin.Y < target.Y) //check up
            {
                for (var i = origin.Y - 1; i >= 0; i--)
                {
                    var newPoint = origin with { Y = i };
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Hazard) return false;
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Wall) return true;
                    if (safeCells[newPoint.X, newPoint.Y]) return true;
                }

            }
            else if (origin.Y > target.Y) //check down
            {
                for (var i = origin.Y + 1; i < height; i++)
                {
                    var newPoint = origin with { Y = i };
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Hazard) return false;
                    if (cells[newPoint.X, newPoint.Y].Type == CellType.Wall) return true;
                    if (safeCells[newPoint.X, newPoint.Y]) return true;
                }
            }
            return true;
        }

        return new ScenarioAnswer(CreateAnswer(drivablePairs));
    }

    private static string CreateAnswer(HashSet<(char, char)> drivablePairs)
    {
        var answer = string.Join(" ", drivablePairs
            .Select(p => new string([p.Item1, p.Item2]))
            .OrderBy(s => s)
        );
        answer = answer == "" ? "NONE" : answer;
        return answer;
    }

    private static void PrintMatrix(bool[,] cells, Point marker)
    {
        for (var i = 0; i < cells.GetLength(1); i++)
        {
            for (var j = 0; j < cells.GetLength(0); j++)
            {
                if (marker.X == j && marker.Y == i)
                {
                    Console.Write("_" + " ");
                }
                else
                {
                    Console.Write((cells[j, i] ? "." : "#") + " ");
                }
            }
            Console.WriteLine();
        }
    }
}