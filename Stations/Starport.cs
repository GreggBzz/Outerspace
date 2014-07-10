#region STARPORT DEFINITION/DESIGN GOALS
// Starport is the space station orbiting the player's home world.
// The station is where the player can handle money, train crew, buy a ship, trade goods and just hang out.
// Most critical operations will be assigned at Starport.

// Main starport view will ba a rotating space station above a rotating planet.
// Each module will be click-able and when picked, will launch a new window.
// A window with an invisible frame will reside on the main view and will contain an orbit and launch buttons.
#endregion

using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D = Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    #region CSTARPORT WINDOW class
    
    // *** CSTARPORTWINDOW class DEFINITION ***
    public class CStarportWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        
        // declared control indexes (create in this order)
        enum StarportButtons
        {
            TradeDepot = 1,
            Operations,
            Bank,
            Personnel,
            DockingBay,
            ObservationDeck,
            Bar,
            Casino,
            OrbitPlanet,
            Launch
        }

        #endregion

        #region class METHODS

        public CStarportWindow(int x, int y, int width, int height, Color wndcolor)
            :this("   ", x, y, width, height, wndcolor)
        {
        }

        public CStarportWindow(string caption, int x, int y, int width, int height, Color wndcolor)
            :base(caption, x, y, width, height, wndcolor)
        {
            CWindow thisWindow = (CWindow)this;

            int btnTop = (OuterSpace.ClientArea.Height / 2) - 196;
            int windowCenterX = windowRectangle.Left + (windowRectangle.Width / 2);

            moveable = false;

            CButton depotButton = new CButton("TRADE DEPOT", windowCenterX - windowRectangle.Left - 87,
                btnTop, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton opsButton = new CButton("OPERATIONS", windowCenterX - windowRectangle.Left - 87,
                btnTop + 40, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton bankButton = new CButton("BANK", windowCenterX - windowRectangle.Left - 87,
                btnTop + 80, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton personnelButton = new CButton("PERSONNEL", windowCenterX - windowRectangle.Left - 87,
                btnTop + 120, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton dockingbayButton = new CButton("DOCKING BAY", windowCenterX - windowRectangle.Left - 87,
                btnTop + 160, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton obsdeckButton = new CButton("OBSERVATION DECK", windowCenterX - windowRectangle.Left - 87,
                btnTop + 200, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton barButton = new CButton("BAR", windowCenterX - windowRectangle.Left - 87,
                btnTop + 240, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton casinoButton = new CButton("CASINO", windowCenterX - windowRectangle.Left - 87,
                btnTop + 280, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow, false);

            CButton orbitButton = new CButton("ORBIT PLANET", windowCenterX - windowRectangle.Left - 87,
                btnTop + 320, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow);

            CButton launchButton = new CButton("LAUNCH", windowCenterX - windowRectangle.Left - 87,
                btnTop + 360, 175, 32, Color.FromArgb(225, 255, 255, 255), ref thisWindow);

            controls.Add(depotButton);
            controls.Add(opsButton);
            controls.Add(bankButton);
            controls.Add(personnelButton);
            controls.Add(dockingbayButton);
            controls.Add(obsdeckButton);
            controls.Add(barButton);
            controls.Add(casinoButton);
            controls.Add(orbitButton);
            controls.Add(launchButton);
        }

        public override bool RunWindow()
        {
            bool bHandled = false;
            int index = 1;

            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

            bHandled = base.RunWindow();

            if (!bHandled)
            {
                // find the control with focus
                if (this.PtInRect(aPoint))
                {
                    bHandled = true;
                    foreach (object aControl in controls)
                    {
                        CWindow aWindow = null;
                        aWindow = (CWindow)aControl;

                        bHandled = aWindow.HandleInput(index);

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
                case (int)StarportButtons.TradeDepot:
                case (int)StarportButtons.Operations:
                case (int)StarportButtons.Bank:
                case (int)StarportButtons.Personnel:
                case (int)StarportButtons.DockingBay:
                case (int)StarportButtons.ObservationDeck:
                case (int)StarportButtons.Bar:
                case (int)StarportButtons.Casino:
                case (int)StarportButtons.OrbitPlanet:
                    if (message == WindowMessages.LeftMouseButtonClicked)
                    {
                        OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.DockedAtStation);
                        OuterSpace.theGameState.AddState(OuterSpace.GameStates.Orbiting);
                        Close();
                    }
                    break;
                case (int)StarportButtons.Launch:
                    if (message == WindowMessages.LeftMouseButtonClicked) 
                    {
                        OuterSpace.theGameState.RemoveState(OuterSpace.GameStates.DockedAtStation);
                        OuterSpace.theGameState.AddState(OuterSpace.GameStates.InSystem);
                        Close();
                    }
                    break;
            }
        }
        
        #endregion
    }

    #endregion

    #region CSTARPORTTRADEDEPOT WINDOW class

    // *** CSTARPORTWINDOW class DEFINITION ***
    public class CStarportTradeDepotWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportTradeDepotWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Trade Depot", x, y, width, height, wndcolor)
        {

        }
        
        #endregion
    }
    
    #endregion

    #region CSTARPORTOPERATIONS WINDOW CLASS
 
    public class CStarportOperationsWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportOperationsWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Operations", x, y, width, height, wndcolor)
        {

        }

        #endregion
    }

    #endregion

    #region CSTARPORTBANK WINDOW CLASS

    public class CStarportBankWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportBankWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Bank", x, y, width, height, wndcolor)
        {

        }

        #endregion
    }

    #endregion

    #region "CSTARPORTPERSONNEL WINDOW CLASS
    
    public class CStarportPersonnelWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion
        
        #region CLASS METHODS

        public CStarportPersonnelWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Personnel", x, y, width, height, wndcolor)
        {

        }

        #endregion
    }
    
    #endregion

    #region CSTARPORTDOCKINGBAY WINDOW CLASS

    public class CStarportDockingBayWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportDockingBayWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Observation Deck", x, y, width, height, wndcolor)
        {

        }

        #endregion
    }
    
    #endregion

    #region CSTARPORTOBSDECK WINDOW CLASS

    public class CStarportObsDeckWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportObsDeckWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Observation Deck", x, y, width, height, wndcolor)
        {

        }
        
        #endregion
    }

    #endregion

    #region CSTARPORTBAR WINDOW CLASS

    public class CStarportBarWindow : CDialogWindow
    {
        #region CONSTANTS/ENUMERATIONS
        #endregion

        #region CLASS METHODS

        public CStarportBarWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Bar", x, y, width, height, wndcolor)
        {

        }
        
        #endregion
    }

    #endregion

    #region CSTARPORTCASINO WINDOW CLASS

    public class CStarportCasinoWindow : CDialogWindow
    {
        #region "CONSTANTS/ENUMERATIONS"
        #endregion

        #region CLASS METHODS

        public CStarportCasinoWindow(int x, int y, int width, int height, Color wndcolor)
            :base("Starport Casino", x, y, width, height, wndcolor)
        {

        }

        #endregion
    }

    #endregion

    #region CSTARPORT CLASS

    // *** CSTARPORT class DEFINITION ***
    public class CStarPort
    {
        #region CONTAINED STRUCTS/CLASSES

        public struct stBoundingSphere
        {
            public string name;
            public Vector3 center;
            public Single radius;
        }

        #endregion

        #region "VARIABLE DECLARATION"

        protected MeshObject starportMesh  = null;
        protected int iSystem = -1;
        protected int iOrbit = -1;
        protected Single fYAngle = 0.0F;
        protected Single fXAngle = 0.0F;
        protected bool bRotating = false;
        protected Point ptDragging;
        protected Single fZoom = 0.0F;
        protected stBoundingSphere[] bndSphere = new stBoundingSphere[8]; 
        protected Single fieldofview = Convert.ToSingle(Math.PI / 4);
        protected Single fovdiv2 = 0.0F;
        protected Single tanfovdiv2 = 0.0F;
        protected Single znearplane = 1.0F;
        protected Single zfarplane = 100.0F;
        protected Single aspectratio = 1.3333F;
        protected string statustext = "";
        protected CStarportWindow wndStarport = null;

        #endregion

        #region CLASS METHODS

        public CStarPort()
        {
            fovdiv2 = fieldofview * 0.5F;
            tanfovdiv2 = (float)Math.Tan(fovdiv2);

            //set these bounding spheres here since we know what they should be initially
            //may need to recalculate if you move

            bndSphere[0].name = "Trade Depot";
            bndSphere[0].center = new Vector3(0.0F, 0.0F, 0.6675F);
            bndSphere[0].radius = 0.2F;

            bndSphere[1].name = "Operations";
            bndSphere[1].center = new Vector3(0.0F, 0.0F, -0.6675F);
            bndSphere[1].radius = 0.2F;

            bndSphere[2].name = "Bank";
            bndSphere[2].center = new Vector3(0.6675F, 0.0F, 0.0F);
            bndSphere[2].radius = 0.2F;

            bndSphere[3].name = "Personnel";
            bndSphere[3].center = new Vector3(-0.6675F, 0.0F, 0.0F);
            bndSphere[3].radius = 0.2F;

            bndSphere[4].name = "Docking Bay";
            bndSphere[4].center = new Vector3(0.0F, -0.325F, 0.0F);
            bndSphere[4].radius = 0.2F;

            bndSphere[5].name = "Observation Deck";
            bndSphere[5].center = new Vector3(0.0F, 0.375F, 0.0F);
            bndSphere[5].radius = 0.05F;

            bndSphere[6].name = "Bar";
            bndSphere[6].center = new Vector3(-0.27F, 0.0F, 0.27F);
            bndSphere[6].radius = 0.1F;

            bndSphere[7].name = "Casino";
            bndSphere[7].center = new Vector3(0.27F, 0.0F, -0.27F);
            bndSphere[7].radius = 0.1F;
        }

        private bool IsStarportSystem(int iSystemID)
        {
            if (iSystemID == iSystem) 
                return true;
            else 
                return false;
        }

        private bool IsStarportOrbit(int iOrbitNum)
        {
            if (iOrbit == iOrbitNum) 
                return true; 
            else 
                return false;
        }

        public bool IsStarport(int iSystemID, int iOrbitNum)
        {
            if (iSystemID == iSystem)
            {
                if (iOrbit == iOrbitNum)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetStarportSystem(int iSysID)
        {
            iSystem = iSysID;
        }

        public void SetStarportOrbit(int iOrbitNum)
        {
            iOrbit = iOrbitNum;
        }

        public int Pick(Single ptX, Single ptY, Mesh meshtochk)
        {
            Vector3 v; 
            Matrix matProj, matView, matInverse, matWorld;
            Matrix m;
            Vector3 rayOrigin, rayDir;
            Vector3 rayObjOrigin, rayObjDirection;
            bool hasHit;
            int iHit;

            iHit = 0;
            hasHit = false;

            matProj = OuterSpace.device.Transform.Projection; //OuterSpace.device.GetTransform(TransformType.Projection)
            matView = OuterSpace.device.Transform.View; //OuterSpace.device.GetTransform(TransformType.View)
            matWorld = OuterSpace.device.Transform.World; //OuterSpace.device.GetTransform(TransformType.World)

            v.X = (((2.0F * ptX) / OuterSpace.device.Viewport.Width) - 1) / matProj.M11;
            v.Y = -(((2.0F * ptY) / OuterSpace.device.Viewport.Height) - 1) / matProj.M22;
            v.Z = 1.0F;

            m = Matrix.Invert(matView);

            // Transform the screen space pick ray into 3D space
            rayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            rayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            rayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;
            rayOrigin.X = m.M41;
            rayOrigin.Y = m.M42;
            rayOrigin.Z = m.M43;

            // Use inverse of matrix
            matInverse = Matrix.Invert(matWorld);

            // Transform ray origin and direction by inv matrix
            rayObjOrigin = Vector3.TransformCoordinate(rayOrigin, matInverse);
            rayObjDirection = Vector3.TransformNormal(rayDir, matInverse);
            rayObjDirection = Vector3.Normalize(rayObjDirection);

            for (int i = 0; i < 8; i++)
            {
                hasHit = Geometry.SphereBoundProbe(bndSphere[i].center, bndSphere[i].radius, 
                    rayObjOrigin, rayObjDirection);

                if (hasHit)
                {
                    iHit = i;
                    break;
                }
            }

            if (!hasHit) // see if it hit the station anywhere
            {
                hasHit = meshtochk.Intersect(rayObjOrigin, rayObjDirection);
                if (hasHit) iHit = -1; //hit station
            }

            return iHit;
        }

        // draw
        public void RenderStarport()
        {
            if (starportMesh == null)
            {
                starportMesh = new MeshObject("\\starport.x");
            }

            if (OuterSpace.theWindowMgr.FindWindow("   ") == -1)
            {
                if (OuterSpace.theWindowMgr.LoadWindow("CStarportWindow", 
                    75, 0, 175, OuterSpace.ClientArea.Height, Color.FromArgb(255, 255, 255, 255)) == true)
                {
                    OuterSpace.theWindowMgr.DoModal(OuterSpace.theWindowMgr.FindWindow("   "));
                    wndStarport = (CStarportWindow)(OuterSpace.theWindowMgr.GetWindow(OuterSpace.theWindowMgr.FindWindow("   ")));
                }
            }

            Mesh moMesh = starportMesh.GetMesh();
            List<Material> materialList = new List<Material>();
            List<Texture> textureList = new List<Texture>();

            HandleMouseInput();

            OuterSpace.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0F, 0);

            OuterSpace.d3d_scene.SetupLights();

            OuterSpace.thisPlanet.drawplanet();

            // render starport
            Direct3D.Cull oldCull = OuterSpace.device.RenderState.CullMode;

            OuterSpace.device.RenderState.CullMode = Cull.CounterClockwise;

            fYAngle += 0.001F;

            OuterSpace.d3d_scene.transformWorld(Matrix.Zero, Matrix.RotationY(fYAngle), Matrix.Zero,
                Matrix.Zero, Matrix.Translation(0.3F, 0.0F, -77.5F));

            OuterSpace.d3d_scene.transformView_Projection(new Vector3(0.0F, 0.1F, -80.0F), new Vector3(0.0F, 0.0F, 0.0F), 
                new Vector3(0.0F, 1.0F, 0.0F), Convert.ToSingle(Math.PI / 4), Convert.ToSingle(OuterSpace.ClientArea.Width / OuterSpace.ClientArea.Height), 
                0.01F, 1000.0F);

            OuterSpace.d3d_scene.transform_Pipeline();

            starportMesh.Drawmesh();

            OuterSpace.device.RenderState.CullMode = oldCull;

            OuterSpace.device.RenderState.Ambient = Color.FromArgb(255, 65, 65, 65);

            OuterSpace.textfont.DrawText(5, 5, Color.Blue, statustext);
        }

        // handle input
        protected void HandleMouseInput()
        {
            Mesh moMesh = starportMesh.GetMesh();
            int iStarportModuleHit = 0;

            wndStarport.RunWindow();

            if (OuterSpace.userinput.CheckButton("Left", false))
            {
                Point newPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

                iStarportModuleHit = Pick(newPoint.X, newPoint.Y, moMesh);

                // if module hit, click the correspoding button
                switch (iStarportModuleHit)
                {
                    case -1: // station hit
                        // don't do anything
                        break;
                    case 0: // depot
                        OnTradeDepot();
                        break;
                    case 1: // ops
                        OnOperations();
                        break;
                    case 2: // bank
                        OnBank();
                        break;
                    case 3: // personnel
                        OnPersonnel();
                        break;
                    case 4: // docking bay
                        OnDockingBay();
                        break;
                    case 5: // observation
                        OnObservationDeck();
                        break;
                    case 6: // bar
                        OnBar();
                        break;
                    case 7: // casino
                        OnCasino();
                        break;
                }
            }
        }

        protected void OnTradeDepot()
        {
        }

        protected void OnOperations()
        {
        }

        protected void OnBank()
        {
        }

        protected void OnPersonnel()
        {
        }

        protected void OnDockingBay()
        {
        }

        protected void OnObservationDeck()
        {
        }

        protected void OnBar()
        {
        }

        protected void OnCasino()
        {
        }

        #endregion
    }

    #endregion
}