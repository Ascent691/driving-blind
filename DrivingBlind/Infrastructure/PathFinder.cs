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
                ConnectNodes(wall, Direction.North);
                ConnectNodes(wall, Direction.South);
                ConnectNodes(wall, Direction.East);
                ConnectNodes(wall, Direction.West);
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
            var existingPaths = IterateNodes().SelectMany((node) => node.Next);
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
