// CDropdownListBox
// Class for all dropdownlistbox controls

using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace
{
    public class CDropdownListBox : CWindow
    {
        protected int lineheight = 0;
        protected List<LISTITEM> item = new List<LISTITEM>();
        protected int scrollposition = 1;
        protected int scrollrange = 0;
        protected Size szFont = OuterSpace.textfont.GetTextExtent("X").ToSize();
        protected bool bShowList = false;
        protected int iBoxHeight = 24;

        public CDropdownListBox(int x, int y, int width, int height, Color wndcolor, ref CWindow pWnd)
            :this(x, y, width, height, wndcolor, ref pWnd, true)
        {
        }
          
        public CDropdownListBox(int x, int y, int width, int height, Color wndcolor, ref CWindow pWnd, 
            bool showWnd)
        {
            IsControl =true;
            Parent = pWnd;
            IsModal = false;

            if (width < 64) width = 64;
            if (height < 32) height = 32;

            // windowRectangle is relative to the parent top, left corner
            Create("CDropdownListBox", "", x, y, x + width, y + height, wndcolor);

            LoadSprite(2, "textboxbackground.bmp", 48, 48, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(3, "vscrollbar.bmp", 16, 64, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(4, "lbitemselection.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());

            lineheight = szFont.Height;

            Show(showWnd);
        }

        public void ScrollTo(int position)
        {
            if (position < 1 || position > scrollrange) return;

            scrollposition = position;
        }

        public int GetScrollPosition()
        {
            return scrollposition;
        }

        public int GetNumberOfItems()
        {
            return item.Count;
        }

        public int GetNextSelection(int start)
        {
            int iSelectedItemIndex = -1;
            
            for (int i = start; i < item.Count; i++)
            {
                LISTITEM aItem = item[i];
                
                if (aItem.selected == true)
                {
                    iSelectedItemIndex = i;
                    break;
                }
            }

            return iSelectedItemIndex;
        }

        public long GetItemData(int index)
        {
            if (index == -1)
                return -1;
            else
            {
                LISTITEM aItem = item[index];
                return aItem.data;
            }
        }

        public string GetItemText(int index)
        {
            if (index == -1) 
                return "Nothing";
            else
                return item[index].text;
        }

        public void AddItem(string text, long data)
        {
            // Add a new item to the Item collection
            LISTITEM newitem = new LISTITEM();

            newitem.text = text;
            newitem.data = data;
            newitem.selected = false;

            item.Add(newitem);

            scrollrange += 1;
        }

        public void RemoveItem(int index)
        {
            item.RemoveAt(index);

            scrollrange -= 1;
        }

        public void RemoveItem(string text)
        {
            for (int i = 0; i < item.Count; i++)
            {
                if (item[i].text == text)
                {
                    RemoveItem(i);
                    break;
                }
            }
        }

        // Do we require a scrollbar?
        protected bool NeedsVScrollBar()
        {
            bool bNeedsIt = false;

            if (item.Count > 0) 
            {
                if (lineheight * item.Count > windowRectangle.Height)
                {
                    bNeedsIt = true;
                }
            }

            return bNeedsIt;
        }

        public void SelectItem(int index, bool selectit)
        {
            LISTITEM li;

            if (index > 0 && index <= item.Count)
            {
                li = item[index];
                li.selected = selectit;
                item.RemoveAt(index);
                item.Insert(index, li);
            }
        }

        public void DeselectAllItems()
        {
            for (int i = 0; i < item.Count; i++)
                SelectItem(i, false);    
        }

        public void SelectAllItems()
        {
            for (int i = 0; i < item.Count; i++)
                SelectItem(i, true);
        }

        public override void Draw()
        {
            int i, j;
            int width = 0;
            int height = 0;
            Rectangle rcParent = Parent.GetWindowRect();
            int vislines = 0;
            bool bShowScroll = NeedsVScrollBar();

            if (IsVisible)
            {
                // how many text lines are visible?
                vislines = Convert.ToInt32((windowRectangle.Height - 10 - iBoxHeight) / lineheight);

                // draw a different style of background than the ancestor
                // DrawBackground()
                OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

                if (IsSpriteLoaded[1])
                {
                    // draw selection text area
                    // top left corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top, 
                        new Rectangle(0, 0, 16, 16), 0, WindowColor);

                    // top edge
                    i = windowRectangle.Left + 16;
                    width = Math.Min(windowRectangle.Width - 32, 16);
                    
                    do
                    {
                        CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Top, 
                            new Rectangle(16, 0, width, 16), 0, WindowColor);
                     
                        i += 16;
                        
                        if (i + width > windowRectangle.Right - 16)
                            width = (windowRectangle.Right - 16) - i;
                    }    
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    // top right corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, rcParent.Top + windowRectangle.Top, 
                        new Rectangle(32, 0, 48, 16), 0, WindowColor);

                    // bottom left corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top + 16, 
                        new Rectangle(0, 40, 16, 48), 0, WindowColor);

                    // bottom edge
                    i = windowRectangle.Left + 16;
                    width = Math.Min(windowRectangle.Width - 32, 16);
                    
                    do
                    {
                        CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Top + 16, 
                            new Rectangle(16, 40, width, 48), 0, WindowColor);

                        i += 16;

                        if (i + width > windowRectangle.Right - 16) 
                            width = (windowRectangle.Right - 16) - i;
                    }
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    // bottom right corner
                    CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, 
                        rcParent.Top + windowRectangle.Top + 16, 
                        new Rectangle(32, 40, 48, 48), 0, WindowColor);

                    // draw drop arrow
                    if (IsSpriteLoaded[2])
                    {
                        CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, 
                            rcParent.Top + windowRectangle.Top + 4, 
                            new Rectangle(0, 48, 16, 16), 0, WindowColor);
                    }
                    
                    // draw list area is visible
                    if (bShowList) 
                    {
                        // top left corner
                        CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Top + iBoxHeight - 4, 
                            new Rectangle(0, 0, 16, 16), 0, WindowColor);

                        // top edge
                        i = windowRectangle.Left + 16;
                        width = Math.Min(windowRectangle.Width - 32, 16);
                        
                        do
                        {
                            CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Top + iBoxHeight - 4, 
                                new Rectangle(16, 0, width, 16), 0, WindowColor);

                            i += 16;
                            
                            if (i + width > windowRectangle.Right - 16) 
                                width = (windowRectangle.Right - 16) - i;
                        }
                        while (i < (windowRectangle.Right - 16) && width > 0);

                        // top right corner
                        CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, rcParent.Top + windowRectangle.Top + iBoxHeight - 4, 
                            new Rectangle(32, 0, 48, 16), 0, WindowColor);

                        //center
                        j = windowRectangle.Top + 16 + iBoxHeight - 4;
                        height = Math.Min(windowRectangle.Height - 32, 16);
                        
                        do
                        {
                            i = windowRectangle.Left;
                            width = Math.Min(windowRectangle.Width - 32, 16);
                            
                            do
                            {
                                if (i == windowRectangle.Left)
                                    CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j, 
                                        new Rectangle(0, 16, width, height), 0, WindowColor);
                                else if (i == windowRectangle.Right - 16) 
                                    CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j, 
                                        new Rectangle(32, 16, width, height), 0, WindowColor);
                                else
                                    CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + j, 
                                        new Rectangle(16, 16, width, height), 0, WindowColor);
                                
                                i += 16;
                                if (i + width > windowRectangle.Right - 16) 
                                {
                                    width = (windowRectangle.Right - 16) - i;
                                    if (width <= 0) 
                                    {
                                        if (width == -16) 
                                        {
                                            i = windowRectangle.Right;
                                            width = 0;
                                        }
                                        else
                                        {
                                            i = windowRectangle.Right - 16;
                                            width = 16;
                                        }
                                    }
                                }
                            }
                            while (i < windowRectangle.Right);

                            j += 16;
                            if (j + height > windowRectangle.Bottom - 16) 
                                height = (windowRectangle.Bottom - 16) - j;                            
                        }
                        while (j < (windowRectangle.Bottom - 16) && height > 0);

                        // bottom left corner
                        CornerSprite[0].Draw(rcParent.Left + windowRectangle.Left, rcParent.Top + windowRectangle.Bottom - 16, 
                            new Rectangle(0, 32, 16, 48), 0, WindowColor);

                        // bottom edge
                        i = windowRectangle.Left + 16;
                        width = Math.Min(windowRectangle.Width - 32, 16);
                        
                        do
                        {
                            CornerSprite[0].Draw(rcParent.Left + i, rcParent.Top + windowRectangle.Bottom - 16, 
                                new Rectangle(16, 32, width, 48), 0, WindowColor);

                            i += 16;
                            if (i + width > windowRectangle.Right - 16) 
                                width = (windowRectangle.Right - 16) - i;
                        }
                        while (i < (windowRectangle.Right - 16) && width > 0);

                        // bottom right corner
                        CornerSprite[0].Draw(rcParent.Left + windowRectangle.Right - 16, 
                            rcParent.Top + windowRectangle.Bottom - 16, 
                            new Rectangle(32, 32, 48, 48), 0, WindowColor);
                    }
                }

                // draw scrollbar
                if (bShowScroll && bShowList) 
                {
                    if (IsSpriteLoaded[2]) 
                    {
                        // draw top arrow
                        CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, rcParent.Top + windowRectangle.Top + iBoxHeight, 
                            new Rectangle(0, 0, 16, 16), 0, WindowColor);

                        // draw thumb background
                        j = windowRectangle.Top + iBoxHeight + 16;
                        height = Math.Min(windowRectangle.Height - 40, 16);
                        
                        do
                        {
                            CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, rcParent.Top + j, 
                                new Rectangle(0, 32, 16, 16), 0, WindowColor);

                            j += 16;
                            if (j + height > windowRectangle.Bottom - 20) 
                                height = (windowRectangle.Bottom - 20) - j;
                        }
                        while (j < (windowRectangle.Bottom - 20) && height > 0);

                        // draw thumb
                        // figure out its location
                        int pixperpos;
                        int thumbpos;

                        pixperpos = Convert.ToInt32((windowRectangle.Height - 36 - iBoxHeight) / (item.Count - 1));
                        if (pixperpos < 2) pixperpos = 2;

                        if (scrollposition == scrollrange) 
                            thumbpos = windowRectangle.Height - 36 - pixperpos - iBoxHeight;
                        else
                            thumbpos = (scrollposition - 1) * pixperpos;
                        
                        CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, rcParent.Top + windowRectangle.Top + iBoxHeight + 16 + thumbpos, 
                            new Rectangle(0, 16, 16, pixperpos - 1), 0, WindowColor);

                        CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, rcParent.Top + windowRectangle.Top + iBoxHeight + 16 + thumbpos + pixperpos - 1, 
                            new Rectangle(0, 31, 16, 1), 0, WindowColor);

                        // draw bottom arrow
                        CornerSprite[1].Draw(rcParent.Left + windowRectangle.Right - 20, 
                            rcParent.Top + windowRectangle.Bottom - 20, 
                            new Rectangle(0, 48, 16, 16), 0, WindowColor);
                    }
                }

                OuterSpace.spriteobj.Flush();
                OuterSpace.spriteobj.End();

                // draw text for selection area
                OuterSpace.textfont.DrawText(rcParent.Left + windowRectangle.Left + 5, rcParent.Top + windowRectangle.Top + 5,
                    Color.Black, CheckTextWidth(GetItemText(GetNextSelection(1))));

                // draw text for listbox
                if (bShowList) 
                {
                    j = rcParent.Top + windowRectangle.Top + iBoxHeight + 1;
                    for (i = scrollposition; i < (scrollposition + vislines - 1); i++)
                    {
                        if (i <= scrollrange && item.Count > 0) 
                        {
                            LISTITEM li = item[i];

                            // is it selected?
                            if (li.selected) 
                            {
                                int ii;
                                int jj = j;
                                int lh = lineheight;
                                int th = 0;
                                int sbw = 0;

                                if (bShowScroll) sbw = 16;

                                // draw selection
                                OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

                                if (IsSpriteLoaded[3]) 
                                {
                                    if (lh > 16) lh = 16;

                                    do
                                    {
                                        ii = windowRectangle.Left + 5;
                                        width = Math.Min(windowRectangle.Width - 10 - sbw, 16);
                                        
                                        do
                                        {
                                            CornerSprite[2].Draw(rcParent.Left + ii, jj, 
                                                new Rectangle(0, 0, width, lh), 0, WindowColor);

                                            ii += 16;
                                            if (ii + width > windowRectangle.Right - 5) 
                                                width = (windowRectangle.Right - 5 - sbw) - ii;
                                        }
                                        while (ii < (windowRectangle.Right - 5 - sbw) && width > 0);

                                        th += lh;
                                        lh = lineheight - th;
                                        jj += lh;
                                    }
                                    while (lh > 0);
                                }

                                OuterSpace.spriteobj.Flush();
                                OuterSpace.spriteobj.End();
                            }

                            OuterSpace.textfont.DrawText(rcParent.Left + windowRectangle.Left + 5, j, 
                                Color.Black, CheckTextWidth(li.text));

                            j += lineheight;
                        }
                    }
                }
            }
        }

        protected string CheckTextWidth(string text)
        {
            string newtext = text;
            int avgwidth = szFont.Width;
            int totalwidth = windowRectangle.Width - 10;

            if (NeedsVScrollBar()) totalwidth -= 16;

            if (szFont.Width > totalwidth) 
            {
                newtext = newtext.Substring(0, (totalwidth / avgwidth) - 3);
                newtext += "...";
            }

            return newtext;
        }

        public override bool HandleInput(int ctrlindex)
        {
            bool bHandled = false;

            Point aPoint = new Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

            // check mouse
            if (PtInRect(aPoint)) 
            {
                if (OuterSpace.userinput.CheckButton("Left", true)) 
                {
                    OnClicked(aPoint);
                    if (parent != null) 
                    {
                        parent.NewMessage(ctrlindex, WindowMessages.LeftMouseButtonClicked);
                    }
                    bHandled = true;
                }
            }

            return bHandled;
        }

        protected void OnClicked(Point pt)
        {
            Rectangle rcParent = parent.GetWindowRect();
            bool bHandled = false;
            int iPosClicked = -1;
            int iLoop;
            int iSelection;

            // clicked in scrollbar or listbox?
            if (NeedsVScrollBar() && bShowList)
            {
                if (pt.X >= rcParent.Left + windowRectangle.Right - 20 && pt.X <= rcParent.Left + windowRectangle.Right - 4)
                {
                    // scrollbar clicked up or down
                    if (pt.Y <= rcParent.Top + windowRectangle.Top + 36 && pt.Y >= rcParent.Top + windowRectangle.Top + 20)
                    {
                        // top arrow clicked
                        ScrollTo(scrollposition - 1);
                        bHandled = true;
                    }
                    else if (pt.Y >= rcParent.Top + windowRectangle.Bottom - 20 && pt.Y <= rcParent.Top + windowRectangle.Bottom - 4)
                    {
                        // bottom arrow clicked
                        ScrollTo(scrollposition + 1);
                        bHandled = true;
                    }
                }
            }

            if (!bHandled)
            {
                if (pt.X >= rcParent.Left + windowRectangle.Right - 20 && pt.X <= rcParent.Left + windowRectangle.Right - 4)
                {
                    if (pt.Y <= rcParent.Top + windowRectangle.Top + 20 && pt.Y >= rcParent.Top + windowRectangle.Top + 4)
                    {
                        // drop arrow clicked
                        bShowList = !bShowList;
                        bHandled = true;
                    }
                }
            }

            if (!bHandled)
            {
                if (pt.Y <= rcParent.Top + windowRectangle.Bottom - 20 && pt.Y >= rcParent.Top + windowRectangle.Top + iBoxHeight + 4)
                {
                    // select item
                    iPosClicked = Convert.ToInt32((pt.Y - windowRectangle.Top - rcParent.Top - iBoxHeight - 1) / szFont.Height);
                    iSelection = scrollposition + iPosClicked;

                    for (iLoop = 0; iLoop < item.Count; iLoop++)
                    {
                        if (iLoop == iSelection)
                        {
                            DeselectAllItems();
                            SelectItem(iLoop, true);
                            bShowList = false;
                            break;
                        }
                    }
                }
            }
        }

    }
}