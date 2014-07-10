#region MISSION STATEMENT
// Purpose:  To become a full fledged game.
// Animation routine for the starship and it's galatic voyages.
// Requires a universe file from UniverseGenerator.
// Written By: 
//    Gregg Brzozowski (gregg.brzozowski@gmail.com), 
//    Jason Henderson (jasonwhenderson@gmail.com)
// Born On:  8/2/2003
#endregion

#region COMMENT DEFINITIONS OF DIRECTIVES, ABBREVIATIONS/ACRONYMS
// #DEPRICATED, = Try not to use this, marked for future removal.
// #SUGGESTION, = Programmer suggestions to be viewed by original programmer _
//    for improving the code design or implementation.
// #IDEA, = Programmer ideas for improving implementation of code.
// #QUESTION, = General question regarding the section of code.
// *** SECTION *** = Comment header for a large code block (or use #Region/#End Region).
// 
// Comments should always go above or sometimes to the right of the code block being commented on. 
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
//using Microsoft.DirectX.Direct3D.Line;
using Microsoft.DirectX.DirectInput;

namespace OuterSpace
{
    // *** OUTERSPACE CLASS DEFINITION ***
    public partial class OuterSpace : Form
    {
        #region CONSTANTS/ENUMERATIONS

        const int MAXSTARS = 39;
        const int MAXOTHERSHIPS = 19;

        public enum GameStates
        {
            MainTitle = 0x001,
            Interstellar = 0x002,
            InSystem = 0x004,
            InEncounter = 0x008,
            DockedAtStation = 0x010,
            Orbiting = 0x020,
            Landing = 0x040,
            Landed = 0x080,
            Disembarked = 0x100,
            Launching = 0x200,
            Starmap = 0x400,
            Paused = 0x800
        }

        #endregion

        #region CONTAINED STRUCTS/CLASSES
        /* Structs and classes contained inside the OuterSpace Form. */

        // *** STAR STRUCTURE ***
        // The backdrop stars. Locations x,y for white dots.
        public struct Star
        {
            public Single X;
            public Single Y;
            public int starFrame;
        }        

        // *** SCREEN SETTING STRUCTURE ***
        public struct ScreenSettings
        {
            private int cx;

	        public int Width
	        {
		        get { return cx;}
		        set { cx = value;}
	        }
	
            private int cy;

	        public int Height
	        {
		        get { return cy;}
		        set { cy = value;}
	        }
	
	        public Point Center
	        {
		        get { return new Point(Width / 2, Height / 2); }
	        }
        }
        
        // *** GAMESTATE CLASS ***
        public class CGameState
        {
            private GameStates gamestate;
            public GameStates GameState
            {
                get { return gamestate; }
                set { gamestate = value; }
            }

            public CGameState()
            {
                // Initial game state
                // Showing title screen
                gamestate = GameStates.MainTitle;
            }

            public void RemoveState(GameStates statetoremove)
            {
                if (IsGameInState(statetoremove))
                    GameState = (GameStates)((int)GameState - (int)statetoremove);
            }

            public void AddState(GameStates statetoadd)
            {
                if (!IsGameInState(statetoadd))
                {
                    GameState = GameState | statetoadd;

                    switch (statetoadd)
                    {
                        case GameStates.Interstellar:
                            OuterSpace.theGameClock.Interval = 5;
                            break;
                        case GameStates.InSystem:
                        case GameStates.Orbiting:
                        case GameStates.Landing:
                            OuterSpace.theGameClock.Interval = 10;
                            break;
                        case GameStates.Disembarked:
                        case GameStates.DockedAtStation:
                        case GameStates.InEncounter:
                        case GameStates.Landed:
                            OuterSpace.theGameClock.Interval = 600;
                            break;
                        case GameStates.MainTitle:
                        case GameStates.Paused:
                            OuterSpace.theGameClock.Interval = 0;
                            break;
                        default:
                            OuterSpace.theGameClock.Interval = (60 * 60 * 24);  // real-time
                            break;
                    }
                }
            }

            public void SetGameState(GameStates newstate)
            {
                GameState = newstate;
            }

            public bool IsGameInState(GameStates condition)
            {
                if ((condition & this.GameState) == condition)
                    return true;
                else
                    return false;
            }
        }

        #endregion 

        #region MEMBER VARIABLES
        /* Member variables for the OuterSpace Form. */

        // *** GAME TITLE SCREEN AND OPTIONS ***
        public static CTitleScreen MainTitle = null;

        // *** GAME MANAGER CLASSES ***
        public static RaceMgr theRaceMgr = null;
        public static CrewMgr theCrewMgr = null;
        public static WindowMgr theWindowMgr = null;

        // *** SOUND CLASS ***
        public static Sound sounds = null;

        // *** TIMING VARIABLES ***
        public static int tickLast = 0; // Holds the value of the last call to GetTick
        public int framecount = 0; // #DEPRICATED, Appears to be unused.
        public int totalticks = 0;

        // *** GAME DIRECTORIES ***
        public static string rootPath = Properties.Settings.Default["RootPath"].ToString();
        public static string applicationPath = rootPath + Properties.Settings.Default["AppPath"].ToString();
        public static string stardir = rootPath;
        public static string sounddir = rootPath + "resources\\sounds\\";
        public static string texturedir = rootPath + "resources\\textures\\";
        public static string meshdir = rootPath + "resources\\meshes\\";
        public static string xmldir = rootPath + "resources\\Xml\\";
        public static string datapath = "";

        // *** DEBUG HELPERS ***
        private string debugdir = rootPath + "debugout\\";
        public static DebugOutput debugfile = null;

        // *** GAME SPRITES ***
        private SpriteClass menuback = null;
        private SpriteClass starship = null;
        public static SpriteClass smallship = null;
        private SpriteClass solarsystem = null;
        private SpriteClass shields = null;
        public static SpriteClass backstars = null;
        private SpriteClass sun = null;
        private SpriteClass planetsprite = null;
        private SpriteClass atmosphere = null;
        public static SpriteClass pointer = null;
        public static SpriteClass msgboxboarder = null;
        public static SpriteClass windowbackground = null;

        // *** GAME MESHES ***
        public static MeshObject tvMesh = null;
        public static MeshObject mineralMesh = null;
        public static MeshObject shipMesh = null;
        public static MeshObject lifemesh = null;

        // *** GAME FONTS ***
        public static GraphicsFont textfont = new GraphicsFont("Fixedsys", FontStyle.Bold, 10);
        public static GraphicsFont linkfont = new GraphicsFont("Fixedsys", FontStyle.Bold | FontStyle.Underline, 10);

        // *** DIRECT3D VARIABLES ***
        public static Microsoft.DirectX.Direct3D.Device device = null;
        public static Microsoft.DirectX.Direct3D.Sprite spriteobj = null;
        public static D3DSceneManager d3d_scene = null;

        // *** STARS VARIABLE ***
        public static Star[] stars = new Star[MAXSTARS];

        // *** MOVEMENT VARIABLES ***
        public static Single XCor = 131; // Stores our position while intersteller
        public static Single YCor = 119;
        public int mouseangle = 0; // Angle between the center of the screen and the mouse cursor
        // Stores (keeps safe) the intersteller position while inter-system. _
        // So we can continue to use our movement sub as it alters XCor and YCor
        public Single SysX = 0.0f;
        public Single encX = 0.0f;
        public Single SysY = 0.0f;
        public Single encY = 0.0f;
        public static Single[] TanRef = new Single[361];
        public static Single[] moveY = new Single[361]; //X and Y distance values for each angle.
        public static Single[] moveX = new Single[361]; 

        // *** SOLAR SYSTEM VARIABLES ***
        //Stores system specific information for each solar system visited. 
        //makes a new object when we enter a new system
        public SolarSystem[,] Currentsystems = new SolarSystem[251, 251];
        public static CPlanet thisPlanet = null; // The planet we are currently orbiting. If any..

        // *** STARPORT VARIABLES ***
        public static CStarPort starport = null;

        // *** UNIVERSE VARIABLES ***
        public static Universe theVerse = null; //A collection of planet objects, basically

        // *** USER INTERFACE OBJECTS ***
        public static CMenuWindow mnu = null; // The main command menu
        public static Messages msgbox = null; // Our cyclic message box "stack" 
        public static userinterface userinput = null; //So we can poll input from other classes
        private starmap theStarmap = null;

        // *** SHIPS AND VEHICLE VARIABLES ***
        public static vehicleStats playership = null;
        public static vehicleStats TV = null;
        public vehicleStats[] otherships = new vehicleStats[MAXOTHERSHIPS]; // Up to twenty AI ships and on screen at once.

        // *** GAME STATE VARIABLES ***
        public static CGameState theGameState = null; // Game State manager
        public static bool drawmouse = true;
        public static CGameClock theGameClock = null;

        // *** WINDOWED OR FULLSCREEN MODE ***
        private static bool isWindowMode = true;

        // *** SCREEN RESOLUTION AND OTHER GAME OPTIONS ***
        private static ScreenSettings clientArea;

        #endregion
        
        #region PROPERTIES
        /* Properties for the OuterSpace Form. */

        public static bool IsWindowMode
        {
            get { return isWindowMode; }
            set { isWindowMode = value; }
        }

        public static ScreenSettings ClientArea
        {
            get { return clientArea; }
            set { clientArea = value; }
        }

        #endregion

        #region CONSTRUCTORS
        /* Constructors for the OuterSpace Form. */

        public OuterSpace()
        {
            InitializeComponent();

            // Initialize game managers
            theRaceMgr = new RaceMgr();
            theWindowMgr = new WindowMgr();
            // The crew manager is initialized AFTER the race manager since crews require races
            theCrewMgr = new CrewMgr();
            theGameState = new CGameState();

            // Initialize the title screen and game options
            MainTitle = new CTitleScreen();

            // Get the screen resolution to use
            clientArea.Width = MainTitle.options.GetScreenWidth();
            clientArea.Height = MainTitle.options.GetScreenHeight();
            
            // Make the title screen draw first
            if (theGameState.IsGameInState(GameStates.MainTitle))
                MainTitle.SetState(0);

            // Set the window size or screen resolution
            this.ClientSize = new Size(clientArea.Width, clientArea.Height);

            // Window or Full Screen mode?
            if (OuterSpace.IsWindowMode == true)
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            starport = new CStarPort();

            // load the galaxy file the user selects.
            theVerse = new Universe();

            // *** THE FOLLOWING IS COMMENTED OUT BUT SHOULD REMAIN FOR FUTURE REFERENCE UNTIL DEAMED UNECESSARY ***
            //
            //Dim aRace As Race = theRaceMgr.GetRace("Vulcan")
            //Dim crew(4) As CrewMember
            //crew(0) = New CrewMember(theCrewMgr.GetNextKey(), 1, "John Smith", 50, 50, 50, 50, 50, 0)
            //theCrewMgr.HireCrewMember(crew(0))
            //crew(1) = New CrewMember(theCrewMgr.GetNextKey(), 2, "Pon Far", 50, 50, 50, 50, 50, 0)
            //theCrewMgr.HireCrewMember(crew(1))
            //crew(2) = New CrewMember(theCrewMgr.GetNextKey(), 3, "Rommy", 50, 50, 50, 50, 50, 0)
            //theCrewMgr.HireCrewMember(crew(2))
            //crew(3) = New CrewMember(theCrewMgr.GetNextKey(), 4, "Worf", 50, 50, 50, 50, 50, 0)
            //theCrewMgr.HireCrewMember(crew(3))

            //theCrewMgr.Train(1, 4, 3)
            //theCrewMgr.Train(1, 3, 10)
            //theCrewMgr.Train(2, 0, 3)
            //theCrewMgr.Train(3, 2, 3)
            //theCrewMgr.Train(4, 1, 3)

            msgbox = new Messages();
            playership = new vehicleStats();
            TV = new vehicleStats();
            debugfile = new DebugOutput(debugdir + "Debug-Out.txt");

            // Set the parameters of the Terrian Vehicle.
            TV.stellerSpeed = 0.03f;
            TV.systemSpeed = 0.03f;
            TV.combatSpeed = 0.03f;
            TV.thetadif = 1;
            TV.yawdif = 1;

            // Setup other ships
            for (int i = 0; i < MAXOTHERSHIPS; i++)
            {
                otherships[i] = new vehicleStats();
                otherships[i].shipName = null;
            }

            // Set up a refrence table for costly trigometric function caclulations.
            for (int i = 0; i < 360; i++)
            {
                TanRef[i] = (float)Math.Tan(i * (Math.PI / 180));
                moveX[i] = (float)Math.Cos(i * (Math.PI / 180));
                moveY[i] = (float)Math.Sin(i * (Math.PI / 180));
            }

            TanRef[0] = 0.0f;
            moveX[0] = 1.0f;
            moveY[0] = 0.0f;
            TanRef[180] = 0.0f;
            moveX[180] = -1.0f;
            moveY[180] = 0.0f;
            TanRef[90] = (float)Math.Tan(90.1 * (Math.PI / 180));
            moveX[90] = 0.0f;
            moveY[90] = 1.0f;
            TanRef[270] = (float)Math.Tan(270.1 * (Math.PI / 180));
            moveX[270] = 0.0f;
            moveY[270] = -1.0f;
            moveX[360] = 1.0f;
            moveY[360] = 0.0f;

            // Set up initial game clock
            theGameClock = new CGameClock(2317, 3, 2, 0);

            // Windows.Forms.Cursor.Hide()
            this.Cursor.Dispose();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
        }

        #endregion

        #region OUTERSPACE FORM EVENTS
        /* Events for the OuterSpace Form. */

        private void OuterSpace_Load(object sender, EventArgs e)
        {
            if (!InitializeGraphics())
                Close();
            
            InitMeshes(); // Set up our 3d Meshes.

            sounds = new Sound(this);

            userinput = new userinterface(this);
            userinput.mousexpos = clientArea.Center.X;
            userinput.mouseypos = clientArea.Center.Y;

            msgbox.pushmsgs("Right click to bring up the menu.");
            msgbox.pushmsgs("Welcome to Outerspace!");

            MainTitle.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            if (Created && !userinput.KeyPressed(Key.Escape, false) && MainTitle.GetState() != -1)
            {
                Render();

                Application.DoEvents();
                SampleMouse();

                theGameClock.CheckInterval();
            }
            else
                Application.Exit();
            
            Invalidate();

        }

        // Direct3D device reset event handler
        void device_DeviceReset(object sender, EventArgs e)
        {
            device.RenderState.ZBufferEnable = true;
            device.RenderState.Lighting = true;
            device.RenderState.CullMode = Cull.Clockwise;
            
            if (spriteobj == null)
            {
                spriteobj = new Sprite(device);
            }
        }

        #endregion

        #region CLASS METHODS
        /* Methods for the OuterSpace Form. */

        public static void SampleMouse()
        {
            if (drawmouse) 
            {
                userinput.MouseXY(16, 25);
            }
            else 
            {
                userinput.MouseXY(0, 0);
            }
        }
        
        // *** Draw background layer ***
        public void BottomDraw()
        {
            //Stuff that is UNDER the 3d objects and UNDER the TopDraw elements.
            if (!theGameState.IsGameInState(GameStates.MainTitle)) 
            {
                spriteobj.Begin(SpriteFlags.AlphaBlend);
               
                drawStars();
               
                if (theGameState.IsGameInState(GameStates.Interstellar) && !theGameState.IsGameInState(GameStates.Starmap)) 
                {
                    drawSolarSystems();
                }
               
                if (theGameState.IsGameInState(GameStates.Landing) && !theGameState.IsGameInState(GameStates.Landed)) 
                {
                    // msgbox.drawBox(129, 162, 272, 528, Color.FromArgb(120, 255, 255, 255))
                }
               
                spriteobj.Flush();
                spriteobj.End();
               
                Draw3Dstuff();
            }
        }

        public void TopDraw()
        {
            if (theGameState.IsGameInState(GameStates.MainTitle))
            {
                MainTitle.RenderTitleScreen();
            }
            else 
            {
                if (theGameState.IsGameInState(GameStates.Landed)) 
                {
                    OuterSpace.d3d_scene.SetupLights();
                    OuterSpace.d3d_scene.transformView_Projection();
                    OuterSpace.thisPlanet.planetpoly.startSphere.render_minerals_ship_lifeforms();
                    tvMesh.Drawmesh(0, 0, OuterSpace.thisPlanet.planetpoly.z, OuterSpace.TV.theta);
                    d3d_scene.nowForTheWeather();
                }
               
                // Draw the message box message box text, ship status box, ship status text, scan boxes, scan box text,
                // menu boarder, menu text and cursor in that order as the last things
               
                // Sprite loop 1 for the scan boxes and message boxes.
                spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);
               
                // Draw the message box
                if (theWindowMgr.FindWindow("HAL-9000") == -1) 
                {
                    if (theWindowMgr.LoadWindow("CmsgWindow", clientArea.Width - 330, clientArea.Height - 138, 320, 128, Color.FromArgb(217, 255, 255, 255)) == true) 
                    {
                        theWindowMgr.ShowWindow(theWindowMgr.FindWindow("HAL-9000"), true);
                    }
                }
               
                //Draw the status box
                if (theWindowMgr.FindWindow(playership.shipName) == -1) 
                {
                    if (theWindowMgr.LoadWindow("CstatusWindow", 5, 5, 272, 144, Color.FromArgb(217, 255, 255, 255)) == true) 
                    {
                        theWindowMgr.ShowWindow(theWindowMgr.FindWindow(playership.shipName), true);
                        mnu.userchoice = -1;
                    }
                }
               
                spriteobj.Flush();
                spriteobj.End();
                // End sprite loop 1
               
                // Draw the appropriate scan text
                if (theGameState.IsGameInState(GameStates.Orbiting) && !theGameState.IsGameInState(GameStates.Landed)) 
                {
                    //In orbit?
                    theGameState.RemoveState(GameStates.InSystem);
                    theGameState.RemoveState(GameStates.Interstellar);
                    theGameState.RemoveState(GameStates.DockedAtStation);
                   
                    thisPlanet.makeScanBox();
                   
                    switch (mnu.userchoice) 
                    {
                        // Navigate, out of orbit.
                        case 7:
                            theGameState.RemoveState(GameStates.Orbiting);
                            theGameState.RemoveState(GameStates.Landing);
                            theGameState.AddState(GameStates.InSystem);
                            thisPlanet.displayScan = false;                                                                                 
                            thisPlanet.displayAnalysis = false;
                            XCor = XCor + 14;
                            OuterSpace.thisPlanet = null;
                            // Get rid of the planet object, it eats memory.
                            break;
                       
                        case 14:
                            thisPlanet.displayScan = true;
                            thisPlanet.displayAnalysis = false;
                            mnu.userchoice = -1;
                            break;
                       
                        case 15:
                            if ((thisPlanet.displayScan == true)) {
                                thisPlanet.displayAnalysis = true;
                            }
                            else {
                                msgbox.othermessage(11);
                            }
        
                            mnu.userchoice = -1;
                            break;
                       
                    }
                }
                //End in Orbit
               
                if (theGameState.IsGameInState(GameStates.DockedAtStation)) 
                {
                    theGameState.RemoveState(GameStates.InSystem);
                    theGameState.RemoveState(GameStates.Interstellar);
                    theGameState.RemoveState(GameStates.Orbiting);
                   
                    thisPlanet.makeScanBox();
                   
                    switch (mnu.userchoice) 
                    {
                        // Navigate, out of orbit.
                        case 7:
                            theGameState.RemoveState(GameStates.DockedAtStation);
                            theGameState.RemoveState(GameStates.Orbiting);
                            theGameState.RemoveState(GameStates.Landing);
                            theGameState.AddState(GameStates.InSystem);
                            thisPlanet.displayScan = false;
                            thisPlanet.displayAnalysis = false;
                            XCor = XCor + 14;
                            OuterSpace.thisPlanet = null;
                            // Get rid of the planet object, it eats memory.
                            break;
                        case 14:
                            thisPlanet.displayScan = true;
                            thisPlanet.displayAnalysis = false;
                            mnu.userchoice = -1;
                            break;
                        case 15:
                            if ((thisPlanet.displayScan == true)) {
                                thisPlanet.displayAnalysis = true;
                            }
                            else {
                                msgbox.othermessage(11);
                            }

                            mnu.userchoice = -1;
                            break;
                    }
                }
            }
           
            drawWindows();
           
            // Sprite loop 3, for the pointer.
            spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);
            // Draw the menu text
           
            userinput.drawmouse();
            userinput.resetlastbutton(userinput.lastbutton);
           
            spriteobj.Flush();
            spriteobj.End();
            // End sprite loop 3
        }
        
        public void Draw3Dstuff()
        {
            if (theGameState.IsGameInState(GameStates.Orbiting)) 
            {
                thisPlanet.drawplanet();
                //Draw the planet at least for one frame before the user can pick "Land"
               
                if (mnu.userchoice == 49) 
                {
                    // The user has picked land. Dispose the vertex buffer, Adjust all the verticies, and recreate the vertex buffer.
                    if (!theGameState.IsGameInState(GameStates.Landing)) 
                    {
                        thisPlanet.LetsLand();
                        msgbox.pushmsgs("Generating Terrain Map");
                        msgbox.pushmsgs("Select landing corrdinates");
                        mnu.items[49] = "Cancel Landing";
                        mnu.Show(false);
                    }
                   
                    theGameState.AddState(GameStates.Landing);
                    mnu.userchoice = -1;
                }
            }
            else if (theGameState.IsGameInState(GameStates.DockedAtStation)) 
            {
                starport.RenderStarport();
            }
        }

        public void DecideWhatToDraw()
        {
            //Decides what needs to be displayed on the screen
            //based on user input, boolean variables inorbit, inencounter, insystem, innersteller and menu selections.
            //Where we decided what to do basically..

            Random rnd = new Random();
           
            spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);
           
            if (theWindowMgr.FindWindow("Command") == -1) 
            {
                if (theWindowMgr.LoadWindow("CMenuWindow", 500, 500, 144, 144, Color.FromArgb(217, 255, 255, 255)) == true) 
                {
                    theWindowMgr.ShowWindow(theWindowMgr.FindWindow("Command"), true);
                }
            }
           
            mnu = (CMenuWindow)theWindowMgr.GetWindow(theWindowMgr.FindWindow("Command"));
           
            //*** Catch all the non-implemented mnu.userchoice selections.
            switch (mnu.userchoice) 
            {
                case 7:
                case 8:
                case 9:
                case 21:
                case 22:
                case 55:
                case 53:
                    break;
                case 49:
                    //Land
                    if (!theGameState.IsGameInState(GameStates.Orbiting)) 
                    {
                        msgbox.pushmsgs("We need to orbit a planet first.");
                        mnu.userchoice = -1;
                    }

                    break;
                case 24:
                    //Engage
                    if (!theGameState.IsGameInState(GameStates.Interstellar)) 
                    {
                        msgbox.pushmsgs("There are no enemy ships nearby.");
                        mnu.userchoice = -1;
                    }

                    break;
                case 54:
                    break;
                case 50:
                    //Disembark
                    if (!theGameState.IsGameInState(GameStates.Landed)) 
                    {
                        if (rnd.NextDouble() < 0.5f) 
                        {
                            msgbox.pushmsgs("to sick bay!");
                            msgbox.pushmsgs("Captain, perhaps you should report");
                        }
                        else 
                        {
                            msgbox.othermessage(14);
                        }
                        mnu.userchoice = -1;
                    }

                    break;
                case 16:
                    break;
                case 14:
                    // Sensors
                    if (!theGameState.IsGameInState(GameStates.Orbiting)) 
                    {
                        msgbox.othermessage(1);
                        mnu.userchoice = -1;
                    }

                    break;
                case 15:
                    //Anylysys
                    if (!theGameState.IsGameInState(GameStates.Orbiting)) 
                    {
                        mnu.userchoice = -1;
                        msgbox.othermessage(11);
                    }

                    break;
                case -1:
                    break;
                default:
                    mnu.userchoice = -1;
                    break;
            }
            //*** End that non-sense!
           
            msgbox.noNewMessages = true;
            // To prevent repeating messages in the message stack. See class messages.           
           
            //Testing for gregg ////////////////////
            if (mnu.userchoice == 53) 
            {
                //Cargo For NOW!
                mnu.userchoice = -1;
            }
            //Testing for gregg ///////////////////
           
            if (mnu.userchoice == 55)
            {
                //Captains PDA
                if (theWindowMgr.FindWindow("Captain's PDA") == -1) 
                {
                    if (theWindowMgr.LoadWindow("CPDAWindow", ClientArea.Center.X - 160, ClientArea.Center.Y - 200, 320, 400, Color.FromArgb(217, 255, 255, 255)) == true) 
                    {
                        theWindowMgr.DoModal(theWindowMgr.FindWindow("Captain's PDA"));
                        mnu.showmenu = false;
                        mnu.userchoice = -1;
                    }
                }
               
                if (theWindowMgr.FindWindow("Captain's PDA") == -1) 
                {
                    mnu.userchoice = -1;
                    //theWindowMgr.RemoveWindow(theWindowMgr.FindWindow("Captain's PDA"))
                }
            }
           
            if (mnu.userchoice == 9) 
            {
                //Exit Hyperspace
                if (theGameState.IsGameInState(GameStates.Orbiting) || theGameState.IsGameInState(GameStates.InEncounter)) 
                {
                    msgbox.othermessage(13);
                    mnu.userchoice = -1;
                }
                else 
                {
                    theGameState.AddState(GameStates.InEncounter);
                    theGameState.RemoveState(GameStates.Interstellar);
                    theGameState.RemoveState(GameStates.Orbiting);
                    mnu.userchoice = -1;
                    encX = XCor;
                    encY = YCor;
                    XCor = 100;
                    YCor = 100;
                }
            }
           
            if (mnu.userchoice == 24) 
            {
                //Engage
                theGameState.AddState(GameStates.InEncounter);
               
                if (theGameState.IsGameInState(GameStates.Interstellar)) 
                {
                    XCor = 100;
                    YCor = 100;
                    theGameState.RemoveState(GameStates.Interstellar);
                }
               
                mnu.userchoice = -1;
                encX = XCor;
                encY = YCor;
               
                // Raise the shields and arm the weapons
                if (playership.shieldsUp == false) 
                {
                    mnu.items[21] = mnu.temps[1];
                    //Swap Shields Up, Shields Down
                    mnu.temps[1] = mnu.temps[2];
                    mnu.temps[2] = mnu.items[21];
                    playership.shieldsUp = true;
                }
               
                if (playership.weaponsArmed == false) 
                {
                    mnu.items[22] = mnu.temps[3];
                    //Same as above.. Disarm Weapons.
                    mnu.temps[3] = mnu.temps[4];
                    //Swap
                    mnu.temps[4] = mnu.items[22];
                    playership.weaponsArmed = true;
                }
               
                mnu.showmenu = false;
                playership.displaystatus = true;
            }
           
            if (mnu.userchoice == 7) 
            {
                //Navigate
                mnu.showmenu = false;
               
                if (!theGameState.IsGameInState(GameStates.Landing))
                {
                    playership.X = XCor;
                    playership.Y = YCor;
                    playership.letsMove();
                    XCor = playership.X;
                    YCor = playership.Y;
                }
                else 
                {
                    msgbox.pushmsgs("terrain vechicle");
                    msgbox.pushmsgs("Disembark to manuver the");
                    mnu.userchoice = -1;
                }
            }
           
            if (mnu.userchoice == 21)
            {
                //Shields!
                mnu.items[21] = mnu.temps[1];
                //Swap Shields Up, Shields Down
                mnu.temps[1] = mnu.temps[2];
                mnu.temps[2] = mnu.items[21];
                mnu.userchoice = -1;
                playership.shieldsUp = !playership.shieldsUp;
            }
           
            if (mnu.userchoice == 22) 
            {
                //Arm Weapons!
                mnu.items[22] = mnu.temps[3];
                //Same as above.. Disarm Weapons.
                mnu.temps[3] = mnu.temps[4];
                //Swap
                mnu.temps[4] = mnu.items[22];
                mnu.userchoice = -1;
                playership.weaponsArmed = !playership.weaponsArmed;
            }
           
           
            if (theGameState.IsGameInState(GameStates.Starmap)) 
            {
                playership.X = XCor;
                playership.Y = YCor;
                //playership.letsMove()
                XCor = playership.X;
                YCor = playership.Y;
                theStarmap.Run();
            }
           
            if (mnu.userchoice == 8) 
            {
                //Show star map
                if (theGameState.IsGameInState(GameStates.Interstellar) && !(theGameState.IsGameInState(GameStates.Starmap))) 
                {
                    theGameState.AddState(GameStates.Starmap);
                    mnu.userchoice = -1;
                    theStarmap = new starmap(0.28F, 0.79F);
                }
                else if (theGameState.IsGameInState(GameStates.Starmap)) 
                {
                    theGameState.RemoveState(GameStates.Starmap);
                    mnu.userchoice = -1;
                    theStarmap = null;
                }
                else 
                {
                    msgbox.othermessage(10);
                    mnu.userchoice = -1;
                }
            }
           
            if (mnu.userchoice == 7)
            {
                // Navigate
                userinput.MouseXY(0, 0);
            }
            else 
            {
                userinput.MouseXY(16, 25);
            }
           
            if (theGameState.IsGameInState(GameStates.InEncounter)) 
            {
                //inencounter
                drawEncounter(otherships);

                if (XCor > ClientArea.Width - 32 || XCor < 2 || YCor > ClientArea.Height - 1 || YCor < 25) 
                {
                    theGameState.RemoveState(GameStates.Orbiting);
                    theGameState.AddState(GameStates.Interstellar);
                   
                    mnu.userchoice = 7;
                    //Navigate
                    mnu.startitem = 0;
                    mnu.selected = 0;
                    XCor = encX + 0.7F;
                    YCor = encY + 0.7F;
                   
                    theGameState.RemoveState(GameStates.InEncounter);
                }
                //endinencounter
            }
           
            if (mnu.userchoice == 7) 
            {
                userinput.MouseXY(0, 0);
            }
            else 
            {
                userinput.MouseXY(16, 25);
            }
           
            // base state of the program. In the title screen.
            if (MainTitle.GetState() == 0) 
            {
                theGameState.AddState(GameStates.MainTitle);
            }
            else 
            {
                theGameState.RemoveState(GameStates.MainTitle);
            }
           
            if (theGameState.IsGameInState(GameStates.InSystem) && !theGameState.IsGameInState(GameStates.InEncounter))
            {
                //In a system?
                theGameState.RemoveState(GameStates.Interstellar);
                theGameState.RemoveState(GameStates.Orbiting);
                theGameState.RemoveState(GameStates.DockedAtStation);
               
                if (Currentsystems[(int)SysX, (int)SysY] == null) 
                {
                    Currentsystems[(int)SysX, (int)SysY] = new SolarSystem((int)SysX, (int)SysY, theVerse);
                }
               
                drawSystem((int)SysX, (int)SysY);
               
                if (XCor > ClientArea.Width - 32 || XCor < 2 || YCor > ClientArea.Height - 1 || YCor < 25) 
                {
                    theGameState.RemoveState(GameStates.Orbiting);
                    theGameState.RemoveState(GameStates.InSystem);
                    theGameState.AddState(GameStates.Interstellar);
                    theGameState.RemoveState(GameStates.Landing);
                   
                    mnu.userchoice = 7;
                    mnu.startitem = 0;
                    mnu.selected = 0;
                    XCor = SysX + 0.7F;
                    YCor = SysY + 0.7F;
                }
            }
            //End Systementered?
           
            // Outside of system, intersteller
            if (theGameState.IsGameInState(GameStates.Interstellar) && !(theGameState.IsGameInState(GameStates.Starmap))) 
            {
                systemEntry();
                //Test to see if we are near a system.
                drawStarShip();
            }
            //end base state
           
            if (mnu.userchoice == 16)
            {
                //Ships status
                playership.displaystatus = !playership.displaystatus;
                mnu.userchoice = -1;
            }
           
            if (theWindowMgr.RunWindows() == false) 
            {
                //userinput not handled by a window
                //If Not (theGameState.IsGameInState(GameStates.MainTitle)) Then
                if (userinput.CheckButton("Right", true)) 
                {
                    mnu.showmenu = !mnu.showmenu;
                    mnu.Move();
                }
            }
            //End If
           
            //spriteobj.Flush()
            spriteobj.End();
        }

        private void Render()
        {
            int tickCurrent = Environment.TickCount;
            int tickDifference = tickCurrent - tickLast;
           
            totalticks = totalticks + tickDifference;
            if (0 == tickDifference)
            {
                return;
            }
           
            if (userinterface.needrestore) 
            {
                userinput.RestoreState(this);
            }
           
            //Reset our basic transformations so we don't do them twice a frame.
            d3d_scene.transformLightOnFrame = false;
            d3d_scene.transformViewOnFrame = false;
            //Clear the backbuffer to a black color
            //.Clear(ClearFlags.Target, System.Drawing.Color.Black, 1.0F, 0)
           
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
           
            device.BeginScene();
           
            BottomDraw();
           
            DecideWhatToDraw();
           
            TopDraw();
           
            device.EndScene();
           
            device.Present();
           
            tickLast = tickCurrent;
        }
        //Display Frame

        private void drawSystem(int X, int Y)
        {
            thisPlanet = null;
            userinput.resetlastkey(userinput.lastkey);
           
            for (int i = 0; i < 8; i++) 
            {
                if (Currentsystems[X, Y].planethere[i]) 
                {
                    planetsprite.Draw(Currentsystems[X, Y].planetXY[0, i], Currentsystems[X, Y].planetXY[1, i], Currentsystems[X, Y].picframe[i], 0, Color.White);
                   
                    //Draw the planets around the ellipses.
                    //Test to see if we are near that planet.
                    if (XCor < Currentsystems[X, Y].planetXY[0, i] + 8 & XCor > Currentsystems[X, Y].planetXY[0, i] - 8) 
                    {
                        // Near the x corrdinate of this planet?
                        if ((ClientArea.Height - YCor) < Currentsystems[X, Y].planetXY[1, i] + 8 && (ClientArea.Height - YCor) > Currentsystems[X, Y].planetXY[1, i] - 8)
                        {
                            // Near the y coordianet?
                            msgbox.OrbitYN(i + 1);
                            //We are near both the X and Y coordinates!
                           
                            if (userinput.KeyPressed(Key.Space, true)) 
                            {
                                //If space is hit, orbit this planet.
                                OuterSpace.msgbox.pushmsgs("Orbiting planet........");
                               
                                CStar star = null;
                                theVerse.GetStarAtLocation(X, Y,ref star);
                                theVerse.GetPlanetInSystemByOrbit(X, Y, i + 1, ref thisPlanet);
                               
                                if (starport.IsStarport(star.ID, i)) 
                                {
                                    // orbiting starport planet
                                    theGameState.AddState(GameStates.DockedAtStation);
                                }
                                else 
                                {
                                    theGameState.AddState(GameStates.Orbiting);
                                }
                               
                                drawmouse = true;
                                thisPlanet.renderPolyPlanet = false;
                                mnu.userchoice = -1;
                            }
                        }
                    }
                }
               
                if (mnu.userchoice == 7) 
                {
                    userinput.MouseXY(0, 0);
                }
                else 
                {
                    userinput.MouseXY(16, 25);
                }
            }
           
            // Took this out of the loop. Doubt it needed to be drawn 8 times
            //Draw the sun.
            sun.Draw(ClientArea.Center.X - 23, ClientArea.Center.Y - 23, sun.sourceFrame[0], 0, Color.White);
            // Draw the lil' ship. Invert YCor..
            smallship.Draw((int)XCor, (int)(ClientArea.Height - YCor), smallship.sourceFrame[0], 
                playership.theta, Color.White);
        }

        private void drawStars()
        {
            if (theGameState.IsGameInState(GameStates.Starmap))
                return;

            Random rnd = new Random();
                       
            for (int i = 1; i < MAXSTARS; i++) 
            {
                //Is "Navigate" the current selection
                //on the menu and are we not in a system?
                if (mnu.userchoice == 7 && !theGameState.IsGameInState(GameStates.InSystem))
                {
                    //Move every 2nd star 30x more for a parrallex effect.
                    if (i % 2 == 0) 
                    {
                    //(stars[0].X and stars[0].Y are used for storing the velocity of the backstars.
                        stars[i].X = stars[i].X - stars[0].X;
                        stars[i].Y = stars[i].Y + stars[0].Y;
                    }
                    else 
                    {
                        stars[i].X = stars[i].X - stars[0].X * playership.stellerSpeed * 30.0F;
                        stars[i].Y = stars[i].Y + stars[0].Y * playership.stellerSpeed * 30.0F;
                    }
                }
               
                if (stars[i].X > ClientArea.Width - 16) 
                {
                    stars[i].X = 0;
                    stars[i].starFrame = Convert.ToInt32(5.0F * rnd.NextDouble());
                }
               
                if (stars[i].X < 0) 
                {
                    stars[i].X = ClientArea.Width - 16;
                    stars[i].starFrame = Convert.ToInt32(5.0F * rnd.NextDouble());
                }

                if (stars[i].Y > ClientArea.Height - 16)
                {
                    stars[i].Y = 0;
                    stars[i].starFrame = Convert.ToInt32(5.0F * rnd.NextDouble());
                }
               
                if (stars[i].Y < 0) 
                {
                    stars[i].Y = ClientArea.Height - 16;
                    stars[i].starFrame = Convert.ToInt32(5.0F * rnd.NextDouble());
                }
               
                backstars.Draw((int)stars[i].X, (int)stars[i].Y, backstars.sourceFrame[stars[i].starFrame], 0, Color.White);
            }
        }

        private void drawSolarSystems()
        {
            //Draws the big stars for each system entry point.
            Color clearWhite;
            int x;
            int y;
            int xPixel;
            int yPixel;
            int offsetX;
            int offsetY;
            int incX;
            int decY;
            clearWhite = Color.FromArgb(225, 255, 255, 255);

            incX = (int)((ClientArea.Width * 1.2) / 17);
            // Expand beyound the viewable area by 20% and divide that into 17 sections.
            decY = (int)((ClientArea.Height * 1.2) / 17);
           
            x = (int)Math.Abs(Math.Floor(XCor));
            // Set the starting integers for our nested loop bellow.
            y = (int)Math.Abs(Math.Floor(YCor));
           
            xPixel = -128;
            //Our starting x screen positon, one sprite width from the left edge.
            for (int ix = (x - 8); ix <= (x + 8); ix++)
            {
                //Scan for systems in a 17 sector sweep
                yPixel = ClientArea.Height;

                //Our starting y pixel positon
                for (int iy = (y - 8); iy <= (y + 8); iy++) 
                {
                    //Scan for systems in a 17 sector sweep
                    if (theVerse.HasStar(Math.Abs(ix), Math.Abs(iy))) 
                    {
                        CStar star = null;
                        theVerse.GetStarAtLocation(ix, iy, ref star);
                        offsetX = (int)((0.07 * ClientArea.Width) * Math.Abs(XCor - x));
                        offsetY = (int)((0.07 * ClientArea.Height) * Math.Abs(YCor - y));
                        // When we cut the 20% expaned view screen into 17 sections, that worked out to sections that are
                        // 7% of the actual screen size each. So we multiply the screen size by 7% and multiply that by the
                        // decimal part of our x or y corrdinate to calculate the offsets.
                        solarsystem.curframe = theVerse.GetTempType(star.SpectralClass);
                        //Select the appropriate sprite
                        solarsystem.Draw(xPixel - offsetX, yPixel + offsetY, solarsystem.sourceFrame[solarsystem.curframe], 0, clearWhite);
                       
                        //Will display the approximate x and y pixel position of each solar system sprite.
                        //OuterSpace.textfont.DrawText(xPixel + 100, yPixel + 100, Color.White, "X:" + Convert.ToString(xPixel) + " Y:" + Convert.ToString(yPixel))
                    }
                    yPixel = yPixel - decY;
                }
                //Adjust in increments equal to (starting xPixel / 17) and (starting yPixel / 17)
                //Since there are 17 iterations in our loop
                xPixel = xPixel + incX;
            }
        }

        private void drawStarShip()
        {
            Random rnd = new Random();

            // draw a shield here
            starship.Draw(ClientArea.Center.X - 61, ClientArea.Center.Y - 47, 
                starship.sourceFrame[0], playership.theta, Color.White);

            if (playership.shieldsUp) 
            {
                // If the shields are up, draw them rotating the sprite, and alpha blende a random amount between 100 and 0.
                shields.Draw(ClientArea.Center.X - 75, ClientArea.Center.Y - 75, shields.sourceFrame[0],
                    (int)(totalticks % 1800) / 5, Color.FromArgb((int)(rnd.NextDouble() * 100.0f), 255, 255, 255));
            }
        }

        private void drawEncounter(vehicleStats[] ships)
        {
            int i = 0;
            userinput.MouseXY(12, 12);

            //Draw the players ship
            smallship.Draw((int)XCor, (int)(ClientArea.Height - YCor), smallship.sourceFrame[0], playership.theta, Color.White);

            //Draw any other ships.          
            while (ships[i].shipName != null) 
            {
                smallship.Draw((int)ships[i].X, (int)(ClientArea.Height - ships[i].Y), smallship.sourceFrame[0], 0, Color.White);
                i++;
            }
        }

        private void drawWindows()
        {
            //loop through the windows collection and draw in that order
            if (theWindowMgr.GetNumWindows() > 0) 
            {
                theWindowMgr.DrawWindows();
            }
        }

        private void systemEntry()
        {
            userinput.resetlastkey(userinput.lastkey);
           
            if (theVerse.HasStar((int)XCor, (int)YCor)) 
            {
                msgbox.EnterSys((int)XCor, (int)YCor);
                playership.stellerSpeed = 0.025F;
               
                if (userinput.KeyPressed(Key.Space, true)) 
                {
                    theGameState.AddState(GameStates.InSystem);
                    SysX = XCor;
                    SysY = YCor;
                    XCor = 100;
                    YCor = 100;
                }
                else 
                {
                    theGameState.RemoveState(GameStates.InSystem);
                }
            }
            else 
            {
                playership.stellerSpeed = 0.05F;
            }
        }

        public bool InitializeGraphics()
        {
            try
            {
                DisplayMode displayMode = Microsoft.DirectX.Direct3D.Manager.Adapters.Default.CurrentDisplayMode;

                PresentParameters presentParams = new PresentParameters();

                presentParams.BackBufferHeight = clientArea.Height;
                presentParams.BackBufferWidth = clientArea.Width;
                presentParams.PresentationInterval = PresentInterval.Immediate;
                presentParams.SwapEffect = SwapEffect.Discard;
                presentParams.AutoDepthStencilFormat = DepthFormat.D16;
                presentParams.EnableAutoDepthStencil = true;
                presentParams.BackBufferCount = 1;

                if (IsWindowMode)
                {
                    presentParams.Windowed = true;
                    presentParams.BackBufferFormat = displayMode.Format;
                }
                else
                {
                    presentParams.Windowed = false;
                    presentParams.FullScreenRefreshRateInHz = 75;
                    presentParams.BackBufferFormat = Format.A8R8G8B8;
                }

                //  Set our device renderstates
                device = new Microsoft.DirectX.Direct3D.Device(
                    Microsoft.DirectX.Direct3D.Manager.Adapters.Default.Adapter, 
                    Microsoft.DirectX.Direct3D.DeviceType.Hardware, this, 
                    CreateFlags.HardwareVertexProcessing, presentParams);

                device.RenderState.ZBufferEnable = true;
                device.RenderState.CullMode = Cull.CounterClockwise;
                device.RenderState.DitherEnable = false;
                device.RenderState.SpecularEnable = false;
                device.RenderState.Lighting = true;
                device.RenderState.FillMode = FillMode.Solid;
                device.RenderState.AntiAliasedLineEnable = true;
                device.RenderState.MultiSampleAntiAlias = true;
                device.RenderState.DitherEnable = true;
                device.RenderState.NormalizeNormals = true;
                device.RenderState.SpecularEnable = true;
                device.RenderState.ShadeMode = ShadeMode.Gouraud;

                this.InitializeSprites();

                d3d_scene = new D3DSceneManager();

                device.DeviceReset += new EventHandler(device_DeviceReset);
                this.device_DeviceReset(device, null);

                return true;
            }
            catch (DirectXException dxe)
            {
                MessageBox.Show("Could not initialize Direct3D.  " + dxe.Message + "  This game will exit.");
                Application.Exit();
                return false;
            }
        }

        // *** Initialize Game Sprites ***
        private void InitializeSprites()
        {
            Random rnd = new Random();

            // Describes a surface
            try 
            {
                int i;

                textfont.InitializeDeviceObjects(device);
                linkfont.InitializeDeviceObjects(device);

                shields = new SpriteClass("sheild.bmp", 150, 150, Color.FromArgb(255,0,0,0).ToArgb());
                shields.sourceFrame[0] = new Rectangle(0, 0, 150, 150);

                atmosphere = new SpriteClass("atmosphere.bmp", 410, 400, Color.FromArgb(255,0,0,0).ToArgb());
                atmosphere.sourceFrame[0] = new Rectangle(0, 0, 410, 400);

                starship = new SpriteClass("starship.bmp", 122, 94, Color.FromArgb(255,0,0,0).ToArgb());
                starship.sourceFrame[0] = new Rectangle(0, 0, 122, 94);

                menuback = new SpriteClass("menuback.bmp", 193, 140, Color.FromArgb(255,0,0,0).ToArgb());
                menuback.sourceFrame[0] = new Rectangle(0, 0, 193, 140);

                sun = new SpriteClass("sun.bmp", 44, 44, Color.FromArgb(255,0,0,0).ToArgb());
                sun.sourceFrame[0] = new Rectangle(0, 0, 44, 44);

                pointer = new SpriteClass("pointer.bmp", 75, 30, Color.FromArgb(255,0,0,0).ToArgb());
                pointer.sourceFrame[0] = new Rectangle(0, 0, 15, 30);
                pointer.sourceFrame[1] = new Rectangle(15, 0, 30, 30);
                pointer.sourceFrame[2] = new Rectangle(45, 0, 30, 30);

                smallship = new SpriteClass("smallship.bmp", 31, 24, Color.FromArgb(255,0,0,0).ToArgb());
                smallship.sourceFrame[0] = new Rectangle(0, 0, 31, 24);

                msgboxboarder = new SpriteClass("msgboxboarder.bmp", 48, 48, Color.FromArgb(255,0,0,0).ToArgb());
                for (i = 0; i < 8; i++) 
                {
                    System.Drawing.Point p = new System.Drawing.Point((i % 3) * 16, Convert.ToInt32(i / 3) * 16);
                    msgboxboarder.sourceFrame[i] = new System.Drawing.Rectangle(p.X, p.Y, 16, 16);
                }

                windowbackground = new SpriteClass("windowbackground.bmp", 48, 48, Color.FromArgb(255,0,0,0).ToArgb());
                for (i = 0; i < 8; i++) 
                {
                    System.Drawing.Point p = new System.Drawing.Point((i % 3) * 16, Convert.ToInt32(i / 3) * 16);
                    windowbackground.sourceFrame[i] = new System.Drawing.Rectangle(p.X, p.Y, 16, 16);
                }

                planetsprite = new SpriteClass("planetmap.bmp", 200, 275, Color.FromArgb(255,0,0,0).ToArgb());

                solarsystem = new SpriteClass("solarsystem.bmp", 1024, 128, Color.FromArgb(255,0,0,0).ToArgb());
                solarsystem.curframe = 0;
                for (i = 0; i < 8; i++) 
                {
                    Point p = new Point(i * 128, 0);
                    solarsystem.sourceFrame[i] = new Rectangle(p.X, p.Y, 128, 128);
                }

                backstars = new SpriteClass("backstars.bmp", 56, 6, Color.FromArgb(255,0,0,0).ToArgb());
                for (i = 0; i < 8; i++) 
                {
                    backstars.sourceFrame[i] = new Rectangle(i * 7, 0, 6, 6);
                }
               
                for (i = 1; i < MAXSTARS; i++) 
                {
                    stars[i].starFrame = Convert.ToInt32(6 * rnd.NextDouble());
                    stars[i].X = Convert.ToInt32((ClientArea.Width + 1) * rnd.NextDouble());
                    stars[i].Y = Convert.ToInt32((ClientArea.Height + 1) * rnd.NextDouble());
                }

                //stars[0] = null;
                // Used to store the movement velocity
            }
            //of the stars when they scroll.
            catch (DirectXException ) 
            {
                MessageBox.Show("Could not initialize Direct3D. This game will exit.");
            }
        }

        // *** Initialize Game Meshes ***
        private void InitMeshes()
        {
            // This needs to be more robust and not tied to the main class.
            // We are going to have to create and load a
            // a bunch of mesh objects for life forms ships etc..
            lifemesh = new MeshObject("distrib-grass-like.X");
            tvMesh = new MeshObject("TV.X");
            mineralMesh = new MeshObject("Sphere.X");
            shipMesh = new MeshObject("SpaceShip.X");
        }

        // *** This subroutine will eventually change screen sizes ***
        public static void OnScreenSizeChanged(Size sz)
        {
            //obj.ClientSize = New System.Drawing.Size(obj.clientarea.cx, obj.clientarea.cy)
            //crashes badly
            //http://www.riemers.net/eng/Tutorials/DirectX/Csharp/ShortTuts/lost_device.php
            ScreenSettings newSettings = new ScreenSettings();

            newSettings.Width = sz.Width;
            newSettings.Height = sz.Height;

            OuterSpace.ClientArea = newSettings;

            // need to change size of window too
        }

        #endregion
    }
}
