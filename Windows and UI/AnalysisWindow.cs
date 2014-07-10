using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CAnalysisWindow : CDialogWindow
    {
        public CAnalysisWindow(int x, int y, int width, int height, Color wndcolor) 
            :base("Scan Analysis:", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            IsModal = false;

            SetSprites();

            //add 2 static lines of text
            controls.Add(new CStaticText("Atmosphere:", 10, 22, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 10, 37, 128, 15, Color.Green, ref thisWindow, true, false));            
            //Atmosphere #1
            controls.Add(new CStaticText("", 10, 52, 128, 15, Color.Green, ref thisWindow, true, false));
            //#2
            controls.Add(new CStaticText("", 10, 67, 128, 15, Color.Green, ref thisWindow, true, false));            
            //#3
            controls.Add(new CStaticText("Hydrosphere:", 10, 82, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 10, 97, 128, 15, Color.Green, ref thisWindow, true, false));            
            //Hydrosphere #1
            controls.Add(new CStaticText("", 10, 112, 128, 15, Color.Green, ref thisWindow, true, false));
            //#2
            controls.Add(new CStaticText("", 10, 127, 128, 15, Color.Green, ref thisWindow, true, false));
            //#3
            controls.Add(new CStaticText("Lithosphere:", 10, 142, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 10, 157, 128, 15, Color.Green, ref thisWindow, true, false));
            //Lithosphere #1
            controls.Add(new CStaticText("", 10, 172, 128, 15, Color.Green, ref thisWindow, true, false));
            //#2
            controls.Add(new CStaticText("", 10, 187, 128, 15, Color.Green, ref thisWindow, true, false));
            //#3
            controls.Add(new CStaticText("Climate:", 10, 202, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 65, 202, 128, 15, Color.Green, ref thisWindow, true, false));
            // Climate value
            controls.Add(new CStaticText("Mass:", 10, 217, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 65, 217, 128, 15, Color.Green, ref thisWindow, true, false));
            controls.Add(new CStaticText("Weather:", 10, 232, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 65, 232, 128, 15, Color.Green, ref thisWindow, true, false));
            // Climate value
            controls.Add(new CStaticText("Gravity:", 10, 247, 128, 15, Color.Blue, ref thisWindow, true, false));
            controls.Add(new CStaticText("", 65, 247, 128, 15, Color.Green, ref thisWindow, true, false));
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
                if (OuterSpace.thisPlanet.displayAnalysis)
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
            CStaticText tempControl = (CStaticText)controls[2];

            Color curcolor = Color.LimeGreen;

            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(0, 0));
            tempControl = (CStaticText)controls[3];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(0, 1));
            tempControl = (CStaticText)controls[4];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(0, 2));
            tempControl = (CStaticText)controls[6];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(1, 0));
            tempControl = (CStaticText)controls[7];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(1, 1));
            tempControl = (CStaticText)controls[8];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(1, 2));
            tempControl = (CStaticText)controls[10];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(2, 0));
            tempControl = (CStaticText)controls[11];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(2, 1));
            tempControl = (CStaticText)controls[12];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetCompounds(2, 2));
            tempControl = (CStaticText)controls[14];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.ClimateRange);
            tempControl = (CStaticText)controls[16];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.Mass.ToString());
            tempControl = (CStaticText)controls[18];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetWeather());
            tempControl = (CStaticText)controls[19];
            tempControl.ChangeColor(curcolor, false);
            tempControl.ChangeCaption(OuterSpace.thisPlanet.GetGravity());
        }
    }
}