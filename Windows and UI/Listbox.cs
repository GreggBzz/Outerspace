// CListBox
// Class for all listbox controls

using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;

// All user interaction code should be in the controls parent window
namespace OuterSpace 
{
    public struct LISTITEM 
    {    
        public long data;
        public string text;
        public bool selected;
    }

    public class CListBox : CWindow 
    {        
        protected int lineheight = 0;        
        protected List<LISTITEM> item = new List<LISTITEM>();        
        protected int scrollposition = 1;        
        protected int scrollrange = 0;        
        protected bool bMultiSelect = false;        
        protected Size szFont = OuterSpace.textfont.GetTextExtent("X").ToSize();

        public CListBox(int x, int y, int width, int height, Color wndcolor, ref CWindow pWnd)
            : this(x, y, width, height, wndcolor, ref pWnd, true)
        {
        }

        public CListBox(int x, int y, int width, int height, Color wndcolor, ref CWindow pWnd, bool showWnd) 
        {
            IsControl = true;
            Parent = pWnd;
            IsModal = false;

            if (width < 64) width = 64;
            if (height < 32) height = 32;
            
            // windowRectangle is relative to the parent top, left corner
            Create("CListBox", "", x, y, x + width, y + height, wndcolor);

            LoadSprite(2, "textboxbackground.bmp", 48, 48, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(3, "vscrollbar.bmp", 16, 64, Color.FromArgb(255,0,0,0).ToArgb());
            LoadSprite(4, "lbitemselection.bmp", 16, 16, Color.FromArgb(255,0,0,0).ToArgb());
            
            lineheight = szFont.Height;
            
            Show(showWnd);
        }
        
        public void SetMultiSelect(bool value) 
        {
            bMultiSelect = value;
        }
        
        public bool GetMultiSelect() 
        {
            return bMultiSelect;
        }
        
        public void ScrollTo(int position) 
        {
            if (position < 1 || position > scrollrange) 
                return;

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
            
            for (int iLoop = start; iLoop < item.Count; iLoop++) 
            {
                LISTITEM aItem = item[iLoop];
                if (aItem.selected == true) 
                {
                    iSelectedItemIndex = iLoop;
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
                return item[index].data;
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
            scrollrange++;
        }
        
        public void RemoveItem(int index) 
        {
            item.RemoveAt(index);
            scrollrange--;
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
                if ((lineheight * item.Count) > windowRectangle.Height) 
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
            for (int iLoop = 0; iLoop < item.Count; iLoop++) 
            {
                SelectItem(iLoop, false);
            }
        }
        
        public void SelectAllItems() 
        {
            for (int iLoop = 0; iLoop < item.Count; iLoop++) 
            {
                SelectItem(iLoop, true);
            }
        }
        
        public override void Draw() 
        {
            int i, j;
            int width = 0;
            int height = 0;
            Rectangle rcParent = parent.GetWindowRect();
            int vislines = 0;
            bool bShowScroll = NeedsVScrollBar();

            if (IsVisible) 
            {
                // how many text lines are visible?
                vislines = Convert.ToInt32((windowRectangle.Height - 10) / lineheight);

                // draw a different style of background than the ancestor
                // DrawBackground()
                OuterSpace.spriteobj.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

                if (IsSpriteLoaded[1])
                {
                    // top left corner
                    CornerSprite[0].Draw((rcParent.Left + windowRectangle.Left), (rcParent.Top + windowRectangle.Top),
                        new Rectangle(0, 0, 16, 16), 0, WindowColor);

                    // top edge
                    i = (windowRectangle.Left + 16);
                    width = Math.Min((windowRectangle.Width - 32), 16);
                    
                    do
                    {
                        CornerSprite[0].Draw((rcParent.Left + i), (rcParent.Top + windowRectangle.Top), 
                            new Rectangle(16, 0, width, 16), 0, WindowColor);

                        i += 16;
                        if (i + width > windowRectangle.Right - 16) 
                        {
                            width = (windowRectangle.Right - 16) - i;
                        }
                    }
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    CornerSprite[0].Draw((rcParent.Left + (windowRectangle.Right - 16)), 
                        (rcParent.Top + windowRectangle.Top), 
                        new Rectangle(32, 0, 48, 16), 0, WindowColor);

                    // center
                    j = (windowRectangle.Top + 16);
                    height = Math.Min((windowRectangle.Height - 32), 16);

                    do
                    {
                        i = windowRectangle.Left;
                        width = Math.Min((windowRectangle.Width - 32), 16);

                        do
                        {
                            if (i == windowRectangle.Left)
                                CornerSprite[0].Draw((rcParent.Left + i), (rcParent.Top + j), new Rectangle(0, 16, width, height), 0, WindowColor);
                            else if (i == (windowRectangle.Right - 16))
                                CornerSprite[0].Draw((rcParent.Left + i), (rcParent.Top + j), new Rectangle(32, 16, width, height), 0, WindowColor);                            
                            else
                                CornerSprite[0].Draw((rcParent.Left + i), (rcParent.Top + j), new Rectangle(16, 16, width, height), 0, WindowColor);
                            
                            i += 16;
                            if ((i + width) > (windowRectangle.Right - 16))
                            {
                                width = ((windowRectangle.Right - 16) - i);
                                if (width <= 0)
                                {
                                    if (width == -16)
                                    {
                                        i = windowRectangle.Right;
                                        width = 0;
                                    }
                                    else
                                    {
                                        i = (windowRectangle.Right - 16);
                                        width = 16;
                                    }
                                }
                            }
                        }
                        while (i < windowRectangle.Right);

                        j += 16;
                        if ((j + height) > (windowRectangle.Bottom - 16))
                        {
                            height = ((windowRectangle.Bottom - 16)  - j);
                        }
                    }
                    while (j < (windowRectangle.Bottom - 16) && height > 0);

                    CornerSprite[0].Draw((rcParent.Left + windowRectangle.Left), 
                        (rcParent.Top + (windowRectangle.Bottom - 16)),
                        new Rectangle(0, 32, 16, 48), 0, WindowColor);

                    // bottom edge
                    i = (windowRectangle.Left + 16);
                    width = Math.Min((windowRectangle.Width - 32), 16);

                    do
                    {
                        CornerSprite[0].Draw((rcParent.Left + i), 
                            (rcParent.Top + (windowRectangle.Bottom - 16)),
                            new Rectangle(16, 32, width, 48), 0, WindowColor);

                        i += 16;
                        if ((i + width) > (windowRectangle.Right - 16))
                        {
                            width = ((windowRectangle.Right - 16) - i);
                        }
                    }
                    while (i < (windowRectangle.Right - 16) && width > 0);

                    CornerSprite[0].Draw((rcParent.Left + (windowRectangle.Right - 16)), 
                        (rcParent.Top + (windowRectangle.Bottom - 16)),
                        new Rectangle(32, 32, 48, 48), 0, WindowColor);
                }

                // draw scrollbar
                if (bShowScroll) 
                {
                    if (IsSpriteLoaded[2]) 
                    {
                        // draw top arrow
                        CornerSprite[1].Draw((rcParent.Left + (windowRectangle.Right - 20)), 
                            (rcParent.Top + (windowRectangle.Top + 4)),
                            new Rectangle(0, 0, 16, 16), 0, WindowColor);

                        // draw thumb background
                        j = (windowRectangle.Top + 20);
                        height = Math.Min((windowRectangle.Height - 40), 16);

                        do
                        {
                            CornerSprite[1].Draw((rcParent.Left + (windowRectangle.Right - 20)), 
                                (rcParent.Top + j),
                                new Rectangle(0, 32, 16, 16), 0, WindowColor);

                            j += 16;
                            if ((j + height) > (windowRectangle.Bottom - 20))
                            {
                                height = ((windowRectangle.Bottom - 20) - j);
                            }
                        }
                        while (j < (windowRectangle.Bottom - 20) && height > 0);

                        int pixperpos;
                        int thumbpos;
                        
                        pixperpos = Convert.ToInt32(((windowRectangle.Height - 40) / (item.Count - 1)));

                        if (pixperpos < 2) 
                            pixperpos = 2;
                        
                        if (scrollposition == scrollrange) 
                            thumbpos = (windowRectangle.Height - (40 - pixperpos));
                        else 
                            thumbpos = ((scrollposition - 1) * pixperpos);

                        CornerSprite[1].Draw((rcParent.Left + (windowRectangle.Right - 20)), 
                            (rcParent.Top + (windowRectangle.Top + (20 + thumbpos))),
                            new Rectangle(0, 16, 16, (pixperpos - 1)), 0, WindowColor);

                        CornerSprite[1].Draw((rcParent.Left + (windowRectangle.Right - 20)), 
                            (rcParent.Top + (windowRectangle.Top + (20 + (thumbpos + (pixperpos - 1))))),
                            new Rectangle(0, 31, 16, 1), 0, WindowColor);

                        // draw bottom arrow
                        CornerSprite[1].Draw((rcParent.Left + (windowRectangle.Right - 20)), 
                            (rcParent.Top + (windowRectangle.Bottom - 20)),
                            new Rectangle(0, 48, 16, 16), 0, WindowColor);
                    }
                }

                OuterSpace.spriteobj.Flush();
                OuterSpace.spriteobj.End();
                
                // draw text
                j = (rcParent.Top + (windowRectangle.Top + 5));

                for (i = scrollposition; i <= (scrollposition + (vislines - 1)); i++) 
                {
                    if (i <= scrollrange && item.Count > 0) 
                    {
                        LISTITEM li = item[i];

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
                                    ii = (windowRectangle.Left + 5);
                                    width = Math.Min((windowRectangle.Width - (10 - sbw)), 16);

                                    do
                                    {
                                        CornerSprite[2].Draw((rcParent.Left + ii), jj, new Rectangle(0, 0, width, lh), 0, WindowColor);

                                        ii += 16;
                                        if ((ii + width) > (windowRectangle.Right - 5))
                                        {
                                            width = ((windowRectangle.Right - (5 - sbw)) - ii);
                                        }
                                    }
                                    while (ii < (windowRectangle.Right - (5 - sbw)) && width > 0);

                                    th += lh;
                                    lh = lineheight - th;
                                    jj += lh;
                                }
                                while (lh > 0);
                            }

                            OuterSpace.spriteobj.Flush();
                            OuterSpace.spriteobj.End();
                        }

                        OuterSpace.textfont.DrawText((rcParent.Left + (windowRectangle.Left + 5)), 
                            j, Color.Black, CheckTextWidth(li.text));

                        j += lineheight;
                    }
                }
            }
        }
        
        protected string CheckTextWidth(string text) 
        {
            string newtext = text;
            int avgwidth = szFont.Width;
            int totalwidth = (windowRectangle.Width - 10);

            if (NeedsVScrollBar())             
                totalwidth -= 16;
            
            if (szFont.Width > totalwidth) 
            {
                newtext = newtext.Substring(0, ((totalwidth / avgwidth) - 3));
                newtext += "...";
            }

            return newtext;
        }
        
        public override bool HandleInput(int ctrlindex) 
        {
            bool bHandled = false;
            Point aPoint = new System.Drawing.Point(OuterSpace.userinput.mousexpos, OuterSpace.userinput.mouseypos);

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
        
        protected void OnClicked(System.Drawing.Point pt) 
        {
            Rectangle rcParent = parent.GetWindowRect();
            bool bHandled = false;
            int iPosClicked = -1;
            int iLoop;
            int iSelection;

            // clicked in scrollbar or listbox?
            if (NeedsVScrollBar()) 
            {
                if ((pt.X >= (rcParent.Left + (windowRectangle.Right - 20))) && 
                    (pt.X <= (rcParent.Left + (windowRectangle.Right - 4))))
                {
                    // scrollbar clicked up or down
                    if ((pt.Y <= (rcParent.Top + (windowRectangle.Top + 20))) && 
                        (pt.Y >= (rcParent.Top + (windowRectangle.Top + 4)))) 
                    {
                        // top arrow clicked
                        ScrollTo(scrollposition - 1);
                    }
                    else if ((pt.Y >= (rcParent.Top + (windowRectangle.Bottom - 20))) && 
                        (pt.Y <= (rcParent.Top + (windowRectangle.Bottom - 4)))) 
                    {
                        // bottom arrow clicked
                        ScrollTo((scrollposition + 1));
                    }

                    bHandled = true;
                }
            }

            if (!bHandled) 
            {
                // select item
                iPosClicked = Convert.ToInt32(((pt.Y - (windowRectangle.Top - rcParent.Top)) / szFont.Height));
                iSelection = (scrollposition + iPosClicked);

                for (iLoop = 0; iLoop < item.Count; iLoop++) 
                {
                    if (iLoop == iSelection) 
                    {
                        if (!bMultiSelect) 
                        {
                            DeselectAllItems();
                            SelectItem(iLoop, true);
                            break;
                        }
                        else 
                        {
                            LISTITEM li = item[iLoop];
                            li.selected = !li.selected;
                            item.RemoveAt(iLoop);

                            item.Insert(iLoop, li);
                            break;
                        }
                    }
                }
            }
        }
    }
}