using Infrastructure;

namespace Runner;

public class PairFinder3
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
            var unsafeCells = new bool[width, height];

            var toCheck = GetValidAdjacent(end).ToHashSet();

            while (toCheck.Count > 0)
            {
                foreach (var point in toCheck.ToList())
                {
                    var adjacent = GetValidAdjacent(point);
                    var unknowns = new List<Point>();

                    if (cells[point.X, point.Y].Type != CellType.Hazard && cells[point.X, point.Y].Type != CellType.Wall)
                    {
                        safeCells[point.X, point.Y] = true;
                    }

                    foreach (var adj in adjacent)
                    {
                        if (safeCells[adj.X, adj.Y]) continue;

                        unknowns.Add(adj);
                    }

                    toCheck.UnionWith(unknowns);

                    toCheck.Remove(point);
                }
            }

            var somethingChanged = true;

            while (somethingChanged)
            {
                somethingChanged = false;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        if (!safeCells[x, y]) continue;

                        var hasSafePath = false;

                        //check horizontal moves
                        if (x + 1 < width && safeCells[x + 1, y])
                        {
                            if (x > 0)
                            {
                                if (cells[x - 1, y].Type != CellType.Hazard && !unsafeCells[x - 1, y])
                                {
                                    hasSafePath = true;
                                }
                            }
                            else
                            {
                                hasSafePath = true;
                            }
                        }

                        if (x > 0 && safeCells[x - 1, y])
                        {
                            if (x + 1 < width)
                            {
                                if (cells[x + 1, y].Type != CellType.Hazard && !unsafeCells[x + 1, y])
                                {
                                    hasSafePath = true;
                                }
                            }
                            else
                            {
                                hasSafePath = true;
                            }
                        }

                        //check vertical moves
                        if (y + 1 < height && safeCells[x, y + 1])
                        {
                            if (y > 0)
                            {
                                if (cells[x, y - 1].Type != CellType.Hazard && !unsafeCells[x, y - 1])
                                {
                                    hasSafePath = true;
                                }
                            }
                            else
                            {
                                hasSafePath = true;
                            }
                        }

                        if (y > 0 && safeCells[x, y - 1])
                        {
                            if (y + 1 < height)
                            {
                                if (cells[x, y + 1].Type != CellType.Hazard && !unsafeCells[x, y + 1])
                                {
                                    hasSafePath = true;
                                }
                            }
                            else
                            {
                                hasSafePath = true;
                            }
                        }

                        if (!hasSafePath)
                        {
                            safeCells[x, y] = false;
                            unsafeCells[x, y] = true;
                            somethingChanged = true;
                        }
                    }
                }
            }

            //DFS to find starts

            toCheck = [end];
            var visited = new HashSet<Point>();

            while (toCheck.Count > 0)
            {
                foreach (var point in toCheck.ToList())
                {
                    visited.Add(point);
                    
                    var adjacent = GetValidAdjacent(point);
                    var unknowns = new List<Point>();

                    if (cells[point.X, point.Y].Type == CellType.InterestingStart)
                    {
                        drivablePairs.Add((cells[point.X, point.Y].Value, cells[end.X, end.Y].Value));
                    }

                    foreach (var adj in adjacent)
                    {
                        if (safeCells[adj.X, adj.Y] && !visited.Contains(adj))
                        {
                            if (CanMove(adj, point))
                            {
                                unknowns.Add(adj);
                            }
                        }
                    }

                    if (unknowns.Count > 0)
                    {
                        toCheck.UnionWith(unknowns);
                    }

                    toCheck.Remove(point);
                }
            }

            Console.WriteLine(cells[end.X, end.Y].Value);
            PrintMatrix(safeCells, end);
            Console.WriteLine();

            bool CanMove(Point from, Point to)
            {
                if (from.X < to.X) //check left
                {
                    if (from.X == 0) return true;
                    if (cells[from.X - 1, from.Y].Type == CellType.Hazard) return false;
                }
                else if (from.X > to.X) //check right
                {
                    if (from.X == width - 1) return true;
                    if (cells[from.X + 1, from.Y].Type == CellType.Hazard) return false;
                }
                else if (from.Y < to.Y) //check above
                {
                    if (from.Y == 0) return true;
                    if (cells[from.X, from.Y - 1].Type == CellType.Hazard) return false;
                }
                else if (from.Y > to.Y) //check below
                {
                    if (from.Y == height - 1) return true;
                    if (cells[from.X, from.Y + 1].Type == CellType.Hazard) return false;
                }

                return true;
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

                        if (IsValid(forward)) yield return forward;
                        if (IsValid(backward)) yield return backward;
                    }
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