using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public class TrackListDragDrop
    {
        public int fromIndex;
        public ushort fromChannel;
        public ushort toChannel;
        public bool moved;

        public TrackListDragDrop(int _fromIndex, ushort _fromChannel)
        {
            fromChannel = _fromChannel;
            toChannel = _fromChannel;
            fromIndex = _fromIndex;
            moved = false;
        }
    }

    public class EntryInfo
    {
        public Command type;
        public string description;
        public bool isSelectedTextItem;

        public EntryInfo(Command _type, string _description)
        {
            type = _type;
            description = _description;
            isSelectedTextItem = false;
        }

        public void ClearSelection() => isSelectedTextItem = false;

        public override string ToString() => description;
    }

    public class RequestChangeEventArgs : EventArgs
    {
        public ChangeType ct;
        public int index;
        public int index2;
        public ushort channel;

        public RequestChangeEventArgs(ushort _channel, ChangeType _ct, int _index, int _index2)
            : base()
        {
            ct = _ct;
            index = _index;
            index2 = _index2;
            channel = _channel;
        }

        public RequestChangeEventArgs(ushort _channel, ChangeType _ct, int _index)
            : this(_channel, _ct, _index, default(int))
        { }
    }

    public delegate void RequestChangeEventHandler(object sender, RequestChangeEventArgs e);

    public partial class TrackList : Control
    {
        private System.Collections.Generic.List<EntryInfo> items = new System.Collections.Generic.List<EntryInfo>();

        public bool IsTextItemAt(int index) =>
            getTrack(index).type == Command.TEXTITEM;

        public EntryInfo getTrack(int i) => i < items.Count ? items[i] : new EntryInfo(Command.TEXTITEM, "NONE");

        public void addTrack(Command type, string descr)
        {
            items.Add(new EntryInfo(type, descr));
            showHideScrollBar();
            Invalidate();
        }

        public void removeTrack(int _index)
        {
            if (_index == items.IndexOf(selectedTextEntry))
            {
                selectedTextEntry = null;
            }
            int viewableItemCount = ClientSize.Height / ItemHeight;
            // fix the scroll bar
            if (items.Count - _index < viewableItemCount && scroll.Value != 0)
            {
                scroll.Value -= 1;
            }
            items.RemoveAt(_index);
            if (LoadedIndex == _index)
            {
                selectedIndex = -1;
            }
            else if (LoadedIndex > _index)
            {
                LoadedIndex -= 1;
            }
            showHideScrollBar();
            Invalidate();
        }

        public void moveTrack(int oldIndex, int newIndex)
        {
            var temp = items[oldIndex];
            items.RemoveAt(oldIndex);
            items.Insert(newIndex, temp);
            if (LoadedIndex == oldIndex)
            {
                LoadedIndex = newIndex;
            }
            else if (oldIndex < LoadedIndex &&
                     newIndex >= LoadedIndex)
            {
                selectedIndex -= 1;
            }
            else if (oldIndex > LoadedIndex &&
                     newIndex <= LoadedIndex)
            {
                LoadedIndex += 1;
            }
            Invalidate();
        }

        public void clearTrackList()
        {
            items.Clear();
            selectedIndex = -1;
            pendingLoadRequest = false;
            selectedTextEntry = null;
            showHideScrollBar();
            Invalidate();
        }

        public void clearPendingLoadRequest() => pendingLoadRequest = false;

        /** What index did the mouse last click over (for use in the context menu **/
        public int LastIndexClicked { get; set; }

        public ushort Channel { get; set; }

        // Get or set index of selected item.
        public int LoadedIndex
        {
            get => selectedIndex;
            set
            {
                if (value < items.Count && value >= 0 && items[value].type != Command.TEXTITEM)
                {
                    selectedIndex = value;
                    pendingLoadRequest = false;
                    EnsureVisible(selectedIndex);
                    Invalidate();
                }
            }
        }

        public int LoadedTextIndex
        {
            set
            {
                selectedTextEntry?.ClearSelection();
                if (value < items.Count && value >= 0 &&
                (items[value].type == Command.TEXTITEM))
                {
                    selectedTextEntry = items[value];
                    items[value].isSelectedTextItem = true;
                }
                Invalidate();
            }
        }

        public int TrackCount => items.Count;

        // If the requested index is before the first visible index then set the
        // first item to be the requested index. If it is after the last visible
        // index, then set the last visible index to be the requested index.
        public void EnsureVisible(int index)
        {
            if (index < scroll.Value)
            {
                scroll.Value = index;
                Invalidate();
            }
            else if (index >= scroll.Value + DrawCount)
            {
                scroll.Value = index - DrawCount + 1;
                Invalidate();
            }
        }

        public event RequestChangeEventHandler RequestChange;

        public TrackList()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.UserMouse,
                     true);

            scroll = new VScrollBar
            {
                Parent = this,
                Visible = false
            };
            scroll.Scroll += (o, e) => Invalidate();
            Controls.Add(scroll);
            AllowDrop = true;
            // Determine what the item height should be
            // by adding 30% padding after measuring
            // the letter A with the selected font.
            using (var g = CreateGraphics())
            {
                ItemHeight = (int)(g.MeasureString("A", Font).Height * 1.3);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // The base class contains a bitmap, offScreen, for constructing
            // the control and is rendered when all items are populated.
            // This technique prevents flicker.
            using (var gOffScreen = Graphics.FromImage(offScreen))
            {
                gOffScreen.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = ClientRectangle;
                rect.Width -= 2;
                rect.Height = ((items.Count < scroll.Value + DrawCount) ? items.Count : scroll.Value + DrawCount);
                gOffScreen.FillRectangle(SystemBrushes.Window, rect);
                rect.Y = rect.Height;
                rect.Height = ClientRectangle.Height - rect.Height;
                if (rect.Height > 0)
                {
                    gOffScreen.FillRectangle(SystemBrushes.Window, rect);
                }
                int itemTop = 0;

                // The index to be shown as loaded
                int loadedIndex = selectedIndex;
                if (selectedIndex == fromIndex)
                {
                    loadedIndex = hoverIndex;
                }
                else if (fromIndex < selectedIndex &&
                         hoverIndex >= selectedIndex)
                {
                    loadedIndex = selectedIndex - 1;
                }
                else if (fromIndex > selectedIndex &&
                         hoverIndex <= selectedIndex)
                {
                    loadedIndex = selectedIndex + 1;
                }

                // Draw the fonts in the list.
                for (int n = scroll.Value; n < ((items.Count < scroll.Value + DrawCount) ? items.Count : scroll.Value + DrawCount); n++)
                {
                    var rect2 = new Rectangle(0,
                                                      itemTop,
                                                      // If the scroll bar is visible, subtract the scrollbar width
                                                      // otherwise subtract 2 for the width of the rectangle
                                                      ClientSize.Width - (scroll.Visible ? scroll.Width : 1) - 1, ItemHeight);
                    Brush brush;
                    Brush textbrush;
                    if (n == loadedIndex)
                    {
                        brush = SystemBrushes.Highlight;
                        textbrush = SystemBrushes.HighlightText;
                        gOffScreen.FillRectangle(brush, rect2);
                    }
                    else
                    {
                        textbrush = SystemBrushes.ControlText;
                    }

                    /** Which index do we display here **/
                    int indexToShow = n;
                    if (n == hoverIndex)
                    {
                        indexToShow = fromIndex;
                    }
                    else if (fromIndex <= n && hoverIndex >= n)
                    {
                        indexToShow = n + 1;
                    }
                    else if (fromIndex >= n && hoverIndex < n)
                    {
                        indexToShow = n - 1;
                    }
                    switch (items[indexToShow].type)
                    {
                        case Command.FILEITEM:
                            gOffScreen.FillEllipse(Brushes.SteelBlue, 4, itemTop + 4, 8, 8);
                            break;

                        case Command.LIBRARYITEM:
                            gOffScreen.FillEllipse(Brushes.LimeGreen, 4, itemTop + 4, 8, 8);
                            break;

                        case Command.TEXTITEM:
                            if (items[indexToShow].isSelectedTextItem)
                            {
                                gOffScreen.FillRectangle(Brushes.Lavender, rect2);
                            }
                            gOffScreen.DrawString("T", Font, Brushes.Blue, 4.0f, itemTop + 1.0f);
                            break;
                    }

                    // Draw the item
                    gOffScreen.DrawString(items[indexToShow].ToString(), Font, textbrush, new Rectangle(18, itemTop, ClientRectangle.Width - 20, ItemHeight), new StringFormat(StringFormatFlags.NoWrap));
                    rect2.Height -= 1;
                    rect2.Width -= 1;
                    // if drawing the loaded index or the index we are hovering over
                    if (n == loadedIndex || hoverIndex == n)
                    {
                        gOffScreen.DrawRectangle(Pens.Black, rect2);
                    }

                    itemTop += ItemHeight;
                }
                rect = ClientRectangle;
                rect.Height -= 3;

                rect.Width -= (scroll.Visible) ? 2 : 3;
                if (scroll.Visible)
                {
                    rect.Width -= scroll.Width;
                }
                // if we are adding to this control
                if (addTo)
                {
                    gOffScreen.DrawRectangle(SystemPens.Highlight, rect);
                    rect.X++;
                    rect.Y++;
                    rect.Height -= 2;
                    rect.Width -= 2;
                    gOffScreen.DrawRectangle(SystemPens.Highlight, rect);
                }
                else gOffScreen.DrawRectangle(SystemPens.WindowFrame, rect);

                e.Graphics.DrawImage(offScreen, 1, 1);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            LastIndexClicked = indexFromY(e.Y);
            if (e.Button != MouseButtons.Left) return;
            var pt = new Point(e.X, e.Y);
            //Retrieve the item at the specified location within the ListBox.
            int index = indexFromY(e.Y);

            // Starts a drag-and-drop operation.
            if (index >= 0)
            {
                var tldd = new TrackListDragDrop(index, Channel);
                savedFromIndex = index;
                var result = DoDragDrop(tldd, DragDropEffects.Move | DragDropEffects.Copy);
                if (result == DragDropEffects.Move)
                {
                    if (!tldd.moved && index != selectedIndex && Channel == tldd.toChannel)
                    {
                        RequestChange(this, new RequestChangeEventArgs(Channel, ChangeType.SELECTEDINDEX, index));
                        Invalidate();
                    }
                }
                hoverIndex = -1;
                fromIndex = -1;
                savedFromIndex = -1;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int newTop = scroll.Value - (e.Delta / 120);
            if (newTop < 0)
            {
                newTop = 0;
            }
            if (newTop > scroll.Maximum - 9)
                newTop = scroll.Maximum - 9;

            // This stops the scroll going out of index
            if (newTop < 0) { newTop = 0; }

            scroll.Value = newTop;
            Invalidate();
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (e.Data.GetDataPresent(typeof(TrackListDragDrop)))
            {
                var tldd = (TrackListDragDrop)e.Data.GetData(typeof(TrackListDragDrop));
                e.Effect = DragDropEffects.Move;
            }
            else if (e.Data.GetDataPresent(typeof(BAPSPresenter.FolderTempStruct)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else e.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            int index = indexFromY(PointToClient(new Point(e.X, e.Y)).Y);
            if (e.Data.GetDataPresent(typeof(TrackListDragDrop)))
            {
                var tldd = (TrackListDragDrop)e.Data.GetData(typeof(TrackListDragDrop));
                if (Channel == tldd.fromChannel && !addTo)
                {
                    if (index != -1 && index != tldd.fromIndex)
                    {
                        RequestChange(this, new RequestChangeEventArgs(Channel, ChangeType.MOVEINDEX, tldd.fromIndex, index));
                    }
                }
                else
                {
                    RequestChange(this, new RequestChangeEventArgs(tldd.fromChannel, ChangeType.COPY, tldd.fromIndex, tldd.toChannel));
                    if ((e.KeyState & 8) == 0)
                    {
                        RequestChange(this, new RequestChangeEventArgs(tldd.fromChannel, ChangeType.DELETEINDEX, tldd.fromIndex));
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(BAPSPresenter.FolderTempStruct)))
            {
                var fts = (BAPSPresenter.FolderTempStruct)e.Data.GetData(typeof(BAPSPresenter.FolderTempStruct));
                RequestChange(this, new RequestChangeEventArgs(Channel, ChangeType.ADD, fts.fromFolder, fts.fromIndex));
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // TODO(@MattWindsor91): this doesn't seem to do anything?

                var Files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                for (int i = 0; i < Files.Length; i++)
                {
                    string F = Files[i].ToString();
                }
            }
            addTo = false;
            Invalidate();
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (e.Data.GetDataPresent(typeof(TrackListDragDrop)))
            {
                if ((e.KeyState & 8) == 0)
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.Copy;
                }
                var tldd = (TrackListDragDrop)e.Data.GetData(typeof(TrackListDragDrop));
                tldd.toChannel = Channel;
                if (Channel == tldd.fromChannel && (e.KeyState & 8) == 0)
                {
                    fromIndex = savedFromIndex;
                    int yValue = PointToClient(new Point(e.X, e.Y)).Y;
                    int hi = indexFromY(yValue);
                    if (hi != fromIndex)
                    {
                        tldd.moved = true;
                    }
                    if (yValue < 8 && scroll.Value != 0)
                    {
                        System.Threading.Thread.Sleep(40);
                        scroll.Value -= 1;
                        Invalidate();
                    }
                    // THIS MIGHT BE WRONG!
                    else if (yValue > ClientRectangle.Height - 8 && scroll.Value != 0)
                    {
                        System.Threading.Thread.Sleep(40);
                        scroll.Value += 1;
                        Invalidate();
                    }
                    if (hi == -1)
                    {
                        hi = fromIndex;
                    }
                    if (hi != hoverIndex)
                    {
                        hoverIndex = hi;
                        Invalidate();
                    }
                    addTo = false;
                }
                else
                {
                    hoverIndex = -1;
                    fromIndex = -1;
                    /** item has been moved between channels **/
                    if (!addTo)
                    {
                        addTo = true;
                        Invalidate();
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(BAPSPresenter.FolderTempStruct)))
            {
                if (!addTo)
                {
                    addTo = true;
                    Invalidate();
                }
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            addTo = false;
            hoverIndex = fromIndex;
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = HandleKey(e.KeyCode);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool retVal = HandleKey(keyData);
            if (!retVal)
            {
                base.ProcessDialogKey(keyData);
            }
            return retVal;
        }

        private bool HandleKey(Keys keyData)
        {
            bool retVal = false;
            int newSelectedIndex = -1;
            switch (keyData)
            {
                case Keys.Down:
                    newSelectedIndex = findAudioItem(LoadedIndex + 1, false);
                    retVal = true;
                    break;

                case Keys.Up:
                    newSelectedIndex = findAudioItem(LoadedIndex - 1, true);
                    retVal = true;
                    break;

                case Keys.PageDown:
                    newSelectedIndex = findAudioItem(LoadedIndex + DrawCount, false);
                    if (newSelectedIndex == -1)
                    {
                        newSelectedIndex = findAudioItem(items.Count - 1, true);
                    }
                    retVal = true;
                    break;

                case Keys.PageUp:
                    newSelectedIndex = findAudioItem(LoadedIndex - DrawCount, true);
                    if (newSelectedIndex == -1)
                    {
                        newSelectedIndex = findAudioItem(0, false);
                    }
                    retVal = true;
                    break;

                case Keys.Home:
                    newSelectedIndex = findAudioItem(0, false);
                    retVal = true;
                    break;

                case Keys.End:
                    newSelectedIndex = findAudioItem(items.Count - 1, true);
                    retVal = true;
                    break;
            }
            if (newSelectedIndex != -1 && newSelectedIndex != LoadedIndex && !pendingLoadRequest)
            {
                pendingLoadRequest = true;
                RequestChange(this, new RequestChangeEventArgs(Channel, ChangeType.SELECTEDINDEX, newSelectedIndex));
            }
            if (keyData == Keys.Delete)
            {
                if (LoadedIndex != -1)
                {
                    RequestChange(this, new RequestChangeEventArgs(Channel, ChangeType.DELETEINDEX, LoadedIndex));
                }
                retVal = true;
            }
            //opLock.ReleaseMutex();

            return retVal;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.DrawRectangle(Pens.LightGray, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }

        protected override void OnResize(EventArgs e)
        {
            scroll.Bounds = new Rectangle(ClientSize.Width - SCROLL_WIDTH,
                                           0,
                                           SCROLL_WIDTH,
                                           ClientSize.Height);
            showHideScrollBar();
            Invalidate();
            base.OnResize(e);
        }

        private void showHideScrollBar()
        {
            var viewableItemCount = ClientSize.Height / ItemHeight;

            // Determine if scrollbars are needed
            if (items.Count > viewableItemCount)
            {
                scroll.Visible = true;
                offScreen = new Bitmap(ClientSize.Width - SCROLL_WIDTH - 1, ClientSize.Height - 2);
            }
            else
            {
                scroll.Visible = false;
                scroll.Value = 0;
                int w = ClientSize.Width - 1;
                if (w <= 0) w = 1;
                offScreen = new Bitmap(w, ClientSize.Height - 2);
            }
            scroll.Maximum = (items.Count - 14 > 0) ? items.Count - 14 : items.Count;
        }

        private int indexFromY(int y)
        {
            int index = scroll.Value + (y / ItemHeight);
            return index >= items.Count ? -1 : index;
        }

        private int findAudioItem(int startIndex, bool goingUp)
        {
            int addMe = 1;
            if (goingUp) addMe = -1;
            for (; ((goingUp) ? -1 : startIndex) < ((goingUp) ? startIndex : items.Count); startIndex += addMe)
            {
                if (items[startIndex].type != Command.TEXTITEM)
                {
                    return startIndex;
                }
            }
            return -1;
        }

        protected int ItemHeight { get; set; }
        protected int DrawCount => ClientRectangle.Height / ItemHeight;

        private const int SCROLL_WIDTH = 20;

        private int selectedIndex = -1;
        private bool pendingLoadRequest = false;
        private EntryInfo selectedTextEntry = null;
        private int hoverIndex = -1;
        private int fromIndex = -1;
        private int savedFromIndex;
        private bool addTo = false;
        private Bitmap offScreen;
        private VScrollBar scroll;
    }
}
