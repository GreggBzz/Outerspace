// Generates a planet mesh by bisecting the veticies of a geosphere, and extending another ray from the center through 
// the bisected midpoint. That ray is then extended by a (repeatable) random amount based on a fractal seed. 
// The geosphere is then tesselated and reconstructed with an order of magnitude more triangles. If we repeat the tesselation process
// 4 or 5 times, we end up with a nice planet like sphere mesh. 
// 
// The entry point of this class, is really the tesselate . Start there, and you should see what it does..
// The original verticies and triangles are specified rather like a large Dungeons&Dragons 20 sided die I bought for a visual refrence,
// before the initial round of tesselation and fractal deformation. 

using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class Geosphere
    {
        #region CONTAINED STRUCTURES
        public struct vertex_structure
        {
            public Single X;
            public Single Y;
            public Single Z;
            public Single color;
            public Single edgeLength;
            public Single elevation;
            public Single texcor;
            public Vector3 normal;
        }
        public struct triangles_structure
        {
            public vertex_structure A;
            public vertex_structure ma;
            public vertex_structure B;
            public vertex_structure mb;
            public vertex_structure C;
            public vertex_structure mc;
        }      
        private struct zobject
        {
            public bool mineral;
            public bool ship;
            //Pointers back to the lifemap index values.
            //So we can fetch the life forms characteristics out of
            //the big bad lifeform map array.
        }

        private struct lifeptr
        {
            public short xptr;
            public short yptr;
        }
        #endregion
        
        #region FLAT MAP VARIABLES
        public vertex_structure[,] map = new vertex_structure[1120, 560]; // The map of the whole planet.
        private vertex_structure[,] zmap = new vertex_structure[160, 160];  // The viewable map while moving in our terrian vehicle.
        private zobject[,] zobjectmap = new zobject[160, 160];  //Objects on the zoomed in map, viewable by the player.
        public int zmapsdim = 60; // The size of our zoomed in map...
        private lifeptr[] zlifemap = new lifeptr[100]; //Life objects on the zoomed in map.
        public Single texCorref;
        public Single texCormax;
        public Single texCormin;
        public Single flatmapXmax = -999;
        public Single flatmapXmin = 999;
        public Single flatmapYmax = -999;
        public Single flatmapYmin = 999;
        //some special exceptions during scanning new terrian.
        public static bool OutOnX; //if we have wraped around the planet, we need to make        
        #endregion

        #region GEOSPHERE ROUND MESH VARIABLES
        private Random rnd;   
        public vertex_structure[] verticies = new vertex_structure[12];
        public triangles_structure[] triangles = new triangles_structure[32400];
        public triangles_structure[] temptriangles = new triangles_structure[32400];
        public int maxtriangles;
        public Single radius;
        public vertex_structure[] oldMidpoints = new vertex_structure[11000];
        public vertex_structure[] newMidpoints = new vertex_structure[11000];
        public static Single maxElev;
        public static Single minElev;
        public int displaceMag; // (25 - 50) Determins the height of the initial fractal displacement.
        public int roughness;  // (Int(displaceMag / iterations) to 1) Determins the change in displacement after each iteration.
        // Effects the randomness of the terrian.
        public static Single planetScaler; // A divisor that determins the size of the planet, the larger, the smaller the planet
        public static Single seed; //Our planet seed
        public static Single iceCapRange; // The ABS(y) value that determines where to draw icecaps. Set to > radius for no ice caps.
        public Single edgeLength;
        #endregion

        public Geosphere(Single scale, Single sd, int rough, int displace, int icecap)
        {
            // Set up our inital geosphere.
            rnd = new Random((int)sd);
            iceCapRange = icecap;
            planetScaler = scale;
            displaceMag = displace;
            roughness = rough;
            seed = sd;
           
            int i;
            for (i = 0; i < 12; i++)
                verticies[i] = new vertex_structure();

            for (i = 0; i < 11000; i++)
            {
                oldMidpoints[i] = new vertex_structure();
                newMidpoints[i] = new vertex_structure();
            }

            for (i = 0; i < 32400; i++)
            {
                triangles[i] = new triangles_structure();
            }

            // The initial coorodinates of our geosphere. Prob could have done one real time.
            // Drew it in milkshap rather and copied the corodinates.
            #region INITAL COORDINATES
            verticies[0].X = -26.2865543F / planetScaler;
            verticies[0].Y = 0.0F / planetScaler;
            verticies[0].Z = -42.53254F / planetScaler;
            verticies[1].X = 26.2865543F / planetScaler;
            verticies[1].Y = 0.0F / planetScaler;
            verticies[1].Z = -42.53254F / planetScaler;
            verticies[2].X = 0.0F / planetScaler;
            verticies[2].Y = -42.53254F / planetScaler;
            verticies[2].Z = -26.2865543F / planetScaler;
            verticies[3].X = -42.53254F / planetScaler;
            verticies[3].Y = -26.2865543F / planetScaler;
            verticies[3].Z = 0.0F / planetScaler;
            verticies[4].X = 0.0F / planetScaler;
            verticies[4].Y = -42.53254F / planetScaler;
            verticies[4].Z = 26.2865543F / planetScaler;
            verticies[5].X = 42.53254F / planetScaler;
            verticies[5].Y = -26.2865543F / planetScaler;
            verticies[5].Z = 0.0F / planetScaler;
            verticies[6].X = 42.53254F / planetScaler;
            verticies[6].Y = 26.2865543F / planetScaler;
            verticies[6].Z = 0.0F / planetScaler;
            verticies[7].X = 26.2865543F / planetScaler;
            verticies[7].Y = 0.0F / planetScaler;
            verticies[7].Z = 42.53254F / planetScaler;
            verticies[8].X = -26.2865543F / planetScaler;
            verticies[8].Y = 0.0F / planetScaler;
            verticies[8].Z = 42.53254F / planetScaler;
            verticies[9].X = 0.0F / planetScaler;
            verticies[9].Y = 42.53254F / planetScaler;
            verticies[9].Z = 26.2865543F / planetScaler;
            verticies[10].X = 0.0F / planetScaler;
            verticies[10].Y = 42.53254F / planetScaler;
            verticies[10].Z = -26.2865543F / planetScaler;
            verticies[11].X = -42.53254F / planetScaler;
            verticies[11].Y = 26.2865543F / planetScaler;
            verticies[11].Z = 0.0F / planetScaler;

            // Assign each triangle 3 verticies.
            triangles[0].A = verticies[0]; 
            triangles[0].B = verticies[2];
            triangles[0].C = verticies[1];
            triangles[1].A = verticies[9];
            triangles[1].B = verticies[8];
            triangles[1].C = verticies[11];
            triangles[2].A = verticies[5];
            triangles[2].B = verticies[2];
            triangles[2].C = verticies[4];
            triangles[3].A = verticies[10];
            triangles[3].B = verticies[6];
            triangles[3].C = verticies[9];
            triangles[4].A = verticies[11];
            triangles[4].B = verticies[0];
            triangles[4].C = verticies[10];
            triangles[5].A = verticies[6];
            triangles[5].B = verticies[5];
            triangles[5].C = verticies[7];
            triangles[6].A = verticies[0];
            triangles[6].B = verticies[3];
            triangles[6].C = verticies[2];
            triangles[7].A = verticies[7];
            triangles[7].B = verticies[4];
            triangles[7].C = verticies[8];
            triangles[8].A = verticies[6];
            triangles[8].B = verticies[1];
            triangles[8].C = verticies[5];
            triangles[9].A = verticies[8];
            triangles[9].B = verticies[4];
            triangles[9].C = verticies[3];
            triangles[10].A = verticies[10];
            triangles[10].B = verticies[1];
            triangles[10].C = verticies[6];
            triangles[11].A = verticies[8];
            triangles[11].B = verticies[3];
            triangles[11].C = verticies[11];
            triangles[12].A = verticies[10];
            triangles[12].B = verticies[0];
            triangles[12].C = verticies[1];
            triangles[13].A = verticies[6];
            triangles[13].B = verticies[7];
            triangles[13].C = verticies[9];
            triangles[14].A = verticies[11];
            triangles[14].B = verticies[3];
            triangles[14].C = verticies[0];
            triangles[15].A = verticies[7];
            triangles[15].B = verticies[5];
            triangles[15].C = verticies[4];
            triangles[16].A = verticies[3];
            triangles[16].B = verticies[4];
            triangles[16].C = verticies[2];
            triangles[17].A = verticies[9];
            triangles[17].B = verticies[11];
            triangles[17].C = verticies[10];
            triangles[18].A = verticies[1];
            triangles[18].B = verticies[2];
            triangles[18].C = verticies[5];
            triangles[19].A = verticies[9];
            triangles[19].B = verticies[7];
            triangles[19].C = verticies[8];
            #endregion
            maxtriangles = 20;
            // We start with 20 triangles / 11 verticies
            maxElev = 50.0F / planetScaler;
            minElev = 50.0F / planetScaler;
            radius = 50.0F / planetScaler;

            for (i = 0; i < 11000; i++)
            {
                oldMidpoints[i].X = 99;
                newMidpoints[i].X = 98;
            }
        }

        public void tesselate(int numTriags, bool finalround)
        {
            int i = 0;
            int i2 = 0;
            //displace all the midpoints,
            displaceMidPoints(numTriags);

            //reconnect the displaced midpoints to make new triangles, going clockwise on the front side.
            for (i = 0; i <= ((numTriags / 4) - 1); i++)
            {
                temptriangles[i2].A = triangles[i].A;
                temptriangles[i2].B = triangles[i].mb;
                temptriangles[i2].C = triangles[i].ma;
                i2 = i2 + 1;
                temptriangles[i2].A = triangles[i].ma;
                temptriangles[i2].B = triangles[i].mb;
                temptriangles[i2].C = triangles[i].mc;
                i2 = i2 + 1;
                temptriangles[i2].A = triangles[i].mb;
                temptriangles[i2].B = triangles[i].B;
                temptriangles[i2].C = triangles[i].mc;
                i2 = i2 + 1;
                temptriangles[i2].A = triangles[i].ma;
                temptriangles[i2].B = triangles[i].mc;
                temptriangles[i2].C = triangles[i].C;
                i2 = i2 + 1;
            }

            maxtriangles = i2;

            for (i = 0; i <= maxtriangles; i++)
            {
                triangles[i] = temptriangles[i];
            }

            // Reset the midpoints for the } round of tesselation.
            for (i = 0; i < 11000; i++)
            {
                oldMidpoints[i].X = 99;
                newMidpoints[i].X = 98;
            }

            // if we//ve done 5 rounds of tesselation, get the colors for each vertice.
            if (finalround)
            {
                getColors();
            }
        }

        private void displaceMidPoints(int numTriags)
        {
            displaceMag = displaceMag - roughness;
            //Effectively reduce our range of possible displacement mangnitude.

            //Walk through the triangle array.
            if (numTriags == 80)
            {
                //if this is the FIRST round of tesselation, we need to displace the initial "perfect"
                // verticies on our geosphere for a more natural look.
                for (int i = 0; i <= 19; i++)
                {
                    triangles[i].A = doOneMidpoint(triangles[i].A); 
                    triangles[i].B = doOneMidpoint(triangles[i].B);
                    triangles[i].C = doOneMidpoint(triangles[i].C);
                }
            }

            for (int i = 0; i <= ((numTriags / 4) - 1); i++)
            {
                triangles[i].ma = displacemidpoint(triangles[i].A, triangles[i].C);
                triangles[i].mb = displacemidpoint(triangles[i].A, triangles[i].B);
                triangles[i].mc = displacemidpoint(triangles[i].B, triangles[i].C);
            }
        }

        private vertex_structure displacemidpoint(vertex_structure vert1, vertex_structure vert2)
        {
            vertex_structure midpoint;
            double rApprox1 = 0.0F;
            double rApprox2 = 0.0F;


            rApprox1 = Math.Sqrt((vert1.X * vert1.X) + (vert1.Y * vert1.Y) + (vert1.Z * vert1.Z));
            rApprox2 = Math.Sqrt((vert2.X * vert2.X) + (vert2.Y * vert2.Y) + (vert2.Z * vert2.Z));
            radius = Convert.ToSingle((rApprox1 + rApprox2) / 2.0F);

            //Fudge on the radius retrieval a little bit by averaging the distance from 0,0,0 of the two passed-in points.

            midpoint = findmidpoint(vert1, vert2);
            // first, find the midpoint.
            return doOneMidpoint(midpoint);
            // Pass it in to be fractally displaced.
        }

        // Now, make sure we don't do the same midpoint twice below.
        private vertex_structure doOneMidpoint(vertex_structure midpoint)
        {
            Single lengMult; //Length scaler to ext} the vert to the outer imaginary sphere around the geosphere.
            int c = 0;
            double midpnt = 0.0;
            while (c != 10999)
            {
                if (oldMidpoints[c].X == 99)
                {
                    //We've searched the array, we have not done this midpoint before.
                    oldMidpoints[c] = midpoint;
                    //Set oldmidpoint to the un-displaced midpoint for future refs
                    midpnt = Math.Sqrt(Math.Pow(midpoint.X, 2.0F) + Math.Pow(midpoint.Y, 2.0F) + Math.Pow(midpoint.Z, 2.0F));
                    midpoint.elevation = (float)midpnt;
                    // Calculate the distance of this midpoint from 0,0,0

                    lengMult = (radius / midpoint.elevation);
                    // Calculate a multiplier, to ext} the midpoint to the edge of the sphere.
                    if (rnd.NextDouble() < 0.5F || Math.Abs(midpoint.Y) > iceCapRange)
                    
                    {
                        midpoint.X = (midpoint.X * lengMult); // Ext} the ray to edge of invisible sphere
                        midpoint.Y = (midpoint.Y * lengMult);
                        midpoint.Z = (midpoint.Z * lengMult);

                        lengMult = (Convert.ToSingle(displaceMag) / 1200.0F * Convert.ToSingle(rnd.NextDouble())) + 1.0F;
                        //lengMult = 1
                        midpoint.X = (midpoint.X * lengMult); // Ext} a bit fractally.
                        midpoint.Y = (midpoint.Y * lengMult);
                        midpoint.Z = (midpoint.Z * lengMult);
                    }
                    else
                    {
                        midpoint.X = (midpoint.X * lengMult); // Ext} the ray to edge of invisible sphere
                        midpoint.Y = (midpoint.Y * lengMult);
                        midpoint.Z = (midpoint.Z * lengMult);
                        lengMult = 1.0F - (Convert.ToSingle(displaceMag) / 1200.0F * Convert.ToSingle(rnd.NextDouble()));
                        //lengMult = 1
                        midpoint.X = (midpoint.X * lengMult);  // Shrink a bit fractally
                        midpoint.Y = (midpoint.Y * lengMult);
                        midpoint.Z = (midpoint.Z * lengMult);
                    }

                    midpnt = Math.Sqrt(Math.Pow(midpoint.X, 2.0F) + Math.Pow(midpoint.Y, 2.0F) + Math.Pow(midpoint.Z, 2.0F));
                    midpoint.elevation = (float)midpnt;
                    // test for our maximum and minimum distances from 0,0,0.
                    if (midpoint.elevation > maxElev)
                        maxElev = midpoint.elevation;

                    if (midpoint.elevation < minElev)
                        minElev = midpoint.elevation;

                    newMidpoints[c] = midpoint; //Set the newmidpoint to the newly displaced midpoint.

                    return midpoint;
                }

                if ((oldMidpoints[c].X == midpoint.X) && (oldMidpoints[c].Y == midpoint.Y) &&
                    (oldMidpoints[c].Z == midpoint.Z))
                {
                    //We may have done this midpoint before
                    if ((oldMidpoints[c].X == newMidpoints[c].X) && (oldMidpoints[c].Y == newMidpoints[c].Y) &&
                        (oldMidpoints[c].Z == newMidpoints[c].Z))
                    {
                        //Nope, we have not done this midpoint before, we need to update newmidpoints(c)
                        // with a newly calculated value.

                        midpnt = Math.Sqrt(Math.Pow(midpoint.X, 2.0F) + Math.Pow(midpoint.Y, 2.0F) + Math.Pow(midpoint.Z, 2.0F));
                        midpoint.elevation = (float)midpnt;
                        lengMult = (radius / midpoint.elevation);
                        if (rnd.NextDouble() < 0.5F || Math.Abs(midpoint.Y) > iceCapRange)
                        {
                            midpoint.X = (midpoint.X * lengMult);
                            midpoint.Y = (midpoint.Y * lengMult);
                            midpoint.Z = (midpoint.Z * lengMult);
                            lengMult = (Convert.ToSingle(displaceMag) / 1200.0F * Convert.ToSingle(rnd.NextDouble())) + 1.0F;
                            midpoint.X = (midpoint.X * lengMult);
                            midpoint.Y = (midpoint.Y * lengMult);
                            midpoint.Z = (midpoint.Z * lengMult);
                        }
                        else
                        {
                            midpoint.X = (midpoint.X * lengMult);
                            midpoint.Y = (midpoint.Y * lengMult);
                            midpoint.Z = (midpoint.Z * lengMult);
                            lengMult = 1.0F - (Convert.ToSingle(displaceMag) / 1200.0F * Convert.ToSingle(rnd.NextDouble()));
                            midpoint.X = (midpoint.X * lengMult);
                            midpoint.Y = (midpoint.Y * lengMult);
                            midpoint.Z = (midpoint.Z * lengMult);
                        }

                        midpnt = Math.Sqrt(Math.Pow(midpoint.X, 2.0F) + Math.Pow(midpoint.Y, 2.0F) + Math.Pow(midpoint.Z, 2.0F));
                        midpoint.elevation = (float)midpnt;
                        newMidpoints[c] = midpoint;

                        if (midpoint.elevation > maxElev)
                            maxElev = midpoint.elevation;

                        if (midpoint.elevation < minElev)
                            minElev = midpoint.elevation;

                        return midpoint;
                    }
                    else
                    {
                        return newMidpoints[c];
                        // We//ve done this midpoint before, simply return previously updated value.
                    }
                }

                c++;
            }
            return newMidpoints[1];
            // We failed every condition above. Return something!
        }

        private vertex_structure findmidpoint(vertex_structure vert1, vertex_structure vert2)
        {
            vertex_structure tempvert = new vertex_structure();

            tempvert.X = (vert1.X + vert2.X) / 2.0F;
            tempvert.Y = (vert1.Y + vert2.Y) / 2.0F;
            tempvert.Z = (vert1.Z + vert2.Z) / 2.0F; // Find the midpoint given our two verticies

            return tempvert;
        }

        public void getColors() //A broken texture mapping method. It's documentation, I have on my haddrive.
        {
            double pointAngle = 0.0F;
            Single sPI = (float)Math.PI;
            for (int i = 0; i <= maxtriangles; i++)
            {
                pointAngle = Math.Atan(triangles[i].A.X / triangles[i].A.Z);
                pointAngle = pointAngle + (0.5F * Math.PI);
                triangles[i].A.texcor = (float)Math.Sin(MathFunctions.scaleValue((float)pointAngle, sPI, 0.0F, 7.0F * sPI, 0.0F));

                pointAngle = Math.Atan(triangles[i].B.X / triangles[i].B.Z);
                pointAngle = pointAngle + (0.5F * Math.PI);
                triangles[i].B.texcor = (float)Math.Sin(MathFunctions.scaleValue((float)pointAngle, sPI, 0.0F, 7.0F * sPI, 0.0F));

                pointAngle = Math.Atan(triangles[i].C.X / triangles[i].C.Z);
                pointAngle = pointAngle + (0.5F * Math.PI);
                triangles[i].C.texcor = (float)Math.Sin(MathFunctions.scaleValue((float)pointAngle, sPI, 0.0F, 7.0F * sPI, 0.0F));
            }
                for (int i = 0; i <= maxtriangles; i++)
            {
                triangles[i].A.color = MathFunctions.scaleValue(triangles[i].A.elevation, maxElev, minElev, 1.0F, 0.0F);
                triangles[i].B.color = MathFunctions.scaleValue(triangles[i].B.elevation, maxElev, minElev, 1.0F, 0.0F);
                triangles[i].C.color = MathFunctions.scaleValue(triangles[i].C.elevation, maxElev, minElev, 1.0F, 0.0F);

                triangles[i].A.normal = (new Vector3(triangles[i].A.X / triangles[i].A.elevation,
                                    triangles[i].A.Y / triangles[i].A.elevation,
                                    triangles[i].A.Z / triangles[i].A.elevation));
                triangles[i].B.normal = (new Vector3(triangles[i].B.X / triangles[i].B.elevation,
                                    triangles[i].B.Y / triangles[i].B.elevation,
                                    triangles[i].B.Z / triangles[i].B.elevation));
                triangles[i].C.normal = (new Vector3(triangles[i].C.X / triangles[i].C.elevation,
                                    triangles[i].C.Y / triangles[i].C.elevation,
                                    triangles[i].C.Z / triangles[i].C.elevation));
            }
        }

        public void Flatten()
        {
            // Flattens the spherical triangle list, into a rectangle for viewing.
            for (int i = 0; i <= maxtriangles; i++)
            {
                if (((triangles[i].A.Z + triangles[i].B.Z + triangles[i].C.Z) / 3) < 0)
                {
                    // if the distance from the middle of the sphere is less  0,  we have
                    // Back side triangles.
                    // Call various s to alter the points.
                    triangles[i].A = normalize(triangles[i].A);
                    triangles[i].B = normalize(triangles[i].B);
                    triangles[i].C = normalize(triangles[i].C);
                    triangles[i].A = flattenvert(triangles[i].A);
                    triangles[i].B = flattenvert(triangles[i].B);
                    triangles[i].C = flattenvert(triangles[i].C);
                    triangles[i].A = rotatevertZ(triangles[i].A, 45);
                    triangles[i].B = rotatevertZ(triangles[i].B, 45);
                    triangles[i].C = rotatevertZ(triangles[i].C, 45);
                    // Center each vertex by moving it down 35.3....
                    triangles[i].A.X = triangles[i].A.X - 35.3555F;
                    triangles[i].B.X = triangles[i].B.X - 35.3555F;
                    triangles[i].C.X = triangles[i].C.X - 35.3555F;
                }
                else /////// Front side triangles
                {
                    triangles[i].A = normalize(triangles[i].A);
                    triangles[i].B = normalize(triangles[i].B);
                    triangles[i].C = normalize(triangles[i].C);
                    triangles[i].A = flattenvert(triangles[i].A);
                    triangles[i].B = flattenvert(triangles[i].B);
                    triangles[i].C = flattenvert(triangles[i].C);
                    // Bring the vertexes of each triangle to the other side of the screen by multiplying the X cor by -1.
                    triangles[i].A.X = triangles[i].A.X * -1;
                    triangles[i].B.X = triangles[i].B.X * -1;
                    triangles[i].C.X = triangles[i].C.X * -1;
                    triangles[i].A = rotatevertZ(triangles[i].A, -45);
                    triangles[i].B = rotatevertZ(triangles[i].B, -45);
                    triangles[i].C = rotatevertZ(triangles[i].C, -45);
                    //Center each vertex by moving it up 35.3......
                    triangles[i].A.X = triangles[i].A.X + 35.3555F;
                    triangles[i].B.X = triangles[i].B.X + 35.3555F;
                    triangles[i].C.X = triangles[i].C.X + 35.3555F;
                }
            }

            // Set the normals so they are all facing the viewpoint.
            for (int i = 0; i <= maxtriangles; i++)
            {
                triangles[i].A.normal = new Vector3(0, 0, -1);
                triangles[i].B.normal = new Vector3(0, 0, -1);
                triangles[i].C.normal = new Vector3(0, 0, -1);
            }

            maperize(maxtriangles);
        }

        private void maperize(int Maxtraingles)
        {
            Single sumcount;
            Single cellsum;
            //int lastnotnull;
            vertex_structure[] allpoints = new vertex_structure[61440];
            int i, iY, iX;
            int tX;
            int ty;
            int txmid;
            int tymid;
            int txmidc;
            int tymidc;
            int deresoluter = 4; // Factor that we want to deresolute the master map by to get the mini-map.
            /////// Initialize the arrays
            int mapsizex = 140 * deresoluter - 1;
            int mapsizey = 70 * deresoluter - 1;

            Single maxXvert = 70.71051F;
            Single maxYvert = 35.35536F;

            for (i = 0; i < 61440; i++)
                allpoints[i].color = 0.0F;

            for (iY = 0; iY <= mapsizey; iY++)
            {
                for (iX = 0; iX <= mapsizex; iX++)
                {
                    map[iX, iY].color = 0.0F;
                    map[iX, iY].elevation = 0; //Well use the elevation member of the vertex_structure as a counter,
                    // count the number of vertecies that have been added to this array element.
                }
            }

            iX = 0;

            // Stuff all vertcies into a one dimensional array
            for (i = 0; i < maxtriangles; i++)
            {
                allpoints[iX] = triangles[i].A;
                allpoints[iX + 1] = triangles[i].B;
                allpoints[iX + 2] = triangles[i].C;
                iX = iX + 3;
            }

            // Figure out the position of the current verticie on our map, and pop it//s elevation into that index.
            // Summing for each vertex popped into each element of the map array and using a counter to count how many
            // we've popped in there. Later well average the elevation of the midpoint by dividing by that counter.
            // First for/} loop, do all points of each triangle.
            // Second for/} Loop, do all the midpoints on each triangle.
            for (i = 0; i < (maxtriangles * 3); i++)
            {
                ///// //Scaled from 0 to 1 and multiplied by the appropriate dimension of the map array, and take Convert.ToInt32
                tX = Convert.ToInt32(MathFunctions.scaleValue(allpoints[i].X, maxXvert, -maxXvert, 1.0F, 0.0F) * mapsizex);
                ty = Convert.ToInt32(MathFunctions.scaleValue(allpoints[i].Y, maxYvert, -maxYvert, 1.0F, 0.0F) * mapsizey);
                ///// sum the total value in each position.
                map[tX, ty].color = map[tX, ty].color + allpoints[i].color;
                ///// number of verticies that are in this element.
                map[tX, ty].elevation = map[tX, ty].elevation + 1;
            }

            for (i = 0; i < (maxtriangles * 3); i += 3)
            {
                ///// //Scaled from 0 to 1 and multiplied by the appropriate dimension of the map array, and take Convert.ToInt32
                tX = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i].X + allpoints[i + 1].X) / 2.0F, maxXvert, -maxXvert, 1.0F, 0.0F) * mapsizex);
                ty = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i].Y + allpoints[i + 1].Y) / 2.0F, maxYvert, -maxYvert, 1.0F, 0.0F) * mapsizey);
                txmid = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i + 1].X + allpoints[i + 2].X) / 2.0F, maxXvert, -maxXvert, 1.0F, 0.0F) * mapsizex);
                tymid = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i + 1].Y + allpoints[i + 2].Y) / 2.0F, maxYvert, -maxYvert, 1.0F, 0.0F) * mapsizey);
                txmidc = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i].X + allpoints[i + 2].X) / 2.0F, maxXvert, -maxXvert, 1.0F, 0.0F) * mapsizex);
                tymidc = Convert.ToInt32(MathFunctions.scaleValue((allpoints[i].Y + allpoints[i + 2].Y) / 2.0F, maxYvert, -maxYvert, 1.0F, 0.0F) * mapsizey);

                ///// sum the total value in each position.
                map[tX, ty].color = map[tX, ty].color + (allpoints[i].color + allpoints[i + 1].color) / 2.0F;
                map[txmid, tymid].color = map[txmid, tymid].color + (allpoints[i + 1].color + allpoints[i + 2].color) / 2.0F;
                map[txmidc, tymidc].color = map[txmidc, tymidc].color + (allpoints[i].color + allpoints[i + 2].color) / 2.0F;
                ///// number of verticies that are in this element.
                map[tX, ty].elevation = map[tX, ty].elevation + 1;
                map[txmid, tymid].elevation = map[txmid, tymid].elevation + 1;
                map[txmidc, tymidc].elevation = map[txmidc, tymidc].elevation + 1;
            }

            ////// Center the map and figure out the color of each elevation point.
            for (iY = 0; iY <= mapsizey; iY++)
            {
                for (iX = 0; iX <= mapsizex; iX++)
                {
                    ////centering
                    map[iX, iY].X = iX - mapsizex / 2;
                    map[iX, iY].Y = iY - mapsizey / 2;
                    map[iX, iY].Z = 0.0F;

                    if (map[iX, iY].color > 0.0)
                    {
                        // if more  one vertex has been stuffed into this element, average them.
                        map[iX, iY].color = map[iX, iY].color / map[iX, iY].elevation;
                    }
                }
            }

            ///// Fill in all the empty cells (in the middle of two filled cells) with the average of the adjacent filled cells.
            ///// Fill in the blanks basically.

      
 
     int left;
     int right;
     for (iY = 0; iY <= mapsizey; iY++) {
         iX = 1;
         while (iX < mapsizex + 1) {
             cellsum = 0.0F;
             sumcount = 0.0F;
             if (map[iX, iY].color == 0.0F) {
                 left = iX;
                 right = iX;
                 // move to the right until we reach a filled cell or the "edge"
                 while (!(map[right + 1, iY].color != 0.0F | right == mapsizex)) {
                     right = right + 1;
                 }
                 // move to the left....
                 while (!(map[left - 1, iY].color != 0.0F | left == 1)) {
                     left = left - 1;
                 }
                 // put the filled cell into the total, sum count.
                 if (left > 0 & map[left - 1, iY].color != 0.0F) {
                     cellsum = cellsum + map[left - 1, iY].color;
                     //Sum up the elevation.
                     sumcount = sumcount + 1.0F;
                     //Add 1 to the number of filled cells in this loop pass.
                 }
                 // put the filled cell into the total, sum count.
                 if (right < mapsizex + 1 & map[right + 1, iY].color != 0.0F) {
                     cellsum = cellsum + map[right + 1, iY].color;
                     sumcount = sumcount + 1.0F;
                 }
                 cellsum = cellsum / sumcount;
                 // fill in the empty cells.
                 if (left == 1)
                     left = 0;
                 for (i = left; i <= right; i++) {
                     map[i, iY].color = cellsum;
                 }
                 cellsum = 0.0F;
                 sumcount = 0.0F;
             }
             iX = iX + 1;
         }
     }
 

            ////////////
            Single texcor = 0.0F;
            int flipper = 1;

            // To re-cycle through the texture map backwards once we reach the end.
            // Changes our dirrection.
            Random rnd = new Random(OuterSpace.thisPlanet.uSeed);
            for (iX = 0; iX <= mapsizex; iX++)
            {
                texcor = (float)texcor + 0.01F * flipper + (float)rnd.NextDouble() / 200F;
                for (iY = 0; iY <= mapsizey; iY++)
                {
                    map[iX, iY].texcor = texcor;
                    if (texcor > 1)
                    {
                        map[iX, iY].texcor = 1;
                        texcor = 0;
                        flipper = flipper * -1;
                    }
                }
            }

            i = 0;
            for (iY = 0; iY <= mapsizey; iY += deresoluter)
            {
                for (iX = 0; iX <= mapsizex + 1; iX += deresoluter)
                {
                    // Use the points organized in a deresoluter dimensional array to make a list of quads.
                    triangles[i].A = map[iX, iY + deresoluter];
                    triangles[i].B = map[iX, iY];
                    triangles[i].C = map[iX + deresoluter, iY + deresoluter];
                    triangles[i + 1].A = map[iX, iY];
                    triangles[i + 1].B = map[iX + deresoluter, iY];
                    triangles[i + 1].C = map[iX + deresoluter, iY + deresoluter];
                    i += 2;
                }
            }
            maxtriangles = i;

            for (i = 0; i <= maxtriangles; i++)
            {
                triangles[i].A.normal = new Vector3(0.0F, 0.0F, -1.0F);
                triangles[i].B.normal = new Vector3(0.0F, 0.0F, -1.0F);
                triangles[i].C.normal = new Vector3(0.0F, 0.0F, -1.0F);
            }

            OuterSpace.thisPlanet.PlanetMaps = new PlanetMap(mapsizex, mapsizey);

            //3-10-06 ...

            // Resize our life and mineral maps to represent the size of our terrian map,
            // and make the life and minerals.

            OuterSpace.thisPlanet.mineralmap = null;
            OuterSpace.thisPlanet.mineralmap = new bool[mapsizex, mapsizey];
            if (OuterSpace.thisPlanet.MinDensity > 0)
            {
                makeminerals();
            }

            //Redim OuterSpace.thisPlanet.lifemap[mapsizex, mapsizey];
            OuterSpace.thisPlanet.lifemap = null;
            OuterSpace.thisPlanet.lifemap = new lifeforms[mapsizex, mapsizey];

            makelife();
        }

        //3-10-06 ...
        private void makeminerals()
        {
            // Reset the random timer so our data is the same for each planet.
            Random rnd = new Random(OuterSpace.thisPlanet.uSeed);
            for (int x = 0; x < OuterSpace.thisPlanet.PlanetMaps.BigMapX; x++)
            {
                for (int y = 0; y < OuterSpace.thisPlanet.PlanetMaps.BigMapY; y++)
                {
                    OuterSpace.thisPlanet.mineralmap[x, y] = false;
                }
            }
            // Populate the bool array so we know where the minerals are!
            for (int x = 0; x < OuterSpace.thisPlanet.PlanetMaps.BigMapX; x++)
            {
                for (int y = 0; y < OuterSpace.thisPlanet.PlanetMaps.BigMapY; y++)
                {
                    if (rnd.NextDouble() + OuterSpace.thisPlanet.MinDensity / 500.0F > 1.0F)
                    {
                        if (map[x, y].color * rnd.NextDouble() > 0.67F)
                        {
                            OuterSpace.thisPlanet.mineralmap[x, y] = true;
                            OuterSpace.debugfile.Output("Mineral! X:" + Convert.ToString(x) + " Y:" + Convert.ToString(y));
                        }
                    }
                }
            }
        }
        private void makelife() //Well do this once for the whole planet.
        {
            Random rnd = new Random(OuterSpace.thisPlanet.uSeed);

            // Adjust our biopercentage so it//s a value between 0 and 100, to make voidsequent stuff eaiser.
            int lifepercentage;
            lifepercentage = Convert.ToInt32(MathFunctions.scaleValue(OuterSpace.thisPlanet.BioDensity, 255.0F, 0.0F, 100.0F, 0.0F));

            //Temporary for debugging
            lifepercentage = 50;
            //Temporary for debugging

            bool gasplanet = false;

            if (OuterSpace.thisPlanet.PClass == "J") gasplanet = true;

            for (int x = 0; x < OuterSpace.thisPlanet.PlanetMaps.BigMapX; x++)
            {
                for (int y = 0; y < OuterSpace.thisPlanet.PlanetMaps.BigMapY; y++)
                {
                    OuterSpace.thisPlanet.lifemap[x, y] = new lifeforms(map[x, y].color, gasplanet, lifepercentage);
                    //if OuterSpace.thisPlanet.lifemap(x, y).type > 0
                    //OuterSpace.debugfile.output("X: " + Convert.ToString(x) + " Y: " + Convert.ToString(y) + " " + _
                    //Convert.ToString(map(x, y).color) + " " + OuterSpace.thisPlanet.lifemap(x, y).FetchDesc(true, false) _
                    //+ " " + OuterSpace.thisPlanet.lifemap(x, y).FetchDesc(false, true) + " " + OuterSpace.thisPlanet.lifemap(x, y).FetchDesc(false, false))
                    //} if
                }
            }
        }

        public void render_minerals_ship_lifeforms()
        {
            for (int iy = 0; iy <= (zmapsdim - 1); iy++)
            {
                for (int ix = 0; ix <= (zmapsdim - 1); ix++)
                {
                    if (zobjectmap[ix, iy].mineral)
                    {
                        OuterSpace.mineralMesh.Drawmesh(zmap[ix, iy].X + OuterSpace.TV.X, zmap[ix, iy].Y + OuterSpace.TV.Y, 0.0F);
                    }
                    if (zobjectmap[ix, iy].ship)
                    {
                        OuterSpace.shipMesh.Drawmesh(zmap[ix, iy].X + OuterSpace.TV.X, zmap[ix, iy].Y + OuterSpace.TV.Y, 0, 0);
                    }
                }
            }

            move_draw_life();
        }

        private void move_draw_life()
        {
            short i = 0;
            Single lx, ly; //Real X, Y posistion of the life form. These are the ones that are
            //smoothly changed by the wander, attack, run, s in the lifeform class.
            //The below block updates these so that are close to each other, as px and py have to be whole integers,
            //and lx and ly can be decimals. Also, shuffles the life
            //in the 2d array if a lifeform has moved to a new position within it.

            while (zlifemap[i].xptr >= -1)
            {
                OuterSpace.thisPlanet.lifemap[zlifemap[i].xptr, zlifemap[i].yptr].animalBehavior(ref zlifemap[i].xptr, ref zlifemap[i].yptr);

                lx = OuterSpace.thisPlanet.lifemap[zlifemap[i].xptr, zlifemap[i].yptr].sX;
                ly = OuterSpace.thisPlanet.lifemap[zlifemap[i].xptr, zlifemap[i].yptr].sY;
                OuterSpace.lifemesh.Drawmesh(lx + OuterSpace.TV.X, ly + OuterSpace.TV.Y, 0, 0);
                i++;
            }
        }

        //3-11-06 ...
        public void scanNewterrian(int startX, int startY, bool reloadVB)
        {
            // Tesselate the 30,30 viewable section of terrian 3 times. Repopulate zmap based on the updated position.
            // reload the vertex buffer with the updated terrian.
            int lifeC;
            int i = 0;
            int iy;
            int ix;
            int zx; // The current x index of the zmap arrary.
            int zy; // The current y index of the zmap arrary.
            int arrayX; // The current x index of the map arrary.
            int arrayY; // The current y index of the map arrary.
            int bigmX; // Store the dimensions of our map locally.
            int bigmY;
            MathFunctions.location testMapOut = new MathFunctions.location();

            bigmX = (int)OuterSpace.thisPlanet.PlanetMaps.BigMapX;
            bigmY = (int)OuterSpace.thisPlanet.PlanetMaps.BigMapY;

            for (ix = 0; ix < 100; ix++) //Reset the zlifemap.
            {
                zlifemap[ix].xptr = -99;
                zlifemap[ix].yptr = -99;
            }

            // Load the zmap with the inital nXn square of viewable terrian. Talking into account the edges.
            zy = 0;
            lifeC = 0;
            for (iy = startY + (zmapsdim / 2 - 1); iy >= startY - (zmapsdim / 2); iy--)
            {
                arrayY = iy;
                zx = 0;

                for (ix = startX + (zmapsdim / 2 - 1); ix >= startX - (zmapsdim / 2); ix--)
                {
                    arrayX = ix;

                    testMapOut = MathFunctions.outOfBounds(arrayX, arrayY, 0, 0, OuterSpace.thisPlanet.PlanetMaps.BigMapX, OuterSpace.thisPlanet.PlanetMaps.BigMapY, 0, true);
                    arrayY = (int)testMapOut.y;
                    arrayX = (int)testMapOut.x;

                    // Assign the spot to the zmap.
                    zmap[zx, zy] = map[bigmX - arrayX,
                        bigmY - arrayY];

                    // Assign our minerals based on the planets mineralmap
                    if (OuterSpace.thisPlanet.mineralmap[bigmX - arrayX, bigmY - arrayY])
                    {
                        zobjectmap[zx, zy].mineral = OuterSpace.thisPlanet.mineralmap[bigmX - arrayX, bigmY - arrayY];
                    }
                    // Assign our lifeforms based on the planets lifeformmap
                    if (OuterSpace.thisPlanet.lifemap[bigmX - arrayX,
                        bigmY - arrayY].type > 0)
                    {
                        // Point to the life form in the big map.
                        zlifemap[lifeC].xptr = (short)(bigmX - arrayX);
                        zlifemap[lifeC].yptr = (short)(bigmY - arrayY);

                        OuterSpace.msgbox.pushmsgs("We are here, getting life");

                        //Have we ever calculated this lifeform//s screen position?
                        if (OuterSpace.thisPlanet.lifemap[bigmX - arrayX,
                            bigmY - arrayY].sX == 0 ||
                            OuterSpace.thisPlanet.lifemap[bigmX - arrayX,
                            bigmY - arrayY].sY == 0)
                        {
                            OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sX = bigmX / 2 - ix;
                            OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sY = bigmY / 2 - iy;
                        }

                        if (OutOnX)  //if we've crossed the x meridian we have to make some special exceptions.
                        {
                            //if it's a plant, lets re-calculate the screen position.
                            if (OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].type / 10 < 10)
                            {
                                OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sX = bigmX / 2 - ix;
                                OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sY = bigmY / 2 - iy;
                            }    //if it's a life form, we need to update it//s x-position.
                            else
                            {
                                OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sX = bigmX / 2 - ix -
                                    OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].deltaX;
                            }
                        }

                        lifeC++;
                        //Set the Sx and Sy variables.
                        OuterSpace.debugfile.Output(Convert.ToString(bigmX - arrayX) + "," + Convert.ToString(bigmY - arrayY) + "," +
                            Convert.ToString(bigmX / 2 - ix) + "," + Convert.ToString(bigmY / 2 - iy) + "," +
                            Convert.ToString(OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sX) + "," +
                            Convert.ToString(OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].sY) + "," +
                            Convert.ToString((OuterSpace.thisPlanet.lifemap[bigmX - arrayX, bigmY - arrayY].deltaX)));
                    }

                    // Assign our ship placement on the zobject map if we are near it in this batch of terrian.
                    if ((ix - OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2.0F) + 0.5F == OuterSpace.playership.X &&
                        (iy - OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2.0F) + 0.5F == OuterSpace.playership.Y)
                    {
                        zobjectmap[zx, zy].ship = true;
                    }
                    else
                    {
                        zobjectmap[zx, zy].ship = false;
                    }

                    //Assign the vertex X,Y positions so we can draw them on the screen even if they are
                    //outside of our logical map dimensions.
                    zmap[zx, zy].X = OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2 - ix;
                    zmap[zx, zy].Y = OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2 - iy;
                    zx++;

                }

                zy++;
            }

            for (iy = 0; iy <= (zmapsdim - 2); iy++)
            {
                for (ix = 0; ix <= (zmapsdim - 2); ix++)
                {
                    // Use the points organized in a deresoluter dimensional array to make a list of quads.
                    triangles[i].A = zmap[ix, iy + 1];
                    triangles[i].B = zmap[ix, iy];
                    triangles[i].C = zmap[ix + 1, iy + 1];
                    triangles[i + 1].A = zmap[ix, iy];
                    triangles[i + 1].B = zmap[ix + 1, iy];
                    triangles[i + 1].C = zmap[ix + 1, iy + 1];
                    i += 2;
                }
            }

            maxtriangles = i;

            for (i = 0; i <= maxtriangles; i++)
            {
                triangles[i].A.normal = new Vector3(0.0F, 0.0F, -1.0F);
                triangles[i].B.normal = new Vector3(0.0F, 0.0F, -1.0F);
                triangles[i].C.normal = new Vector3(0.0F, 0.0F, -1.0F);
            }

            if (reloadVB) OuterSpace.thisPlanet.ReSetVB();

            OutOnX = false;
        }

        public Single getElevationofspot(int xCor, int yCor)
        {
            if (xCor > OuterSpace.thisPlanet.PlanetMaps.BigMapX || xCor < 0)
                return 0;

            if (yCor > OuterSpace.thisPlanet.PlanetMaps.BigMapY || yCor < 0)
                return 0;

            return map[xCor, yCor].color;
        }

        private vertex_structure normalize(vertex_structure curvert)
        {
            curvert.X = curvert.X / curvert.elevation;
            curvert.Y = curvert.Y / curvert.elevation;
            curvert.Z = curvert.Z / curvert.elevation;

            return curvert;
        }

        private vertex_structure flattenvert(vertex_structure curvert)
        {
            //Map the X,Y corrdinates based on sphereical trig and the normalized verticie
            curvert.X = (float)(Math.Asin(curvert.X) / Math.PI + 0.5F) * 100.0F;
            curvert.Y = (float)(Math.Asin(curvert.Y) / Math.PI + 0.5F) * 100.0F;
            curvert.Z = 0.0F;

            return curvert;
        }

        private vertex_structure rotatevertZ(vertex_structure curvert, int angle)
        {
            Single ang = (float)(angle * (Math.PI / 180.0F) * -1.0F);
            Vector4 tempvect = new Vector4(curvert.X, curvert.Y, curvert.Z, 0.0F);
            // Cheat and use the nice built in matrix transformations.

            //Used to be below in C#, will it work like this?!?!
            //tempvect = tempvect.Transform(tempvect, Matrix.RotationZ(ang));
            //
            tempvect = Microsoft.DirectX.Vector4.Transform(tempvect, Matrix.RotationZ(ang));

            curvert.X = tempvect.X;
            curvert.Y = tempvect.Y;
            curvert.Z = tempvect.Z;

            // Find the maximum X and maximum Y values for our flatt} geosphere so we can use them later when we scale everything
            // into a two dimmensional array in the maperize .
            if (curvert.X < flatmapXmin) flatmapXmin = curvert.X;
            if (curvert.X > flatmapXmax) flatmapXmax = curvert.X;
            if (curvert.Y < flatmapYmin) flatmapXmin = curvert.Y;
            if (curvert.Y > flatmapYmax) flatmapXmax = curvert.Y;

            return curvert;
        }
    }
}