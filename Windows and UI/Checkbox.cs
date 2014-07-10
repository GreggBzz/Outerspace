// CCheckbox class
// Class for all checkbox and radio button controls

using System;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace 
{
    public class CCheckbox : CWindow 
    {        
        private bool isChecked = false;
        private bool bRadioMode = false;
        private int iGroupID = -1;
        
        public CCheckbox(string caption, int x, int y, int width, int height, 
            System.Drawing.Color wndcolor, ref CWindow pWnd)
            :this(caption, x, y, width, height, wndcolor, ref pWnd, true, false, -1)
        {
        }
            
        public CCheckbox(string caption, int x, int y, int width, int height, 
            System.Drawing.Color wndcolor, ref CWindow pWnd, bool showWnd)
            : this(caption, x, y, width, height, wndcolor, ref pWnd, showWnd, false, -1)
        {
        } 
        
        public CCheckbox(string caption, int x, int y, int width, int height, 
            System.Drawing.Color wndcolor, ref CWindow pWnd, bool showWnd, 
            bool radio)
            : this(caption, x, y, width, height, wndcolor, ref pWnd, showWnd, radio, -1)
        {
        }

        public CCheckbox(string caption, int x, int y, int width, int height, 
            System.Drawing.Color wndcolor, ref CWindow pWnd, bool showWnd, 
            bool radio, int group)
        {
            IsControl = true;
            Parent = pWnd;
            IsModal = false;
            SetRadioMode(radio);
            SetGroupID(group);

            if (width < 16) width = 16;
            if (height < 16) height = 16;

            // windowRectangle is relative to the parent top, left corner
            Create("CCheckbox", caption, x, y, (x + width), (y + height), wndcolor);

            LoadSprite(2, "chkbox_unchecked.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(3, "chkbox_checked.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(4, "radiobtn_unchecked.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(5, "radiobtn_checked.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());

            Show(showWnd);
        }
        
        public void SetRadioMode(bool value) 
        {
            bRadioMode = value;
        }
        
        public void SetCheck(bool value) 
        {
            isChecked = value;
        }
        
        public bool GetCheck() 
        {
            return isChecked;
        }
        
        public void SetGroupID(int value) 
        {
            iGroupID = value;
        }
        
        public int GetGroupID() 
        {
            return iGroupID;
        }
        
        public override void Draw() 
        {
            Rectangle rcParent = parent.GetWindowRect();

            if (IsVisible) 
            {
                // draw a different style of background than the ancestor
                // DrawBackground()
                OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

                if (bRadioMode) 
                {
                    // draw single circle sprite (on or off)
                    if (IsSpriteLoaded[3] && IsSpriteLoaded[4]) 
                    {
                        if (!isChecked) 
                        {
                            CornerSprite[2].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top,
                                new Rectangle(0, 0, 16, 16), 0, WindowColor);
                        }
                        else 
                        {
                            CornerSprite[3].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top,
                                new Rectangle(0, 0, 16, 16), 0, WindowColor);
                        }
                    }
                }
                else 
                {
                    // draw single square sprite (on or off)
                    if (IsSpriteLoaded[1] && IsSpriteLoaded[2]) 
                    {
                        if (!isChecked) 
                        {
                            CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top, 
                                new Rectangle(0, 0, 16, 16), 0, WindowColor);
                        }
                        else 
                        {
                            CornerSprite[1].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top,
                                new Rectangle(0, 0, 16, 16), 0, WindowColor);
                        }
                    }
                }

                OuterSpace.spriteobj.Flush();
                OuterSpace.spriteobj.End();
                
                // draw text
                OuterSpace.textfont.DrawText(rcParent.Left + windowRectangle.Left + 20, rcParent.Top + windowRectangle.Top, 
                    Color.White, GetWindowText());
            }
        }
        
        public override bool HandleInput(int ctrlindex) 
        {
            bool bHandled = false;
            Point aPoint = new System.Drawing.Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

            // check mouse
            if (PtInRect(aPoint)) 
            {
                if (OuterSpace.userinput.CheckButton("Left", true)) 
                {
                    if (parent != null) 
                    {
                        if (bRadioMode) 
                        {
                            isChecked = true;
                        }
                        else 
                        {
                            isChecked = !isChecked;
                        }

                        parent.NewMessage(ctrlindex, WindowMessages.LeftMouseButtonClicked);
                    }

                    bHandled = true;
                }
            }

            return bHandled;
        }
    }
}
