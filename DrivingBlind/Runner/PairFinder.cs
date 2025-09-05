using System.Collections;
using System.Xml.Linq;
using Infrastructure;
using Point = Infrastructure.Point;

namespace Runner;

public record struct Pair(char Start, char Finish);

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

            var stack = new Stack<List<Point>>();
            stack.Push([start]);

            while (stack.Count > 0)
            {
                var currentPath = stack.Pop();
                var currCell = currentPath[^1];

                visited.Add(currCell);

                if (cells[currCell.X, currCell.Y].Type == CellType.InterestingFinish)
                {
                    var s = cells[start.X, start.Y].Value;
                    var f = cells[currCell.X, currCell.Y].Value;
                    var pair = new Pair(s, f);

                }

                foreach (var dest in GetAdjacent(currCell))
                {
                    if (!currentPath.Contains(dest))
                    {
                        stack.Push([..currentPath, dest]);
                    }
                }
            }
        }

        IEnumerable<Point> GetAdjacent(Point point)
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

                    yield return new Point(newX, newY);
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

        var answer = string.Join(" ", drivablePairs
            .Select(p => new string([p.Item1, p.Item2]))
            .OrderBy(s => s)
        );
        answer = answer == "" ? "NONE" : answer;

        return new ScenarioAnswer(answer);
    }
}