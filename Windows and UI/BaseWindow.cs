// CWindow class
// Base class for all windows

using System;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CWindow
    {        
        #region Enumerations

        public enum WindowMessages
        {
            Nothing = 0,
            LeftMouseButtonClicked = 1,
            RightMouseButtonClicked = 2,
            AlphaNumericKeyPressed, 
            TextChanged,
            MessageBoxOK,
            MessageBoxCancel,
            MessageBoxYes,
            MessageBoxNo
        }

        #endregion

        #region Member Variables

        protected string caption;
        protected Rectangle windowRectangle;
        protected bool visible;
        protected Color windowColor;
        protected SpriteClass backgroundsprite = null;
        protected SpriteClass[] cornersprite = new SpriteClass[4];
        protected SpriteClass[] borderSprite = new SpriteClass[4];
        protected SpriteClass captionSprite = null;
        protected bool[] spriteLoaded = new bool[10];
        protected string className;
        protected bool modal = false;
        protected int windowIndexInManager = 0;
        protected bool hasFocus = false;
        protected CWindow parent = null;
        protected Point ptDragging = new Point(-1, -1);
        protected bool dragging = false;
        protected bool control = false;
        protected bool moveable = true;

        #endregion

        #region Properties

        protected string Caption
        {
          get { return caption; }
          set { caption = value; }
        }        

        protected Rectangle WindowRectangle
        {
            get { return windowRectangle; }
            set { windowRectangle = value; }
        }        

        public bool IsVisible
        {
          get { return visible; }
          set { visible = value; }
        }
        
        protected Color WindowColor
        {
          get { return windowColor; }
          set { windowColor = value; }
        }        

        protected SpriteClass BackgroundSprite
        {
          get { return backgroundsprite; }
          set { backgroundsprite = value; }
        }
        
        protected SpriteClass[] CornerSprite
        {
          get { return cornersprite; }
            set { cornersprite = value; }
        }
        
        protected SpriteClass[] BorderSprite
        {
          get { return borderSprite; }
          set { borderSprite = value; }
        }
        
        protected SpriteClass CaptionSprite
        {
          get { return captionSprite; }
          set { captionSprite = value; }
        }
        
        protected bool[] IsSpriteLoaded
        {
          get { return spriteLoaded; }
          set { spriteLoaded = value; }
        }
        
        public string ClassName
        {
          get { return className; }
          protected set { className = value; }
        }
        
        public bool IsModal
        {
          get { return modal; }
          protected set { modal = value; }
        }
        
        public int WindowIndexInManager
        {
          get { return windowIndexInManager; }
          set { windowIndexInManager = value; }
        }
        
        public bool HasFocus
        {
          get { return hasFocus; }
          protected set { hasFocus = value; }
        }
        
        public CWindow Parent
        {
          get { return parent; }
          set { parent = value; }
        }

        protected Point PtDragging
        {
          get { return ptDragging; }
          set { ptDragging = value; }
        }
        
        protected bool IsDragging
        {
          get { return dragging; }
          set { dragging = value; }
        }
        
        protected bool IsControl
        {
          get { return control; }
          set { control = value; }
        }

        #endregion

        #region Methods

        public void Create(string nameOfClass, string caption, int left, int top, int right, 
            int bottom, Color wndColor)
        {
            ClassName = nameOfClass;
            SetWindowText(caption);
            windowColor = wndColor;
            SetWindowRect(left, top, right, bottom);
            Show(false);
                        
            for (int i = 0; i < 10; i++)
            {
                IsSpriteLoaded[i] = false;
            }
        }

        public void DoModal()
        {
            IsModal = true;
            Show(true);
        }

        public void LoadSprite(int spriteNum, string spriteFile, int width, int height, int colorKey)
        {
            switch (spriteNum)
            {
                case 1:
                    BackgroundSprite = new SpriteClass(spriteFile, width, height, colorKey);
                    for (int i = 0; i < 9; i++)
                    {
                        Point pt = new Point((i % 3) * 16, (int)(i / 3) * 16);
                        BackgroundSprite.sourceFrame[i] = new Rectangle(pt.X, pt.Y, 16, 16);
                    }
                    break;
                case 2:
                    CornerSprite[0] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 3:
                    CornerSprite[1] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 4:
                    CornerSprite[2] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 5:
                    CornerSprite[3] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 6:
                    BorderSprite[0] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 7:
                    BorderSprite[1] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 8:
                    BorderSprite[2] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 9:
                    BorderSprite[3] = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
                case 10:
                    CaptionSprite = new SpriteClass(spriteFile, width, height, colorKey);
                    break;
            }

            IsSpriteLoaded[spriteNum - 1] = true;
        }

        public void SetWindowText(string value)
        {
            Caption = value;
        }

        public string GetWindowText()
        {
            return Caption;
        }

        protected void SetWindowRect(Rectangle rc)
        {
            WindowRectangle = rc;
        }

        protected void SetWindowRect(int left, int top, int right, int bottom)
        {
            windowRectangle.Location = new Point(left, top);
            windowRectangle.Size = new Size(right - left, bottom - top);
        }

        public Rectangle GetWindowRect()
        {
            return WindowRectangle;
        }

        public bool PtInRect(Point PtToTest)
        {
            ScreenToClient(ref PtToTest);

            if (PtToTest.X >= windowRectangle.Left && PtToTest.X <= windowRectangle.Right)
            {
                if (PtToTest.Y >= windowRectangle.Top && PtToTest.Y <= windowRectangle.Bottom)
                    return true;
            }

            return false;
        }

        public void ScreenToClient(ref Point PtToChange)
        {
            if (IsControl)
            {
                Rectangle rcParent = Parent.GetWindowRect();

                // make pt relative to parent
                PtToChange.X = PtToChange.X - rcParent.Left;
                PtToChange.Y = PtToChange.Y - rcParent.Top;
            }
        }

        public virtual void Draw()
        {
            if (IsVisible)
                DrawBackground();
        }
        
        public void DrawBackground()
        {
            int i, i2, frame;
            Rectangle srcCornerRect = new Rectangle(0, 0, 16, 16);

            frame = 0;

            OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

            //draw background
            if (IsSpriteLoaded[0] == true) 
            {
                for (i = windowRectangle.Left; i <= (windowRectangle.Right - 16); i += 16) 
                {  
                    if (i < (windowRectangle.Right - 16) && i > windowRectangle.Left)
                        frame = 1;
                   
                    BackgroundSprite.Draw(i, windowRectangle.Top, BackgroundSprite.sourceFrame[frame], 0, WindowColor);
                   
                    i2 = windowRectangle.Top + 16;
                    while (i2 < (windowRectangle.Bottom - 16)) 
                    {
                        BackgroundSprite.Draw(i, i2, BackgroundSprite.sourceFrame[frame + 3], 0, WindowColor);
                        i2 = i2 + 16;
                    }

                    BackgroundSprite.Draw(i, windowRectangle.Bottom - 16, BackgroundSprite.sourceFrame[frame + 6], 0, WindowColor);

                    frame++;
                }
            }
           
            //draw corners
            if (IsSpriteLoaded[1]) 
            {
                CornerSprite[0].Draw(windowRectangle.Left, windowRectangle.Top, new Rectangle(0, 0, 16, 32), 0,
                    Color.FromArgb(255, 255, 255, 255));
            }

            if (IsSpriteLoaded[2]) 
            {
                CornerSprite[1].Draw(windowRectangle.Right - CornerSprite[1].spritewidth, windowRectangle.Top,
                    new Rectangle(0, 0, 16, 32), 0, Color.FromArgb(255, 255, 255, 255));
            }

            if (IsSpriteLoaded[3]) 
            {
                CornerSprite[2].Draw(windowRectangle.Right - CornerSprite[1].spritewidth,
                    windowRectangle.Bottom - CornerSprite[2].spriteheight, srcCornerRect, 0,
                    Color.FromArgb(255, 255, 255, 255));
            }

            if (IsSpriteLoaded[4]) 
            {
                CornerSprite[3].Draw(windowRectangle.Left, windowRectangle.Bottom - CornerSprite[3].spriteheight,
                    srcCornerRect, 0, Color.FromArgb(255, 255, 255, 255));
            }
           
            //draw borders
            if (IsSpriteLoaded[5]) 
            {
                for (i = (windowRectangle.Left + BorderSprite[0].spritewidth); i <= (windowRectangle.Right - (BorderSprite[0].spritewidth * 2)); i += BorderSprite[0].spritewidth) 
                {
                    BorderSprite[0].Draw(i, windowRectangle.Top, new Rectangle(0, 0, 16, 32), 0, 
                        Color.FromArgb(255, 255, 255, 255));
                }
            }
            
            if (IsSpriteLoaded[6]) 
            {
                for (i = (windowRectangle.Top + CornerSprite[1].spriteheight); i <= (windowRectangle.Bottom - (CornerSprite[2].spriteheight * 2)); i += BorderSprite[1].spriteheight) 
                {
                    BorderSprite[1].Draw(windowRectangle.Right - BorderSprite[1].spritewidth, i, 
                        srcCornerRect, 0, Color.FromArgb(255, 255, 255, 255));
                }
            }
            
            if (IsSpriteLoaded[7]) 
            {
                for (i = (windowRectangle.Right - (CornerSprite[1].spritewidth * 2)); i >= (windowRectangle.Left + CornerSprite[3].spritewidth); i += -BorderSprite[2].spritewidth) 
                {
                    BorderSprite[2].Draw(i, windowRectangle.Bottom - BorderSprite[2].spriteheight, 
                        srcCornerRect, 0, Color.FromArgb(255, 255, 255, 255));
                }
            }

            if (IsSpriteLoaded[8]) 
            {
                for (i = (windowRectangle.Bottom - (CornerSprite[3].spriteheight * 2)); i >= (windowRectangle.Top + CornerSprite[0].spriteheight); i += -BorderSprite[3].spriteheight) 
                {
                    BorderSprite[3].Draw(windowRectangle.Left, i, srcCornerRect, 0, 
                        Color.FromArgb(255, 255, 255, 255));
                }
            }
           
            //draw caption
            if (IsSpriteLoaded[9]) 
            {
                int width = (CaptionSprite.spritewidth / 3);
                int newwidth = width;
                Rectangle captionSrcRect = new Rectangle(width, 0, width, CaptionSprite.spriteheight);
               
                i = windowRectangle.Left;
                CaptionSprite.Draw(windowRectangle.Left, windowRectangle.Top,
                    new Rectangle(0, 0, width, CaptionSprite.spriteheight), 0, WindowColor);
               
                i += width;
                do 
                {
                    CaptionSprite.Draw(i, windowRectangle.Top, captionSrcRect, 0, WindowColor);
                   
                    i += newwidth;
                   
                    if (i + newwidth > windowRectangle.Right - newwidth) 
                    {
                        newwidth = (windowRectangle.Right - newwidth) - i;
                    }
                }
                while (i < (windowRectangle.Right - newwidth) & newwidth > 0);

                CaptionSprite.Draw(i, windowRectangle.Top,
                    new Rectangle(width * 2, 0, width, CaptionSprite.spriteheight), 0, WindowColor);
            }
           
            OuterSpace.spriteobj.Flush();
            OuterSpace.spriteobj.End();
           
            //draw caption text
            OuterSpace.textfont.DrawText(windowRectangle.Left + 12, windowRectangle.Top + 8, Color.Black, GetWindowText());
            OuterSpace.textfont.DrawText(windowRectangle.Left + 10, windowRectangle.Top + 6, WindowColor, GetWindowText());
        }

        public virtual bool RunWindow()
        {
            bool bHandled = false;
            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

            // only handle a caption drag
            if (PtInRect(aPoint) || IsDragging)
            {
                if ((aPoint.Y > (windowRectangle.Top + 7) && aPoint.Y < (windowRectangle.Top + 23)) || IsDragging)
                {
                    // its in the caption
                    bHandled = HandleInput(0);
                }
            }

            return bHandled;
        }

        public virtual bool HandleInput(int ctrlindex)
        {
            bool bHandled = false;
            Point newPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
            int xdiff = 0;
            int ydiff = 0;

            if (ctrlindex == 0 && OuterSpace.drawmouse)
            {
                if (OuterSpace.userinput.CheckButton("Left", false))
                {
                    if (IsDragging)
                    {
                        // we're dragging!
                        xdiff = newPoint.X - ptDragging.X;
                        ydiff = newPoint.Y - ptDragging.Y;

                        SetWindowRect(windowRectangle.Left + xdiff,
                            windowRectangle.Top + ydiff,
                            windowRectangle.Right + xdiff,
                            windowRectangle.Bottom + ydiff);

                        ptDragging = newPoint;

                        bHandled = true;
                    }
                    else
                    {
                        if (moveable)
                        {
                            //maybe starting to drag
                            IsDragging = true;
                            ptDragging = newPoint;
                            bHandled = true;
                        }
                    }
                }
                else
                {
                    IsDragging = false;
                    bHandled = false;
                }
            }
            
            return bHandled;
        }

        public void Show(bool value)
        {
            IsVisible = value;
        }

        public virtual void SetFocus()
        {
            HasFocus = true;
        }

        public virtual void KillFocus()
        {
            HasFocus = false;

            if (WindowIndexInManager > 1)
                KillControlFocus();
        }

        public virtual void KillControlFocus()
        {
            // override in descendents
        }
        
        public virtual void NewMessage(int ctrlIndex, WindowMessages message)
        {
            // override in main windows, called by controls
        }

        public void Close()
        {
            OuterSpace.theWindowMgr.RemoveWindow(WindowIndexInManager);
        }

        public virtual void Dispose()
        {
            // override
        }

        #endregion
    }    
}