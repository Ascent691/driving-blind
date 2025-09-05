using Infrastructure;

namespace Runner;

public class AdjacencyMatrix (int vertices)
{
    private readonly Cell?[,] _matrix = new Cell[vertices, vertices];
    private readonly int _vertices = vertices;

    public Cell? At(int v1, int v2)
    {
        return _matrix[v1, v2];
    }

    public void Add(int v1, int v2, Cell cell)
    {
        _matrix[v1, v2] = cell;
    }

    public IEnumerable<int> GetConnected(int node)
    {
        for (var i = 0; i < _vertices; i++)
        {
            if (_matrix[node, i] != null)
            {
                yield return i;
            }
        }
    }
}