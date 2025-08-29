using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public record CoolerScenario(Cell[,] Cells, int Width, int Height);
}
