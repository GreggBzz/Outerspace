// CStaticText class
// Class for all statictext controls

using System;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace 
{
    public class CStaticText : CWindow 
    {       
        private int lineheight = 0;        
        private int maxlines = 0;        
        private Color txtColor = Color.White;        
        private Color lnkColor = Color.Cyan;        
        public bool bHyperlink = false;
        private string[] splittext = new string[40];
        
        public CStaticText(string caption, int x, int y, int width, int height, 
            Color wndcolor, ref CWindow pWnd)
            :this(caption, x, y, width, height, wndcolor, ref pWnd, true, false)
        {
        }

        public CStaticText(string caption, int x, int y, int width, int height,
            Color wndcolor, ref CWindow pWnd, bool showWnd)
            : this(caption, x, y, width, height, wndcolor, ref pWnd, showWnd, false)
        {
        }

        public CStaticText(string caption, int x, int y, int width, int height, 
            Color wndcolor, ref CWindow pWnd, bool showWnd, bool link) 
        {
            IsControl = true;
            Parent = pWnd;
            IsModal = false;

            if (link) 
            {
                SetAsHyperlink();
            }

            if (width < 64) width = 64;
            if (height < 32) height = 32;
            
            // windowRectangle is relative to the parent top, left corner
            Create("CStaticText", caption, x, y, (x + width), (y + height), wndcolor);

            lineheight = OuterSpace.textfont.GetTextExtent("X").ToSize().Height;
            
            maxlines = Convert.ToInt32((windowRectangle.Height - 10) / lineheight);

            splittext.Initialize();
            
            Show(showWnd);
        }
        
        public void ChangeCaption(string newCaption) 
        {
            this.caption = newCaption;
        }
        
        public void ChangeColor(System.Drawing.Color newColor, bool link) 
        {
            if (link) 
                this.lnkColor = newColor;
            else 
                this.txtColor = newColor;
        }
        
        public void SetAsHyperlink() 
        {
            bHyperlink = true;
        }
        
        public new void SetFocus() 
        {
            base.SetFocus();
        }
        
        public override void Draw() 
        {
            Rectangle rcParent = Parent.GetWindowRect();

            if (IsVisible) 
            {
                // draw text, no border or background
                SplitTheText(GetWindowText());

                for (int i = 0; i < maxlines; i++) 
                {
                    if (bHyperlink) 
                    {
                        OuterSpace.linkfont.DrawText((rcParent.Left + (windowRectangle.Left + 5)), 
                            (rcParent.Top + (windowRectangle.Top + (5 + (lineheight * i)))), 
                            lnkColor, splittext[i]);
                    }
                    else 
                    {
                        OuterSpace.textfont.DrawText((rcParent.Left + (windowRectangle.Left + 5)), 
                            (rcParent.Top + (windowRectangle.Top + (5 + (lineheight * i)))), 
                            txtColor, splittext[i]);
                    }
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

            return bHandled;
        }
        
        protected void SplitTheText(string newtext) 
        {
            int i;
            int index = 0;
            string strTemp = newtext;
            Size sz;
            
            for (i = 0; i < maxlines; i++) 
            {
                splittext[i] = "";
            }

            if (newtext == "") return;
            
            i = 1;
            
            do
            {
                sz = OuterSpace.textfont.GetTextExtent(strTemp.Substring(0, i)).ToSize();
                if (sz.Width > windowRectangle.Width) 
                {
                    splittext[index] = strTemp.Substring(0, (i - 1));
                    strTemp = strTemp.Substring((i - 1), ((strTemp.Length - i) + 1));
                    i = 0;
                    index++;
                }
                i++;
            }
            while (i <= strTemp.Length && index <= maxlines);

            splittext[index] = strTemp;
        }
    }
}