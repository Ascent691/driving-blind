using Infrastructure;

namespace Runner;

public class PairFinder4
{
    public static ScenarioAnswer Calculate(CoolerScenario scenario)
    {
        var cells = scenario.Cells;
        var width = scenario.Width;
        var height = scenario.Height;

        var (starts, ends) = FindInteresting(scenario);

        var drivablePairs = new HashSet<(char, char)>();

        foreach (var start in starts)
        {
            Stack<Point> toCheck = [];
            toCheck.Push(start);

            while (toCheck.Count > 0)
            {
                var curr = toCheck.Pop();

                var adj = GetValidAdjacent(curr);

                if (CanMoveHorizontal(curr))
                {

                }
            }
        }

        bool CanMoveHorizontal(Point point)
        {
            if (point.X > 0)
            {
                if (cells[point.X - 1])
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
            if (cells[x, y].Type == CellKind.Wall) return false;
            if (cells[x, y].Type == CellKind.Hazard) return false;
            return true;
        }

        return new ScenarioAnswer(CreateAnswer(drivablePairs));
    }

    private static (List<Point> starts, List<Point> ends) FindInteresting(CoolerScenario scenario)
    {
        var cells = scenario.Cells;
        var width = scenario.Width;
        var height = scenario.Height;

        var starts = new List<Point>();
        var ends = new List<Point>();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (cells[x, y].Type == CellKind.InterestingStart)
                {
                    starts.Add(new Point(x, y));
                }

                if (cells[x, y].Type == CellKind.InterestingFinish)
                {
                    ends.Add(new Point(x, y));
                }
            }
        }

        return (starts, ends);
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
}