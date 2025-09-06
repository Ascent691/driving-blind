using System.Diagnostics.Tracing;

namespace Infrastructure
{
    public class Navigator
    {
        public static string FindPairs(IEnumerable<Node> nodes)
        {
            var interestingStarts = nodes.Where((node) => node.Current.Kind == CellKind.InterestingStart);
            var pairs = interestingStarts.SelectMany((start) => FindInterestingEndsFromNode(start).Select((finish) => $"{start}{finish}"));
            return pairs.Any() ? string.Join(' ', pairs.Distinct().Order()) : "NONE";
        }

        private static IEnumerable<Node> FindInterestingEndsFromNode(Node startingNode)
        {
            var travelled = new HashSet<Node>() { startingNode };
            var queue = new Queue<Node>();
            queue.Enqueue(startingNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Current.Kind == CellKind.InterestingFinish) yield return node;
                foreach (var next in node.Next.Where((node) => !travelled.Contains(node)))
                {
                    queue.Enqueue(next);
                    travelled.Add(next);
                }
            }
        }
    }
}
