using System.Diagnostics.Tracing;

namespace Infrastructure
{
    public class Navigator
    {
        private readonly Node[,] _nodes;
        private readonly long _width;
        private readonly long _height;

        public Navigator(CellType[,] cells, long width, long height)
        {
            _width = width;
            _height = height;
            _nodes = new Node[width, height];
            InitNodes(cells);
            BuildPaths();
        }


        public string FindPairs()
        {
            var interestingStarts = IterateNodes().Where((node) => node.Current.Kind == CellKind.InterestingStart);
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

        private IEnumerable<Node> IterateNodes()
        {
            return Iterate().Select((x) => _nodes[x.Item1, x.Item2]);
        }

        private void InitNodes(CellType[,] cells)
        {
            foreach (var (column, row) in Iterate())
            {
                _nodes[column, row] = new Node(cells[column, row], column, row);
            }
        }

        private void BuildPaths()
        {
            foreach (var wall in IterateNodes().Where((node) => node.Current == CellType.Wall))
            {
                ConnectNodes(wall, (column) => column, (row) => row + 1);
                ConnectNodes(wall, (column) => column, (row) => row - 1);
                ConnectNodes(wall, (column) => column + 1, (row) => row);
                ConnectNodes(wall, (column) => column - 1, (row) => row);
            }

            var standableNodes = IterateNodes().Where((node) => !IsObstruction(node));
            foreach (var edge in standableNodes.Where((node) => node.Row == 0))
                ConnectNodes(edge, (column) => column, (row) => row + 1);
            
            foreach (var edge in standableNodes.Where((node) => node.Row == _height - 1))
                ConnectNodes(edge, (column) => column, (row) => row - 1);
            
            foreach (var edge in standableNodes.Where((node) => node.Column == 0))
                ConnectNodes(edge, (column) => column + 1, (row) => row);
            
            foreach (var edge in standableNodes.Where((node) => node.Column == _width - 1))
                ConnectNodes(edge, (column) => column - 1, (row) => row);
        }

        private void ConnectNodes(Node start, Func<int, int> NextColumn, Func<int, int> NextRow)
        {
            var currentNode = start;
            while (true)
            { 
                var nextColumn = NextColumn(currentNode.Column);
                var nextRow = NextRow(currentNode.Row);
                if (IsOutOfBounds(nextColumn, nextRow)) return;
                var nextNode = _nodes[nextColumn, nextRow];
                if (IsObstruction(nextNode)) return;
                currentNode.AddNext(nextNode);
                currentNode = nextNode;
            }
        }

        private static bool IsObstruction(Node node)
        {
            return node.Current == CellType.Hazard || node.Current == CellType.Wall;
        }

        private bool IsOutOfBounds(int column, int row)
        {
            return column < 0 || row < 0 || column >= _width || row >= _height;
        }
    }
}
