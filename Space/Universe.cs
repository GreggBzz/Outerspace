//  Loads our galaxy from a file. 
//  Loads the Planets array with all the planet objects,
//  also loads the Systems() array and the SysRefs array.
//  Systems() only is used in the drawing of the starmap. I'll probabbly remove it 
//  in the near future because it really serves no meaningfull purpose.. 

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace OuterSpace
{
    public class Universe
    {
        private CStar[,] stars = new CStar[251, 251];

        public Universe()
        {
            string strUniversePath = "";

            OpenFileDialog openUniverse = new OpenFileDialog();

            openUniverse.Title = "Select A Universe to Explore!";
            openUniverse.DefaultExt = "starsystems.xml";
            openUniverse.Filter = "Universe Files|starsystems.xml";
            
            openUniverse.ShowDialog();
            
            strUniversePath = openUniverse.FileName;
            
            for (int i = 0; i < 251; i++)
            {
                for (int j = 0; j < 251; j++)
                {
                    stars[i, j] = null;
                }
            }
            
            if (strUniversePath != "")
            {
                LoadUniverse(strUniversePath);
            }
        }

        public bool LoadUniverse(string strPath)
        {
            int nNumStars = 0;
            int nNumPlanets = 0;
            int nNumNebulae = 0;
            int nNumStations = 0;

            DataTable dtStars = null;
            DataTable dtPlanets = null;
            DataTable dtNebulae = null;
            DataTable dtStations = null;

            DataRow row = null;
            DataSet ds = new DataSet();
            bool bRetVal = false;
            
            ds.ReadXml(strPath, XmlReadMode.InferSchema);
            
            //  load stars
            dtStars = ds.Tables["Star"];
            nNumStars = dtStars.Rows.Count;

            for (int nRow = 0; nRow < nNumStars; nRow++)
            {
                int x = Convert.ToInt16(dtStars.Rows[nRow]["X"]);
                int y = Convert.ToInt16(dtStars.Rows[nRow]["Y"]);

                stars[x, y] = new CStar();

                row = dtStars.Rows[nRow];

                stars[x, y].ID = Convert.ToInt16(dtStars.Rows[nRow]["ID"]);
                // stars[x, y].GroupID = Convert.ToInt16(dtStars.Rows[nRow]["GroupID"])
                stars[x, y].Name = dtStars.Rows[nRow]["Name"].ToString();
                stars[x, y].Coordinates = new Point(x, y);
                // stars[x, y].Z = Convert.ToInt16(dtStars.Rows[nRow]["Z"])
                stars[x, y].SpectralClass = dtStars.Rows[nRow]["SpectralClass"].ToString();
                stars[x, y].Temperature = Convert.ToInt32(dtStars.Rows[nRow]["Temperature"]);
                stars[x, y].InitPlanets(Convert.ToInt16(dtStars.Rows[nRow]["NumberOfPlanets"]));

                bRetVal = true;
            }

            //  load planets
            dtPlanets = ds.Tables["Planet"];
            nNumPlanets = dtPlanets.Rows.Count;

            for (int nRow = 0; nRow < nNumPlanets; nRow++)
            {
                CPlanet planet = null;
                int nSysID = Convert.ToInt16(dtPlanets.Rows[nRow]["SystemID"]);
                int nPlanetID = Convert.ToInt16(dtPlanets.Rows[nRow]["ID"]);
                bool bFound = false;
                int x = 0, y = 0;

                for (y = 0; y < 251; y++)
                {
                    for (x = 0; x < 251; x++)
                    {
                        if (stars[x, y] != null)
                        {
                            if (stars[x, y].ID == nSysID)
                            {
                                bFound = true;
                                break;
                            }
                        }
                    }

                    if (bFound) break;
                }

                if (bFound)
                {
                    stars[x, y].GetPlanet(nPlanetID - 1, ref planet);

                    planet.SystemID = nSysID;
                    planet.ID = nPlanetID;
                    planet.Orbit = Convert.ToInt16(dtPlanets.Rows[nRow]["Orbit"]);
                    planet.Name = dtPlanets.Rows[nRow]["Name"].ToString();
                    planet.PClass = dtPlanets.Rows[nRow]["Class"].ToString();
                    planet.Density = (float)Convert.ToDouble(dtPlanets.Rows[nRow]["Density"]);
                    planet.Radius = Convert.ToInt32(dtPlanets.Rows[nRow]["Radius"]);
                    planet.Mass = (float)Convert.ToDouble(dtPlanets.Rows[nRow]["Mass"]);
                    planet.SurfaceGravity = (float)Convert.ToDouble(dtPlanets.Rows[nRow]["SurfaceGravity"]);
                    planet.Weather = Convert.ToInt16(dtPlanets.Rows[nRow]["Weather"]);
                    planet.ClimateRange = dtPlanets.Rows[nRow]["ClimateRange"].ToString();
                    planet.AtmosphericPressure = (float)Convert.ToDouble(dtPlanets.Rows[nRow]["AtmosphericPressure"]);
                    planet.Atmosphere = dtPlanets.Rows[nRow]["Atmosphere"].ToString();
                    planet.Lithosphere = dtPlanets.Rows[nRow]["Lithosphere"].ToString();
                    planet.Hydrosphere = dtPlanets.Rows[nRow]["Hydrosphere"].ToString();
                    planet.MinDensity = Convert.ToInt16(dtPlanets.Rows[nRow]["MinDensity"]);
                    planet.BioDensity = Convert.ToInt16(dtPlanets.Rows[nRow]["BioDensity"]);
                }
                else
                {
                    // rogue planet
                }
            }

            //  load nebulae from xml if it exists in the same path
            ds.Reset();
            strPath = Path.GetDirectoryName(strPath) + "\\nebulae.xml";

            if (File.Exists(strPath))
            {
                ds.ReadXml(strPath, XmlReadMode.InferSchema);
                dtNebulae = ds.Tables["Nebula"];
                nNumNebulae = dtNebulae.Rows.Count;

                //  load into nebula class
                // #SUGGESTION - FINISH
                // DOES NOT EXIST YET
            }

            //  load spacestations from xml. if not exists throw error
            ds.Reset();
            strPath = Path.GetDirectoryName(strPath) + "\\spacestations.xml";

            if (File.Exists(strPath))
            {
                ds.ReadXml(strPath, XmlReadMode.InferSchema);
                dtStations = ds.Tables["Station"];
                nNumStations = dtStations.Rows.Count;

                //  load into stations class
                //  we don't have one yet and not sure if we need one.  only if multiple stations
                //  right now I'll assume only starport
                if (nNumStations > 0)
                {
                    OuterSpace.starport.SetStarportSystem(Convert.ToInt32(dtStations.Rows[0]["SystemID"]));
                    OuterSpace.starport.SetStarportOrbit(Convert.ToInt32(dtStations.Rows[0]["OrbitNum"]));
                }
            }

            return bRetVal;
        }

        public void GetStarAtLocation(int x, int y, ref CStar star)
        {
            star = stars[x, y];
        }

        public void GetStarByID(int id, ref CStar star)
        {
            for (int y = 0; y < 251; y++)
            {
                for (int x = 0; x < 251; x++)
                {
                    if (stars[x, y] != null)
                    {
                        if (stars[x, y].ID == id)
                        {
                            star = stars[x, y];
                            return;
                        }
                    }
                }
            }
        }

        public void GetStarByName(string name, ref CStar star)
        {
            for (int y = 0; y < 251; y++)
            {
                for (int x = 0; x < 251; x++)
                {
                    if (stars[x, y] != null)
                    {
                        if (stars[x, y].Name == name)
                        {
                            star = stars[x, y];
                            return;
                        }
                    }
                }
            }
        }

        public void GetPlanetInSystem(int x, int y, int planetindex, ref CPlanet planet)
        {
            CStar star = null;

            GetStarAtLocation(x, y, ref star);
            star.GetPlanet(planetindex, ref planet);
        }

        public void GetPlanetInSystemByOrbit(int x, int y, int orbitnum, ref CPlanet planet)
        {
            CStar star = null;
            
            GetStarAtLocation(x, y, ref star);
            star.GetPlanetByOrbit(orbitnum, ref planet);
        }

        public bool HasStar(int x, int y)
        {
            if ((x > 0 && x < 251) && (y > 0 && y < 251))
            {
                if (stars[x, y] != null)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public int GetTempType(string starSpecClass)
        {
            int type = 0;

            switch (starSpecClass.Substring(0, 1))
            {
                case "O":
                    type = 0;
                    break;
                case "B":
                    type = 1;
                    break;
                case "A":
                    type = 2;
                    break;
                case "F":
                    type = 3;
                    break;
                case "G":
                    type = 4;
                    break;
                case "K":
                    type = 5;
                    break;
                case "M":
                    type = 6;
                    break;
                case "0":
                    type = 7;
                    break;
            }

            return type;
        }
    }
}
