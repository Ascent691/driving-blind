using Infrastructure;

namespace Runner
{
    internal class SafePathFinder(Scenario scenario)
    {
        public Scenario Scenario { get; } = scenario;
        private int Width { get; } = scenario.Width;
        private int Height { get; } = scenario.Height;

        public void GetSafePaths(Point start)
        {
            // iterate
                // get safe directions
                    // multiple safe directions ? Mark point as having branching paths
                // choose and move in safe direction
                // continue moving until interesting end reached

            // next
                // start from branching paths and repeat, remove branching path from list

            // if interesting end reached, guaranteed safe path => add to list of safePaths (start-END)
                // otherwise NONE


            var branchingPaths = new Dictionary<Point, List <Direction>>();
        }



        private IEnumerable<Direction> GetSafeDirections(Point point)
        {
            var directions = new Dictionary<Direction, bool>();

            var isNorthSafe = checkUp(point);
            var isSouthSafe = checkDown(point);
            var isEastSafe = checkRight(point);
            var isWestSafe = checkLeft(point);

            directions.Add(Direction.North, isNorthSafe);
            directions.Add(Direction.South, isSouthSafe);
            directions.Add(Direction.East, isEastSafe);
            directions.Add(Direction.West, isWestSafe);

            foreach (var direction in directions)
            {
                if (direction.Value)
                yield return direction.Key;
            }
            // iterate through that axes and search for dangers or wall or end
            // if danger in direction (+ or -), set isSafe bool for that direction to false, if at edge, stays false
            // if wall or end reached, add safe directions to list and return
        }


        // WE CAN'T JUST CHECK ALL THE WAY IN ONE DIRECTION IN THE CASE THAT THE CAR TURNS BEFORE IT WOULD HAVE REACHED A HAZARD ********************
        private bool checkLeft(Point p)
        {
            for(int i = p.X; i >= 0;  i--)
            {
                var curr = GetCell(i, p.Y);
                if (curr == CellType.Hazard) return false;
            }

            return true;
        }
        private bool checkRight(Point p)
        {
            for(int i = p.X; i < Width;  i++)
            {
                var curr = GetCell(i, p.Y);
                if (curr == CellType.Hazard) return false;
            }

            return true;
        }

        private bool checkUp(Point p)
        {
            for(int i = p.Y; i >= 0;  i--)
            {
                var curr = GetCell(p.X, i);
                if (curr == CellType.Hazard) return false;
            }

            return true;
        }
        private bool checkDown(Point p)
        {
            for(int i = p.Y; i < Height;  i++)
            {
                var curr = GetCell(p.X, i);
                if (curr == CellType.Hazard) return false;
            }

            return true;
        }

        private CellType GetCell(int x, int y)
        {
            return Scenario.Cells[x, y];
        }
    }
}
