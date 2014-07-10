using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

//  Generates a what's needed to draw a system the first time it is entered in gameplay.
//  after which, it is stored in the main class in a two dimensional array.
//  also created a refrence array of pointers pointing to the appropriate planets in here.Planets().
// PLEASE DO NOT MODIFY, DISTRIBUTE OR SELL FOR PROFIT, THIS SOURCE CODE AND ITS ASSOCIATED PROJECT.
// See Readme.txt for more information.

namespace OuterSpace
{
    public class SolarSystem
    {
        public double sysX, sysY;
        public int[,] planetXY = new int[2,8]; // The x y coordinates, location about the ellipse. 
        // Planet(0, 1) = planet 1's X corrdinate, Planet(1, 1) Y coordinate.
        public int[] x1 = new int[8], 
            x2 = new int[8], y1 = new int[8], 
            y2 = new int[8]; // Dimensions for the elipse. 
        public Rectangle[] picframe = new Rectangle[8]; // Planets picture.
        public bool[] planethere = new bool[8]; // Is there a planet at this orbit
        public int[] planetrefs = new int[8]; // A refrence (pointer) integer to the appropriate indicies in here.Planets()
        // For each planet.
        protected CPlanet[] Planets = new CPlanet[8];
        private Random rnd = new Random();

        public SolarSystem(int X, int Y, Universe here)
        {
            // Get the system data here in the constructor
            // to avoid passing our (huge) galaxy object into yet another sub routine.
            CStar star = null;
            bool[] orbits = new bool[8];
            int[] PlanetTypes = new int[8];

            for (int i = 0; i < 8; i++)
            {
                orbits[i] = false;
                Planets[i] = null;
            }

            here.GetStarAtLocation(X, Y, ref star);
            
            if (star != null)
            {
                for (int i = 0; i < star.NumberOfPlanets; i++)
                {
                    CPlanet planet = null;
                    int orbitnum;

                    star.GetPlanet(i, ref planet);
                    orbitnum = planet.Orbit;

                    orbits[orbitnum - 1] = true;
                    Planets[orbitnum - 1] = planet;
                    planetrefs[orbitnum - 1] = i;
                }
            }

            EllipsePoints();
            SetplanetXY();

            for (int i = 0; i < 8; i++)
            {
                if (orbits[i])
                {
                    picframe[i] = GetPicFrame(Planets[i].PClass); //PlanetTypes(i)) 'We have a planet, get it's picture
                    planethere[i] = true;
                }
                else
                {
                    planethere[i] = false;
                }
            }
        }

        private void EllipsePoints()
        {
            // Set the geometric points for our elipses. 8 orbits in all...
            for (int i = 0; i < 8; i++)
            {
                x1[i] = (400 - (44 * (i + 1)));
                y1[i] = (300 - (34 * (i + 1)));
                x2[i] = (400 + (44 * (i + 1)));
                y2[i] = (300 + (34 * (i + 1)));
            }
        }

        private void SetplanetXY() // Set the location of each planet upon the ellipse.
        {
            int a, b;
            double angle, q;
            
            for (int i = 0; i < 8; i++)
            {
                a = (x2[i] - x1[i]) / 2; //ellipse radius 1 (width)
                b = (y2[i] - y1[i]) / 2; //ellipse radius 2 (height)

                angle = Convert.ToDouble(360.0F * rnd.NextDouble());
                angle = angle * (Math.PI / 180); // convert to radians..
                
                double test = (angle * (180 / Math.PI));
                {
                    if (test >= 0.0 && test <= 90.0)
                    {
                        // The parametric equations for an ellipse.
                        q = Math.Atan((a * Math.Tan(angle) / b));
                        planetXY[0, i] = 400 + (Convert.ToInt32(a * Math.Cos(q))) - 13; // 13 pixes offset so the 
                        planetXY[1, i] = 300 - (Convert.ToInt32(b * Math.Sin(q))) - 13; // planet appears at the center of the orbit
                    }
                    else if (test > 90.0 && test <= 180.0)     
                    {
                        q = Math.Atan((a * Math.Tan(angle) / b));
                        planetXY[0, i] = 400 - (Convert.ToInt32(a * Math.Cos(q))) - 13;
                        planetXY[1, i] = 300 - (Convert.ToInt32(b * Math.Sin(q))) - 13;
                    }
                    else if (test > 180.0 && test <= 270.0)
                    {
                        q = Math.Atan((a * Math.Tan(angle) / b));
                        planetXY[0, i] = 400 - (Convert.ToInt32(a * Math.Cos(q))) - 13;
                        planetXY[1, i] = 300 + (Convert.ToInt32(b * Math.Sin(q))) - 13;
                    }
                    else if (test > 270.0 && test <= 360.0)
                    {
                        q = Math.Atan((a * Math.Tan(angle) / b));
                        planetXY[0, i] = 400 + (Convert.ToInt32(a * Math.Cos(q))) - 13;
                        planetXY[1, i] = 300 + (Convert.ToInt32(b * Math.Sin(q))) - 13;
                    }
                    else if (test > 360.0)
                    {
                        angle = 6.28;
                        q = Math.Atan((a * Math.Tan(angle) / b));
                        planetXY[0, i] = 400 + (Convert.ToInt32(a * Math.Cos(q))) - 13;
                        planetXY[1, i] = 300 + (Convert.ToInt32(b * Math.Sin(q))) - 13;
                    }
                }
            }
        }

        private Rectangle GetPicFrame(string CurrentPlanetType)
        {
            // Select a picture frame for each planets sprite.
            Rectangle frame = new Rectangle();
            int frameref;

            frameref = Convert.ToInt32(4 * rnd.NextDouble());

            switch (CurrentPlanetType)
            {
                case "V": // Pick one of the frames for a molten planet.
                    frame = new Rectangle((frameref % 4) * 25, 0, 25, 25);
                    break;
                case "I": // Pick one of the frames for a inferno planet.
                    frame = new Rectangle((frameref % 4) * 25, 25, 25, 25);
                    break;
                case "P": // Pick one of the frames for a poison planet.
                    frame = new Rectangle((frameref % 4) * 30, 50, 30, 30);
                    break;
                case "T": // Pick one of the frames for a rock/water planet.
                    frame = new Rectangle((frameref % 4) * 30, 80, 30, 30);
                    break;
                case "R": // Pick one of the frames for a radiated planet.
                    frame = new Rectangle((frameref % 4) * 30, 110, 30, 30);
                    break;
                case "X": // Pick one of the frames for a exotic planet.
                    frame = new Rectangle((frameref % 4) * 30, 140, 30, 30);
                    break;
                case "J": // Pick one of the frames for a gas giant
                    frame = new Rectangle((frameref % 4) * 50, 170, 50, 50);
                    break;
                case "B": // Pick one of the frames for a barren planet.
                    frame = new Rectangle((frameref % 4) * 30, 220, 30, 30);
                    break;
                case "F": // Pick one of the frames for frozen planet
                    frame = new Rectangle((frameref % 4) * 25, 250, 25, 25);
                    break;
            }

            return frame;
        }
    }
}