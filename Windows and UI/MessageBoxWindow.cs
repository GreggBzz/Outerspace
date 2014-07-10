//CMessageBox class
//Class for the captain's pda window

using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CMessageBoxWindow : CDialogWindow
    {       
        protected string strMessageText = "";
        protected int iIconToDisplay = 0;
        protected int iButtonsToDisplay = 0;
        protected int iParentBoxID = 1;
       
        public CMessageBoxWindow(string strTitle, string strMessage, int icon, int buttons, int boxid, ref CWindow myparent) 
            :base(strTitle, 0, 0, 0, 0, Color.FromArgb(255, 255, 255, 255))
        {
            CWindow thisWindow = (CWindow)this;
            iParentBoxID = boxid;
            Parent = myparent;
           
            int textleft;
            int texttop;
            int textwidth;
            int textheight;
            int anIcon = 0;
           
            if (icon > 0) anIcon = 1;
           
            strMessageText = strMessage;
            iIconToDisplay = icon;
            iButtonsToDisplay = buttons;
           
            SetMsgBoxRect();
           
            SetSprites();
           
            SetButtons(buttons);
           
            SetIcon(icon);
           
            //add static text for message
            textleft = 12 + (anIcon * 32) + (anIcon * 12);
            //12 pixel spacer + icon and spacer
            texttop = 44;
            //32 pixel caption + 12 pixel spacer
            textwidth = windowRectangle.Width - 24 - (anIcon * 32) - (anIcon * 12);
            //2, 12 pixel spacers + icon and spacer
            textheight = windowRectangle.Height - 100;
            //32 pixel caption + 3, 12 pixel spacers, + 32 pixel button
           
            controls.Add(new CStaticText(strMessage, textleft, texttop, textwidth, textheight, Color.FromArgb(255, 255, 255, 255), ref thisWindow));
           
            this.Show(true);
        }
       
        protected void SetMsgBoxRect()
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            int maxwidth;
            int maxheight;
            int txtwidth;
            int txtheight;
            int minheight;
            int minwidth;
            int tmpwidth;
            int tmpheight;
            int modres;
            Size sz;           
          
            minwidth = 196;
            minheight = 132;
            maxwidth = Convert.ToInt32(((OuterSpace.ClientArea.Center.X * 2) / 4) * 3);
            //3/4 of game width
            maxheight = Convert.ToInt32(((OuterSpace.ClientArea.Center.Y * 2) / 4) * 3);
            //3/4 of game height
           
            sz = OuterSpace.textfont.GetTextExtent(strMessageText).ToSize();
            txtwidth = sz.Width;
            txtheight = sz.Height;
           
            tmpwidth = Math.Min(txtwidth, maxwidth);
            if (iIconToDisplay > 0) tmpwidth += 32;
            tmpwidth += 24;
            //2, 12 pixel spacers on the sides, plus 5 pixel cushion
            tmpwidth = Math.Max(tmpwidth, minwidth);
           
            modres = tmpwidth % 16;
            if (!(modres == 0)) tmpwidth += 16 - modres;
           
            rect.Location = new Point(OuterSpace.ClientArea.Center.X - (tmpwidth / 2), rect.Top);
            rect.Size = new Size(tmpwidth, rect.Height);

            tmpheight = Convert.ToInt32(txtwidth / rect.Width) * txtheight;
            tmpheight += 68;
            //height of text area + 3, 12 pixel spacers and the 32 pixel caption
            tmpheight = Math.Max(tmpheight, minheight);
            tmpheight = Math.Min(tmpheight, maxheight);
           
            modres = tmpheight % 16;
            if (!(modres == 0)) tmpheight += 16 - modres;

            rect.Location = new Point(rect.Left, OuterSpace.ClientArea.Center.Y - (tmpheight / 2));
            rect.Size = new Size(rect.Width, tmpheight);
           
            SetWindowRect(rect);
        }
       
        public void SetIcon(int icon)
        {
            iIconToDisplay = 0; //not implemented
        }
       
        public void SetButtons(int buttons)
        {
            // 1 = OK
            // 2 = OK/Cancel
            // 3 = Yes/No
            string strText = "";
            string strText2 = "";

            if (buttons == 0) buttons = 1;
            if (buttons > 3) buttons = 3;
            iButtonsToDisplay = buttons;
           
            switch (buttons) 
            {
                case 1:
                    strText = "OK";
                    break;
                case 2:
                    strText = "OK";
                    strText2 = "Cancel";
                    break;
                case 3:
                    strText = "Yes";
                    strText2 = "No";
                    break;
            }
            
            CWindow thisWindow = (CWindow)this;

            //add first button
            controls.Add(new CButton(strText, 12, windowRectangle.Height - 44, 80, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
           
            if (buttons > 1) 
            {
                controls.Add(new CButton(strText2, 104, windowRectangle.Height - 44, 80, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
            }
        }
       
        protected void SetSprites()
        {
            define_sprite_struct[] defs = new define_sprite_struct[10];
           
            defs[0].number = 1;
            defs[0].file = "windowbackground.bmp";
            defs[0].width = 48;
            defs[0].height = 48;
            defs[0].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[1].number = 2;
            defs[1].file = "windowcorner1.bmp";
            defs[1].width = 16;
            defs[1].height = 32;
            defs[1].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[2].number = 3;
            defs[2].file = "windowcorner2.bmp";
            defs[2].width = 16;
            defs[2].height = 32;
            defs[2].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[3].number = 4;
            defs[3].file = "windowcorner3.bmp";
            defs[3].width = 16;
            defs[3].height = 16;
            defs[3].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[4].number = 5;
            defs[4].file = "windowcorner4.bmp";
            defs[4].width = 16;
            defs[4].height = 16;
            defs[4].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[5].number = 6;
            defs[5].file = "windowborder1.bmp";
            defs[5].width = 16;
            defs[5].height = 32;
            defs[5].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[6].number = 7;
            defs[6].file = "windowborder2.bmp";
            defs[6].width = 16;
            defs[6].height = 16;
            defs[6].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[7].number = 8;
            defs[7].file = "windowborder3.bmp";
            defs[7].width = 16;
            defs[7].height = 16;
            defs[7].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[8].number = 9;
            defs[8].file = "windowborder4.bmp";
            defs[8].width = 16;
            defs[8].height = 16;
            defs[8].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            defs[9].number = 10;
            defs[9].file = "windowcaption.bmp";
            defs[9].width = 48;
            defs[9].height = 32;
            defs[9].Colorkey = Color.FromArgb(255,0,0,0).ToArgb();
           
            for (int i = 0; i < (defs.GetUpperBound(0) + 1); i++) 
            {
                LoadSprite(defs[i].number, defs[i].file, defs[i].width, defs[i].height, defs[i].Colorkey);
            }
        }
       
        public override bool RunWindow()
        {
            bool bHandled = false;
            int index = 1;
           
            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
           
            //find the control with focus
            if (this.PtInRect(aPoint)) 
            {
                bHandled = true;
                foreach (CWindow wnd in controls) 
                {                   
                    bHandled = wnd.HandleInput(index);
                   
                    index += 1;
                    if (bHandled) break;
                }
            }
           
            return bHandled;
        }

        public override void NewMessage(int ctrlIndex, CWindow.WindowMessages message)
        {
            switch (ctrlIndex) 
            {
                case 1:
                    //button 1
                    if (iButtonsToDisplay < 3)
                        OnOK();
                    else
                        OnYes();

                    break;
                case 2:
                    //button 2
                    //Cancel or No
                    if (iButtonsToDisplay == 2)
                        OnCancel();
                    else
                        OnNo();

                    break;
                case 3:
                    //icon
                    break;
                //nothing
                case 4:
                    //text
                    break;
                //nothing
            }
        }
       
        protected void OnOK()
        {
            if (parent != null) 
            {
                parent.NewMessage(iParentBoxID, WindowMessages.MessageBoxOK);
            }

            Close();
        }
       
        protected void OnCancel()
        {
            if (parent != null) 
            {
                parent.NewMessage(iParentBoxID, WindowMessages.MessageBoxCancel);
            }
            Close();
        }
       
        protected void OnYes()
        {
            if (parent != null) 
            {
                parent.NewMessage(iParentBoxID, WindowMessages.MessageBoxYes);
            }
            Close();
        }
       
        protected void OnNo()
        {
            if (parent != null) 
            {
                parent.NewMessage(iParentBoxID, WindowMessages.MessageBoxNo);
            }
            Close();
        }
       
    }
}