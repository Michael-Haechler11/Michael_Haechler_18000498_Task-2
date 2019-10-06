using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Task_1_POE_18000498
{
    [Serializable]
    class Map
    {

        public string[,] map = new string[20, 20];

        
        
        public List<Unit> units = new List<Unit>();
        public List<Unit> rangedUnits = new List<Unit>();
        public List<Unit> meleeUnits = new List<Unit>();
        public buildings[,] buildingMap = new buildings[20, 20];

        public List<buildings> buildings = new List<buildings>();
        public List<ResourceBuilding> BitCoinMine = new List<ResourceBuilding>();
        public List<FactoryBuildings> Barracks = new List<FactoryBuildings>();
        public Unit[,] uniMap = new Unit [20, 20];
 

        Random Rd = new Random();

        int BuildingNum;

        public Map(int UnitN = 0)
        {
            BuildingNum = UnitN;

        }

        public void GenerateBattleField() // method to allow the random number of units, including the ranged and the melee units
        {

            for (int i = 0; i < BuildingNum; i++)
            {
                if (Rd.Next(0, 2) == 0)
                {

                    ResourceBuilding DiamondMine = new ResourceBuilding(0, 0, 100, Faction.Overwatch, "**");
                    BitCoinMine.Add(DiamondMine);
                }
                else
                {
                    int UnitNum = Rd.Next(0, 2);
                    string UnitName;
                    if (UnitNum == 0)
                    {
                        UnitName = "Melee";
                    }
                    else
                    {
                        UnitName = "Ranged";
                    }

                    FactoryBuildings Barrack = new FactoryBuildings(0, 0, 100, Faction.Overwatch, "$$", Rd.Next(3, 10), UnitName);
                    Barracks.Add(Barrack);
                }
            }
            for (int i = 0; i < BuildingNum; i++)
            {
                if (Rd.Next(0, 2) == 0)
                {
                    ResourceBuilding DiamondMine = new ResourceBuilding(0, 0, 100, Faction.Talon, "**");
                    BitCoinMine.Add(DiamondMine);
                }
                else
                {
                    int UnitNum = Rd.Next(0, 2);
                    string UnitName;
                    if (UnitNum == 0)
                    {
                        UnitName = "Melee";
                    }
                    else
                    {
                        UnitName = "Ranged";
                    }

                    FactoryBuildings barrack = new FactoryBuildings(0, 0, 100, Faction.Talon, "$$", Rd.Next(3, 10), UnitName);
                    Barracks.Add(barrack);
                }
            }

            foreach (ResourceBuilding u in BitCoinMine)
            {
                for (int i = 0; i < BitCoinMine.Count; i++)
                {
                    int xPos = Rd.Next(0, 20);
                    int yPos = Rd.Next(0, 20);

                    while (xPos == BitCoinMine[i].PosX && yPos == BitCoinMine[i].PosY && xPos == Barracks[i].PosX && yPos == Barracks[i].PosY)
                    {
                        xPos = Rd.Next(0, 20);
                        yPos = Rd.Next(0, 20);
                    }

                    u.PosX = xPos;
                    u.PosY = yPos;
                    
                }
                buildingMap[u.PosY, u.PosX] = (buildings)u;
                buildings.Add(u);
            }

            foreach (FactoryBuildings u in Barracks)
            {
                for (int i = 0; i < Barracks.Count; i++)
                {
                    int xPos = Rd.Next(0, 20);
                    int yPos = Rd.Next(0, 20);

                    while (xPos == Barracks[i].PosX && yPos == Barracks[i].PosY && xPos == BitCoinMine[i].PosX && yPos == BitCoinMine[i].PosY)
                    {
                        xPos = Rd.Next(0, 20);
                        yPos = Rd.Next(0, 20);
                    }

                    u.PosX = xPos;
                    u.PosY = yPos;
                    
                }
                buildingMap[u.PosY, u.PosX] = (buildings)u;
                buildings.Add(u);

                u.SpawnPointY = u.PosY;
                if (u.PosX < 19)
                {
                    u.SpawnPointX = u.PosX + 1;
                }
                else
                {
                    u.SpawnPointX = u.PosX - 1;
                }
            }
            Populate();
            PlaceBuildings();
        }

        public void Populate() // method used to populate the map with units
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    map[i, j] = "";
                }
            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    uniMap[i, j] = null;
                }
            }

            foreach (Unit u in units)
            {
                uniMap[u.posY, u.posX] = u;
            }

            foreach (RangedUnit u in rangedUnits)
            {
                Debug.Print("Place Ranged");
                map[u.posY, u.posX] = "R";
            }

            foreach (MeleeUnit u in meleeUnits)
            {
                Debug.Print("Place melee");
                map[u.posY, u.posX] = "M";
            }


        }

        public void PlaceBuildings()  //method that places the buidlings on the map
        {

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    buildingMap[i, j] = null;
                }
            }

            foreach (buildings u in buildings)
            {
                if (u is FactoryBuildings)
                {
                    FactoryBuildings Factory = (FactoryBuildings)u;
                    buildingMap[Factory.PosY, Factory.PosX] = u;
                }
                else if (u is ResourceBuilding)
                {
                    ResourceBuilding factory = (ResourceBuilding)u;
                    buildingMap[factory.PosY, factory.PosX] = u;
                }
            }

            foreach (ResourceBuilding u in BitCoinMine)
            {
                map[u.PosY, u.PosX] = "RB";
            }
            foreach (FactoryBuildings u in Barracks)
            {
                map[u.PosY, u.PosX] = "FB";
            }
        }

        public void SpawnUnits(int x, int y, Faction fac, string unitType)
        {
            if (unitType == "Ranged")
            {
                RangedUnit sniper = new RangedUnit("sniper", x, y, fac, 30, 1, 5, 3, "->", false);
                rangedUnits.Add(sniper);
                units.Add(sniper);
            }
            else if (unitType == "Melee")
            {
                Debug.Print("Create melee");
                MeleeUnit Cavalry = new MeleeUnit("Cavalry", x, y, fac, 50, 1, 10, 1, "#", false);
                meleeUnits.Add(Cavalry);
                units.Add(Cavalry);
            }
        }



        public void PlaceUnits()  //places units on the map in accordance to the buildings
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    map[i, j] = "";
                }
            }

            for (int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 20; j++)
                {
                    uniMap[i, j] = null;
                }
            }

            foreach(Unit u in units)
            {
                uniMap[u.posY, u.posX] = u;
            }

            foreach(Unit u in rangedUnits)
            {
                map[u.posY, u.posX] = "R";
            }

            foreach (Unit u in meleeUnits)
            {
                map[u.posY, u.posX] = "M";
            }
        }          
    }
}
