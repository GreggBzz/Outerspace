using System;
using System.Drawing;
using Microsoft.DirectX;

namespace OuterSpace
{
    class starmap
    {
        private Point[] mapArea = new System.Drawing.Point[2];
        private Microsoft.DirectX.Direct3D.Line aLine;
        private Vector2[] aLineVectors = new Vector2[2];
        private SpriteClass resizeButton;
        private bool resizing;
        private Color resizeButtonColor;
        private Random rnd = new Random();

        public starmap(float xScale, float yScale)
        {
            aLine = new Microsoft.DirectX.Direct3D.Line(OuterSpace.device);
            mapArea[1].Y = (int)(OuterSpace.ClientArea.Height * yScale);
            //Bottom
            mapArea[1].X = (int)OuterSpace.ClientArea.Width;
            //Right
            mapArea[0].Y = 0;
            //Top
            mapArea[0].X = (int)(OuterSpace.ClientArea.Width * xScale);
            //Left
            resizeButton = new SpriteClass("resizeButton.bmp", 35, 35, Convert.ToInt32(-16777216));
            resizeButton.sourceFrame[0] = new System.Drawing.Rectangle(0, 0, 35, 35);
            resizeButtonColor = Color.FromArgb(75, 255, 255, 255);
            resizing = false;
        }

        public void Run()
        {
            handleMouse();
            drawScaleLines();
            drawStars();
        }

        private void drawScaleLines()
        {
            int x;
            int x1;
            int y;
            int y1;

            //Draw the scale lines.
            //Verticle first, then horizontal.
            aLine.Width = 1;
            aLineVectors[0].X = mapArea[0].X;
            aLineVectors[0].Y = mapArea[0].Y;
            aLineVectors[1].Y = mapArea[1].Y;
            aLineVectors[1].X = mapArea[0].X;
            aLine.Draw(aLineVectors, Color.White);
           
            aLineVectors[0].Y = mapArea[1].Y;
            aLineVectors[1].X = mapArea[1].X;
            aLine.Draw(aLineVectors, Color.White);
           
            // Draw the horozontal hash marks
            for (y = 0; y <= 249; y += 50) 
            {
                aLineVectors[0].X = mapArea[0].X;
                aLineVectors[0].Y = MathFunctions.scaleValue(y, 250, 0, mapArea[1].Y, mapArea[0].Y);
                aLineVectors[1].Y = aLineVectors[0].Y;
                aLineVectors[1].X = mapArea[0].X + 20;
                aLine.Draw(aLineVectors, Color.Red);

                // Biggers ones every 50
                for (y1 = 0; y1 <= 50; y1 += 10) 
                {
                    aLineVectors[0].X = mapArea[0].X;
                    aLineVectors[0].Y = MathFunctions.scaleValue(y + y1, 250, 0, mapArea[1].Y, mapArea[0].Y);
                    aLineVectors[1].Y = aLineVectors[0].Y;
                    aLineVectors[1].X = mapArea[0].X + 9;
                    aLine.Draw(aLineVectors, Color.White);
                    //Smaller ones every 10
                }
            }
           
            // Draw the verticle hash marks
            for (x = 0; x <= 249; x += 50) 
            {
                aLineVectors[0].Y = mapArea[1].Y;
                aLineVectors[0].X = MathFunctions.scaleValue(x, 250, 0, mapArea[1].X, mapArea[0].X);
                aLineVectors[1].Y = mapArea[1].Y - 20;
                aLineVectors[1].X = aLineVectors[0].X;
                aLine.Draw(aLineVectors, Color.Red);

                for (x1 = 0; x1 <= 50; x1 += 10) 
                {
                    aLineVectors[0].Y = mapArea[1].Y;
                    aLineVectors[0].X = MathFunctions.scaleValue(x + x1, 250, 0, mapArea[1].X, mapArea[0].X);
                    aLineVectors[1].Y = mapArea[1].Y - 9;
                    aLineVectors[1].X = aLineVectors[0].X;
                    aLine.Draw(aLineVectors, Color.White);
                }
            }

            resizeButton.Draw(mapArea[0].X, mapArea[1].Y, resizeButton.sourceFrame[0], 0, resizeButtonColor);
        }

        private void drawStars()
        {
            int xPos;
            int yPos;
            Point starScreen = new Point();

            for (xPos = 0; xPos <= 250; xPos++) 
            {
                for (yPos = 0; yPos <= 250; yPos++) 
                {
                    if (OuterSpace.theVerse.HasStar(xPos, yPos))
                    {
                        CStar star = null;
                        OuterSpace.theVerse.GetStarAtLocation(xPos, yPos, ref star);
                        starScreen.X = (int)MathFunctions.scaleValue(xPos, 250, 0, mapArea[1].X, mapArea[0].X);
                        starScreen.Y = (int)MathFunctions.scaleValue(250 - yPos, 250, 0, mapArea[1].Y, mapArea[0].Y);
                        // Invert the yPos cause the screen corrdinates are top 0, bottom max.
                        OuterSpace.backstars.Draw(starScreen.X, starScreen.Y, OuterSpace.backstars.sourceFrame[OuterSpace.theVerse.GetTempType(star.SpectralClass)], 0,
                            Color.FromArgb(100 + (int)(rnd.NextDouble() * 150) + 1, 255, 255, 255));
                    }
                }
            }

            starScreen.X = (int)MathFunctions.scaleValue(OuterSpace.XCor, 250, 0, mapArea[1].X, mapArea[0].X);
            starScreen.Y = (int)MathFunctions.scaleValue(250 - OuterSpace.YCor, 250, 0, mapArea[1].Y, mapArea[0].Y);
           
            OuterSpace.smallship.Draw(starScreen.X - 8, starScreen.Y - 5, OuterSpace.smallship.sourceFrame[0], 
                OuterSpace.playership.theta, Color.FromArgb(90, 255, 255, 255));
           
        }

        private void resizeMap(System.Drawing.Point newMapArea)
        {
            mapArea[0].X = newMapArea.X;
            mapArea[1].Y = newMapArea.Y;
        }

        private void handleMouse()
        {
            Point pointingAt = new Point();
            Point resizeCenter = new Point();
            Point newMapArea = new Point();

            resizeCenter.Y = (int)mapArea[1].Y + resizeButton.sourceFrame[0].Height / 2;
            resizeCenter.X = (int)mapArea[0].X + resizeButton.sourceFrame[0].Width / 2;

            if (OuterSpace.userinput.mousexpos > mapArea[0].X & OuterSpace.userinput.mouseypos < mapArea[1].Y) 
            {
                pointingAt.X = (int) MathFunctions.scaleValue(OuterSpace.userinput.mousexpos, mapArea[1].X, mapArea[0].X, 250, 0);
                pointingAt.Y = (int) MathFunctions.scaleValue(mapArea[1].Y - OuterSpace.userinput.mouseypos, mapArea[1].Y, mapArea[0].Y, 250, 0);
                OuterSpace.textfont.DrawText(OuterSpace.userinput.mousexpos - 60, OuterSpace.userinput.mouseypos - 8, Color.LimeGreen, "X:" + pointingAt.X.ToString("0.0"));
                OuterSpace.textfont.DrawText(OuterSpace.userinput.mousexpos - 60, OuterSpace.userinput.mouseypos + 8, Color.LimeGreen, "Y:" + pointingAt.Y.ToString("0.0"));
            }

            pointingAt.X = OuterSpace.userinput.mousexpos;
            pointingAt.Y = OuterSpace.userinput.mouseypos;

            if (MathFunctions.findDistance(resizeCenter, pointingAt) > resizeButton.sourceFrame[0].Width / 2) 
            {
                resizeButtonColor = Color.FromArgb(75, 255, 255, 255);
            }
            else 
            {
                resizeButtonColor = Color.White;
                if (OuterSpace.userinput.CheckButton("Left", false)) 
                {
                    resizing = true;
                }
            }
           
            if (resizing) 
            {
                if (OuterSpace.userinput.CheckButton("Left", false)) 
                {
                    newMapArea.X = mapArea[0].X + OuterSpace.userinput.mousexpos - resizeCenter.X;
                    newMapArea.Y = mapArea[1].Y + OuterSpace.userinput.mouseypos - resizeCenter.Y;
                    resizeMap(newMapArea);
                    return;
                }
                else 
                {
                    resizing = false;
                }
            }
        }
    }
}