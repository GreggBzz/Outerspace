using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CMenuWindow : CDialogWindow
    {
        public string[] items = new string[100];
        public int[] lastitem = new int[16];
        public string[] temps = new string[6];

        // the index of the item that is selected.
        public int selected;
        //the starting index indicating what items to display on the menu
        public int startitem;
        public int userchoice;
        public bool showmenu = true;

        public CMenuWindow(int x, int y, int width, int height, Color wndcolor) 
            :base("Command", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            IsModal = false;

            SetSprites();
        
            showmenu = false;
 
            for (int i = 0; i < 100; i++) 
            {
                items[i] = "";
            }
           
            // Main menu, dictates our startindex..
            items[0] = "Helm";
            items[1] = "Science";
            items[2] = "Tactical";
            items[3] = "Engineering";
            items[4] = "Doctor";
            items[5] = "Communications";
            items[6] = "Captian";
            lastitem[0] = 6;
            // Helmsman
            items[7] = "Navigate";
            items[8] = "StarMap";
            items[9] = "Exit Hyperspace";
            items[10] = "Bridge";
            lastitem[1] = 10;
            // Science
            items[14] = "Sensors";
            items[15] = "Analysis";
            items[16] = "Ship Status";
            items[17] = "Probe 1";
            items[18] = "Probe 2";
            items[19] = "Probe 3";
            items[20] = "Bridge";
            lastitem[2] = 20;
            // Tactical
            items[21] = "Raise Sheilds";
            items[22] = "Arm Weapons";
            items[23] = "Red Alert";
            items[24] = "Engage";
            items[25] = "Bridge";
            lastitem[3] = 25;
            // Engineering
            items[28] = "Damage Report";
            items[29] = "Repair";
            items[30] = "Bridge";
            lastitem[4] = 30;
            // Doctor
            items[35] = "Examine";
            items[36] = "Treat";
            items[37] = "Moral Report";
            items[38] = "Bridge";
            lastitem[5] = 38;
            // Communications
            items[42] = "Hail";
            items[43] = "Distress";
            items[44] = "Bridge";
            lastitem[6] = 44;
            // Captian
            items[49] = "Land";
            items[50] = "Disembark";
            items[51] = "Captian's Log";
            items[52] = "Log Planet";
            items[53] = "Cargo";
            items[54] = "Bridge";
            items[55] = "Captian's PDA";
            lastitem[7] = 55;
           
            //Science Sub
            items[56] = "Deploy";
            items[57] = "Retrieve Data";
            items[58] = "Self-Destruct";
            items[59] = "Bridge";
            lastitem[8] = 59;
           
            temps[5] = "temp";
            temps[0] = "temp";
            temps[4] = "Arm Weapons";
            temps[3] = "Disarm Weapons";
            temps[2] = "Raise Shields";
            temps[1] = "Lower Shields";
            //temp for raise shields, lower shields etc..

            //add 7 static lines text, default with the crew selection.
            controls.Add(new CStaticText(items[0], 10, 20, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[1], 10, 35, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[2], 10, 50, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[3], 10, 65, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[4], 10, 80, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[5], 10, 95, 100, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText(items[6], 10, 110, 100, 15, Color.Blue, ref thisWindow, true, false));

            selected = 0;
            startitem = 0;
            userchoice = -1;
        }

        private void SetSprites()
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
            if (showmenu == true) 
            {
                base.Show(true);

                if (userchoice == -1) 
                {
                    RunTextSelect();
                }

                updateStatus();
                return base.RunWindow();
            }
            else 
            {
                //base.Close();
                base.Show(false);

                return false;
            }
        }

        private void updateStatus()
        {
            Color curcolor;
            curcolor = Color.FromArgb(100, 255, 255, 255);
            CStaticText tempControl;   
            int y = 0;
            for (int x = startitem; x <= startitem + 6; x++) 
            {
                //lastitem(startitem \ 7)
                if (selected == x) 
                {
                    curcolor = Color.White;
                }
                else 
                {
                    curcolor = Color.Blue;
                }

                tempControl = (CStaticText)controls[y];
                tempControl.ChangeCaption(items[x]);
                tempControl.ChangeColor(curcolor, false);
                y = y + 1;
            }
        }

        public void Move()
        {
            base.SetWindowRect(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos, OuterSpace.userinput.mousexpos + base.GetWindowRect().Width, OuterSpace.userinput.mouseypos + base.GetWindowRect().Height);
        }

        private void RunTextSelect()
        {
            // If the mouse is within our menu.
           
            Rectangle windowLoc;
            windowLoc = base.GetWindowRect();
            windowLoc.Location = new Point(windowLoc.Left, windowLoc.Top + 20);

            if (OuterSpace.userinput.mouseypos > windowLoc.Top & OuterSpace.userinput.mouseypos < windowLoc.Top + 15 * (lastitem[startitem / 7] - startitem + 1)) 
            {
                if (OuterSpace.userinput.mousexpos < windowLoc.Right & OuterSpace.userinput.mousexpos > windowLoc.Left) 
                {
                    int menumove = ((OuterSpace.userinput.mouseypos - windowLoc.Top) / 15);
                
                    // Temporarily dim the the number of line items down in the menu we are
                    if (selected != startitem + menumove) 
                    {
                        // Does our line item match previous frames?
                        selected = startitem + menumove;
                        // Set highlited line item to new our position.
                        OuterSpace.sounds.PlaySingle(OuterSpace.sounddir + "\\mnupop.wav");
                        // Play a beep sound
                    }

                    if (OuterSpace.userinput.CheckButton("Left", true)) 
                    {
                        OuterSpace.sounds.PlaySingle(OuterSpace.sounddir + "\\menuselect.wav");
                        if (startitem == 0) 
                        {
                            //Officer selections.. Helm, Science etc..
                            startitem = (selected + 1) * 7;
                            //Go to the startitem, this officers first action..
                            selected = startitem;
                        }
                        else if (items[selected] == "Bridge") 
                        {
                            //If not an officer selection.. must be bridge
                            startitem = 0;
                            selected = 0;
                        }
                        else 
                        {
                            userchoice = selected;
                            return;
                        }
                    }
                }
            }

            OuterSpace.userinput.resetlastbutton(OuterSpace.userinput.lastbutton);
        }
    }
}