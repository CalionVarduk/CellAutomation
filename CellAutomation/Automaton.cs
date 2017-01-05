using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellAutomation
{
    public class Automaton
    {
        public Cells Cells { get; private set; }
        private TerrainCell[,] terrainBuffer;
        private BitArray lifeBuffer;
        private int[] terrainNeighbours;
        private int[] lifeNeighbours;

        private int[] terrainCount;
        private int[] lifeCount;

        public int Rows { get { return Cells.Rows; } }
        public int Columns { get { return Cells.Columns; } }
        public int Count { get { return Cells.Count; } }

        public EventChances Chances { get; set; }
        public NeighbourRules Rules { get; set; }

        public Automaton(Cells cells)
            : this(cells, new EventChances(), null)
        {
            Rules = new NeighbourRules(Chances.Rng);
        }

        public Automaton(Cells cells, EventChances events, NeighbourRules rules)
        {
            Chances = events;
            Rules = rules;
            Cells = cells;

            terrainNeighbours = new int[Enum.GetNames(typeof(TerrainCell)).Length];
            lifeNeighbours = new int[Enum.GetNames(typeof(LifeCell)).Length];

            terrainBuffer = new TerrainCell[Rows, Columns];
            lifeBuffer = new BitArray(Rows * Columns);
            ResetBuffers();

            terrainCount = new int[Enum.GetNames(typeof(TerrainCell)).Length];
            lifeCount = new int[Enum.GetNames(typeof(LifeCell)).Length];
            ResetCount();
        }

        public int CountOf(TerrainCell cell)
        {
            return terrainCount[(int)cell];
        }

        public int CountOf(LifeCell cell)
        {
            return lifeCount[(int)cell];
        }

        public void Reset()
        {
            Cells.Reset();
            ResetBuffers();
            ResetCount();
        }

        public void Advance()
        {
            int rows = Rows;
            int columns = Columns;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < columns; ++j)
                    AdvanceTerrain(i, j);

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < columns; ++j)
                    AdvanceLife(i, j);

            ResetBuffers();
        }

        private void AdvanceTerrain(int r, int c)
        {
            TerrainCell cell = Cells.Terrain[r, c];

            if(cell != TerrainCell.Grassland && Cells.Life[r, c] != LifeCell.Blank && Rules.Rng.Next(0, 14) == 0)
            {
                Cells.Terrain[r, c] = TerrainCell.Grassland;
                return;
            }

            LoadTerrainNeighbours(r, c);

            
            int count = Rules.CountOf(cell);

            for (int i = 0; i < count; ++i)
            {
                TerrainCell neighbour = Rules.NeighbourAt(cell, i);
                if (Rules.Try(cell, neighbour, terrainNeighbours[(int)neighbour]))
                {
                    Cells.Terrain[r, c] = neighbour;
                    return;
                }
            }
            Cells.Terrain[r, c] = Chances.Try(cell);
        }

        private void AdvanceLife(int r, int c)
        {
            int index = r * Columns + c;
            if(!lifeBuffer[index] && Cells.Terrain[r, c] != TerrainCell.Blank)
            {
                if (Cells.Terrain[r, c] == TerrainCell.OnFire)
                {
                    Cells.Life[r, c] = LifeCell.Blank;
                    return;
                }

                LoadLifeNeighbours(r, c);

                LifeCell cell = Cells.Life[r, c];
                int count = Rules.CountOf(cell);

                for (int i = 0; i < count; ++i)
                {
                    LifeCell neighbour = Rules.NeighbourAt(cell, i);
                    if (Rules.Try(cell, neighbour, lifeNeighbours[(int)neighbour]))
                    {
                        if (cell == LifeCell.Blank && neighbour != LifeCell.Blank)
                        {
                            if (Cells.Terrain[r, c] != TerrainCell.OnFire)
                            {
                                Cells.Life[r, c] = neighbour;
                                lifeBuffer[index] = true;
                                return;
                            }
                        }
                        else
                        {
                            Cells.Life[r, c] = neighbour;
                            return;
                        }
                    }
                }

                if (cell == LifeCell.Alive || cell == LifeCell.Infected)
                {
                    int dir = Rules.Rng.Next(0, 10);
                    if (dir < 4)
                    {
                        if (dir == 0)
                        {
                            if (c > 0 && Cells.Terrain[r, c - 1] != TerrainCell.Blank && Cells.Terrain[r, c - 1] != TerrainCell.OnFire && Cells.Life[r, c - 1] == LifeCell.Blank)
                            {
                                Cells.Life[r, c - 1] = cell;
                                Cells.Life[r, c] = LifeCell.Blank;
                                lifeBuffer[index - 1] = true;
                                Cells.Life[r, c - 1] = Chances.Try(cell);
                                return;
                            }
                        }
                        else if (dir == 1)
                        {
                            if (r > 0 && Cells.Terrain[r - 1, c] != TerrainCell.Blank && Cells.Terrain[r - 1, c] != TerrainCell.OnFire && Cells.Life[r - 1, c] == LifeCell.Blank)
                            {
                                Cells.Life[r - 1, c] = cell;
                                Cells.Life[r, c] = LifeCell.Blank;
                                lifeBuffer[index - Columns] = true;
                                Cells.Life[r - 1, c] = Chances.Try(cell);
                                return;
                            }
                        }
                        else if (dir == 2)
                        {
                            if (c < Columns - 1 && Cells.Terrain[r, c + 1] != TerrainCell.Blank && Cells.Terrain[r, c + 1] != TerrainCell.OnFire && Cells.Life[r, c + 1] == LifeCell.Blank)
                            {
                                Cells.Life[r, c + 1] = cell;
                                Cells.Life[r, c] = LifeCell.Blank;
                                lifeBuffer[index + 1] = true;
                                Cells.Life[r, c + 1] = Chances.Try(cell);
                                return;
                            }
                        }
                        else
                        {
                            if (r < Rows - 1 && Cells.Terrain[r + 1, c] != TerrainCell.Blank && Cells.Terrain[r + 1, c] != TerrainCell.OnFire && Cells.Life[r + 1, c] == LifeCell.Blank)
                            {
                                Cells.Life[r + 1, c] = cell;
                                Cells.Life[r, c] = LifeCell.Blank;
                                lifeBuffer[index + Columns] = true;
                                Cells.Life[r + 1, c] = Chances.Try(cell);
                                return;
                            }
                        }
                    }
                }
                Cells.Life[r, c] = Chances.Try(cell);
            }
        }

        private void LoadTerrainNeighbours(int r, int c)
        {
            for (int i = 0; i < terrainNeighbours.Length; ++i)
                terrainNeighbours[i] = 0;

            if (r > 0) ++terrainNeighbours[(int)terrainBuffer[r - 1, c]];
            if (c > 0) ++terrainNeighbours[(int)terrainBuffer[r, c - 1]];
            if (r < Rows - 1) ++terrainNeighbours[(int)terrainBuffer[r + 1, c]];
            if (c < Columns - 1) ++terrainNeighbours[(int)terrainBuffer[r, c + 1]];
        }

        private void LoadLifeNeighbours(int r, int c)
        {
            for (int i = 0; i < lifeNeighbours.Length; ++i)
                lifeNeighbours[i] = 0;

            if (r > 0) ++lifeNeighbours[(int)Cells.Life[r - 1, c]];
            if (c > 0) ++lifeNeighbours[(int)Cells.Life[r, c - 1]];
            if (r < Rows - 1) ++lifeNeighbours[(int)Cells.Life[r + 1, c]];
            if (c < Columns - 1) ++lifeNeighbours[(int)Cells.Life[r, c + 1]];
        }

        private void ResetBuffers()
        {
            int rows = Rows;
            int count = Columns;

            for (int i = 0; i < rows; ++i)
                for (int j = 0; j < count; ++j)
                    terrainBuffer[i, j] = Cells.Terrain[i, j];

            lifeBuffer.SetAll(false);
        }

        private void ResetCount()
        {
            int count = terrainCount.Length;
            for (int i = 0; i < count; ++i)
                terrainCount[i] = 0;

            count = lifeCount.Length;
            for (int i = 0; i < count; ++i)
                lifeCount[i] = 0;

            int rows = Rows;
            count = Columns;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < count; ++j)
                {
                    ++terrainCount[(int)Cells.Terrain[i, j]];
                    ++lifeCount[(int)Cells.Life[i, j]];
                }
            }
        }

        private static int LifeBufferIndex(int r, int c, int cols)
        {
            return (r * cols + c);
        }
    }
}
