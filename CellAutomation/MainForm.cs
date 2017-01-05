using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellAutomation
{
    public partial class MainForm : Form
    {
        private Automaton automaton;
        private CellsPanel panel;
        private Timer timer;

        public MainForm()
        {
            timer = new Timer();
            timer.Interval = 125;
            timer.Tick += this.Event_Tick;

            automaton = new Automaton(new Cells(120, 200));
            var chances = automaton.Chances;

            chances[TerrainCell.Blank, TerrainCell.Grassland] = 1.0 / 300000;
            chances[TerrainCell.Blank, TerrainCell.Forest] = 1.0 / 800000;
            chances[TerrainCell.Grassland, TerrainCell.Forest] = 1.0 / 200000;
            chances[TerrainCell.Grassland, TerrainCell.OnFire] = 1.0 / 2000000;
            //chances[TerrainCell.Forest, TerrainCell.Grassland] = 1.0 / 350000;
            chances[TerrainCell.Forest, TerrainCell.OnFire] = 1.0 / 800000;
            chances[TerrainCell.OnFire, TerrainCell.Blank] = 1.0 / 6;

            chances[LifeCell.Blank, LifeCell.Alive] = 1.0 / 2000000;
            chances[LifeCell.Alive, LifeCell.Blank] = 1.0 / 50000;
            chances[LifeCell.Alive, LifeCell.Infected] = 1.0 / 500000;
            chances[LifeCell.Infected, LifeCell.Blank] = 1.0 / 70;

            var rules = automaton.Rules;

            rules.Add(TerrainCell.Blank, TerrainCell.Grassland, 1.0 / 40);
            rules.Add(TerrainCell.Blank, TerrainCell.Forest, 1.0 / 80);
            rules.Add(TerrainCell.Grassland, TerrainCell.OnFire, 1.0 / 12);
            rules.Add(TerrainCell.Grassland, TerrainCell.Forest, 1.0 / 130);
            rules.Add(TerrainCell.Forest, TerrainCell.OnFire, 1.0 / 3);

            rules.Add(LifeCell.Blank, LifeCell.Alive, 1.0 / 2000);
            rules.Add(LifeCell.Alive, LifeCell.Infected, 1.0 / 30);

            panel = new CellsPanel();
            panel.Model = automaton.Cells;
            panel.ClientSize = panel.SizeToFit;
            panel.Parent = this;

            InitializeComponent();

            ClientSize = panel.Size;
            timer.Start();
        }

        private void Event_Tick(object sender, EventArgs e)
        {
            automaton.Advance();
            panel.Refresh();
        }
    }
}
