namespace Infrastructure
{
    public class PathBuilder
    {
        private readonly long _width;
        private readonly long _height;
        private readonly Node[,] _nodes;

        public PathBuilder(CellType[,] cells, long width, long height)
        {
            _width = width;
            _height = height;
            _nodes = new Node[width, height];
            InitNodes(cells);
        }

        public IEnumerable<Node> BuildPaths()
        {
            BuildPathsFromWalls();
            BuildPathsFromEdges();
            BuildPathsFromExistingPaths();
            return IterateNodes();
        }

        private void BuildPathsFromWalls()
        {
            foreach (var wall in IterateNodes().Where((node) => node.Current == CellType.Wall))
            {
                foreach (var direction in IterateDirections())
                    ConnectNodes(wall, direction);
            }
        }

        private void BuildPathsFromEdges()
        {
            var standableNodes = IterateNodes().Where((node) => !IsObstruction(node));
            foreach (var edge in standableNodes.Where((node) => node.Row == 0))
                ConnectNodes(edge, Direction.South);
            
            foreach (var edge in standableNodes.Where((node) => node.Row == _height - 1))
                ConnectNodes(edge, Direction.North);
            
            foreach (var edge in standableNodes.Where((node) => node.Column == 0))
                ConnectNodes(edge, Direction.East);
            
            foreach (var edge in standableNodes.Where((node) => node.Column == _width - 1))
                ConnectNodes(edge, Direction.West);
        }

        private void BuildPathsFromExistingPaths()
        {
            var connections = GetTotalConnections();
            while (true)
            {
                foreach (var path in IterateNodes().SelectMany((node) => node.Next).ToList())
                {
                    foreach (var direction in IterateDirections())
                    {
                        if (CanLoopInDirection(path, direction))
                        {
                            ConnectNodes(path, GetOppositeDirection(direction));
                        }
                    }
                }
                var newConnections = GetTotalConnections();
                if (connections == newConnections) break;
                connections = newConnections;
            }
        }

        private void ConnectNodes(Node start, Direction direction)
        {
            var current = start;
            foreach (var node in IterateInDirection(start, direction))
            {
                current.AddNext(node);
                current = node;
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

        private bool CanLoopInDirection(Node start, Direction direction)
        {
            var queue = new Queue<Node>();
            var visited = new HashSet<Node>();
            foreach (var node in IterateInDirection(start, direction))
            {
                queue.Enqueue(node);
                visited.Add(node);
            }

            while (queue.Count > 0) {
                var node = queue.Dequeue();
                if (node == start) return true;

                foreach (var nextNode in node.Next.Where((x) => !visited.Contains(x))) {
                    queue.Enqueue(nextNode);
                    visited.Add(nextNode);
                }
            }
            return false;
        }

        private static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction) {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.West: return Direction.East;
                case Direction.East: return Direction.West;
                default:
                    return Direction.None;
            }
        }

        private int GetTotalConnections()
        {
            return IterateNodes().Sum((node) => node.Next.Count);
        }

        private static IEnumerable<Direction> IterateDirections()
        {
            yield return Direction.North;
            yield return Direction.South;
            yield return Direction.West;
            yield return Direction.East;
        }

        private IEnumerable<Node> IterateNodes()
        {
            return Iterate().Select((x) => _nodes[x.Item1, x.Item2]);
        }

        private IEnumerable<Node> IterateInDirection(Node start, Direction direction)
        {
            switch (direction) {
                case Direction.North:
                    return IterateLine(start, (node) => (node.Column, node.Row - 1));
                case Direction.South:
                    return IterateLine(start, (node) => (node.Column, node.Row + 1));
                case Direction.East:
                    return IterateLine(start, (node) => (node.Column + 1, node.Row));
                case Direction.West:
                    return IterateLine(start, (node) => (node.Column - 1, node.Row));
                default:
                    return [];
            }
        }

        private IEnumerable<Node> IterateLine(Node start, Func<Node, (int, int)> Next)
        {
            var current = start;
            while (true)
            {
                var (nextColumn, nextRow) = Next(current);
                if (IsOutOfBounds(nextColumn, nextRow)) break;
                var nextNode = _nodes[nextColumn, nextRow];
                if (IsObstruction(nextNode)) break;
                yield return nextNode;
                current = nextNode;
            }
        }

        private void InitNodes(CellType[,] cells)
        {
            foreach (var (column, row) in Iterate())
            {
                _nodes[column, row] = new Node(cells[column, row], column, row);
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
