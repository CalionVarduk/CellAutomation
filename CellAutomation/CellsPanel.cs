using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace CellAutomation
{
    public class CellsPanel : Panel
    {
        public int CellSize { get; set; }
        public Cells Model { get; set; }

        public Size SizeToFit
        {
            get
            {
                return new Size(Model.Columns * CellSize + Model.Columns + 1, Model.Rows * CellSize + Model.Rows + 1);
            }
        }

        private SolidBrush[] terrainBrushes;
        private SolidBrush[] lifeBrushes;
        private Pen borderPen;

        public CellsPanel()
        {
            DoubleBuffered = true;
            CellSize = 5;

            terrainBrushes = new SolidBrush[Enum.GetNames(typeof(TerrainCell)).Length];
            terrainBrushes[(int)TerrainCell.Blank] = new SolidBrush(Color.Black);
            terrainBrushes[(int)TerrainCell.Grassland] = new SolidBrush(Color.GreenYellow);
            terrainBrushes[(int)TerrainCell.Forest] = new SolidBrush(Color.ForestGreen);
            terrainBrushes[(int)TerrainCell.OnFire] = new SolidBrush(Color.Red);

            lifeBrushes = new SolidBrush[Enum.GetNames(typeof(LifeCell)).Length];
            lifeBrushes[(int)LifeCell.Blank] = terrainBrushes[(int)TerrainCell.Blank];
            lifeBrushes[(int)LifeCell.Alive] = new SolidBrush(Color.Blue);
            lifeBrushes[(int)LifeCell.Infected] = new SolidBrush(Color.Violet);

            borderPen = new Pen(Color.Black, 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Model != null)
            {
                int rows = Model.Rows;
                int columns = Model.Columns;

                for (int i = 0; i < rows; ++i)
                    for (int j = 0; j < columns; ++j)
                        DrawCell(e.Graphics, i, j);
            }
        }

        private void DrawCell(Graphics g, int r, int c)
        {
            int x = c * CellSize + c + 1;
            int y = r * CellSize + r + 1;
            SolidBrush brush = Model.HasLife(r, c) ? lifeBrushes[(int)Model.Life[r, c]] : terrainBrushes[(int)Model.Terrain[r, c]];

            g.DrawRectangle(borderPen, x - 1, y - 1, CellSize + 2, CellSize + 2);
            g.FillRectangle(brush, x, y, CellSize, CellSize);
        }
    }
}
