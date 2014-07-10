//CPDAWindow class
//Class for the captain's pda window

using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CPDAWindow : CDialogWindow
    {       
        public CPDAWindow(int x, int y, int width, int height, System.Drawing.Color wndcolor) 
            :base("Captain's PDA", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;

            SetSprites();
           
            //add some static text
            controls.Add(new CStaticText("Static:", 10, 60, 30, 26, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, true));
           
            //add a textbox
            CTextBox aTxtbox = new CTextBox("", 60, 60, 185, 24, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, 30);
            aTxtbox.SetFocus();
            controls.Add(aTxtbox);
           
            //add a checkbox
            controls.Add(new CCheckbox("Checkbox", 60, 100, 60, 16, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
           
            //add a radio button
            CCheckbox aRadioBtn = new CCheckbox("RB1", 60, 120, 40, 16, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, true, 1);
            aRadioBtn.SetCheck(true);
            controls.Add(aRadioBtn);
           
            //add a radio button
            controls.Add(new CCheckbox("RB2", 130, 120, 40, 16, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, true, 1));
           
            //add a listbox control
            CDropdownListBox aListbox = new CDropdownListBox(60, 150, 185, 100, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true);
          
            aListbox.AddItem("item 1", 1);
            aListbox.AddItem("item 2", 2);
            aListbox.AddItem("item 3", 3);
            aListbox.AddItem("item 4", 4);
            aListbox.AddItem("item 5", 5);
            aListbox.AddItem("item 6", 6);
            aListbox.AddItem("item 7", 7);
            aListbox.AddItem("item 8", 8);
           
            controls.Add(aListbox);

            //add a button
            controls.Add(new CButton("OK", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 48, 
                windowRectangle.Height - 75, 96, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
        }
       
        public void SetSprites()
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
           
            System.Drawing.Point aPoint = new System.Drawing.Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
           
            bHandled = base.RunWindow();
           
            if (!bHandled) 
            {
                //find the control with focus
                if (this.PtInRect(aPoint)) 
                {
                    bHandled = true;

                    foreach (CWindow wnd in controls) 
                    {
                        bHandled = wnd.HandleInput(index);
                       
                        index += 1;
                        if (bHandled)
                            break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
           
            return bHandled;
        }

        public override void NewMessage(int ctrlIndex, CWindow.WindowMessages message)
        {
            switch (ctrlIndex) 
            {
                case -1:
                    if (message == CWindow.WindowMessages.MessageBoxOK) OnOK();
                    break;
                case 1:
                    //CStaticText (hyperlink)
                    break;
                //nothing
                case 2:
                    //CTextbox
                    break;
                //nothing
                case 3:
                    //CCheckbox
                    break;
                //nothing
                case 4:
                    //CCheckbox (radio mode)
                    if (message == CWindow.WindowMessages.LeftMouseButtonClicked) 
                        OnRadioBtnClicked(ctrlIndex);
                    break;
                case 5:
                    //CCheckbox (radio mode)
                    if (message == CWindow.WindowMessages.LeftMouseButtonClicked) 
                        OnRadioBtnClicked(ctrlIndex);
                    break;
                case 6:
                    //CListBox
                    break;
                //nothing
                case 7:
                    //CButton
                    if (message == CWindow.WindowMessages.LeftMouseButtonClicked) 
                    {
                        CWindow thisWindow = (CWindow)this;
                        CDropdownListBox aListbox = (CDropdownListBox)controls[5];
                        OuterSpace.theWindowMgr.MessageBox("You Selected",
                            aListbox.GetItemText(aListbox.GetNextSelection(1)), 0, 1, -1, ref thisWindow);
                       
                        //OuterSpace.theWindowMgr.MessageBox("Confirm OK", "Are you sure you want to close the PDA? " + _
                        //"If so, click Yes. If not, click No. Now for a bunch of gibberish to fill space and test things.", 0, 3, -1, Me)
                    }

                    break;
            }
        }
       
        protected void OnRadioBtnClicked(int ctrlindex)
        {
            CCheckbox aControl;
            int iGroup = -1;
           
            aControl = (CCheckbox)controls[ctrlindex];
            iGroup = aControl.GetGroupID();
           
            for (int i = 0; i < controls.Count; i++) 
            {
                CWindow aWindow = (CWindow)controls[i];
                if ((i != ctrlindex)) 
                {
                    if (aWindow.ClassName == "CCheckbox") 
                    {
                        CCheckbox aRadioBtn = (CCheckbox)controls[i];
                        if (aRadioBtn.GetGroupID() == iGroup) 
                        {
                            aRadioBtn.SetCheck(false);
                        }
                    }
                }
            }
        }
       
        protected void OnOK()
        {
            Close();
        }
    }
}