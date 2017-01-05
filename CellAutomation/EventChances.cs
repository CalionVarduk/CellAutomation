using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellAutomation
{
    public class EventChances
    {
        private const int MaxChanceMult = 1000000000;

        public Random Rng { get; set; }

        public double this[TerrainCell from, TerrainCell to]
        {
            get { return (terrainChances[(int)from, (int)to] / (double)MaxChanceMult); }
            set
            {
                if (value < 0) value = 0;
                else if (value > 1) value = 1;

                totalTerrainChances[(int)from] -= terrainChances[(int)from, (int)to];

                int maxChance = MaxChanceOf(totalTerrainChances);
                int chance = (int)(value * MaxChanceMult + 0.5);
                if (chance > maxChance) chance = maxChance;

                terrainChances[(int)from, (int)to] = chance;
                totalTerrainChances[(int)from] += chance;
            }
        }

        public double this[LifeCell from, LifeCell to]
        {
            get { return (lifeChances[(int)from, (int)to] / (double)MaxChanceMult); }
            set
            {
                if (value < 0) value = 0;
                else if (value > 1) value = 1;

                totalLifeChances[(int)from] -= lifeChances[(int)from, (int)to];

                int maxChance = MaxChanceOf(totalLifeChances);
                int chance = (int)(value * MaxChanceMult + 0.5);
                if (chance > maxChance) chance = maxChance;

                lifeChances[(int)from, (int)to] = chance;
                totalLifeChances[(int)from] += chance;
            }
        }

        private int[,] terrainChances;
        private int[] totalTerrainChances;
        private int[,] lifeChances;
        private int[] totalLifeChances;

        public EventChances()
            : this(new Random())
        { }

        public EventChances(Random rng)
        {
            Rng = rng;

            int count = Enum.GetNames(typeof(TerrainCell)).Length;
            terrainChances = new int[count, count];
            totalTerrainChances = new int[count];

            count = Enum.GetNames(typeof(LifeCell)).Length;
            lifeChances = new int[count, count];
            totalLifeChances = new int[count];
        }

        public TerrainCell Try(TerrainCell cell)
        {
            if (totalTerrainChances[(int)cell] == 0) return cell;

            int rand = Rng.Next(0, MaxChanceMult);

            if (rand < totalTerrainChances[(int)cell])
            {
                int i = 0;
                while (rand >= 0)
                {
                    if (rand < terrainChances[(int)cell, i]) return (TerrainCell)i;
                    rand -= terrainChances[(int)cell, i++];
                }
            }
            return cell;
        }

        public LifeCell Try(LifeCell cell)
        {
            if (totalLifeChances[(int)cell] == 0) return cell;

            int rand = Rng.Next(0, MaxChanceMult);

            if (rand < totalLifeChances[(int)cell])
            {
                int i = 0;
                while (rand >= 0)
                {
                    if (rand < lifeChances[(int)cell, i]) return (LifeCell)i;
                    rand -= lifeChances[(int)cell, i++];
                }
            }
            return cell;
        }

        private static int MaxChanceOf(int[] totalChances)
        {
            int sum = totalChances[0];

            for (int i = 1; i < totalChances.Length; ++i)
                sum += totalChances[i];

            return (MaxChanceMult - sum);
        }
    }
}
