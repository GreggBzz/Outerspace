// CTextbox class
// Class for all textbox controls

using System;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace
{
    public class CTextBox : CWindow
    {
        protected int limit = -1;
        protected int lineheight = 0;
        protected int maxlines = 0;
        protected string[] splittext = new string[40];

        public CTextBox(string caption, int x, int y, int width, int height,
            System.Drawing.Color wndcolor, ref CWindow pWnd)
            : this(caption, x, y, width, height, wndcolor, ref pWnd, true, -1)
        {
        }

        public CTextBox(string caption, int x, int y, int width, int height,
           System.Drawing.Color wndcolor, ref CWindow pWnd, bool showWnd)
            : this(caption, x, y, width, height, wndcolor, ref pWnd, showWnd, -1)
        {
        }

        public CTextBox(string caption, int x, int y, int width, int height,
            System.Drawing.Color wndcolor, ref CWindow pWnd, bool showWnd,
            int charlimit)
        {
            IsControl = true;
            Parent = pWnd;
            IsModal = false;

            if (width < 64) width = 64;
            if (height < 32) height = 32;

            // windowRectangle is relative to the parent top, left corner
            Create("CTextbox", caption, x, y, x + width, y + height, wndcolor);

            LoadSprite(2, "textboxbackground.bmp", 48, 48, Color.FromArgb(255,0,0,0).ToArgb());

            lineheight = OuterSpace.textfont.GetTextExtent("X").ToSize().Height;
            maxlines = Convert.ToInt32((windowRectangle.Height - 10) / lineheight);

            splittext.Initialize();

            Show(showWnd);
        }

        public void SetLimit(int value)
        {
            limit = value;
        }

        public int GetLimit()
        {
            return limit;
        }

        public override void Dispose()
        {
            OuterSpace.userinput.EndKeyboardCapture();
            base.Dispose();
        }

        public override void SetFocus()
        {
            OuterSpace.userinput.StartKeyboardCapture();
            OuterSpace.userinput.text_input.myString = GetWindowText();

            base.SetFocus();
        }

        public override void Draw()
        {
            int i, j;
            int width = 0;
            int height = 0;
            Rectangle rcParent = parent.GetWindowRect();

            if (IsVisible)
            {
                // draw a different style of background than the ancestor
                // DrawBackground()
                OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

                if (IsSpriteLoaded[1])
                {
                    // top left corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top, 
                        new Rectangle(0, 0, 16, 16), 0, WindowColor);

                    // top edge
                    i = windowRectangle.Left + 16;
                    width = Math.Min(windowRectangle.Width - 32, 16);
                    
                    do
                    {
                        CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Top, 
                            new Rectangle(16, 0, width, 16), 0, WindowColor);

                        i += 16;
                        if (i + width > windowRectangle.Right - 16) 
                        {
                            width = (windowRectangle.Right - 16) - i;
                        }
                    }
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    // top right corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, rcParent.Top + windowRectangle.Top, 
                        new Rectangle(32, 0, 48, 16), 0, WindowColor);

                    // center
                    j = windowRectangle.Top + 16;
                    height = Math.Min(windowRectangle.Height - 32, 16);

                    do
                    {
                        i = windowRectangle.Left;
                        width = Math.Min(windowRectangle.Width - 32, 16);

                        do
                        {
                            if (i == windowRectangle.Left) 
                            {
                                CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j, 
                                    new Rectangle(0, 16, width, height), 0, WindowColor);
                            }
                            else if (i == windowRectangle.Right - 16) 
                            {
                                CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j,
                                    new Rectangle(32, 16, width, height), 0, WindowColor);
                            }
                            else
                            {
                                CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j,
                                    new Rectangle(16, 16, width, height), 0, WindowColor);
                            }

                            i += 16;
                            if (i + width > windowRectangle.Right - 16) 
                            {
                                width = (windowRectangle.Right - 16) - i;
                                if (width <= 0) 
                                {
                                    if (width == -16) 
                                    {
                                        i = windowRectangle.Right;
                                        width = 0;
                                    }
                                    else
                                    {
                                        i = windowRectangle.Right - 16;
                                        width = 16;
                                    }
                                }
                            }
                        }
                        while (i < windowRectangle.Right);

                        j += 16;
                        if (j + height > windowRectangle.Bottom - 16) 
                            height = (windowRectangle.Bottom - 16) - j;
                    }                        
                    while (j < (windowRectangle.Bottom - 16) && height > 0);

                    // bottom left corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Bottom - 16,
                        new Rectangle(0, 32, 16, 48), 0, WindowColor);

                    // bottom edge
                    i = windowRectangle.Left + 16;
                    width = Math.Min(windowRectangle.Width - 32, 16);

                    do
                    {
                        CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Bottom - 16,
                            new Rectangle(16, 32, width, 48), 0, WindowColor);

                        i += 16;
                        if (i + width > windowRectangle.Right - 16) 
                            width = (windowRectangle.Right - 16) - i;
                    }
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    // top right corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, 
                        rcParent.Top + windowRectangle.Bottom - 16,
                        new Rectangle(32, 32, 48, 48), 0, WindowColor);
                }

                OuterSpace.spriteobj.Flush();
                OuterSpace.spriteobj.End();

                // draw text
                SplitTheText(GetWindowText());
                for (i = 0; i < maxlines; i++)
                {
                    OuterSpace.textfont.DrawText(rcParent.Left + windowRectangle.Left + 5, 
                        rcParent.Top + windowRectangle.Top + 5 + (lineheight * i), 
                        Color.Black, splittext[i + 1]);
                }
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
                        parent.NewMessage(ctrlindex, WindowMessages.LeftMouseButtonClicked);
                    }

                    bHandled = true;
                }
            }

            if (!bHandled && HasFocus)
            {
                // check text input
                string strNewText = OuterSpace.userinput.text_input.myString;

                if (strNewText != GetWindowText())
                {
                    SetWindowText(strNewText);
                    bHandled = true;
                }
            }

            return bHandled;
        }

        protected void SplitTheText(string newtext)
        {
            int i, index;
            string strTemp = newtext;
            Size sz;

            for (i = 1; i < maxlines; i++)
            {
                splittext[i] = "";
            }

            if (newtext == "") return;

            if (newtext.Length > limit && limit > 0)
            {
                strTemp = newtext.Substring(0, limit);
                SetWindowText(strTemp);
                OuterSpace.userinput.text_input.myString = strTemp;
            }

            index = 1;
            i = 1;

            do
            {
                sz = OuterSpace.textfont.GetTextExtent(strTemp.Substring(0, i)).ToSize();
                if (sz.Width > windowRectangle.Width - 10)
                {
                    splittext[index] = (strTemp.Substring(0, i - 1));
                    strTemp = strTemp.Substring(i - 1, strTemp.Length - i + 1);
                    i = 0;
                    index += 1;
                }

                i += 1;
            }
            while (i <= strTemp.Length && index <= maxlines);

            splittext[index] = strTemp;
        }

    }
}