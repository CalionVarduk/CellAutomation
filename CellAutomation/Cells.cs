using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellAutomation
{
    public class Cells
    {
        public TerrainCell[,] Terrain { get; private set; }
        public LifeCell[,] Life { get; private set; }

        public int Rows { get { return Terrain.GetLength(0); } }
        public int Columns { get { return Terrain.GetLength(1); } }
        public int Count { get { return Terrain.Length; } }

        public Cells(int rows, int columns)
        {
            Terrain = new TerrainCell[rows, columns];
            Life = new LifeCell[rows, columns];
            Reset();
        }

        public void Reset()
        {
            int rows = Rows;
            int columns = Columns;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    Terrain[i, j] = TerrainCell.Blank;
                    Life[i, j] = LifeCell.Blank;
                }
            }
        }

        public bool HasLife(int row, int column)
        {
            return (Life[row, column] != LifeCell.Blank);
        }
    }
}
