using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class MeshObject
    {
        private Mesh moMesh;
        private int mlMeshMaterials = -1;
        private List<Material> materialsList = new List<Material>();
        private List<Texture> texturesList = new List<Texture>();
        private bool havemesh;
        //private Single mx, my, mz;
        public GraphicsStream adj;

        public MeshObject(string Meshname)
        {
            LoadMesh(Meshname);
        }

        public void LoadMesh(string Meshname)
        {
            string sModel;
            List<ExtendedMaterial> mtrlBuffer = new List<ExtendedMaterial>();
            string sTemp;

            sModel = OuterSpace.meshdir + Meshname;

            if (File.Exists(sModel))
            {
                ExtendedMaterial[] mtrlArray = mtrlBuffer.ToArray();
                moMesh = Mesh.FromFile(sModel, MeshFlags.Managed, OuterSpace.device, out adj, out mtrlArray);

                mtrlBuffer.InsertRange(0, mtrlArray);

                if ((moMesh.VertexFormat & VertexFormats.Normal) != VertexFormats.Normal)
                {
                    Mesh tempMesh = moMesh.Clone(moMesh.Options.Value, moMesh.VertexFormat |
                        VertexFormats.Normal, OuterSpace.device);

                    tempMesh.ComputeNormals();
                    moMesh.Dispose();
                    moMesh = tempMesh;
                }

                // now initialize our materials and textures
                mlMeshMaterials = mtrlBuffer.Count;
                materialsList.Clear();
                texturesList.Clear();

                // Now load our textures and materials
                for (int x = 0; x < mtrlBuffer.Count; x++)
                {
                    Material mtrl = new Material();// materialsList[x];
                    mtrl = mtrlBuffer[x].Material3D;
                    mtrl.Ambient = mtrl.Diffuse; // because directx sometimes doesnt do this for you
                    materialsList.Insert(x, mtrl);

                    if (mtrlBuffer[x].TextureFilename != String.Empty)
                    {
                        sTemp = OuterSpace.meshdir + mtrlBuffer[x].TextureFilename;
                        texturesList.Insert(x, TextureLoader.FromFile(OuterSpace.device, sTemp));
                    }
                }

                havemesh = true;
            }
            else
                havemesh = false;
        }

        private void SetupMatrices(Single mx, Single my, Single mz, Single RotateZ)
        {
            int iTime = Environment.TickCount % 100000;
            Single fAngle = (float)(iTime * (2.0F * Math.PI) / 1000.0F);
            //OuterSpace.device.Transform.World = Matrix.Translation(mz, my, OuterSpace.thisPlanet.planetpoly.z)
            //OuterSpace.device.Transform.World = Matrix.Translation(mx, my, mz)
            //OuterSpace.msgbox.pushmsgs("X:" + Convert.ToString(OuterSpace.TV.X) + " Y:" + Convert.ToString(OuterSpace.TV.Y))
            if (RotateZ == 999)
                OuterSpace.device.Transform.World = Matrix.Multiply(Matrix.RotationY(fAngle), Matrix.Translation(mx, my, mz));
            else
                OuterSpace.device.Transform.World = Matrix.Multiply(Matrix.RotationZ((float)(RotateZ * Math.PI / 180.0F)), Matrix.Translation(mx, my, mz));
        }

        public void Drawmesh()
        {
            for (int x = 0; x < mlMeshMaterials; x++)
            {
                OuterSpace.device.Material = materialsList[x];
                OuterSpace.device.SetTexture(0, texturesList[x]);
                moMesh.DrawSubset(x);
            }
        }

        public void Drawmesh(Single spotX, Single spotY, Single spotZ)
        {
            SetupMatrices(spotX, spotY, spotZ, 999);

            for (int x = 0; x < mlMeshMaterials; x++)
            {
                OuterSpace.device.Material = materialsList[x];
                OuterSpace.device.SetTexture(0, texturesList[x]);
                moMesh.DrawSubset(x);
            }
        }

        public void Drawmesh(Single spotx, Single spoty, Single spotz, Single RotateZ)
        {
            SetupMatrices(spotx, spoty, spotz, RotateZ);

            for (int x = 0; x < mlMeshMaterials; x++)
            {
                OuterSpace.device.Material = materialsList[x];
                OuterSpace.device.SetTexture(0, texturesList[x]);
                moMesh.DrawSubset(x);
            }
        }

        public Mesh GetMesh()
        {
            return moMesh;
        }

        public int GetNumMeshMaterials()
        {
            return mlMeshMaterials;
        }

        public List<Material> GetMeshMaterials()
        {
            return materialsList;
        }

        public List<Texture> GetTextures()
        {
            return texturesList;
        }
    }
}
