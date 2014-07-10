//  Stores the current ship or other ship objects state(s).
//  Not yet fully implemented. 

using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

namespace OuterSpace
{
    public class vehicleStats
    {
        public bool shieldsUp;
        public int theta;
        // Public yaw As Integer
        public int thetadif;
        public int yawdif;
        public float combatSpeed;
        // in combat, encounter ship speed
        public float systemSpeed;
        // in system ship speed
        private float inertia;
        // The precentage of inertia
        public float stellerSpeed;
        // inter-steller ship speed.
        public bool weaponsArmed;
        public string shipName;
        public bool displaystatus;
        public int lightson;
        public float X;
        // position of the vehicle in the world
        public float Y;
        public System.Drawing.Point lastCenter;
        public missle[] missles = new missle[30];

        public vehicleStats()
        {
            int i;
            shieldsUp = false;
            combatSpeed = 1;
            systemSpeed = 1;
            inertia = 0;
            stellerSpeed = 0.04f;
            thetadif = 3;
            yawdif = 2;
            lightson = 0;
            weaponsArmed = false;

            for (i = 0; i < 30; i++)
            {
                missles[i] = new missle();
                missles[i].xy.Y = 0;
                // Five missles, their X,Y corrdinates as they travel.
                missles[i].xy.X = 0;
                missles[i].moving = false;
            }

            X = 0;
            Y = 0;
            
            shipName = "ISS Purple Dart";
        }

        public void watchFuel()
        {
            //  Called when moving or if shields are up, to monitor fuel consumption.
        }

        public void letsMove() 
        {
            OuterSpace.drawmouse = false;
            OuterSpace.stars[0].X = 0;
            // Stop the stars..
            OuterSpace.stars[0].Y = 0;
            // Until we click the mouse button to call moving, and move the stars.
            OuterSpace.userinput.MouseXY(0, 0);

            if ((OuterSpace.userinput.mousexpos < 25) || (OuterSpace.userinput.mousexpos > 775)) 
            {
                OuterSpace.userinput.mousexpos = 400;
                OuterSpace.userinput.mouseXdelta = 400;
            }

            if ((OuterSpace.userinput.mouseypos > 575) || (OuterSpace.userinput.mouseypos < 25)) 
            {
                OuterSpace.userinput.mouseypos = 400;
                OuterSpace.userinput.mouseYdelta = 400;
            }

            // ///////////// Alter the rotation about the z axis for top down flight or the 
            // z axis for first person travel accordingly.
            if (OuterSpace.userinput.KeyPressed(Key.LeftArrow, false) || 
                ((OuterSpace.userinput.mouseXdelta - OuterSpace.userinput.mousexpos) > 0)) 
            {
                OuterSpace.userinput.mouseXdelta = OuterSpace.userinput.mousexpos;

                if (theta >= (360 - thetadif))
                {
                    theta = 0;
                }
                else 
                {
                    theta = theta + thetadif;
                }
            }

            if (OuterSpace.userinput.KeyPressed(Key.RightArrow, false) || 
                ((OuterSpace.userinput.mouseXdelta - OuterSpace.userinput.mousexpos) < 0)) 
            {
                OuterSpace.userinput.mouseXdelta = OuterSpace.userinput.mousexpos;

                if (theta <= thetadif) 
                {
                    theta = 360 - (thetadif - 1);
                }
                else 
                {
                    theta = theta - thetadif;
                }
            }

            // ////////////////////////////////
            OuterSpace.userinput.mouseXdelta = OuterSpace.userinput.mousexpos;

            if (OuterSpace.userinput.CheckButton("Left", false) || 
                (inertia > 0 || OuterSpace.userinput.KeyPressed(Key.UpArrow, false))) 
            {
                if (OuterSpace.userinput.CheckButton("Left", false) || 
                    OuterSpace.userinput.KeyPressed(Key.UpArrow, false)) 
                {
                    inertia = 1;
                }

                moving();
                
                return;
            }

            if (OuterSpace.userinput.CheckButton("Right", false)) 
            {
                OuterSpace.userinput.mousexpos = 400;
                OuterSpace.userinput.mouseypos = 300;
                OuterSpace.mnu.userchoice = -1;
                OuterSpace.mnu.showmenu = true;
                OuterSpace.drawmouse = true;
                return;
            }

            if (OuterSpace.userinput.CheckButton("Left", false) && inertia > 0) 
            {
                inertia = (inertia - 0.03f);
            }

            OuterSpace.userinput.resetlastkey(OuterSpace.userinput.lastkey);
            
            if (OuterSpace.userinput.KeyPressed(Key.L, true)) 
            {
                OuterSpace.TV.lightson = (OuterSpace.TV.lightson + 2);
                if (OuterSpace.TV.lightson > 4) 
                {
                    OuterSpace.TV.lightson = 0;
                }
            }
        }

        public void WatchMap(float CurrentX, float CurrentY)
        {
            // Manipulates the cartisian corrdinates of our map array if we are in the terrian vehicle so that we
            // traverse it as you would a globe..
            MathFunctions.location outOfBounds;
            outOfBounds = MathFunctions.outOfBounds(CurrentX, CurrentY, 
                (OuterSpace.thisPlanet.PlanetMaps.BigMapX / -2.0f), 
                (OuterSpace.thisPlanet.PlanetMaps.BigMapY / -2.0f), 
                (OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2.0f), 
                (OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2.0f), (short)theta, false);

            if (CurrentX != outOfBounds.x)
            {
                theta = outOfBounds.angle;
                X = outOfBounds.x;
                Y = outOfBounds.y;
                Geosphere.OutOnX = true;
                OuterSpace.msgbox.othermessage(16);
                updateterrian((int)OuterSpace.TV.X, (int)OuterSpace.TV.Y, true, false);
            }

            if (CurrentY != outOfBounds.y)
            {
                theta = outOfBounds.angle;
                X = outOfBounds.x;
                Y = outOfBounds.y;
                OuterSpace.msgbox.othermessage(17);
                updateterrian((int)OuterSpace.TV.X, (int)OuterSpace.TV.Y, true, false);
            }
        }

        public void updateterrian(int CurrentX, int CurrentY, bool force, bool quiet)
        {
            // Scans new terrian and moves life around if we need.
            //  Scan new terrian.. we pass in X and Y as our TV corrdinates. 
            //  We need to make them relative to the map's array coordinates. So, we add the total dimension of the 
            //  big maps size, to our TV.X and TV.Y corros.. because, TV.X and TV.Y have there 0,0 spot smack in the middle 
            //  of the map. So, a TV.X of -1 would equal [the total map size x / 2 (middle x in the map)] - 1, for one space to the 
            //  left. 
            Point curpt = new Point();
            curpt.X = CurrentX;
            curpt.Y = CurrentY;

            if (MathFunctions.findDistance(lastCenter, curpt) > 5.0F)
            {
                OuterSpace.thisPlanet.planetpoly.startSphere.scanNewterrian(cnvtX(OuterSpace.TV.X), cnvtY(OuterSpace.TV.Y), true);
                lastCenter.Y = (int)(OuterSpace.TV.Y);
                lastCenter.X = (int)(OuterSpace.TV.X);

                if (!quiet)
                {
                    OuterSpace.msgbox.pushmsgs("Scan New Terrian " + "X:"+ 
                        ((int)(OuterSpace.TV.X)).ToString() + 
                        " Y:" + ((int)(OuterSpace.TV.Y)).ToString());
                }
            }
        }

        public void moving()
        {
            float shipvelocity;

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Interstellar))
            {
                shipvelocity = (stellerSpeed * inertia);
            }
            else if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.InEncounter))
            {
                shipvelocity = (combatSpeed * inertia);
            }
            else
            {
                shipvelocity = (systemSpeed * inertia);
            }

            X = X + (OuterSpace.moveX[theta] * shipvelocity);
            Y = Y + (OuterSpace.moveY[theta] * shipvelocity);

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed))
            {
                WatchMap(X, Y);
                mineralPickup();
                updateterrian((int)OuterSpace.TV.X, (int)OuterSpace.TV.Y, false, false);
            }

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Interstellar))
            {
                OuterSpace.stars[0].X = OuterSpace.moveX[theta];
                OuterSpace.stars[0].Y = OuterSpace.moveY[theta];
            }
            
            inertia = (inertia - 0.04f);
        }

        public void mineralPickup()
        {
            if (OuterSpace.thisPlanet.mineralmap[(int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2.0f) - X)), 
                (int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2.0f) - Y))])
            {
                int seed;
                mineral cur_mineral;

                seed = (int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2.0f) - X)) + 
                    (int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2.0f) - Y));

                seed = seed + OuterSpace.thisPlanet.uSeed;
                cur_mineral = new mineral(seed, OuterSpace.thisPlanet.Lithosphere, 1);

                OuterSpace.msgbox.pushmsgs(Convert.ToString(Convert.ToUInt16(cur_mineral.Volume / 10)) + 
                    " Cubic Meters Of " + cur_mineral.type);

                OuterSpace.thisPlanet.mineralmap[(int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2) - X)), 
                    (int)(((OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2) - Y))] = false;

                updateterrian((int)(OuterSpace.TV.X), (int)(OuterSpace.TV.Y), true, true);
            }
        }

        private int cnvtX(float curX)
        {
            return (int)(curX + (OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2));
        }

        private int cnvtY(float curY)
        {
            return (int)(curY + (OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2));
        }

        public class missle
        {
            // Sub class missle
            public System.Drawing.Point xy;
            public bool moving;
            public int theta;

            // the angle at which the missle is traveling.
            public void fire(int ShipX, int ShipY)
            {
                //  Sets the missles starting point, angle of tragectory and 
                //  it's boolean moving to true.
                System.Drawing.Point p1 = new System.Drawing.Point(ShipX, ShipY);
                System.Drawing.Point p2 = new System.Drawing.Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
                moving = true;
                xy.Y = ShipY;
                xy.X = ShipX;
                theta = MathFunctions.findAngle(p1, p2);
                OuterSpace.sounds.PlaySingle((OuterSpace.sounddir + "\\Missle.wav"));
            }

            public void move()
            {
                // Works out movement based on the angle of the missle
                if ((xy.X > 785 || xy.X < 15) || 
                    (xy.Y > 598 || xy.Y < 15))
                {
                    // out of bounds?
                    moving = false;
                    return;
                }

                xy.X = (int)(xy.X + OuterSpace.moveX[theta]);
                xy.Y = (int)(xy.Y + OuterSpace.moveY[theta]);
            }
        }
    }
}
