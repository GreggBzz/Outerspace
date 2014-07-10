// Desc: Shortcut functions for using DX objects

 using System;
 using System.Drawing;
 using Microsoft.DirectX;
 using Microsoft.DirectX.Direct3D;
 using Direct3D = Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class GraphicsFont
    {
        public const int MaxNumfontVertices = 50 * 6;
        // Font rendering flags
        [System.Flags()]
        public enum RenderFlags
        {
            Centered = 1,
            TwoSided = 2,
            Filtered = 4
        }
        //RenderFlags
        private System.Drawing.Font systemFont;

        private bool isZEnable = false;

        public bool ZBufferEnable
        {
            get { return isZEnable; }
            set { isZEnable = value; }
        }
        // Font properties
        private string ourFontName;
        private int ourFontHeight;

        private Direct3D.Device device;
        private TextureStateManager textureState0;
        private TextureStateManager textureState1;
        private SamplerStateManager samplerState0;
        private RenderStateManager renderState;
        private Direct3D.Texture fontTexture;
        private Direct3D.VertexBuffer vertexBuffer;
        private CustomVertex.TransformedColoredTextured[] fontVertices = new CustomVertex.TransformedColoredTextured[MaxNumfontVertices];

        // Texture dimensions
        private int textureWidth;
        private int textureHeight;
        private float textureScale;
        private int spacingChar;
        private float[,] textureCoords = new float[128 - 31, 5];

        // Stateblocks for setting and restoring render states
        private StateBlock savedStateBlock;
        private StateBlock drawTextStateBlock;





        //-----------------------------------------------------------------------------
        // Name: Constructor
        // Desc: Create a new font object
        //-----------------------------------------------------------------------------
        public GraphicsFont(System.Drawing.Font f)
        {
            ourFontName = f.Name;
            ourFontHeight = (int)f.Size;
            systemFont = f;
        }
        //New

        public GraphicsFont(string strFontName)
        {
            ourFontName = strFontName;
            ourFontHeight = 12;
            systemFont = new System.Drawing.Font(ourFontName, ourFontHeight);
        }
        //New

        public GraphicsFont(string strFontName, FontStyle Style)
        {
            ourFontName = strFontName;
            ourFontHeight = 12;
            systemFont = new System.Drawing.Font(ourFontName, ourFontHeight, Style);
        }
        //New

        public GraphicsFont(string strFontName, FontStyle Style, int size)
        {
            ourFontName = strFontName;
            ourFontHeight = size;
            systemFont = new System.Drawing.Font(ourFontName, ourFontHeight, Style);
        }
        //New




        //-----------------------------------------------------------------------------
        // Name: PaintAlphabet
        // Desc: Attempt to draw the systemFont alphabet onto the provided graphics
        //-----------------------------------------------------------------------------
        public void PaintAlphabet(Graphics g, bool measureOnly)
        {
            string str = "";
            float x = 0;
            float y = 0;
            PointF p = new PointF(0, 0);
            Size size = new Size(0, 0);

            // Calculate the spacing between characters based on line height
            size = g.MeasureString(" ", systemFont).ToSize();
            spacingChar = (int)Math.Ceiling(size.Height * 0.3);
            x = spacingChar;

            byte c;
            for (c = 32; c < 127; c++)
            {
                str = ((char)c).ToString();

                // We need to do some things here to get the right sizes. The default implemententation of MeasureString
                // will return a resolution independant size. For our height, this is what we want. However, for our width, we
                // want a resolution dependant size.
                Size resSize = g.MeasureString(str, systemFont).ToSize();
                size.Height = resSize.Height;

                // Now the Resolution independent width
                if (((char)c) != ' ')
                {
                    // We need the special case here because a space has a 0 width in GenericTypoGraphic stringformats
                    resSize = g.MeasureString(str, systemFont, p, StringFormat.GenericTypographic).ToSize();
                    size.Width = resSize.Width;
                }
                else
                {
                    size.Width = resSize.Width;
                }
                if (x + size.Width + 1 > textureWidth)
                {
                    x = 0;
                    y += size.Height;
                }

                if (y + size.Height > textureHeight)
                {
                    throw new System.InvalidOperationException("Texture too small for alphabet");
                }

                if (!measureOnly)
                {
                    if (((char)c) != ' ')
                    {
                        // We need the special case here because a space has a 0 width in GenericTypoGraphic stringformats
                        g.DrawString(str, systemFont, Brushes.White, new PointF((int)x, (int)y), StringFormat.GenericTypographic);
                    }
                    else
                    {
                        g.DrawString(str, systemFont, Brushes.White, new PointF((int)x, (int)y));
                    }
                    textureCoords[c - 32, 0] = (float)x / textureWidth;
                    textureCoords[c - 32, 1] = (float)y / textureHeight;
                    textureCoords[c - 32, 2] = (float)(x + size.Width) / textureWidth;
                    textureCoords[c - 32, 3] = (float)(y + size.Height) / textureHeight;
                }

                x += size.Width + 1;
            }

        }
        //PaintAlphabet

        //-----------------------------------------------------------------------------
        // Name: InitializeDeviceObjects
        // Desc: Initialize the device objects
        //-----------------------------------------------------------------------------
        public void InitializeDeviceObjects(Direct3D.Device dev)
        {
            if (dev != null)
            {
                // Set up our events
                dev.DeviceReset += this.RestoreDeviceObjects;
            }

            // Keep a local copy of the device
            device = dev;
            textureState0 = device.TextureState[0];
            textureState1 = device.TextureState[1];
            samplerState0 = device.SamplerState[0];
            renderState = device.RenderState;

            // Create a bitmap on which to measure the alphabet
            Bitmap bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.TextContrast = 0;

            // Establish the font and texture size
            textureScale = 1f;
            // Draw fonts into texture without scaling

            // Calculate the dimensions for the smallest power-of-two texture which can
            // hold all the printable characters
            textureWidth = 128;
            textureHeight = 128;
            while (true)
            {
                try
                {
                    // Measure the alphabet
                    PaintAlphabet(g, true);
                    break; // TODO: might not be correct. Was : Exit While
                }
                catch (InvalidOperationException)
                {
                    // Scale up the texture size and try again
                    textureWidth *= 2;
                    textureHeight *= 2;
                }
            }

            // If requested texture is too big, use a smaller texture and smaller font,
            // and scale up when rendering.
            Direct3D.Caps d3dCaps = device.DeviceCaps;

            // If the needed texture is too large for the video card...
            if (textureWidth > d3dCaps.MaxTextureWidth)
            {
                // Scale the font size down to fit on the largest possible texture
                textureScale = (float)d3dCaps.MaxTextureWidth / (float)textureWidth;
                textureWidth = d3dCaps.MaxTextureWidth;
                textureHeight = d3dCaps.MaxTextureWidth;

                while (true)
                {
                    // Create a new, smaller font
                    ourFontHeight = (int)Math.Floor(ourFontHeight * textureScale);
                    systemFont = new System.Drawing.Font(systemFont.Name, ourFontHeight, systemFont.Style);

                    try
                    {
                        // Measure the alphabet
                        PaintAlphabet(g, true);
                        break; // TODO: might not be correct. Was : Exit While
                    }
                    catch (InvalidOperationException)
                    {
                        // If that still doesn't fit, scale down again and continue
                        textureScale *= 0.9f;
                    }

                }
            }

            // Release the bitmap used for measuring and create one for drawing
            bmp.Dispose();
            bmp = new Bitmap(textureWidth, textureHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.TextContrast = 0;

            // Draw the alphabet
            PaintAlphabet(g, false);

            // Create a new texture for the font from the bitmap we just created
            fontTexture = Texture.FromBitmap(device, bmp, 0, Pool.Managed);
            RestoreDeviceObjects(null, null);
        }
        //InitializeDeviceObjects




        public void DrawText(float sx, float sy, Color color, string strText)
        {
            DrawText(sx, sy, color, strText, RenderFlags.Filtered);
        }
        //DrawText






        //-----------------------------------------------------------------------------
        // Name: RestoreDeviceObjects
        // Desc: Restore the font
        //-----------------------------------------------------------------------------
        public void RestoreDeviceObjects(object sender, EventArgs e)
        {
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.TransformedColoredTextured), MaxNumfontVertices, device, Usage.WriteOnly | Usage.Dynamic, 0, Pool.Default);

            Surface surf = device.GetRenderTarget(0);
            bool supportsAlphaBlend = Manager.CheckDeviceFormat(device.DeviceCaps.AdapterOrdinal, device.DeviceCaps.DeviceType, device.DisplayMode.Format, Usage.RenderTarget | Usage.QueryPostPixelShaderBlending, ResourceType.Surface, surf.Description.Format);

            // Create the state blocks for rendering text
            for (int which = 0; which < 2; which++)
            {
                device.BeginStateBlock();
                device.SetTexture(0, fontTexture);

                if (isZEnable)
                {
                    renderState.ZBufferEnable = true;
                }
                else
                {
                    renderState.ZBufferEnable = false;
                }
                if (supportsAlphaBlend)
                {
                    renderState.AlphaBlendEnable = true;
                    renderState.SourceBlend = Blend.SourceAlpha;
                    renderState.DestinationBlend = Blend.InvSourceAlpha;
                }
                else
                {
                    renderState.AlphaBlendEnable = false;
                }

                renderState.AlphaTestEnable = true;
                renderState.ReferenceAlpha = 8;
                renderState.AlphaFunction = Compare.GreaterEqual;
                renderState.FillMode = FillMode.Solid;
                renderState.CullMode = Cull.CounterClockwise;
                renderState.StencilEnable = false;
                renderState.Clipping = true;
                device.ClipPlanes.DisableAll();
                renderState.VertexBlend = VertexBlend.Disable;
                renderState.IndexedVertexBlendEnable = false;
                renderState.FogEnable = false;
                renderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
                textureState0.ColorOperation = TextureOperation.Modulate;
                textureState0.ColorArgument1 = TextureArgument.TextureColor;
                textureState0.ColorArgument2 = TextureArgument.Diffuse;
                textureState0.AlphaOperation = TextureOperation.Modulate;
                textureState0.AlphaArgument1 = TextureArgument.TextureColor;
                textureState0.AlphaArgument2 = TextureArgument.Diffuse;
                textureState0.TextureCoordinateIndex = 0;
                textureState0.TextureTransform = TextureTransform.Disable;
                // REVIEW
                textureState1.ColorOperation = TextureOperation.Disable;
                textureState1.AlphaOperation = TextureOperation.Disable;
                samplerState0.MinFilter = TextureFilter.Point;
                samplerState0.MagFilter = TextureFilter.Point;
                samplerState0.MipFilter = TextureFilter.None;

                if (which == 0)
                {
                    savedStateBlock = device.EndStateBlock();
                }
                else
                {
                    drawTextStateBlock = device.EndStateBlock();
                }
            }
        }
        //RestoreDeviceObjects


        //-----------------------------------------------------------------------------
        // Name: DrawText
        // Desc: Draw some text on the screen
        //-----------------------------------------------------------------------------
        public void DrawText(float sx, float sy, Color color, string strText, RenderFlags flags)
        {
            if (strText == null)
            {
                return;
            }
            // Setup renderstate
            savedStateBlock.Capture();
            drawTextStateBlock.Apply();
            device.SetTexture(0, fontTexture);
            device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
            device.PixelShader = null;
            device.SetStreamSource(0, vertexBuffer, 0);

            // Set filter states
            if ((flags & RenderFlags.Filtered) != 0)
            {
                samplerState0.MinFilter = TextureFilter.Linear;
                samplerState0.MagFilter = TextureFilter.Linear;
            }

            float fStartX = sx;

            // Fill vertex buffer
            int iv = 0;
            int dwNumTriangles = 0;

            foreach (char c in strText)
            {
                if (c == '\n')
                {
                    sx = fStartX;
                    sy += (textureCoords[0, 3] - textureCoords[0, 1]) * textureHeight;
                }

                if ((((int)c) - 32) < 0 || (((int)c) - 32) >= (128 - 32))
                {
                    continue;
                }

                float tx1 = textureCoords[((int)c) - 32, 0];
                float ty1 = textureCoords[((int)c) - 32, 1];
                float tx2 = textureCoords[((int)c) - 32, 2];
                float ty2 = textureCoords[((int)c) - 32, 3];

                float w = (tx2 - tx1) * textureWidth / textureScale;
                float h = (ty2 - ty1) * textureHeight / textureScale;

                int intColor = color.ToArgb();
                if (c != ' ')
                {
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + h - 0.5f, 0.9f, 1f), intColor, tx1, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + 0 - 0.5f, 0.9f, 1f), intColor, tx1, ty1);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + h - 0.5f, 0.9f, 1f), intColor, tx2, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + 0 - 0.5f, 0.9f, 1f), intColor, tx2, ty1);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + h - 0.5f, 0.9f, 1f), intColor, tx2, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + 0 - 0.5f, 0.9f, 1f), intColor, tx1, ty1);
                    iv += 1;
                    dwNumTriangles += 2;

                    if (dwNumTriangles * 3 > MaxNumfontVertices - 6)
                    {
                        // Set the data for the vertexbuffer
                        vertexBuffer.SetData(fontVertices, 0, LockFlags.Discard);
                        device.DrawPrimitives(PrimitiveType.TriangleList, 0, dwNumTriangles);
                        dwNumTriangles = 0;
                        iv = 0;
                    }
                }

                sx += w;
            }

            // Set the data for the vertex buffer
            vertexBuffer.SetData(fontVertices, 0, LockFlags.Discard);
            if (dwNumTriangles > 0)
            {
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, dwNumTriangles);
            }
            // Restore the modified renderstates
            savedStateBlock.Apply();
        }
        //DrawText




        //-----------------------------------------------------------------------------
        // Name: DrawTextScaled()
        // Desc: Draws scaled 2D text. Note that x and y are in viewport coordinates
        // (ranging from -1 to +1). fXScale and fYScale are the size fraction
        // relative to the entire viewport. For example, a fXScale of 0.25 is
        // 1/8th of the screen width. This allows you to output text at a fixed
        // fraction of the viewport, even if the screen or window size changes.
        //-----------------------------------------------------------------------------
        public void DrawTextScaled(float x, float y, float z, float fXScale, float fYScale, System.Drawing.Color color, string text, RenderFlags flags)
        {
            if (device == null)
            {
                throw new System.ArgumentNullException();
            }
            // Set up renderstate
            savedStateBlock.Capture();
            drawTextStateBlock.Apply();
            device.VertexFormat = CustomVertex.TransformedColoredTextured.Format;
            device.PixelShader = null;
            device.SetStreamSource(0, vertexBuffer, 0);

            // Set filter states
            if ((flags & RenderFlags.Filtered) != 0)
            {
                samplerState0.MinFilter = TextureFilter.Linear;
                samplerState0.MagFilter = TextureFilter.Linear;
            }

            Viewport vp = device.Viewport;
            float sx = (x + 1f) * vp.Width / 2;
            float sy = (y + 1f) * vp.Height / 2;
            float sz = z;
            float rhw = 1f;
            float fLineHeight = (textureCoords[0, 3] - textureCoords[0, 1]) * textureHeight;

            // Adjust for character spacing
            sx -= spacingChar * (fXScale * vp.Height) / fLineHeight;
            float fStartX = sx;

            // Fill vertex buffer
            int numTriangles = 0;
            int realColor = color.ToArgb();
            int iv = 0;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    sx = fStartX;
                    sy += fYScale * vp.Height;
                }

                if ((((int)c) - 32) < 0 || (((int)c) - 32) >= (128 - 32))
                {
                    continue;
                }

                float tx1 = textureCoords[(int)c - 32, 0];
                float ty1 = textureCoords[(int)c - 32, 1];
                float tx2 = textureCoords[(int)c - 32, 2];
                float ty2 = textureCoords[(int)c - 32, 3];

                float w = (tx2 - tx1) * textureWidth;
                float h = (ty2 - ty1) * textureHeight;

                w *= fXScale * vp.Height / fLineHeight;
                h *= fYScale * vp.Height / fLineHeight;

                if (c != ' ')
                {
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + h - 0.5f, sz, rhw), realColor, tx1, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + 0 - 0.5f, sz, rhw), realColor, tx1, ty1);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + h - 0.5f, sz, rhw), realColor, tx2, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + 0 - 0.5f, sz, rhw), realColor, tx2, ty1);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + w - 0.5f, sy + h - 0.5f, sz, rhw), realColor, tx2, ty2);
                    iv += 1;
                    fontVertices[iv] = new CustomVertex.TransformedColoredTextured(new Vector4(sx + 0 - 0.5f, sy + 0 - 0.5f, sz, rhw), realColor, tx1, ty1);
                    iv += 1;
                    numTriangles += 2;

                    if (numTriangles * 3 > MaxNumfontVertices - 6)
                    {
                        // Unlock, render, and relock the vertex buffer
                        vertexBuffer.SetData(fontVertices, 0, LockFlags.Discard);
                        device.DrawPrimitives(PrimitiveType.TriangleList, 0, numTriangles);
                        numTriangles = 0;
                        iv = 0;
                    }
                }

                sx += w - 2 * spacingChar * (fXScale * vp.Height) / fLineHeight;
            }

            // Unlock and render the vertex buffer
            vertexBuffer.SetData(fontVertices, 0, LockFlags.Discard);
            if (numTriangles > 0)
            {
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, numTriangles);
            }
            // Restore the modified renderstates
            savedStateBlock.Apply();
        }
        //DrawTextScaled

        public void DrawTextScaled(float x, float y, float z, float fXScale, float fYScale, System.Drawing.Color color, string text)
        {
            this.DrawTextScaled(x, y, z, fXScale, fYScale, color, text, 0);
        }
        //DrawTextScaled



        //-----------------------------------------------------------------------------
        // Name: Render3DText()
        // Desc: Renders 3D text
        //-----------------------------------------------------------------------------
        public void Render3DText(string text, RenderFlags flags)
        {
            if (device == null)
            {
                throw new System.ArgumentNullException();
            }

            // Set up renderstate
            savedStateBlock.Capture();
            drawTextStateBlock.Apply();
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            device.PixelShader = null;
            device.SetStreamSource(0, vertexBuffer, 0, VertexInformation.GetFormatSize(CustomVertex.PositionNormalTextured.Format));

            // Set filter states
            if ((flags & RenderFlags.Filtered) != 0)
            {
                samplerState0.MinFilter = TextureFilter.Linear;
                samplerState0.MagFilter = TextureFilter.Linear;
            }

            // Position for each text element
            float x = 0f;
            float y = 0f;

            // Center the text block at the origin
            if ((flags & RenderFlags.Centered) != 0)
            {
                System.Drawing.SizeF sz = GetTextExtent(text);
                x = -((float)sz.Width / 10f) / 2f;
                y = -((float)sz.Height / 10f) / 2f;
            }

            // Turn off culling for two-sided text
            if ((flags & RenderFlags.TwoSided) != 0)
            {
                renderState.CullMode = Cull.None;
            }
            // Adjust for character spacing
            x -= spacingChar / 10f;
            float fStartX = x;

            // Fill vertex buffer
            GraphicsStream strm = vertexBuffer.Lock(0, 0, LockFlags.Discard);
            int numTriangles = 0;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    x = fStartX;
                    y -= (textureCoords[0, 3] - textureCoords[0, 1]) * textureHeight / 10f;
                }

                if (((int)c - 32) < 0 || ((int)c - 32) >= (128 - 32))
                {
                    continue;
                }
                float tx1 = textureCoords[(int)c - 32, 0];
                float ty1 = textureCoords[(int)c - 32, 1];
                float tx2 = textureCoords[(int)c - 32, 2];
                float ty2 = textureCoords[(int)c - 32, 3];

                float w = (tx2 - tx1) * textureWidth / (10f * textureScale);
                float h = (ty2 - ty1) * textureHeight / (10f * textureScale);

                if (c != ' ')
                {
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + 0, y + 0, 0), new Vector3(0, 0, -1), tx1, ty2));
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + 0, y + h, 0), new Vector3(0, 0, -1), tx1, ty1));
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + w, y + 0, 0), new Vector3(0, 0, -1), tx2, ty2));
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + w, y + h, 0), new Vector3(0, 0, -1), tx2, ty1));
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + w, y + 0, 0), new Vector3(0, 0, -1), tx2, ty2));
                    strm.Write(new CustomVertex.PositionNormalTextured(new Vector3(x + 0, y + h, 0), new Vector3(0, 0, -1), tx1, ty1));
                    numTriangles += 2;

                    if (numTriangles * 3 > MaxNumfontVertices - 6)
                    {
                        // Unlock, render, and relock the vertex buffer
                        vertexBuffer.Unlock();
                        device.DrawPrimitives(PrimitiveType.TriangleList, 0, numTriangles);
                        strm = vertexBuffer.Lock(0, 0, LockFlags.Discard);
                        numTriangles = 0;
                    }
                }

                x += w - 2 * spacingChar / 10f;
            }

            // Unlock and render the vertex buffer
            vertexBuffer.Unlock();
            if (numTriangles > 0)
            {
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, numTriangles);
            }
            // Restore the modified renderstates
            savedStateBlock.Apply();
        }
        //Render3DText


        //-----------------------------------------------------------------------------
        // Name: GetTextExtent()
        // Desc: Get the dimensions of a text string
        //-----------------------------------------------------------------------------
        public System.Drawing.SizeF GetTextExtent(string text)
        {
            if (null == text | text == string.Empty)
            {
                throw new System.ArgumentNullException();
            }

            float fRowWidth = 0f;
            float fRowHeight = (textureCoords[0, 3] - textureCoords[0, 1]) * textureHeight;
            float fWidth = 0f;
            float fHeight = fRowHeight;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    fRowWidth = 0f;
                    fHeight += fRowHeight;
                }

                if (((int)c - 32) < 0 || ((int)c - 32) >= (128 - 32))
                {
                    continue;
                }
                float tx1 = textureCoords[(int)c - 32, 0];
                float tx2 = textureCoords[(int)c - 32, 2];

                fRowWidth += (tx2 - tx1) * (textureWidth - 2 * spacingChar);

                if (fRowWidth > fWidth)
                {
                    fWidth = fRowWidth;
                }
            }

            return new System.Drawing.SizeF(fWidth, fHeight);
        }
        //GetTextExtent



        //-----------------------------------------------------------------------------
        // Name: Dispose
        // Desc: Cleanup any resources being used
        //-----------------------------------------------------------------------------
        public void Dispose(object sender, EventArgs e)
        {
            if ((systemFont != null))
            {
                systemFont.Dispose();
            }
            systemFont = null;
        }
        //Dispose
    }
    //GraphicsFont
}
