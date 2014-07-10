using System;
using System.Drawing;

namespace OuterSpace
{
    public static class MathFunctions
    {
        public struct location
        {
            public Single x;
            public Single y;
            public short angle;
        }

        public static location outOfBounds(Single x, Single y, Single xMin, Single yMin, Single xMax, 
            Single yMax, short angle, bool ScanNewT)
        {
            if (ScanNewT)
            {
                if (y < yMin)
                {
                    angle = (short)(360 - angle);
                    y = y * -1;
                }
                else if (y > yMax)
                {
                    y = y - (2 * (y - yMax));
                }
            }                
            else
            {
                if (y > yMax)
                {
                    angle = (short)(360 - angle);
                    y = y - (y - yMax);
                }
                
                if (y < yMin)
                {
                    angle = (short)(360 - angle);
                    y = yMin + yMin - y;
                }

                if (x > xMax)
                {
                    x = x - xMax + xMin;
                }
            }

            if (x < xMin)
            {
                x = xMax - Math.Abs(x - xMin);
            }

            location updatedLocation;
            updatedLocation.x = x;
            updatedLocation.y = y;
            updatedLocation.angle = angle;

            return updatedLocation;
        }

        public static int findDistance(Point p1, Point p2)
        {
            return Convert.ToInt32(Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));
        }

        public static Single scaleValue(Single x, Single maxvalue, Single minxvalue, Single maxreturnvalue, 
            Single minreturnvalue)
        {
            Single returnval;

            // Scales a value from a collection of values to a new value between minreturnvalue and maxreturnvalue. "Normalizes it"
            // Switches the scale of the value..
            returnval = (x - minxvalue) / (maxvalue - minxvalue); // find the percentage of the passed in value.
            returnval = ((maxreturnvalue - minreturnvalue) * returnval + minreturnvalue);

            return returnval;
        }

        public static int findAngle(Point p1, Point p2)
        {
            if (p1.X == p2.X)
            {
                if (p1.Y > p2.Y)
                    return 90;
                else if (p1.Y < p2.Y)
                    return 270; 
            }

            if (p1.Y == p2.Y)
            {
                if (p1.X > p2.X)
                    return 180;
                else if (p1.X < p2.X)
                    return 0;
            }
            
            Double ttheta; //Finds the closes integer angle in degress between two points
            // Calculate temporary theta in degrees.
            ttheta = ((600 - p2.Y) - (p1.Y)) / (p2.X - p1.X);
            ttheta = Math.Atan(ttheta) * (180 / Math.PI);
            // -------------------------------------

            // We are in quadrant 2 or 4 ------------
            if (ttheta < 0) //quadrant 4
            {
                if (((600 - p2.Y) - (p1.Y)) < 0)
                    ttheta = 360 + ttheta;
                else //quadrant 2
                    ttheta = 90 + (90 + ttheta);
            }
            // We are in quadrant 1 or 3 --------
            else
            {
                if (((600 - p2.Y) - (p1.Y)) < 0)
                    ttheta = 180 + ttheta; // quadrant 3

                // No modifications required for quadrant 1.. 
            }

            return Convert.ToInt32(ttheta);
        }
    }    
}