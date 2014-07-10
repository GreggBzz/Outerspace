using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class SpriteClass
    {
        public Texture spriteTx;
        public Vector3 position; // X,Y position about the screen
        public Vector3 center;
        public const int numFrames = 20;  // Number of animation frames.
        public Rectangle[] sourceFrame = new Rectangle[numFrames];  // Holds the animation frame dimensions
        public int curframe; // The current index of the rectangle that holds the image we want to show
        public int lastframe; // The frame that was being displayed previous to the current frame.
        public int theta;

        public int spritewidth;
        public int spriteheight;

        private string spriteName;

        private Matrix translateMatrix;
        private Matrix rotateMatrix;

        public SpriteClass(string file, int width, int height)
            :this(file, width, height, Color.FromArgb(255,0,0,0).ToArgb())
        {
        }

        public SpriteClass(string file, int width, int height, Int32 Colorkey)
        {
            spriteName = file;

            try
            {
                spriteTx = TextureLoader.FromFile(
                    OuterSpace.device, 
                    OuterSpace.texturedir + file,
                    width, 
                    height, 
                    0, 
                    0, 
                    Format.A8B8G8R8, 
                    Pool.Managed, 
                    Filter.None, 
                    Filter.Linear, 
                    Colorkey);
            }
            catch (DirectXException ex)
            {
                System.Windows.Forms.MessageBox.Show("Outerspace barfs while loading a sprite.  " + ex.Message);
            }

            spritewidth = width;
            spriteheight = height;
        }

        public void Draw(int scrX, int scrY, Rectangle sourceRect, int angle, Color lightColor)
        {
            // Calculate the angle in radians given an integer
            Single ang = (float)(angle * (Math.PI / 180.0F) * -1.0);
            
            // calculate the source center
            center = new Vector3(sourceRect.Width / 2.0F, sourceRect.Height / 2.0F, 0.0F);
            
            // Rotate and move the texture around center
            translateMatrix = Microsoft.DirectX.Matrix.Translation(new Vector3(scrX + center.X, scrY + center.Y, 0.0F));
            rotateMatrix = Microsoft.DirectX.Matrix.RotationZ(ang);

            OuterSpace.spriteobj.Transform = Matrix.Multiply(rotateMatrix, translateMatrix);
            // Add to batched list
            OuterSpace.spriteobj.Draw(spriteTx, sourceRect, center, new Vector3(0.0F, 0.0F, 0.0F), lightColor);
        }

        public void Draw(int scrX, int scrY, Rectangle sourceRect, int angle, Single scaleX, Single scaleY, 
            Color lightcolor)
        {
            Vector3 scaleVector = new Vector3();
            Matrix scaleMatrix = new Matrix();
            Matrix result = new Matrix();
            Single ang = (float)(angle * (Math.PI / 180.0F) * -1.0F);

            // calculate the source center
            center = new Vector3(sourceRect.Width / 2.0F, sourceRect.Height / 2.0F, 0.0F);

            // Rotate and move the texture around center
            result = Matrix.Identity;
            translateMatrix = Matrix.Translation(new Vector3(scrX + center.X, scrY + center.Y, 0.0F));
            result = Matrix.Multiply(result, translateMatrix);
            rotateMatrix = Matrix.RotationZ(ang);
            result = Matrix.Multiply(result, rotateMatrix);
            scaleMatrix = Matrix.Scaling(new Vector3(scaleX, scaleY, 1.0F));
            result = Matrix.Multiply(result, scaleMatrix);

            // transform it
            OuterSpace.spriteobj.Transform = result;

            // Draw the sprite
            OuterSpace.spriteobj.Draw(spriteTx, sourceRect, center, Vector3.Empty, lightcolor);
        }
    }
}