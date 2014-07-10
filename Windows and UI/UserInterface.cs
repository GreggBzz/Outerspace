// Fetches all the user input, from the keyboard,
// mouse etc... Also, resets keys if one at a time no repeat is needed.

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX;

namespace OuterSpace
{
    public class userinterface
    {
        public Microsoft.DirectX.DirectInput.Device keysDev = null;
        public Microsoft.DirectX.DirectInput.Device mouseDev = null;
        public KeyboardState keys = null;
        public MouseState mousedata = new MouseState();
        public int mousexpos;
        public int mouseypos;
        public int rightnow;
        public int lasttime;
        public Key lastkey;
        public string lastbutton;
        public Key[] pressedKeys = new Key[9];
            //For finding the amount of change in mouse movement.
        public int mouseXdelta;
        public int mouseYdelta;
        public MouseState mouseStateData;
        //private Key lastkeyback;
        //private string lastbuttonback;
            // ...4-02-06
        public textInput text_input = null;
        public static bool needrestore = false;
        public Form targetform = null;
        public int lastWheelZ;
        private Random rnd = new Random();
       
        public class textInput : System.Windows.Forms.NativeWindow, IDisposable
        {
            // ...4-02-06
            // ...4-02-06
           
                // ...4-02-06
            const int WM_CHAR = 258;
                // ...4-02-06
            public string myString;
           
            public textInput(IntPtr handle)
            {
                // ...4-02-06
                base.AssignHandle(handle);
                // ...4-02-06
                myString = "";
                // ...4-02-06
            }
            // ...4-02-06

            ~textInput()
            {
                Dispose(false);
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
           
            public virtual void Dispose(bool disposing)
            {
                if (disposing) 
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
            }
           
            protected override void WndProc(ref Message m)
            {
                // ...4-02-06
                if (m.Msg == WM_CHAR) 
                {
                    // ...4-02-06
                    switch (m.WParam.ToInt32()) 
                    {
                        // ...4-02-06
                        case 8:
                            //Backspace ' ...4-02-06
                            if (myString.Length <= 1) 
                            {
                                myString = "";
                            }
                            else 
                            {
                                myString = myString.Substring(0, myString.Length - 1);
                                // ...4-02-06
                            }

                            break;
                        default:
                            //Anything else' ...4-02-06
                            myString = myString + ((char)m.WParam.ToInt32()).ToString();
                            // ...4-02-06
                            break;
                    }
                    // ...4-02-06
                }
                // ...4-02-06
                base.WndProc(ref m);
                // ...4-02-06
            }
          
        }
        // ...4-02-06
       
        public userinterface(Form target)
        {
            mouseypos = 300;
            mousexpos = 400;
            mouseXdelta = 400;
            mouseYdelta = 300;
           
            try 
            {
                targetform = target;
                mouseDev = new Device(SystemGuid.Mouse);
                keysDev = new Device(SystemGuid.Keyboard);
                keysDev.Properties.BufferSize = 0;
               
                if (OuterSpace.IsWindowMode) 
                {
                    mouseDev.SetCooperativeLevel(target, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                    keysDev.SetCooperativeLevel(target, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
                }
                else 
                {
                    mouseDev.SetCooperativeLevel(target, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.NonExclusive);
                    keysDev.SetCooperativeLevel(target, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.NoWindowsKey);
                }
               
                mouseDev.Acquire();
                keysDev.Acquire();
                mouseStateData = mouseDev.CurrentMouseState;
            }
            catch (Exception) 
            {
                MessageBox.Show("DirectInput Failed to Start (userinterface.vb). Outerspace will exit");
                targetform.Dispose();
                Application.Exit();
            }
           
            //text_input = New textInput(target.Handle) ' ...4-02-06
        }
       
        public void StartKeyboardCapture()
        {
            if (text_input == null) 
            {
                text_input = new textInput(targetform.Handle);
                // ...4-02-06
            }
            else 
            {
                //already started
            }
        }
       
        public void EndKeyboardCapture()
        {
            if (text_input != null) 
            {
                text_input.Dispose(true);
                text_input = null;
            }
        }
       
        public void drawmouse()
        {
            int mouseframe = 0;

            if (OuterSpace.drawmouse) 
            {
                MouseXY(16, 25);
                if (OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landing) & !OuterSpace.theGameState.IsGameInState(OuterSpace.GameStates.Landed)) 
                {
                    // If we are landing and inside of the backdrop map box.. Draw a crosshair and allow the user to click to
                    // select the landing coordinates.
                    drawmouse_onmap();
                }
                else 
                {
                    OuterSpace.pointer.Draw(mousexpos, mouseypos, OuterSpace.pointer.sourceFrame[mouseframe], 0, Color.White);
                }
            }
        }
       
        private void drawmouse_onmap()
        {
            int mouseframe = 0;
            if (mousexpos < 654 & mousexpos > 122) 
            {
                if (mouseypos > 148 & mouseypos < 416) 
                {
                    if (!OuterSpace.mnu.showmenu) 
                    {
                        OuterSpace.textfont.DrawText(100, 100, Color.White, "Latitude: " + Convert.ToString((int)MathFunctions.scaleValue(mouseypos, 416, 148, OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2, -OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2)));
                        OuterSpace.textfont.DrawText(100, 120, Color.White, "Longitude: " + Convert.ToString(-(int)MathFunctions.scaleValue(mousexpos, 654, 122, OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2, -OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2)));
                        OuterSpace.textfont.DrawText(100, 140, Color.White, "MouseX: " + Convert.ToString(mousexpos));
                        OuterSpace.textfont.DrawText(100, 160, Color.White, "MouseY: " + Convert.ToString(mouseypos));
                        
                        // Set up to land at specified coordinates.
                        if (CheckButton("Left", true)) 
                        {
                            // Once the user clicks..
                            // Scale the mouse pointer position to a relative Terrian Vehicle position.
                            // 654, 122, 416 and 148 represent the dimensions of the map displayed on the screen.
                            OuterSpace.TV.X = -(int)MathFunctions.scaleValue(mousexpos, 654, 122, OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2, -OuterSpace.thisPlanet.PlanetMaps.BigMapX / 2);
                            // Invert the passed in Y value, because DirectX 3d vectors has +Y going up, and 2D screen vectors have +Y going DOWN.
                            OuterSpace.TV.Y = (int)MathFunctions.scaleValue(mouseypos, 416, 148, OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2, -OuterSpace.thisPlanet.PlanetMaps.BigMapY / 2);
                           
                            // Assign the ship's position to a spot that matches our object map dimensions
                            OuterSpace.playership.X = OuterSpace.TV.X + 3.0F;
                            OuterSpace.playership.Y = OuterSpace.TV.Y + 3.0F;
                           
                            // Pass our selected corrdinates into scan new terrian, forcing it to load.
                            OuterSpace.TV.updateterrian((int)OuterSpace.TV.X, (int)OuterSpace.TV.Y, true, true);
                           
                            //Set the ships position in planetside world
                           
                            OuterSpace.theGameState.AddState(OuterSpace.GameStates.Landed);
                            OuterSpace.mnu.items[49] = "Launch";
                        }
                    }

                    if (rnd.NextDouble() < 0.5f) 
                    {
                        mouseframe = 2;
                    }
                    else 
                    {
                        mouseframe = 1;
                    }
                }
            }

            OuterSpace.pointer.Draw(mousexpos, mouseypos, OuterSpace.pointer.sourceFrame[mouseframe], 0, Color.White);
        }
       
        public bool noKeysPressed()
        {
            if (KState() == null)
                return false;

            keys = KState();
            for (Key k = Key.Escape; k <= Key.MediaSelect; k++) 
            {
                if (keys[k]) 
                    return false;
            }

            return true;
        }
       
        public bool KeyPressed(Key CheckKey, bool checkForRepeat)
        {
            // Recieveis as key and checks to see if it's been pressed.
            if (KState() == null)
                return false;

            keys = KState();
            if (checkForRepeat) 
            {
                if (lastkey == CheckKey) 
                {
                    return false;
                }
            }

            if (keys[CheckKey]) 
            {
                lastkey = CheckKey;
                return true;
            }

            return false;
        }
       
        public void resetlastkey(Key keyToReset)
        {
            //Sub for stuff that we don't want repeating every frame.
            //When the user releases the key, it will reset lastkey to
            // nothing, and than that stuff mentioned above can check lastkey for nothing and do what it dose again.
            if (KState() == null) 
            {
                return;
            }

            keys = KState();
            if (!keys[keyToReset]) 
            {
                lastkey = Key.Unlabeled;  // can't set to null
            }
        }
       
        public bool CheckButton(string Button, bool checkForRepeat)
        {
            if (checkForRepeat) 
            {
                if (lastbutton == Button) 
                {
                    return false;
                }
            }

            byte[] buttons = MState().GetMouseButtons();
            switch (Button) 
            {
                case "Left":
                    if (buttons[0] != 0) 
                    {
                        lastbutton = Button;
                        return true;
                    }

                    break;
                case "Right":
                    if (buttons[1] != 0) 
                    {
                        lastbutton = Button;
                        return true;
                    }

                    break;
                case "Middle":
                    if (buttons[2] != 0) 
                    {
                        lastbutton = Button;
                        return true;
                    }

                    break;
            }

            return false;
        }
       
        public void resetlastbutton(string Button)
        {
            byte[] buttons = MState().GetMouseButtons();

            switch (Button) 
            {
                case "Left":
                    if (buttons[0] == 0) 
                    {
                        lastbutton = "";
                    }

                    break;
                case "Right":
                    if (buttons[1] == 0) 
                    {
                        lastbutton = "";
                    }

                    break;
                case "Middle":
                    if (buttons[2] == 0) 
                    {
                        lastbutton = "";
                    }

                    break;
            }
        }
       
        //Checks the mouse wheel for up or down.
        public bool CheckWheel(string Wheel)
        {
            int z = MState().Z;
            lastWheelZ = z;

            switch (Wheel) 
            {
                case "Up":
                    if (z > 0)
                        return true;

                    break;
                case "Down":
                    if (z < 0)
                        return true;

                    break;
            }

            return false;
        }
       
        public void MouseXY(int spriteW, int spriteH)
        {
            //where's the mouse? also, clipping
            //safeguards if we are rendering a sprite where the mouse is.
            mousedata = mouseDev.CurrentMouseState;
            mouseypos = mouseypos + mousedata.Y;
            mousexpos = mousexpos + mousedata.X;

            // If spriteW + spriteH = 0 Then Return
            if (mousexpos >= OuterSpace.MainTitle.options.GetScreenWidth() - spriteW) 
            {
                mousexpos = OuterSpace.MainTitle.options.GetScreenWidth() - spriteW;
            }

            if (mousexpos < 2) 
            {
                mousexpos = 2;
            }
           
            if (mouseypos >= OuterSpace.MainTitle.options.GetScreenHeight() - spriteH) 
            {
                mouseypos = OuterSpace.MainTitle.options.GetScreenHeight() - spriteH;
            }

            if (mouseypos < 2) 
            {
                mouseypos = 2;
            }
        }
       
        public KeyboardState KState()
        {
            try 
            {
                return keysDev.GetCurrentKeyboardState();
                //return the current key
            }
            catch 
            {
                try 
                {
                    keysDev.Acquire();
                    return keysDev.GetCurrentKeyboardState();
                    //try to regain the device if there is an input error
                }
                catch (InputException ex) 
                {
                    //catch an error while regaining the device
                    if (ex is OtherApplicationHasPriorityException || ex is NotAcquiredException) 
                    {
                        return null;
                        //if those excpetions occur return nothing as the device is not ready now
                    }
                    return null;
                }
            }
        }
       
        //For some reason this function has/creates either timing issues or
        public MouseState MState()
        {
            try 
            {
                mouseStateData = mouseDev.CurrentMouseState;
                return mouseStateData;
                //return the state of the mouse device
            }
            catch 
            {
                //catch any error that occurs
                try 
                {
                    mouseDev.Acquire();
                    mouseStateData = mouseDev.CurrentMouseState;
                    return mouseStateData;
                    //try to get the device back after the error occurs
                }
                catch (InputException ex) 
                {
                    //if regaining device failed then catch the error
                    if (ex is OtherApplicationHasPriorityException || ex is NotAcquiredException) 
                    {
                        //check what type of input exception occured
                        return new MouseState();
                        //return nothing as the MouseState
                    }

                    return new MouseState();
                }
            }
        }
       
        public void RestoreState(Form target)
        {
            keysDev.Dispose();
            mouseDev.Dispose();
            mouseDev = null;
            keysDev = null;

            try 
            {
                //create our mouse device
                mouseDev = new Device(SystemGuid.Mouse);
                mouseDev.SetCooperativeLevel(target, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.Exclusive);
                mouseDev.Acquire();

                //create our keyboard device
                keysDev = new Device(SystemGuid.Keyboard);
                keysDev.Properties.BufferSize = 0;
                keysDev.SetCooperativeLevel(target, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.NoWindowsKey);
                keysDev.Acquire();
            }
            catch (Exception ) 
            {
                MessageBox.Show("DirectInput Faild to Start (userinterface.vb). Outerspace will exit");
                target.Dispose();
                Application.Exit();
            }

            Cursor.Hide();
            needrestore = false;
        }
       
        public void Dispose()
        {
            keysDev.Unacquire();
            keysDev.Dispose();
            mouseDev.Unacquire();
            mouseDev.Dispose();
        }       
    }
}