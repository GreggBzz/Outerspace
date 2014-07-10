using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CstatusWindow : CDialogWindow
    {
        public CstatusWindow(int x, int y, int width, int height, System.Drawing.Color wndcolor) 
            :base(OuterSpace.playership.shipName, x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            base.IsModal = false;
           
            SetSprites();

            //add 9 static lines of text
            controls.Add(new CStaticText("", 10, 22, 94, 15, Color.Blue, ref thisWindow, true, false));
            //X Corrdinate
            controls.Add(new CStaticText("", 10, 37, 94, 15, Color.Blue, ref thisWindow, true, false));
            //Y Corrdinate
            controls.Add(new CStaticText("", 10, 52, 94, 15, Color.Blue, ref thisWindow, true, false));
            //Theta
            controls.Add(new CStaticText("SHEILDS", 10, 72, 64, 15, Color.Blue, ref thisWindow, true, false));
            //Sheilds
            controls.Add(new CStaticText("ARMOR", 10, 102, 64, 15, Color.Blue, ref thisWindow, true, false));
            //Armor
            controls.Add(new CStaticText("DAY", 125, 22, 96, 15, Color.Blue, ref thisWindow, true, false));
            //Day
            controls.Add(new CStaticText("YEAR", 125, 37, 96, 15, Color.Blue, ref thisWindow, true, false));
            //Year
            controls.Add(new CStaticText("WEAPONS", 125, 72, 64, 15, Color.Blue, ref thisWindow, true, false));
            //Weapons
            controls.Add(new CStaticText("CARGO", 125, 102, 64, 15, Color.Blue, ref thisWindow, true, false));
            //Cargo
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
            updateStatus();

            return base.RunWindow();
        }

        private void updateStatus()
        {
            int screenX;
            int screenY;
            int screenH;
            Color curcolor = Color.FromArgb(100, 255, 255, 255);

            if (!OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed)) 
            {
                screenX = (int)OuterSpace.XCor;
                screenY = (int)OuterSpace.YCor;
                screenH = (int)OuterSpace.playership.theta;
            }
            else 
            {
                screenX = (int)OuterSpace.TV.X;
                screenY = (int)OuterSpace.TV.Y;
                screenH = (int)OuterSpace.TV.theta;
            }
           
            CStaticText tempControl = (CStaticText)controls[0];
            tempControl.ChangeCaption("X: " + Convert.ToString(screenX));
            tempControl.ChangeColor(curcolor, false);
            tempControl = (CStaticText)controls[1];
            tempControl.ChangeCaption("Y: " + Convert.ToString(screenY));
            tempControl.ChangeColor(curcolor, false);
            tempControl = (CStaticText)controls[2];
            tempControl.ChangeCaption("H: " + Convert.ToString(screenH));
            tempControl.ChangeColor(curcolor, false);
            tempControl = (CStaticText)controls[5];
            tempControl.ChangeCaption("STARDATE:");
            tempControl.ChangeColor(curcolor, false);
            tempControl = (CStaticText)controls[6];
            curcolor = Color.LimeGreen;
            tempControl.ChangeCaption(Convert.ToString(OuterSpace.theGameClock.Clock.ToString("MM-dd-yyyy")));
            tempControl.ChangeColor(curcolor, false);
           
            if (OuterSpace.playership.displaystatus) 
            {
                curcolor = Color.BlueViolet;
                tempControl = (CStaticText)controls[3];
                tempControl.ChangeCaption("SHEILDS");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[4];
                tempControl.ChangeCaption("ARMOR");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[7];
                tempControl.ChangeCaption("WEAPONS");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[8];
                tempControl.ChangeCaption("CARGO");
                tempControl.ChangeColor(curcolor, false);
            }
            else 
            {
                tempControl = (CStaticText)controls[3];
                tempControl.ChangeCaption("");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[4];
                tempControl.ChangeCaption("");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[7];
                tempControl.ChangeCaption("");
                tempControl.ChangeColor(curcolor, false);
                tempControl = (CStaticText)controls[8];
                tempControl.ChangeCaption("");
                tempControl.ChangeColor(curcolor, false);
            }
        }
    }
}