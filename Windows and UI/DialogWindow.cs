// CDialogWindow class

using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

namespace OuterSpace
{
    public class CDialogWindow : CWindow
    {
        protected List<CWindow> controls = new List<CWindow>();

        public CDialogWindow(string caption, int x, int y, int width, int height, Color wndcolor)            
        {
            Create("CDialogWindow", caption, x, y, x + width, y + height, wndcolor);
        }

        public override void Dispose()
        {
            // dispose each control
            foreach (CWindow wnd in controls)
            {
                wnd.Dispose();
            }

            base.Dispose();
        }

        public override void Draw()
        {
            if (IsVisible) 
            {
                // draw the background
                DrawBackground();

                // draw controls
                DrawControls();
            }
        }

        public void DrawControls()
        {
            if (controls.Count > 0)
            {
                foreach (CWindow wnd in controls)
                {
                    wnd.Draw();
                }
            }
        }

        public override void KillControlFocus()
        {
            if (controls.Count > 0) 
            {
                foreach(CWindow wnd in controls)
                {
                    wnd.KillFocus();
                }
            }
        }

        public override bool RunWindow()
        {
            return base.RunWindow();
        }
    }
}