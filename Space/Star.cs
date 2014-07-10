using System;
using System.Drawing;

namespace OuterSpace
{
    public class CStar
    {
        #region Member Variables

        private int id;
        private string name;
        private string spectralClass;
        private long temperature;
        private Point coordinates;
        private int nNumPlanets = 0;
        private CPlanet[] planets = null;

        #endregion

        #region Properties

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string SpectralClass
        {
            get
            {
                return spectralClass;
            }
            set
            {
                spectralClass = value;
            }
        }

        public long Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value;
            }
        }

        public Point Coordinates
        {
            get
            {
                return coordinates;
            }
            set
            {
                coordinates = value;
            }
        }

        public int NumberOfPlanets
        {
            get
            {
                return nNumPlanets;
            }
        }

        #endregion

        #region Methods

        public void InitPlanets(int nNum)
        {
            nNumPlanets = nNum;
            if (nNumPlanets > 0)
            {
                planets = new CPlanet[nNumPlanets];
                for (int i = 0; i < nNumPlanets; i++)
                {
                    planets[i] = new CPlanet();
                    planets[i].SystemID = ID;
                    planets[i].uSeed = Convert.ToInt32((Coordinates.X.ToString() + 
                        (Coordinates.Y.ToString() + (i + 1).ToString())));
                }
            }
        }

        public void GetPlanet(int index, ref CPlanet planet)
        {
            planet = planets[index];
        }

        public void GetPlanetByOrbit(int orbit, ref CPlanet planet)
        {
            for (int i = 0; i < nNumPlanets; i++)
            {
                if (planets[i].Orbit == orbit)
                {
                    planet = planets[i];
                    break;
                }
            }
        }

        #endregion
    }
}