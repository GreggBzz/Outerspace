using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
/// Take a Geosphere in, and generate a flattened map.
/// An effort to scale down Geosphere.cs
namespace OuterSpace
{
    public class PlanetMap
    {
        private Single bigmapX;
        public Single BigMapX
        {
            get { return bigmapX; }
            set { bigmapX = value; }
        }

        private Single bigmapY;
        public Single BigMapY
        {
            get { return bigmapY; }
            set { bigmapY = value; }
        }	
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
        
        public PlanetMap(Single mapX, Single mapY) // List<triangles_structure> triangles)
        {
            bigmapX = mapX;
            bigmapY = mapY;
                                              
        }
   
    }
}