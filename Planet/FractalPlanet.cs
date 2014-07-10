// The fractal planet renderer. Generates a random repeatable planet by 
// tesselating a geosphere, then fractally displacing the midpoints on each vertex.

using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class FractalPlanet
    {
        //Inherits Form
        // Our global variables for this project
        private VertexBuffer vertexBuffer = null;
        public Geosphere startSphere = null;
        public Texture texture = null;
        public bool land = false;
        public Single angle;
        public Single xdelta;
        public Single z;
        public Single x;
        public Single y;
        private Single fRotYAngle = 0.0F;
        private Single fCloudRotAngle = 0.0F;
        //private MeshObject planetcloudsMesh = null;
        private Int32 iBlend, jBlend;
        private Random rnd = new Random(OuterSpace.thisPlanet.uSeed);

        public FractalPlanet(int iceCapRange, int displaceMag, Single planetscaler, int roughness,
            Single seed, string txrMap)
        {
            startSphere = new Geosphere(planetscaler, seed, roughness, displaceMag, iceCapRange);
            land = false;
            LoadTexture(txrMap);
            CreateVertexBuffer();
            x = 0;
            y = 0;
            z = 0;
            fRotYAngle = (float)(rnd.NextDouble() / Convert.ToSingle(Environment.TickCount));
            fCloudRotAngle = (float)(rnd.NextDouble() / Convert.ToSingle(Environment.TickCount));

            iBlend = 1;
            jBlend = 0;
        }

        public void CreateVertexBuffer()
        {
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 65536,
                OuterSpace.device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            OnCreateVertexBuffer(vertexBuffer, null, false);
        }

        public void disposevertexbuffer()
        {
            vertexBuffer.Dispose();
        }

        public void LoadTexture(string txrMap)
        {
            texture = TextureLoader.FromFile(OuterSpace.device, OuterSpace.texturedir + "/" + txrMap);
        }

        public void OnCreateVertexBuffer(object sender, EventArgs e, bool secondbuffer)
        {
            VertexBuffer vb = sender as VertexBuffer;
            CustomVertex.PositionNormalTextured[] verts = vb.Lock(0, 0) as CustomVertex.PositionNormalTextured[];
            int i, i2;

            if (!land)
            {
                startSphere.tesselate(80, false);
                startSphere.tesselate(320, false); // Primative count rises x 3 for each round of tesselation
                startSphere.tesselate(1280, false); // The tesselate function calls other functions for fractal displacement.
                startSphere.tesselate(5120, false);
                startSphere.tesselate(20480, true);
            }

            i2 = 0;

            // If maxtriangles is larger then our vertex buffer, for who knows what reason, print the value, and set it back down
            // so we don't overflow the VB.
            // OuterSpace.msgbox.pushmsgs(Str(startSphere.maxtriangles))'' DEBUG_MESSAGE
            if (startSphere.maxtriangles > 20480)
            {
                OuterSpace.msgbox.pushmsgs("Maxtrianges to big!");
                startSphere.maxtriangles = 20480;
            }

            // Assign our newly created triangle points to the vertice.
            for (i = 0; i < (startSphere.maxtriangles * 3); i += 3)
            {
                verts[i].X = startSphere.triangles[i2].A.X;
                verts[i].Y = startSphere.triangles[i2].A.Y;
                verts[i].Z = startSphere.triangles[i2].A.Z;

                verts[i].Normal = startSphere.triangles[i2].A.normal;
                verts[i].Tv = startSphere.triangles[i2].A.color;
                verts[i].Tu = (float)startSphere.triangles[i2].A.texcor;

                verts[i + 1].X = startSphere.triangles[i2].B.X;
                verts[i + 1].Y = startSphere.triangles[i2].B.Y;
                verts[i + 1].Z = startSphere.triangles[i2].B.Z;

                verts[i + 1].Normal = startSphere.triangles[i2].B.normal;
                verts[i + 1].Tv = startSphere.triangles[i2].B.color;
                verts[i + 1].Tu = (float)startSphere.triangles[i2].B.texcor;

                verts[i + 2].X = startSphere.triangles[i2].C.X;
                verts[i + 2].Y = startSphere.triangles[i2].C.Y;
                verts[i + 2].Z = startSphere.triangles[i2].C.Z;

                verts[i + 2].Normal = startSphere.triangles[i2].C.normal;
                verts[i + 2].Tv = startSphere.triangles[i2].C.color;
                verts[i + 2].Tu = (float)startSphere.triangles[i2].C.texcor;

                i2++;
            }

            i2 = 0;
            vb.Unlock();
        }

        public void RenderPlanet()
        {
            transform_world();

            if (!OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.DockedAtStation))
            {
                OuterSpace.d3d_scene.SetupLights();
                OuterSpace.d3d_scene.transformView_Projection();
            }
            else
            {
                OuterSpace.d3d_scene.transformView_Projection(new Vector3(0.0F, 0.0F, -80.0F), 
                    new Vector3(0.0F, 0.0F, 0.0F), new Vector3(0.0F, 1.0F, 0.0F), 
                    Convert.ToSingle(Math.PI / 2.0), OuterSpace.ClientArea.Width / OuterSpace.ClientArea.Height, 
                    0.01F, 1000.0F);
            }

            OuterSpace.d3d_scene.transform_Pipeline();

            OuterSpace.device.RenderState.AlphaBlendEnable = false;

            OuterSpace.device.SetTexture(0, texture);
            OuterSpace.device.TextureState[0].ColorOperation = TextureOperation.Modulate;
            OuterSpace.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            OuterSpace.device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
            OuterSpace.device.TextureState[0].AlphaOperation = TextureOperation.Disable;
            OuterSpace.device.SetStreamSource(0, vertexBuffer, 0);
            OuterSpace.device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            OuterSpace.device.DrawPrimitives(PrimitiveType.TriangleList, 0, startSphere.maxtriangles);

            //If (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.DockedAtStation)) Then
            OuterSpace.device.RenderState.AlphaBlendEnable = true;

            Microsoft.DirectX.Direct3D.Cull oldCull = OuterSpace.device.RenderState.CullMode;
            Microsoft.DirectX.Direct3D.Blend oldSrcBlend = OuterSpace.device.RenderState.SourceBlend;
            Microsoft.DirectX.Direct3D.Blend OldDestBlend = OuterSpace.device.RenderState.DestinationBlend;
            Microsoft.DirectX.Direct3D.BlendOperation oldBlendOp = OuterSpace.device.RenderState.BlendOperation;

            if (Environment.TickCount % 5000 == 0)
            {
                jBlend++;
                if (jBlend > 15)
                {
                    iBlend += 1;
                    jBlend = 1;
                }
                
                if (iBlend > 15)
                    iBlend = 1;
            }

            OuterSpace.device.RenderState.CullMode = Cull.CounterClockwise;
            OuterSpace.device.RenderState.SourceBlend = Blend.SourceColor;
            OuterSpace.device.RenderState.DestinationBlend = Blend.BothSourceAlpha;
            OuterSpace.device.RenderState.BlendOperation = BlendOperation.Add;

            // render clouds around planet
            OuterSpace.d3d_scene.transformWorld(Matrix.Zero, Matrix.RotationY(fCloudRotAngle), 
                Matrix.Zero, Matrix.Scaling(1.135F, 1.135F, 1.135F), Matrix.Translation(x, y, z));
            OuterSpace.d3d_scene.transform_Pipeline();

            //planetcloudsMesh.Drawmesh();

            OuterSpace.device.RenderState.CullMode = oldCull;
            OuterSpace.device.RenderState.SourceBlend = oldSrcBlend;
            OuterSpace.device.RenderState.DestinationBlend = OldDestBlend;
            OuterSpace.device.RenderState.BlendOperation = oldBlendOp;

            OuterSpace.device.RenderState.AlphaBlendEnable = false;
            //End If
            ///3-10-06 ...
        }

        //3-10-06 ...
        public void transform_world()
        {
            int iTime = Environment.TickCount % 100000;

            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.DockedAtStation))
            {
                fRotYAngle -= 0.0001F; //iTime * (2.0F * Math.PI) / 500000.0F
                fCloudRotAngle -= 0.0002F;
                //x = -20.0F;
                //y = -8.0F;
            }
            else
            {
                //fRotYAngle = iTime * (2.0F * Math.PI) / 300000.0F;
                fRotYAngle -= 0.00075F; //iTime * (2.0F * Math.PI) / 500000.0F
                fCloudRotAngle -= 0.001F;
            }

            if (land)
            {
                OuterSpace.d3d_scene.transformWorld(Matrix.Zero, Matrix.Zero, Matrix.Zero, Matrix.Zero, Matrix.Translation(OuterSpace.TV.X, OuterSpace.TV.Y, OuterSpace.thisPlanet.planetpoly.z));
                //OuterSpace.device.Transform.World = Matrix.Translation(OuterSpace.TV.X, OuterSpace.TV.Y, OuterSpace.thisPlanet.planetpoly.z)
            }
            else
            {
                // Set up the rotation matrix to generate 1 full rotation (2*PI radians) 
                // every 1000 ms. 
                // the system time is modulated by the rotation 
                // period before conversion to a radian angle.
                // OuterSpace.device.Transform.World = Matrix.Multiply(Matrix.RotationY(-fAngle), Matrix.Translation(x, y, z));
                OuterSpace.d3d_scene.transformWorld(Matrix.Zero, Matrix.RotationY(fRotYAngle), Matrix.Zero, Matrix.Zero, Matrix.Translation(x, y, z));
            }
        }
    }
}