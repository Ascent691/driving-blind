using Infrastructure;

namespace Infrastructure;

public class ScenarioParser
{
    public ScenarioParser() { }

    public Scenario[] Parse(string[] lines)
    {
        int lineIndex = 0;
        var totalScenarios = int.Parse(lines[lineIndex++].Trim());
        var results = new Scenario[totalScenarios];

        for (int i = 0; i < results.Length; i++)
        {
            string[] parts = lines[lineIndex++].Split(' ');
            var numRows = int.Parse(parts[0]);
            var numColumns = int.Parse(parts[1]);
            var cells = new CellType[numColumns, numRows];

            for (int r = 0; r < numRows; r++)
            {
                var cellDesignations = lines[lineIndex++].ToCharArray();

                if (cellDesignations.Length != numColumns)
                    throw new FormatException("Inconsistent number of columns...");

                for (int c = 0; c < cellDesignations.Length; c++)
                {
                    var designation = cellDesignations[c];
                    var cellType = CellType.Empty;

                    if (designation == '*')
                        cellType = CellType.Hazard;
                    else if (designation == '#')
                        cellType = CellType.Wall;
                    else if (designation != '.')
                    {
                        if (Char.IsUpper(designation))
                            cellType = CellType.InterestingFinish(designation);
                        else
                            cellType = CellType.InterestingStart(designation);
                    }


                    cells[c, r] = cellType;
                }
            }

            results[i] = new Scenario(cells, numColumns, numRows);
        }

        return results;
    }
}