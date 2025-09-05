namespace Infrastructure
{
    public class Node(CellType current, int column, int row)
    {
        public CellType Current = current;
        public int Row = row;
        public int Column = column;
        public HashSet<Node> Next = [];

        public void AddNext(Node node)
        {
            Next.Add(node);
        }

        public override string ToString()
        {
            return Current.Identifier.ToString();
        }
    }
}
