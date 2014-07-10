//CTitleScreen, CTitleWindow, COptionsWindow, COptions
//classes for the title screen and main title menu items

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    //COptions Class
    //***********************************************************************************************
    public class COptions
    {
        protected bool bItemChanged = false;
        protected int ScreenCX = 1024;
        protected int ScreenCY = 768;
       
        public COptions()
        {
            // Load options from xml file on disk
            ScreenCX = 1024;
            ScreenCY = 768;
            bItemChanged = false;
        }
       
        public int GetScreenWidth()
        {
            return ScreenCX;
        }
       
        public void SetScreenWidth(int width)
        {
            if ((ScreenCX != width))
                bItemChanged = true;
            ScreenCX = width;
        }
       
        public int GetScreenHeight()
        {
            return ScreenCY;
        }
       
        public void SetScreenHeight(int height)
        {
            if ((ScreenCY != height))
                bItemChanged = true;
            ScreenCY = height;
        }
       
        public Point GetScreenCenterPt()
        {
            return new Point(ScreenCX / 2, ScreenCY / 2);
        }
       
        public bool GetItemChanged()
        {
            bool bRetVal = bItemChanged;
           
            bItemChanged = false;
           
            return bItemChanged;
        }
    }
   
    //COptionsWindow Class
    //***********************************************************************************************
    public class COptionsWindow : CDialogWindow
    {
        private const int SCRRESTXT = 1;
        private const int RADBTN800 = 2;
        private const int RADBTN1024 = 3;
        private const int OKBTN = 4;
        private const int CANCELBTN = 5;
       
        public COptionsWindow(int x, int y, int width, int height, Color wndcolor) 
            :base("Game Options", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            SetSprites();
           
            //add static text for screen res
            controls.Add(new CStaticText("Screen Resolution:", 10, 60, 150, 26, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, false));
           
            //add two radio buttons for screen res
            controls.Add(new CCheckbox("800 x 600", 30, 90, 40, 16, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, true, 1));

            CCheckbox resRadioBtn2 = new CCheckbox("1024 x 768", 130, 90, 40, 16, Color.FromArgb(225, 255, 255, 255), ref thisWindow, true, true, 1);
            resRadioBtn2.SetCheck(true);
            controls.Add(resRadioBtn2);
           
            //add New Game button
            //add Load Game button
            controls.Add(new CButton("OK", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 100, windowRectangle.Bottom - windowRectangle.Top - 40, 96, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
            controls.Add(new CButton("Cancel", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left + 4, windowRectangle.Bottom - windowRectangle.Top - 40, 96, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
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
           
            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
           
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
                            break;
                    }
                }
            }
           
            return bHandled;
        }

        public override void NewMessage(int ctrlIndex, CWindow.WindowMessages message)
        {
            switch (ctrlIndex) 
            {
                case RADBTN800:
                    //CCheckbox (radio mode)
                    if (message == WindowMessages.LeftMouseButtonClicked)
                        OnRadioBtnClicked(ctrlIndex);

                    break;
                case RADBTN1024:
                    //CCheckbox (radio mode)
                    if (message == WindowMessages.LeftMouseButtonClicked)
                        OnRadioBtnClicked(ctrlIndex);

                    break;
                case OKBTN:
                    //ok button
                    if (message == WindowMessages.LeftMouseButtonClicked)
                    {
                        OnOK();
                    }

                    break;
                case CANCELBTN:
                    //cancel button
                    if (message == WindowMessages.LeftMouseButtonClicked)
                    {
                        Close();
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

                if (i != ctrlindex) 
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
            int scrWidth;
            int scrHeight;
            CCheckbox aControl;
            OuterSpace app = (OuterSpace)Form.ActiveForm;
           
            aControl = (CCheckbox)controls[RADBTN800];
           
            if (aControl.GetCheck()) 
            {
                scrWidth = 800;
                scrHeight = 600;
            }
            else 
            {
                scrWidth = 1024;
                scrHeight = 768;
            }
       
            OuterSpace.OnScreenSizeChanged(new Size(scrWidth, scrHeight));
        }
    }
   
    //CTitleWindow Class
    //***********************************************************************************************
    public class CTitleWindow : CDialogWindow
    {
        // declared control indexes (create in this order)
        private const int NEWGAMEBTN = 1;
        private const int LOADGAMEBTN = 2;
        private const int OPTIONSBTN = 3;
        private const int QUITBTN = 4;
       
        public CTitleWindow(int x, int y, int width, int height, Color wndcolor) 
            :base(" ", x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;
            int btnTop = 300;
           
            moveable = false;
           
            //add New Game button
            //add Load Game button
            //add options button
            //add quit button
            controls.Add(new CButton("NEW GAME", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 75, btnTop, 150, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
            controls.Add(new CButton("LOAD GAME", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 75, btnTop + 40, 150, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
            controls.Add(new CButton("OPTIONS", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 75, btnTop + 80, 150, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
            controls.Add(new CButton("QUIT", (windowRectangle.Left + windowRectangle.Width / 2) - windowRectangle.Left - 75, btnTop + 120, 150, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow));
        }
       
        public override bool RunWindow()
        {
            bool bHandled = false;
            int index = 1;
           
            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
           
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
                        if (bHandled) break;
                    }
                }
            }
           
            return bHandled;
        }

        public override void NewMessage(int ctrlIndex, CWindow.WindowMessages message)
        {
            switch (ctrlIndex) 
            {
                case NEWGAMEBTN:
                    //new game
                    if (message == WindowMessages.LeftMouseButtonClicked) 
                    {
                        OuterSpace.MainTitle.SetState(1);
                        OuterSpace.theGameState.AddState(OuterSpace.GameStates.Interstellar);
                        Close();
                    }

                    break;
                case LOADGAMEBTN:
                    //load game
                    break;
                case OPTIONSBTN:
                    //options
                    if (OuterSpace.theWindowMgr.FindWindow("Game Options") == -1) 
                    {
                        if (OuterSpace.theWindowMgr.LoadWindow("COptionsWindow", OuterSpace.ClientArea.Center.X - 256, OuterSpace.ClientArea.Center.Y - 256, 512, 512, Color.FromArgb(225, 255, 255, 255)) == true) 
                        {
                            OuterSpace.theWindowMgr.DoModal(OuterSpace.theWindowMgr.FindWindow("Game Options"));
                        }
                    }

                    break;
                case QUITBTN:
                    //quit
                    if (message == WindowMessages.LeftMouseButtonClicked) 
                    {
                        OuterSpace.MainTitle.SetState(-1);
                        Close();
                    }

                    break;
            }
        }
    }
   
    //CTitleScreen Class
    //***********************************************************************************************
    public class CTitleScreen
    {
        //variables
        protected MeshObject RotatingPlanetMesh = null;
        protected SpriteClass bgSprite = null;
        protected SpriteClass fgSprite = null;
        protected int iState;
        protected float fAngle = 0f;
        private bool bStarted = false;
        public COptions options = null;
       
        //construct
        public CTitleScreen()
        {
            options = new COptions();
        }
       
        public void Start()
        {
            if (RotatingPlanetMesh == null) 
            {
                RotatingPlanetMesh = new MeshObject("title_planet.x");
                bgSprite = new SpriteClass("title_bg.jpg", 1024, 768);
                fgSprite = new SpriteClass("title_fg.bmp", 1024, 256);
            }
           
            if (OuterSpace.theWindowMgr.FindWindow(" ") == -1)
            {
                if (OuterSpace.theWindowMgr.LoadWindow("CTitleWindow", options.GetScreenWidth() - 200, 0, 150, options.GetScreenHeight(), Color.FromArgb(255, 255, 255, 255)) == true) 
                {
                    OuterSpace.theWindowMgr.DoModal(OuterSpace.theWindowMgr.FindWindow(" "));
                }
            }
           
            bStarted = true;
        }
       
        public void StopIt()
        {
            bStarted = false;
        }
       
        public int GetState()
        {
            return iState;
        }
       
        public void SetState(int newstate)
        {
            iState = newstate;
        }
       
        //draw
        public void RenderTitleScreen()
        {
            Mesh moMesh;
            List<Material> moMaterials = new List<Material>();
            List<Texture> moTextures = new List<Texture>();
            int iTime = Environment.TickCount % 100000;
            Rectangle Rect = new Rectangle(0, 0, 1024, 768);
            float ratioX;
            float ratioY;
           
            if (!bStarted) return;
           
            // draw background
            OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);
           
            ratioX = OuterSpace.ClientArea.Width / bgSprite.spritewidth;
            ratioY = ratioX;
            bgSprite.Draw(0, 0, Rect, 0, ratioX, ratioY, Color.White);
           
            OuterSpace.spriteobj.Flush();
            OuterSpace.spriteobj.End();

            // draw planet
            fAngle -= 0.001f;

            //OuterSpace.d3d_scene.transformview_projection()
            OuterSpace.device.Lights[0].Type = LightType.Directional;
            OuterSpace.device.Lights[0].Diffuse = Color.Azure;
            OuterSpace.device.Lights[0].Direction = new Vector3(-0.5f, -0.25f, -0.1f);
            OuterSpace.device.Lights[0].Update();
            OuterSpace.device.Lights[0].Enabled = true;
            //turn it on

            OuterSpace.device.RenderState.ZBufferEnable = true;
            OuterSpace.device.RenderState.CullMode = Cull.Clockwise;

            moMesh = RotatingPlanetMesh.GetMesh();
            moMaterials = RotatingPlanetMesh.GetMeshMaterials();
            moTextures = RotatingPlanetMesh.GetTextures();

            float yaw = (float)((fAngle / Math.PI) * -1.0F);
            float pitch = -0.5f;
            //(fAngle / Math.PI * 2.0F)
            float Roll = 0f;
            //(fAngle / Math.PI / 4.0F)

            try
            {
                OuterSpace.d3d_scene.transformWorld(Matrix.RotationX(pitch), Matrix.RotationY(yaw), Matrix.RotationZ(Roll), Matrix.Zero, Matrix.Translation(-1f, -0.75f, -196.5f));
                //OuterSpace.device.Transform.World = Matrix.Multiply(Matrix.RotationYawPitchRoll(yaw, pitch, Roll), _
                // Matrix.Translation(-1.0F, -0.75F, -196.5F))

                OuterSpace.d3d_scene.transformView_Projection();
                OuterSpace.d3d_scene.transform_Pipeline();

                for (int x = 0; x < RotatingPlanetMesh.GetNumMeshMaterials(); x++)
                {
                    OuterSpace.device.Material = moMaterials[x];
                    OuterSpace.device.SetTexture(0, moTextures[x]);
                    moMesh.DrawSubset(x);
                }
            }
            catch (Microsoft.DirectX.Direct3D.Direct3DXException ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);
           
            ratioX = OuterSpace.ClientArea.Width / fgSprite.spritewidth;
            ratioY = ratioX;
            fgSprite.Draw(0, 0, new Rectangle(0, 0, 1024, 256), 0, ratioX, ratioY, Color.White);
           
            OuterSpace.spriteobj.Flush();
            OuterSpace.spriteobj.End();
        }
       
        //handle input
       
        //open windows
       
    }
}