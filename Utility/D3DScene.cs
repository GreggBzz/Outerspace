using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class D3DSceneManager
    {
        // Class for weather, lightning and special effects while planetside.
        // Also, transforms the lights, I don't know I should probabbyl split this stuff up.
        // So we don't tranform the lights or the view more than once a frame.
        public bool transformViewOnFrame;
        public bool transformLightOnFrame;

        // Varibles for the cyclic day/night planetside cycles.
        private int lasttick;
        //private int dayflipper = 0;
        private Vector4 sunposition;
        //private int sunintensity = 0;
        private SpriteClass snowSprite;
        private SpriteClass rainSprite;
        private bool snowing;
        private bool raining;
        private int duration; // How long is it going to rain?
        private Random rnd = new Random();

        private class TransformationMatrices
        {
            public Matrix worldTransMtx;
            public Matrix viewTransMtx;
            public Matrix projectionTransMtx;
        }

        private class pricip
        {
            public Single X;
            public Single Y;
            public int Frame;
        }

        private pricip[] precipitation = new pricip[160];
        private TransformationMatrices Matrices = new TransformationMatrices();

        public D3DSceneManager()
        {            
            //dayflipper = 1;
            lasttick = Environment.TickCount / 3600;
            //sunintensity = 250;
            sunposition = new Vector4(-1500.0F, 0.0F, 500.0F, 0.0F);
            snowSprite = new SpriteClass("snow.bmp", 30, 30, Color.FromArgb(255,0,0,0).ToArgb());

            for (int i = 0; i < 8; i++)
            {
                Point pt = new Point((i % 4) * 10, (int)(i / 3) * 10);
                snowSprite.sourceFrame[i] = new Rectangle(pt.X, pt.Y, 10, 10);
            }

            rainSprite = new SpriteClass("rain.bmp", 10, 10, Color.FromArgb(255,0,0,0).ToArgb());
            rainSprite.sourceFrame[0] = new Rectangle(0, 0, 10, 10);

            for (int i = 0; i < 160; i++)
            {
                precipitation[i] = new pricip();
                precipitation[i].Frame = Convert.ToInt32((int)(8 * rnd.NextDouble()));
                precipitation[i].X = Convert.ToInt32((int)((OuterSpace.ClientArea.Width + 1) * rnd.NextDouble()));
                precipitation[i].Y = Convert.ToInt32((int)((OuterSpace.ClientArea.Height + 1) * rnd.NextDouble()));
            }
        }

        public void SetupLights()
        {
            if (transformLightOnFrame) return;

            // Turn off all the lights to start.
            for (int i = 0; i < 4; i++)
            {
                OuterSpace.device.Lights[i].Enabled = false;
            }

            // Set our material and it's colors. 
            Color col = Color.White; ;
            Direct3D.Material mtrl = new Direct3D.Material();

            mtrl.Diffuse = col;
            mtrl.Ambient = col;
            OuterSpace.device.Material = mtrl;

            Vector4 curSunPos = new Vector4();

            curSunPos = getsunpos();
            OuterSpace.device.Lights[0].Type = LightType.Directional;
            OuterSpace.device.Lights[0].Diffuse = Color.White; 
            OuterSpace.device.Lights[0].Direction = new Vector3(curSunPos.X, curSunPos.Y, curSunPos.Z + 1500.0F);
            OuterSpace.device.Lights[0].Enabled = true;
            OuterSpace.device.RenderState.Ambient = Color.FromArgb(65, 65, 65, 65);
            OuterSpace.msgbox.pushmsgs(Convert.ToString(curSunPos.Z));
            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed) && curSunPos.Z < 250)
            {
                OuterSpace.TV.lightson = 4;
                TVHeadlights();
            }

            transformLightOnFrame = true;
        }

        private void TVHeadlights()
        {
            Vector4 headLightDir = new Vector4(-100.0F, 1.0F, 0.0F, 0.0F);
            Vector4 headlightsource = new Vector4(0.0F, 0.0F, -2.2F, 0.0F);

            headLightDir.Transform(Matrix.RotationZ((float)(OuterSpace.TV.theta * Math.PI / 180.0F)));

            // Set up the headlight.
            for (int i = 0; i < OuterSpace.TV.lightson; i++)
            {
                OuterSpace.device.Lights[i].Type = LightType.Spot;
                OuterSpace.device.Lights[i].Position = new Vector3(headlightsource.X, headlightsource.Y, headlightsource.Z);
                OuterSpace.device.Lights[i].Direction = new Vector3(headLightDir.X, headLightDir.Y, headLightDir.Z);
                OuterSpace.device.Lights[i].InnerConeAngle = 1.0F;
                OuterSpace.device.Lights[i].OuterConeAngle = 1.2F;
                OuterSpace.device.Lights[i].Falloff = 1.0F;
                OuterSpace.device.Lights[i].Attenuation0 = 1.0F;
                OuterSpace.device.Lights[i].Diffuse = Color.FromArgb(255, 255, 255, 255);
                OuterSpace.device.Lights[i].Range = 800.0F;
                OuterSpace.device.Lights[i].Enabled = true;
            }
        }

        private Vector4 getsunpos()
        {
            // We want to rotate the sun around the planet every so many seconds.
            // Sorry Copernicus.

            // If we are not landed then return a sun position that's nice for light.
            if (!OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed)) 
            {
                return new Vector4(-500.0F, -100.0F, 500.0F, 0.0F);
            }

            int iTime = (int)(Environment.TickCount / 30.0F);
            sunposition = Vector4.Transform(sunposition, Matrix.RotationY((float)((lasttick - iTime) * Math.PI / 1000.0F)));
            lasttick = (int)(Environment.TickCount / 30.0F);

            return sunposition;
        }

        public void transformWorld(Matrix rotationX, Matrix rotationY, Matrix rotationZ, Matrix scaleXYZ, 
            Matrix translateXYZ)
        {
            Matrices.worldTransMtx = Matrix.Identity;

            if (!rotationZ.Equals(Matrix.Zero)) Matrices.worldTransMtx = Matrix.Multiply(Matrices.worldTransMtx, rotationZ);
            if (!rotationY.Equals(Matrix.Zero)) Matrices.worldTransMtx = Matrix.Multiply(Matrices.worldTransMtx, rotationY);
            if (!rotationX.Equals(Matrix.Zero)) Matrices.worldTransMtx = Matrix.Multiply(Matrices.worldTransMtx, rotationX);
            if (!scaleXYZ.Equals(Matrix.Zero)) Matrices.worldTransMtx = Matrix.Multiply(Matrices.worldTransMtx, scaleXYZ);
            if (!translateXYZ.Equals(Matrix.Zero)) Matrices.worldTransMtx = Matrix.Multiply(Matrices.worldTransMtx, translateXYZ);
        }

        public void transformView_Projection(Vector3 eyeVector, Vector3 lookVector, Vector3 upVector, 
            Single fovY, Single aspectRatio, Single zNear, Single zFar)
        {
            Matrices.viewTransMtx = Matrix.LookAtLH(eyeVector, lookVector, upVector);
            Matrices.projectionTransMtx = Matrix.PerspectiveFovLH(fovY, aspectRatio, zNear, zFar);
        }

        public void transformView_Projection()
        {
            if (transformViewOnFrame) return;

            Vector4 lookvect = new Vector4(0.0F, 0.0F, 0.0F, 0.0F);
            Vector4 eyepoint = new Vector4(0.0F, 0.0F, -200.0F, 0.0F);
            Vector3 upvect = new Vector3(0.0F, 1.0F, 0.0F);
            Vector4 lookvect_3d = new Vector4(-45.0F, 0.0F, 0.0F, 0.0F); // 3D MODE
            Vector4 eyepoint_3d = new Vector4(0.0F, 0.0F, -10.0F, 0.0F); // 3D MODE
            Vector3 upvect_3d = new Vector3(0.0F, 0.0F, -1.0F); // 3D MODE;

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed))
            {
                eyepoint.Z = -35;
            }
            else if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landing))
            {
                eyepoint.Z = -780;

                // If it was raining when we left planetside last, we want to reset that..
                duration = 0;
            }
            else if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.DockedAtStation))
            {
                eyepoint.Z = -97;
            }

            // //// Below, view martix, 3 verticies, (One: Eye Point, Two: Point we are looking at, Three: Which way is up?)
            //OuterSpace.device.Transform.View = Matrix.LookAtLH(New Vector3(eyepoint.X, eyepoint.Y, eyepoint.Z), New Vector3(lookvect.X, lookvect.Y, lookvect.Z), upvect)
            // For the projection matrix, we set up a perspective transform (which
            // transforms geometry from 3D view space to 2D viewport space, with            
            // a perspective divide making objects smaller in the distance). To build
            // a perpsective transform, we need the field of view (1/4 pi is common),
            // the aspect ratio, and the near and far clipping planes (which define at
            // what distances geometry should be no longer be rendered).
            //OuterSpace.device.Transform.Projection = Matrix.PerspectiveFovLH(CSng(Math.PI) / 4, OuterSpace.clientarea.cx / OuterSpace.clientarea.cy, -75.0F, 0.0F)

            transformView_Projection(new Vector3(eyepoint.X, eyepoint.Y, eyepoint.Z), 
                new Vector3(lookvect.X, lookvect.Y, lookvect.Z), upvect, 
                Convert.ToSingle(Math.PI / 4), 
                ((float)OuterSpace.ClientArea.Width / (float)OuterSpace.ClientArea.Height), 
                -75.0F, 0.0F);

            transformViewOnFrame = true;
        }

        public void transform_Pipeline()
        {
            if (!Matrices.worldTransMtx.Equals(Matrix.Zero)) OuterSpace.device.Transform.World = Matrices.worldTransMtx;
            if (!Matrices.viewTransMtx.Equals(Matrix.Zero)) OuterSpace.device.Transform.View = Matrices.viewTransMtx;
            if (!Matrices.projectionTransMtx.Equals(Matrix.Zero)) OuterSpace.device.Transform.Projection = Matrices.projectionTransMtx;
        }

        private void adjustWeather()
        {
            // There are 3 types of snow/rain
            // Inert(Water Planet - Ice Planet)/Acid-Molten(Hot Planet)/Slime(Gas Giant)
            // Inert is white, acid is yellow, slime is green or blue
            // Molten planets don't have precipitaion for now, thankfully, it won't be raining lava.
            Single chanceofrain;

            Random newRnd = new Random();

            if (OuterSpace.thisPlanet.Weather == 0)  return;

            if (raining || snowing)
            {
                if (duration > 0) duration = duration - 1;
                if (duration == 0) 
                {
                    raining = false;
                    snowing = false;
                }
            }
            else
            {
                chanceofrain = (float)(newRnd.NextDouble() * 7500.1F);
                if ((int)chanceofrain > 7499 - OuterSpace.thisPlanet.Weather)
                {
                    //The above formula is derived from (Roughly)
                    //Planet Weather Type    Rain every n Seconds    Rain every n Ticks
                    //5                      25                      1500
                    //4                      50                      3000
                    //3                      75                      4500
                    //2                      100                     5000
                    //1                      125                     7500
                    //We therefore make a random number between 1 and 7500 and test if our number is 
                    //greater then the # of ticks between rain/norain
                    if (newRnd.NextDouble() > 0.6F)
                    {
                        OuterSpace.msgbox.pushmsgs("It is raining..");
                        raining = true;
                        snowing = false;
                        chanceofrain = 0.0F;
                    }
                    else
                    {
                        OuterSpace.msgbox.pushmsgs("It is snowing..");
                        snowing = true;
                        raining = false;
                        chanceofrain = 0.0F;
                    }

                    duration = 300 + (int)(newRnd.NextDouble() * (1000 * OuterSpace.thisPlanet.Weather));
                }
            }
        }

        public void nowForTheWeather()
        {
            adjustWeather();

            if (snowing) 
                precipitate("Snow");
            else if (raining) 
                precipitate("Rain");
        }

        private void precipitate(string type)
        {
            int amount;
            int fallspeed;

            amount = 145;
            if (type == "Snow") fallspeed = 2; else fallspeed = 4;

            OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

            if (duration < 160)
            {
                amount = duration;
                duration = duration - 1;
            }
            
            for (int i = 0; i < amount; i++)
            {
                precipitation[i].X = (float)(precipitation[i].X + (OuterSpace.moveX[102] * rnd.NextDouble() * fallspeed) + 0.5F * OuterSpace.moveX[360 - OuterSpace.TV.theta]);
                precipitation[i].Y = (float)(precipitation[i].Y + (OuterSpace.moveY[102] * rnd.NextDouble() * fallspeed) + 0.5F * OuterSpace.moveY[360 - OuterSpace.TV.theta]);
  
                if (precipitation[i].X > OuterSpace.ClientArea.Width - 10)
                {
                    precipitation[i].X = 0;
                    precipitation[i].Frame = Convert.ToInt32(5 * rnd.NextDouble());
                }
                
                if (precipitation[i].X < 0)
                {
                    precipitation[i].X = OuterSpace.ClientArea.Width - 10;
                    precipitation[i].Frame = Convert.ToInt32(5 * rnd.NextDouble());
                }

                if (precipitation[i].Y > OuterSpace.ClientArea.Height - 10)
                {
                    precipitation[i].Y = 0;
                    precipitation[i].X = Convert.ToInt32(801 * rnd.NextDouble());
                    precipitation[i].Frame = Convert.ToInt32(5 * rnd.NextDouble());
                }

                if (precipitation[i].Y < 0)
                {
                    precipitation[i].Y = OuterSpace.ClientArea.Height - 10;
                    precipitation[i].Frame = Convert.ToInt32(5 * rnd.NextDouble());
                }

                if (type == "Snow")
                    snowSprite.Draw((int)precipitation[i].X, (int)precipitation[i].Y, snowSprite.sourceFrame[precipitation[i].Frame], 0, Color.FromArgb(150, 255, 255, 255));
                else
                    rainSprite.Draw((int)precipitation[i].X, (int)precipitation[i].Y, rainSprite.sourceFrame[0], 0, Color.FromArgb(150, 255, 255, 255));
            }

            OuterSpace.spriteobj.Flush();
            OuterSpace.spriteobj.End();
        }
    }
}
