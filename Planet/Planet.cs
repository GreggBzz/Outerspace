using System;
using System.Drawing;
using System.Collections.Generic;

//  The class planet. Data for each planet.
//  Contains rendering functions for the planet as we orbit it.
//  Copyright� 2003-2004 Gregg Brzozowski, gregg.brzozowski@gmail.com
// PLEASE DO NOT MODIFY, DISTRIBUTE OR SELL FOR PROFIT, THIS SOURCE CODE AND ITS ASSOCIATED PROJECT.
// See Readme.txt for more information.

namespace OuterSpace
{
    public class CPlanet
    {
        private int _id;
        private int system_id;
        private string _name;
        private int orbitnum;
        private string _pclass;
        private float _mass;
        private int _radius;
        private float _density;
        private float gravity;
        private string _lithosphere;
        private string _atmosphere;
        private string _hydrosphere;
        private int biological_density;
        private int mineral_density;
        private int _weather;
        private string climate_range;
        private float atmospheric_pressure;
        private bool bHabitable;
        private Int32 seed;
        public bool snowing;
        public string BioList;
        public string Special;
        public bool displayScan;
        public bool displayAnalysis;
        public FractalPlanet planetpoly;
        public bool renderPolyPlanet;

        public bool[,] mineralmap = new bool[1, 1];
        public lifeforms[,] lifemap = new lifeforms[1, 1];

        // 3-10-06 ...
        public PlanetMap PlanetMaps = null;

        public int ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public int SystemID
        {
            get
            {
                return system_id;
            }
            set
            {
                system_id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Orbit
        {
            get
            {
                return orbitnum;
            }
            set
            {
                orbitnum = value;
            }
        }

        public string PClass
        {
            get
            {
                return _pclass;
            }
            set
            {
                _pclass = value;
            }
        }

        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                _mass = value;
            }
        }

        //  in km
        public int Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        public float Density
        {
            get
            {
                return _density;
            }
            set
            {
                _density = value;
            }
        }

        public float SurfaceGravity
        {
            get
            {
                return gravity;
            }
            set
            {
                gravity = value;
            }
        }

        public string Lithosphere
        {
            get
            {
                return _lithosphere;
            }
            set
            {
                _lithosphere = value;
            }
        }

        public string Atmosphere
        {
            get
            {
                return _atmosphere;
            }
            set
            {
                _atmosphere = value;
            }
        }

        public string Hydrosphere
        {
            get
            {
                return _hydrosphere;
            }
            set
            {
                _hydrosphere = value;
            }
        }

        public int BioDensity
        {
            get
            {
                return biological_density;
            }
            set
            {
                biological_density = value;
            }
        }

        public int MinDensity
        {
            get
            {
                return mineral_density;
            }
            set
            {
                mineral_density = value;
            }
        }

        public int Weather
        {
            get
            {
                return _weather;
            }
            set
            {
                _weather = value;
            }
        }

        public string ClimateRange
        {
            get
            {
                return climate_range;
            }
            set
            {
                climate_range = value;
            }
        }

        public float AtmosphericPressure
        {
            get
            {
                return atmospheric_pressure;
            }
            set
            {
                atmospheric_pressure = value;
            }
        }

        public bool Habitable
        {
            get
            {
                return bHabitable;
            }
            set
            {
                bHabitable = value;
            }
        }

        public Int32 uSeed
        {
            get
            {
                return seed;
            }
            set
            {
                seed = value;
            }
        }

        public void drawplanet()
        {
            int iceCapRange;
            int displaceMag;
            float planetScalar;
            int roughness;
            string txrMap = "";
            
            if (renderPolyPlanet == false)
            {
                OuterSpace.msgbox.pushmsgs("Orbit Established");
                OuterSpace.msgbox.pushmsgs("+/- to zoom in and out");
                // OuterSpace.msgbox.pushmsgs("Use arrow keys to fly around,")
                //  Set our random seed based on each planets unique coordinates..
                iceCapRange = 100;
                displaceMag = 12;
                planetScalar = 1;
                roughness = 2;

                switch (PClass)
                {
                    case "V":
                        txrMap = "txrMoltenPlanet.bmp";
                        break;
                    case "I":
                        txrMap = "txrInfernoPlanet.bmp";
                        break;
                    case "P":
                        txrMap = "txrPoisonPlanet.bmp";
                        break;
                    case "T":
                        txrMap = "txrEarthPlanet.bmp";
                        break;
                    case "R":
                        txrMap = "txrRadiatedPlanet.bmp";
                        break;
                    case "X":
                        txrMap = "txrExoticPlanet.bmp";
                        break;
                    case "J":
                        planetScalar = 0.7F;
                        txrMap = "txrGasPlanet.bmp";
                        break;
                    case "B":
                        txrMap = "txrBarrenPlanet.bmp";
                        break;
                    case "F":
                        planetScalar = 1.3F;
                        txrMap = "txrIcePlanet.bmp";
                        break;
                }

                planetpoly = new FractalPlanet(iceCapRange, displaceMag, planetScalar, roughness, 
                    Convert.ToSingle(uSeed), txrMap);
            }

            renderPolyPlanet = true;

            planetpoly.RenderPlanet();

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landing))
            {
                if (OuterSpace.mnu.userchoice == 49)
                {
                    OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.Landing);
                    OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.Landed);
                    renderPolyPlanet = false;
                    OuterSpace.TV.X = 0;
                    OuterSpace.TV.Y = 0;
                    OuterSpace.TV.lastCenter.X = 0;
                    OuterSpace.TV.lastCenter.Y = 0;
                    planetpoly.disposevertexbuffer();
                    OuterSpace.mnu.userchoice = -1;
                    OuterSpace.mnu.items[49] = "Land";
                }
            }

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed))
            {
                if (OuterSpace.mnu.userchoice == 50)
                {
                    //  Move the terrian vehicle if the user picks disembark. 
                    OuterSpace.mnu.showmenu = false;
                    OuterSpace.TV.letsMove();
                    OuterSpace.textfont.DrawText(20, 150, Color.White, Convert.ToString(OuterSpace.TV.X));
                    OuterSpace.textfont.DrawText(20, 172, Color.White, Convert.ToString(OuterSpace.TV.Y));
                    OuterSpace.textfont.DrawText(20, 194, Color.White, ("Theta: " + Convert.ToString(OuterSpace.TV.theta)));
                    // Believe it or not kids, the below is one function call!
                    OuterSpace.textfont.DrawText(
                        20, 
                        216, 
                        Color.White, 
                        ("Elevation: " + 
                            OuterSpace.thisPlanet.planetpoly.startSphere.getElevationofspot(
                                int.Parse(((int)(OuterSpace.TV.X + (OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2))).ToString()), 
                                int.Parse(((int)(OuterSpace.TV.Y + (OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2))).ToString())
                            ).ToString()
                         )
                    );
                }

                if ((OuterSpace.mnu.userchoice == 49))
                {
                    OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.Landing);
                    OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.Landed);
                    renderPolyPlanet = false;
                    planetpoly.disposevertexbuffer();
                    OuterSpace.mnu.userchoice = -1;
                    OuterSpace.mnu.items[49] = "Land";
                }
            }
        }

        public void ReSetVB()
        {
            planetpoly.disposevertexbuffer();
            planetpoly.CreateVertexBuffer();
        }

        public void LetsLand()
        {
            planetpoly.disposevertexbuffer();
            planetpoly.startSphere.Flatten();
            planetpoly.z = 0;
            planetpoly.land = true;
            planetpoly.CreateVertexBuffer();
        }

        public string GetGravity()
        {
            return (gravity.ToString("F2") + " G");
        }

        public string GetCompounds(int type, int precedence)
        {
            string[] comps = new string[3];

            // Function to return the appropriate string values from the 
            // hydro litho and atmosphere integer values. 
            List<string> atmoGases = new List<string>();
            atmoGases.Add("Hydrogen");
            atmoGases.Add("Helium");
            atmoGases.Add("Argon");
            atmoGases.Add("Oxygen");
            atmoGases.Add("Ozone");
            atmoGases.Add("Oxygen Isotopes");
            atmoGases.Add("Nitrogen");
            atmoGases.Add("Nitrogen Isotopes");
            atmoGases.Add("Nitrogen Compounds");
            atmoGases.Add("Carbon Dioxide");
            atmoGases.Add("Carbon Monoxide");
            atmoGases.Add("Sodium");
            atmoGases.Add("Potassium");
            atmoGases.Add("Ammonia Compounds");
            atmoGases.Add("Chlorine Compounds");
            atmoGases.Add("Methane");
            atmoGases.Add("Ethane");
            atmoGases.Add("Acetylene");
            atmoGases.Add("Diacetylene");
            atmoGases.Add("Hydrocarbons");
            atmoGases.Add("Aerosols");
            atmoGases.Add("Chlorine Gas");
            atmoGases.Add( "Sulfuric Acid");
            atmoGases.Add("Hydrochloric Acid");
            atmoGases.Add("Neon");
            atmoGases.Add("Flourine");
            atmoGases.Add("Water Vapor");
            atmoGases.Add("Nitric Acid");
            atmoGases.Add("Radon");
            atmoGases.Add("Unknown");

            List<string> hydroLiquids = new List<string>();
            hydroLiquids.Add("Hydrogen");
            hydroLiquids.Add("Helium");
            hydroLiquids.Add("Argon");
            hydroLiquids.Add("Oxygen");
            hydroLiquids.Add("Nitrogen");
            hydroLiquids.Add("Carbon Dioxide");
            hydroLiquids.Add("Sodium");
            hydroLiquids.Add("Potassium");
            hydroLiquids.Add("Ammonia");
            hydroLiquids.Add("Ammonia Compounds");
            hydroLiquids.Add("Chlorine Compounds");
            hydroLiquids.Add("Methane");
            hydroLiquids.Add("Ethane");
            hydroLiquids.Add("Acetylene");
            hydroLiquids.Add("Diacetylene");
            hydroLiquids.Add("Hydrocarbons");
            hydroLiquids.Add("Aerosols");
            hydroLiquids.Add("Mercury");
            hydroLiquids.Add("Sulfuric Acid");
            hydroLiquids.Add("Hydrochloric Acid");
            hydroLiquids.Add("Water");
            hydroLiquids.Add("Unknown");

            List<string> lithoSolids = new List<string>();            
            lithoSolids.Add("Aluminium");
            lithoSolids.Add("Antimony");
            lithoSolids.Add("Chromium");
            lithoSolids.Add("Cobalt");
            lithoSolids.Add("Carbon");
            lithoSolids.Add("Mercury");
            lithoSolids.Add("Gold");
            lithoSolids.Add("Silver");
            lithoSolids.Add("Hydrocarbons");
            lithoSolids.Add("Iron");
            lithoSolids.Add("Copper");
            lithoSolids.Add("Lead");
            lithoSolids.Add("Magnesium");
            lithoSolids.Add("Manganese");
            lithoSolids.Add("Molybdenum");
            lithoSolids.Add("Nickel");
            lithoSolids.Add("Platinum");
            lithoSolids.Add("Uranium");
            lithoSolids.Add("Promethium");
            lithoSolids.Add("Silicon");
            lithoSolids.Add("Tin");
            lithoSolids.Add("Titanium");
            lithoSolids.Add("Tungsten");
            lithoSolids.Add("Zinc");
            lithoSolids.Add("Sodium");
            lithoSolids.Add("Francium");
            lithoSolids.Add("Helium 3");
            lithoSolids.Add("Deuterium");
            lithoSolids.Add("Calcium");
            lithoSolids.Add("Bismuth");
            lithoSolids.Add("Boron");
            lithoSolids.Add("Sulfer");
            lithoSolids.Add("Unknown");
            
            if (type == 0)
            {
                // Atmosphere
                comps[0] = atmoGases[Convert.ToInt32(Atmosphere.Substring(0, 2))];
                comps[1] = atmoGases[Convert.ToInt32(Atmosphere.Substring(3, 2))];
                comps[2] = atmoGases[Convert.ToInt32(Atmosphere.Substring(6, 2))];
            }
            else if (type == 1)
            {
                // Hydrosphere
                comps[0] = hydroLiquids[Convert.ToInt32(Hydrosphere.Substring(0, 2))];
                comps[1] = hydroLiquids[Convert.ToInt32(Hydrosphere.Substring(3, 2))];
                comps[2] = hydroLiquids[Convert.ToInt32(Hydrosphere.Substring(6, 2))];
            }
            else if (type == 2)
            {
                // Lithosphere
                comps[0] = lithoSolids[Convert.ToInt32(Lithosphere.Substring(0, 2))];
                comps[1] = lithoSolids[Convert.ToInt32(Lithosphere.Substring(3, 2))];
                comps[2] = lithoSolids[Convert.ToInt32(Lithosphere.Substring(6, 2))];
            }

            return comps[precedence];
        }

        private string ConvertMass()
        {
            // ByVal MassRef As Integer) As String
            return (Mass.ToString() + "kg");
        }

        public string GetWeather()
        {
            string[] theWeather = {
                "None",
                "Calm",
                "Normal",
                "Severe",
                "Violent",
                "Extreme"
            };

            return theWeather[Weather];
        }

        public void makeScanBox()
        {
            // ByVal thisPlanet As Planet)
            if (displayScan)
            {
                //  We are safe with static numbers here since we 
                //  are going from left to right, starting at 0. 
                //  This will work regardless of resolution.
                if (OuterSpace.theWindowMgr.FindWindow("Scan Results:") == -1)
                {
                    if (OuterSpace.theWindowMgr.LoadWindow("CScanWindow", 282, 5, 128, 64, Color.FromArgb(217, 255, 255, 255)) == true)
                    {
                        OuterSpace.theWindowMgr.ShowWindow(OuterSpace.theWindowMgr.FindWindow("Scan Results:"), true);
                    }
                }
            }
            if (displayAnalysis)
            {
                if (OuterSpace.theWindowMgr.FindWindow("Scan Analysis:") == -1)
                {
                    if (OuterSpace.theWindowMgr.LoadWindow("CAnalysisWindow", 282, 72, 256, 288, Color.FromArgb(217, 255, 255, 255)) == true)
                    {
                        OuterSpace.theWindowMgr.ShowWindow(OuterSpace.theWindowMgr.FindWindow("Scan Analysis:"), true);
                    }
                }
            }
        }
    }
}