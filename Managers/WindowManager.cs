//WindowMgr class
//Class to manage a list of open windows

using System;
using System.Drawing;
using System.Collections.Generic;

namespace OuterSpace
{
    public struct define_sprite_struct
    {
        public int number;
        public string file;
        public int width;
        public int height;
        public int Colorkey;
    }
   
    public class WindowMgr
    {
        private List<CWindow> WindowCollection = new List<CWindow>();
       
        public bool LoadWindow(string wndType, int x, int y, int width, int height, Color wndcolor)
        {
           
            define_sprite_struct[] defs = new define_sprite_struct[0];
            return LoadWindow(wndType, x, y, width, height, wndcolor, defs);
        }
       
        public bool LoadWindow(string wndType, int x, int y, int width, int height, Color wndcolor, define_sprite_struct[] spritedefs)
        {           
            CWindow aWindow = null;
            bool bRetVal = false;
           
            switch (wndType) 
            {
                case "CPDAWindow":
                    aWindow = new CPDAWindow(x, y, width, height, wndcolor);
                    break;
                case "CTitleWindow":
                    aWindow = new CTitleWindow(x, y, width, height, wndcolor);
                    break;
                case "COptionsWindow":
                    aWindow = new COptionsWindow(x, y, width, height, wndcolor);
                    break;
                case "CmsgWindow":
                    aWindow = new CMsgWindow(x, y, width, height, wndcolor);
                    break;
                case "CstatusWindow":
                    aWindow = new CstatusWindow(x, y, width, height, wndcolor);
                    break;
                case "CStarportWindow":
                    aWindow = new CStarportWindow(x, y, width, height, wndcolor);
                    break;
                case "CScanWindow":
                    aWindow = new CScanWindow(x, y, width, height, wndcolor);
                    break;
                case "CAnalysisWindow":
                    aWindow = new CAnalysisWindow(x, y, width, height, wndcolor);
                    break;
                case "CMenuWindow":
                    aWindow = new CMenuWindow(x, y, width, height, wndcolor);
                    break;               
            }

            if (spritedefs != null)
            {
                for (int i = 0; i < (spritedefs.GetUpperBound(0) + 1); i++)
                {
                    aWindow.LoadSprite(spritedefs[i].number, spritedefs[i].file, spritedefs[i].width,
                        spritedefs[i].height, spritedefs[i].Colorkey);
                }
            }

            if (aWindow != null) 
            {
                AddWindow(aWindow);
                bRetVal = true;
            }
           
            return bRetVal;
        }
       
        public void AddWindow(CWindow value)
        {
            WindowCollection.Add(value);
            ResetWindowIndexes();
        }
       
        public void RemoveWindow(int index)
        {
            if (index >= 0 && index < WindowCollection.Count)
            {
                GetWindow(index).Dispose();
                WindowCollection.RemoveAt(index);
                ResetWindowIndexes();
            }
        }
       
        public CWindow GetWindow(int index)
        {
            CWindow aWindow = null;
           
            if (index < WindowCollection.Count) 
            {
                aWindow = (CWindow)WindowCollection[index];
            }
           
            return aWindow;
        }
       
        public int GetNumWindows()
        {
            return WindowCollection.Count;
        }
       
        public int FindWindow(string caption)
        {
            int index = -1;
           
            for (int i = 0; i < WindowCollection.Count; i++) 
            {
                CWindow aWindow = null;
                aWindow = (CWindow)WindowCollection[i];
                if (aWindow.GetWindowText() == caption) 
                {
                    index = i;
                    break;
                }
            }
           
            return index;
        }
       
        public void ShowWindow(int index, bool value)
        {
            CWindow aWindow = GetWindow(index);
            aWindow.Show(value);
           
            WindowCollection.RemoveAt(index);
            WindowCollection.Insert(index, aWindow);
           
            ResetWindowIndexes();
        }
       
        public void DoModal(int index)
        {
            CWindow aWindow = GetWindow(index);
            aWindow.DoModal();
           
            WindowCollection.RemoveAt(index);
            WindowCollection.Insert(index, aWindow);
           
            ResetWindowIndexes();
        }
       
        public void DrawWindows()
        {
            foreach (object aObject in WindowCollection) 
            {
                CWindow aWindow = null;
                aWindow = (CWindow)aObject;
                aWindow.Draw();
            }
        }
       
        public void KillFocusForAll()
        {
            foreach (object aObject in WindowCollection) 
            {
                CWindow aWindow = null;
                aWindow = (CWindow)aObject;
                aWindow.KillFocus();
            }
        }
        public bool RunWindows()
        {
            bool bRetHandled = false;
           
            for (int i = (WindowCollection.Count - 1); i >= 0; i--) 
            {
                CWindow aWindow = null;
                System.Drawing.Point aPoint;
               
                aWindow = WindowCollection[i];
               
                aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
                //If (aWindow.PtInRect(aPoint) Or OuterSpace.userinput.noKeysPressed() = False) Then
                //If (OuterSpace.userinput.noKeysPressed() = False) Then
                bRetHandled = aWindow.RunWindow();
                //End If
               
                if (aWindow.IsModal) 
                {
                    bRetHandled = true;
                    break; 
                }
            }
           
            return bRetHandled;
        }
        public bool RunWindows2()
        {
            bool bRetHandled = false;
            int i;

            for (i = (WindowCollection.Count - 1); i >= 0; i--) 
            {
                CWindow aWindow = null;
                System.Drawing.Point aPoint;
               
                aWindow = WindowCollection[i];
               
                aPoint = new System.Drawing.Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);
                if (aWindow.PtInRect(aPoint) || OuterSpace.userinput.noKeysPressed() == false)
                {
                    bRetHandled = aWindow.RunWindow();
                }
               
                if (aWindow.IsModal) 
                {
                    bRetHandled = true;
                    break; 
                }
            }

            return bRetHandled;
        }
       
        private void ResetWindowIndexes()
        {
            for (int i = 0; i < WindowCollection.Count; i++) 
            {
                CWindow aWindow = WindowCollection[i];
                aWindow.WindowIndexInManager = i;
            }
        }
       
        public void MessageBox(string caption, string text, int icon, int buttons, int parentboxid, ref CWindow parent)
        {
            CMessageBoxWindow aMsgBox = new CMessageBoxWindow(caption, text, icon, buttons, parentboxid, ref parent);
           
            aMsgBox.DoModal();
            WindowCollection.Add(aMsgBox);
           
            ResetWindowIndexes();
        }
    }
}