// CButton class
// Class for all button controls
using System;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace 
{   
    public class CButton : CWindow 
    {
        public CButton(string caption, int x, int y, int width, int height, Color wndcolor, ref CWindow pWnd)
            :this(caption, x, y, width, height, wndcolor, ref pWnd, true)
        {
        }

        public CButton(string caption, int x, int y, int width, int height, 
            Color wndcolor, ref CWindow pWnd, bool showWnd) 
        {
            IsControl = true;
            Parent = pWnd;
            IsModal = false;

            if (width < 64) width = 64;
            if (height < 32) height = 32;

            // windowRectangle is relative to the parent top, left corner
            Create("CButton", caption, x, y, (x + width), (y + height), wndcolor);

            LoadSprite(2, "buttonleft.bmp", 32, 32, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(3, "buttoncenter.bmp", 32, 32, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(4, "buttonright.bmp", 32, 32, Color.FromArgb(255,0,0,0).ToArgb());

            Show(showWnd);
        }
        
        public override void Draw() 
        {
            int i;
            int iTextPosX;
            int iTextPosY;
            System.Drawing.SizeF szText;
            Rectangle btnSrcRect = new Rectangle(0, 0, 32, 32);
            Rectangle rcParent = parent.GetWindowRect();

            if (IsVisible) 
            {
                // draw a different style of background than the ancestor
                // DrawBackground()
                OuterSpace.spriteobj.Begin((SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront));

                if (IsSpriteLoaded[1]) 
                {
                    CornerSprite[0].Draw((rcParent.Left + windowRectangle.Left), (rcParent.Top + windowRectangle.Top), btnSrcRect, 0, WindowColor);
                }

                if (IsSpriteLoaded[2]) 
                {
                    i = (windowRectangle.Left + 32);
                    int width = Math.Min((windowRectangle.Width - 64), 32);

                    do
                    {
                        Rectangle btnSrcRect2 = new Rectangle(0, 0, width, 32);
                        CornerSprite[1].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Top,
                            btnSrcRect2, 0, WindowColor);
                        i += 32;

                        if ((i + width) > (windowRectangle.Right - 32))
                        {
                            width = (windowRectangle.Right - 32) - i;
                        }
                    }
                    while (i < (windowRectangle.Right - 32) && width > 0);
                }

                if (IsSpriteLoaded[3]) 
                {
                    CornerSprite[2].Draw(rcParent.Left + (windowRectangle.Right - 32),
                        rcParent.Top + windowRectangle.Top, btnSrcRect, 0, WindowColor);
                }

                OuterSpace.spriteobj.Flush();
                OuterSpace.spriteobj.End();
                
                // draw caption, centered
                // find avg width and height of characters
                szText = OuterSpace.textfont.GetTextExtent(caption);
                iTextPosX = rcParent.Left + (int)((windowRectangle.Left + (windowRectangle.Width / 2)) - (szText.Width / 2));
                iTextPosY = rcParent.Top + (int)((windowRectangle.Top + (windowRectangle.Height / 2)) - (szText.Height / 2));

                // shadow height is 6 px
                OuterSpace.textfont.DrawText((iTextPosX + 2), (iTextPosY + 2), Color.Black, GetWindowText());
                OuterSpace.textfont.DrawText(iTextPosX, iTextPosY, WindowColor, GetWindowText());
            }
        }
        
        public override bool HandleInput(int ctrlindex) 
        {
            bool bHandled = false;

            Rectangle rcParent = parent.GetWindowRect();
            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
            
            // check mouse
            if (PtInRect(aPoint)) 
            {
                if (OuterSpace.userinput.CheckButton("Left", true)) 
                {
                    if (parent != null) 
                    {
                        parent.NewMessage(ctrlindex, WindowMessages.LeftMouseButtonClicked);
                    }

                    bHandled = true;
                }
            }
            return bHandled;
        }
    }
}