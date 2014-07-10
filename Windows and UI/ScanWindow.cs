using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CScanWindow : CDialogWindow
    {
        public CScanWindow(int x, int y, int width, int height, System.Drawing.Color wndcolor) 
            :base("Scan Results:", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            IsModal = false;
            SetSprites();

            //add 2 static lines of text
            controls.Add(new CStaticText("Bio:", 10, 22, 50, 15, System.Drawing.Color.Blue, ref thisWindow, true, false)); //X Corrdinate
            controls.Add(new CStaticText("Min:", 10, 37, 50, 15, System.Drawing.Color.Blue, ref thisWindow, true, false)); //Y Corrdinate
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
            if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.InSystem))
            {
                base.Close();
                return false;
            }
            if (OuterSpace.thisPlanet.displayScan) 
            {
                updateStatus();
                return base.RunWindow();
            }
            else 
            {
                base.Close();
                return false;
            }
        }

        private void updateStatus()
        {
            Color curcolor;
            curcolor = System.Drawing.Color.FromArgb(100, 255, 255, 255);

            CStaticText tempControl = (CStaticText)controls[0];

            tempControl.ChangeCaption("Bio: " + OuterSpace.thisPlanet.BioDensity.ToString());
            tempControl.ChangeColor(curcolor, false);
            tempControl = (CStaticText)controls[1];
            tempControl.ChangeCaption("Min: " + OuterSpace.thisPlanet.MinDensity.ToString());
            tempControl.ChangeColor(curcolor, false);
        }
    }
}