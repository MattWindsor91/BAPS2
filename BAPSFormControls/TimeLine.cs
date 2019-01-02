using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BAPSFormControls
{
    public partial class TimeLine : Control
    {
        public class TimeLineEventArgs : EventArgs
        {
            public int channel;
            public int startTime;

            public TimeLineEventArgs(int _channel, int _startTime)
            {
                channel = _channel;
                startTime = _startTime;
            }
        }

        public delegate void TimeLineEventHandler(object sender, TimeLineEventArgs e);

        public TimeLine() : base()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor,
                     true);
            TabStop = false;

            channelLines = new ChannelLine[3];
            for (var i = 0; i < channelLines.Length; i++)
            {
                channelLines[i] = new ChannelLine(i);
                channelLines[i].StartTimeChanged += OnStartTimeChanged;
            }

            cachedTime = DateTime.Now;

            Font = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private ChannelLine ChannelAt(int index) => channelLines.ElementAtOrDefault(index);

        public void UpdateDuration(int channel, int duration) => ChannelAt(channel)?.SetDuration(duration, IsDragging);

        public void UpdatePosition(int channel, int position) => ChannelAt(channel)?.SetPosition(position, IsDragging);

        public void UpdateStartTime(int channel, int newStartTime) => ChannelAt(channel)?.SetStartTime(newStartTime, IsDragging);

        public bool DragEnabled
        {
            set
            {
                dragEnabled = value;
                for (int i = 0; i < channelLines.Length; i++) UpdateStartTime(i, 0);
            }
        }

        public event TimeLineEventHandler StartTimeChanged;

        public static int ToTimelineWidth(int rw) => rw / 1_000 * thirtySecondPixels / 30;

        public static int OfTimelineWidth(int tw) => tw * 30_000 / thirtySecondPixels;

        protected override void OnPaint(PaintEventArgs e)
        {
            const int drawStartPosition = 20;
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var dt = cachedTime;
            foreach (var (cl, i) in channelLines.Select((cl, i) => (cl, i)))
            {
                PaintChannel(e, drawStartPosition, dt, cl, i);
            }

            e.Graphics.DrawLine(Pens.Black, 0, 33, ClientRectangle.Width, 33);

            var rect2 = new Rectangle(drawStartPosition + 10, 40, thirtySecondPixels, 10);

            while (rect2.X + thirtySecondPixels - 5 < ClientRectangle.Width)
            {
                e.Graphics.DrawLine(Pens.Black, rect2.X + 30, 0, rect2.X + 30, 38);
                e.Graphics.DrawString(dt.ToString("T"), Font, (rect2.X == drawStartPosition + 10) ? Brushes.Black : Brushes.DarkGray, rect2, sf);
                rect2.X += thirtySecondPixels;
                dt = dt.AddSeconds(30);
            }
        }

        private void PaintChannel(PaintEventArgs e, int drawStartPosition, DateTime dt, ChannelLine cl, int i)
        {
            var rect = new Rectangle(2, (i * 11) - 2, 10, 14);
            e.Graphics.DrawString((i + 1).ToString(), Font, Brushes.Black, rect);
            var widthRaw = cl.Duration - (cl.locked ? cl.Position : 0);
            int width = ToTimelineWidth(widthRaw);
            int startOffset = 0;
            int timeOffset = 0;
            if (cl.StartTime != -1)
            {
                startOffset = ToTimelineWidth(cl.StartTime);
                timeOffset = cl.StartTime;
            }
            if (startOffset + cl.moveOffset < 0)
            {
                cl.moveOffset = -startOffset;
                startOffset = 0;
            }
            else
            {
                startOffset += cl.moveOffset;
            }
            timeOffset += OfTimelineWidth(cl.moveOffset);
            rect = new Rectangle(drawStartPosition + 40 + startOffset, i * 11, width, 8);
            cl.boundingBox = rect;
            e.Graphics.FillRectangle(cl.locked ? runningColour : stoppedColour, rect);
            int starttextx = rect.X - 50;
            rect.X += rect.Width + 1;
            rect.Y -= 2;
            rect.Height += 4;
            rect.Width = 50;
            if (width != 0) DrawTime(e.Graphics, rect, dt.AddMilliseconds(timeOffset + widthRaw));
            if (timeOffset != 0)
            {
                rect.X = starttextx;
                DrawTime(e.Graphics, rect, dt.AddMilliseconds(timeOffset));
            }
        }

        private void DrawTime(Graphics g, Rectangle rect, DateTime time)
        {
            g.DrawString(time.ToLongTimeString(), Font, Brushes.Black, rect);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!dragEnabled) return;
            Cursor.Current = Cursors.Default;

            currentlyMovingChannel = channelLines.First(cl => cl.IsDraggable(e.X, e.Y));
            if (currentlyMovingChannel != null) startMoveAtX = e.X;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor.Current = GetLineHoverCursor(e);
            if (currentlyMovingChannel == null || e.Button != MouseButtons.Left) return;
            currentlyMovingChannel.moveOffset = e.X - startMoveAtX;
            Invalidate();
        }

        private Cursor GetLineHoverCursor(MouseEventArgs e)
        {
            var isOverDraggable = channelLines.Any(cl => cl.IsDraggable(e.X, e.Y));
            return isOverDraggable ? Cursors.SizeWE : Cursors.Default;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            currentlyMovingChannel?.StopDragging(cachedTime);
            currentlyMovingChannel = null;
            foreach (var cl in channelLines) cl.CopyCaches();
            Invalidate();
        }

        public void Tick()
        {
            if (IsDragging) return;
            cachedTime = DateTime.Now;
            Invalidate();
        }

        private bool IsDragging => currentlyMovingChannel != null;

        public bool Lock(ushort channelID) => channelLines[channelID].locked = true;

        public bool Unlock(ushort channelID) => channelLines[channelID].locked = false;

        private class ChannelLine
        {
            public int channelID;

            public bool locked = false;

            public int Duration { get; private set; } = 0;
            public int Position { get; private set; } = 0;
            public int StartTime { get; private set; } = -1;

            private int trackDurationCache = 0;
            private int trackPositionCache = 0;
            private int startTimeCache = -1;
            public int moveOffset = 0;
            public Rectangle boundingBox = new Rectangle();

            public ChannelLine(int channelID)
            {
                this.channelID = channelID;
            }

            public void SetDuration(int value, bool isDragging)
            {
                trackDurationCache = value;
                if (!isDragging) Duration = value;
            }

            public void SetPosition(int value, bool isDragging)
            {
                trackPositionCache = value;
                if (!isDragging) Position = value;
            }

            public void SetStartTime(int value, bool isDragging)
            {
                startTimeCache = value;
                if (!isDragging) StartTime = value;
            }

            public event TimeLineEventHandler StartTimeChanged;

            public void CopyCaches()
            {
                Duration = trackDurationCache;
                Position = trackPositionCache;
                StartTime = startTimeCache;
                moveOffset = 0;
            }

            public bool IsDraggable(int x, int y) => boundingBox.Contains(x, y) && !locked;

            public void StopDragging(DateTime cachedTime)
            {
                StartTime += OfTimelineWidth(moveOffset);
                startTimeCache = StartTime;
                if (0 < StartTime)
                {
                    var cachedTimeMsecs = cachedTime.TimeOfDay.Ticks / TimeSpan.TicksPerMillisecond;
                    var absoluteStartTime = (int)Math.Min(int.MaxValue, StartTime + cachedTimeMsecs);
                    StartTimeChanged?.Invoke(this, new TimeLineEventArgs(channelID, absoluteStartTime));
                }
            }
        }

        private ChannelLine[] channelLines;

        private DateTime cachedTime;

        private Brush stoppedColour = Brushes.DeepSkyBlue;
        private Brush runningColour = Brushes.Orchid;

        private bool dragEnabled = true;
        private ChannelLine currentlyMovingChannel = null;
        private int startMoveAtX;
        private const int thirtySecondPixels = 60;

        public void OnStartTimeChanged(object sender, TimeLineEventArgs e) => StartTimeChanged?.Invoke(sender, e);
    }
}
