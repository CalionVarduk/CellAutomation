using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellAutomation
{
    public class NeighbourRules
    {
        public Random Rng { get; set; }

        private List<TerrainCell>[] handledTerrain;
        private List<LifeCell>[] handledLife;
        private int[,] terrainChances;
        private int[,] lifeChances;

        public NeighbourRules()
            : this(new Random())
        { }

        public NeighbourRules(Random rng)
        {
            Rng = rng;

            int count = Enum.GetNames(typeof(TerrainCell)).Length;
            terrainChances = new int[count, count];
            handledTerrain = new List<TerrainCell>[count];
            for(int i = 0; i < count; ++i)
                handledTerrain[i] = new List<TerrainCell>();

            count = Enum.GetNames(typeof(LifeCell)).Length;
            lifeChances = new int[count, count];
            handledLife = new List<LifeCell>[count];
            for (int i = 0; i < count; ++i)
                handledLife[i] = new List<LifeCell>();

            Clear();
        }

        public TerrainCell NeighbourAt(TerrainCell cell, int i)
        {
            return handledTerrain[(int)cell][i];
        }

        public LifeCell NeighbourAt(LifeCell cell, int i)
        {
            return handledLife[(int)cell][i];
        }

        public int CountOf(TerrainCell cell)
        {
            return handledTerrain[(int)cell].Count;
        }

        public int CountOf(LifeCell cell)
        {
            return handledLife[(int)cell].Count;
        }

        public bool HasRuleOf(TerrainCell cell, TerrainCell neighbour)
        {
            return handledTerrain[(int)cell].Contains(neighbour);
        }

        public bool HasRuleOf(LifeCell cell, LifeCell neighbour)
        {
            return handledLife[(int)cell].Contains(neighbour);
        }

        public double ChanceOf(TerrainCell cell, TerrainCell neighbour)
        {
            return (1.0 / terrainChances[(int)cell, (int)neighbour]);
        }

        public double ChanceOf(LifeCell cell, LifeCell neighbour)
        {
            return (1.0 / lifeChances[(int)cell, (int)neighbour]);
        }

        public int MaxRngOf(TerrainCell cell, TerrainCell neighbour)
        {
            return terrainChances[(int)cell, (int)neighbour];
        }

        public int MaxRngOf(LifeCell cell, LifeCell neighbour)
        {
            return lifeChances[(int)cell, (int)neighbour];
        }

        public bool Remove(TerrainCell cell, TerrainCell neighbour)
        {
            return handledTerrain[(int)cell].Remove(neighbour);
        }

        public bool Remove(LifeCell cell, LifeCell neighbour)
        {
            return handledLife[(int)cell].Remove(neighbour);
        }

        public void Add(TerrainCell cell, TerrainCell neighbour, double chance)
        {
            if (chance < 0) chance = 0;
            else if (chance > 1) chance = 1;

            if (chance == 0)
            {
                Remove(cell, neighbour);
                terrainChances[(int)cell, (int)neighbour] = 0;
            }
            else
            {
                if (!HasRuleOf(cell, neighbour)) handledTerrain[(int)cell].Add(neighbour);
                terrainChances[(int)cell, (int)neighbour] = (int)(1 / chance + 0.5);
            }
        }

        public void Add(LifeCell cell, LifeCell neighbour, double chance)
        {
            if (chance < 0) chance = 0;
            else if (chance > 1) chance = 1;

            if (chance == 0)
            {
                Remove(cell, neighbour);
                lifeChances[(int)cell, (int)neighbour] = 0;
            }
            else
            {
                if (!HasRuleOf(cell, neighbour)) handledLife[(int)cell].Add(neighbour);
                lifeChances[(int)cell, (int)neighbour] = (int)(1 / chance + 0.5);
            }
        }

        public bool Try(TerrainCell cell, TerrainCell neighbour, int neighbourCount)
        {
            return ((neighbourCount > 0) && (Rng.Next(0, MaxRngOf(cell, neighbour)) < neighbourCount));
        }

        public bool Try(LifeCell cell, LifeCell neighbour, int neighbourCount)
        {
            return ((neighbourCount > 0) && (Rng.Next(0, MaxRngOf(cell, neighbour)) < neighbourCount));
        }

        public void Clear(TerrainCell cell)
        {
            handledTerrain[(int)cell].Clear();
            int count = handledTerrain.Length;
            for (int i = 0; i < count; ++i)
                terrainChances[(int)cell, i] = 0;
        }

        public void Clear(LifeCell cell)
        {
            handledLife[(int)cell].Clear();
            int count = handledLife.Length;
            for (int i = 0; i < count; ++i)
                lifeChances[(int)cell, i] = 0;
        }

        public void Clear()
        {
            int count = handledTerrain.Length;
            for (int i = 0; i < count; ++i) Clear((TerrainCell)i);

            count = handledLife.Length;
            for (int i = 0; i < count; ++i) Clear((LifeCell)i);
        }
    }
}
