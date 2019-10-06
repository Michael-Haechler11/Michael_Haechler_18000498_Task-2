using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Task_1_POE_18000498
{
    enum Faction
    {
        Overwatch,
        Talon
    }

    enum ResourceType
    {
        Gems,
        oil

    }

    public partial class Form1 : Form
    {
        Button[,] buttons = new Button[20, 20]; //an array that sets the grid 

        static int UnitNum = 8;
        public int Round = 1;

        Map m = new Map(8);

   

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GameTick.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m.GenerateBattleField();
            Placebuttons();
        }

        public void GameEng()
        {
            int hero = 0;
            int villian = 0;

            foreach (ResourceBuilding u in m.BitCoinMine)
            {
                if (u.Faction == Faction.Overwatch)
                {
                    hero++;
                }
                else
                {
                    villian++;
                }
            }

            foreach (FactoryBuildings u in m.Barracks)
            {
                if (u.Faction == Faction.Overwatch)
                {
                    hero++;
                }
                else
                {
                    villian++;
                }
            }

            foreach (Unit u in m.units)
            {
                if (u.factionType == Faction.Overwatch)
                {
                    villian++;
                }
                else
                {
                    villian++;
                }
            }


            if (hero > 0 && villian > 0)
            {
                foreach (ResourceBuilding Rb in m.BitCoinMine)
                {
                    Rb.GenerateResources();
                }

                foreach (FactoryBuildings Fb in m.Barracks)
                {
                    if (Round % Fb.ProductionSpeed == 0)
                    {
                        m.SpawnUnits(Fb.SpawnPointX, Fb.SpawnPointY, Fb.Faction, Fb.SpawnUnits());
                    }
                }

                foreach (Unit u in m.units)
                {
                    u.CheckAttackRange(m.units, m.buildings);
                }

                m.Populate();
                m.PlaceBuildings();
                Round++;
                Placebuttons();

            }
            else
            {
                m.Populate();
                m.PlaceBuildings();
                Placebuttons();
                GameTick.Enabled = false;

                if (hero > villian)
                {
                    MessageBox.Show("Hero Wins on Round: " + Round);
                }
                else
                {
                    MessageBox.Show("Villian Wins on Round: " + Round);
                }
            }

            for (int i = 0; i < m.rangedUnits.Count; i++)
            {
                if (m.rangedUnits[i].Death())
                {
                    m.map[m.rangedUnits[i].posX, m.rangedUnits[i].posX] = "";
                    m.rangedUnits.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.meleeUnits.Count; i++)
            {
                if (m.meleeUnits[i].Death())
                {
                    m.map[m.meleeUnits[i].posX, m.meleeUnits[i].posX] = "";
                    m.meleeUnits.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.units.Count; i++)
            {
                if (m.units[i].Death())
                {
                    m.map[m.units[i].posX, m.units[i].posX] = "";
                    m.units.RemoveAt(i);
                }
            }
            for(int i = 0; i < m.Barracks.Count; ++i)
            {
                if (m.Barracks[i].Destruction())
                {
                    m.map[m.Barracks[i].PosX, m.Barracks[i].PosY] = "";
                    m.Barracks.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.BitCoinMine.Count; ++i)
            {
                if (m.BitCoinMine[i].Destruction())
                {
                    m.map[m.BitCoinMine[i].PosX, m.BitCoinMine[i].PosY] = "";
                    m.BitCoinMine.RemoveAt(i);
                }
            }

            for (int i = 0; i < m.buildings.Count; ++i)
            {
                if (m.buildings[i].Destruction())
                {
                    if(m.buildings[i] is FactoryBuildings)
                    {
                        FactoryBuildings FB = (FactoryBuildings)m.buildings[i];
                        m.map[FB.PosX, FB.PosY] = "";

                    }
                    else if (m.buildings[i] is ResourceBuilding)
                    {
                        ResourceBuilding RB = (ResourceBuilding)m.buildings[i];
                        m.map[RB.PosX, RB.PosY] = "";
                    }
                    m.buildings.RemoveAt(i);
                }
            }

        }

        public void Placebuttons()  //places the buttons in the text box to create the map
        {
            gb1.Controls.Clear();

            Size btnSize = new Size(30, 30);

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    Button btn = new Button();

                    btn.Size = btnSize;
                    btn.Location = new Point(x * 30, y * 30);

                    if (m.map[x, y] == "R")
                    {
                        btn.Text = "︻デ═一";
                        btn.Name = m.uniMap[x, y].ToString();
                        btn.Click += MyButtonClick;

                        if (m.uniMap[x, y].factionType == Faction.Overwatch)
                        {
                            btn.BackColor = Color.Red;
                        }
                        else
                        {
                            btn.BackColor = Color.Green;
                        }
                    }
                    else if (m.map[x, y] == "M")
                    {
                        btn.Text = "▬===>";
                        btn.Name = m.uniMap[x, y].ToString();
                        btn.Click += MyButtonClick;

                        if (m.uniMap[x, y].factionType == Faction.Overwatch)
                        {
                            btn.BackColor = Color.Red;
                        }
                        else
                        {
                            btn.BackColor = Color.Green;
                        }
                    }
                    else if (m.map[x, y] == "FB")
                    {
                        FactoryBuildings FB = (FactoryBuildings)m.buildingMap[x, y];
                        btn.Text = FB.Symbol;
                        if (FB.Faction == Faction.Overwatch)
                        {
                            btn.BackColor = Color.Red;
                        }
                        else
                        {
                            btn.BackColor = Color.Green;
                        }

                        btn.Name = m.buildingMap[x, y].ToString();
                        btn.Click += MyButtonClick;
                    }
                    else if (m.map[x, y] == "RB")
                    {
                        ResourceBuilding RB = (ResourceBuilding)m.buildingMap[x, y];
                        btn.Text = RB.Symbol;
                        if (RB.Faction == Faction.Overwatch)
                        {
                            btn.BackColor = Color.Red;
                        }
                        else
                        {
                            btn.BackColor = Color.Green;
                        }

                        btn.Name = m.buildingMap[x, y].ToString();
                        btn.Click += MyButtonClick;

                        
                    }
                    buttons[x, y] = btn;
                }
            }
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    gb1.Controls.Add(buttons[x, y]);
                }
            }

        }


        public void MyButtonClick(object sender, EventArgs e)
        {
            Button btn = ((Button)sender);

            foreach (Unit u in m.units)
            {
                if (btn.Name == u.ToString())
                {
                    rtb1.Text = u.ToString();
                }
            }

            foreach (buildings b in m.buildings)
            {
                if (btn.Name == b.ToString())
                {
                    rtb1.Text = b.ToString();
                }
            }
        }

        

        

        private void btnread_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("SaveFile.dat", FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                using (fs)
                {
                    m = (Map)bf.Deserialize(fs);
                    Placebuttons();
                    MessageBox.Show(text: "File Successfully Loaded!");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream("SaveFile.dat", FileMode.Create, FileAccess.Write, FileShare.None);

            try
            {
                using (fs)
                {
                    bf.Serialize(fs, m);
                    MessageBox.Show("File Saved");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            GameTick.Enabled = true;
        }

        private void GameTick_Tick(object sender, EventArgs e)
        {
            GameEng();

            lbl1.Text = "Round: " + Round;
        }
    }
}
