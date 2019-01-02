using System;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSFormControls
{
    public partial class TimeLine : Control
    {
        private enum TimeLineMoveStatus
        {
            TIMELINE_MOVE_CHAN0 = 0,
            TIMELINE_MOVE_CHAN1 = 1,
            TIMELINE_MOVE_CHAN2 = 2,
            TIMELINE_MOVE_NONE
        };

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
            trackDuration = new int[3];
            trackDuration[0] = 0;
            trackDuration[1] = 0;
            trackDuration[2] = 0;

            trackPosition = new int[3];
            trackPosition[0] = 0;
            trackPosition[1] = 0;
            trackPosition[2] = 0;

            startTime = new int[3];
            startTime[0] = -1;
            startTime[1] = -1;
            startTime[2] = -1;

            trackDurationCache = new int[3];
            trackDurationCache[0] = 0;
            trackDurationCache[1] = 0;
            trackDurationCache[2] = 0;

            trackPositionCache = new int[3];
            trackPositionCache[0] = 0;
            trackPositionCache[1] = 0;
            trackPositionCache[2] = 0;

            startTimeCache = new int[3];
            startTimeCache[0] = -1;
            startTimeCache[1] = -1;
            startTimeCache[2] = -1;

            moveOffset = new int[3];
            moveOffset[0] = 0;
            moveOffset[1] = 0;
            moveOffset[2] = 0;

            locked = new bool[3];
            locked[0] = false;
            locked[1] = false;
            locked[2] = false;

            moveStatus = TimeLineMoveStatus.TIMELINE_MOVE_NONE;
            startMoveAtX = 0;

            boundingBox = new Rectangle[3];
            boundingBox[0] = new Rectangle(0, 0, 0, 0);
            boundingBox[1] = new Rectangle(0, 0, 0, 0);
            boundingBox[2] = new Rectangle(0, 0, 0, 0);


            cachedTime = DateTime.Now;

            dragEnabled = true;

            Font = new Font("Segoe UI", 8, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        public void UpdateDuration(int channel, int duration)
        {
            trackDurationCache[channel] = duration;
            if (moveStatus == TimeLineMoveStatus.TIMELINE_MOVE_NONE)
            {
                trackDuration[channel] = duration;
            }
        }
        public void UpdatePosition(int channel, int position)
        {
            trackPositionCache[channel] = position;
            if (moveStatus == TimeLineMoveStatus.TIMELINE_MOVE_NONE)
            {
                trackPosition[channel] = position;
            }
        }
        public void UpdateStartTime(int channel, int newStartTime)
        {
            startTimeCache[channel] = newStartTime;
            if (moveStatus == TimeLineMoveStatus.TIMELINE_MOVE_NONE)
            {
                startTime[channel] = newStartTime;
            }
        }

        public bool DragEnabled
        {
	        set
		    {
                dragEnabled = value;
                UpdateStartTime(0, 0);
                UpdateStartTime(1, 0);
                UpdateStartTime(2, 0);
            }
        }

        public event TimeLineEventHandler StartTimeChanged;

	    protected override void OnPaint(PaintEventArgs e)
        {
            const int drawStartPosition = 20;
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var dt = cachedTime;
            for (int i = 0; i < 3; i++)
            {
                var rect = new Rectangle(2, (i * 11) - 2, 10, 14);
                e.Graphics.DrawString((i + 1).ToString(), Font, Brushes.Black, rect);
                int width = (((trackDuration[i] - (locked[i] ? trackPosition[i] : 0)) / 1000) * thirtySecondPixels) / 30;
                int startOffset = 0;
                int timeOffset = 0;
                if (startTime[i] != -1)
                {
                    startOffset = ((startTime[i] / 1000) * thirtySecondPixels) / 30;
                    timeOffset = startTime[i];
                }
                if (startOffset + moveOffset[i] < 0)
                {
                    moveOffset[i] = -startOffset;
                    startOffset = 0;
                }
                else
                {
                    startOffset += moveOffset[i];
                }
                timeOffset += (moveOffset[i] * 30000) / thirtySecondPixels;
                rect = new Rectangle(drawStartPosition + 40 + startOffset, i * 11, width, 8);
                boundingBox[i] = rect;
                e.Graphics.FillRectangle((locked[i]) ? runningColour : stoppedColour, rect);
                int starttextx = rect.X - 50;
                rect.X += rect.Width + 1;
                rect.Y -= 2;
                rect.Height += 4;
                rect.Width = 50;
                if (width != 0)
                {
                    e.Graphics.DrawString(dt.AddMilliseconds(timeOffset + trackDuration[i] - (locked[i] ? trackPosition[i] : 0)).ToString("T"), this.Font, Brushes.Black, rect);
                }
                if (timeOffset != 0)
                {
                    rect.X = starttextx;
                    e.Graphics.DrawString(dt.AddMilliseconds(timeOffset).ToString("T"), this.Font, Brushes.Black, rect);
                }
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!dragEnabled) return;
            Cursor.Current = Cursors.Default;
            for (int i = 0; i < 3; i++)
            {
                if (boundingBox[i].Contains(e.X, e.Y) && !locked[i])
                {
                    moveStatus = (TimeLineMoveStatus)i;
                    startMoveAtX = e.X;
                Cursor.Current = Cursors.SizeWE;
                    break;
                }
            }
        }

	    protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (moveStatus != TimeLineMoveStatus.TIMELINE_MOVE_NONE && e.Button == MouseButtons.Left)
            {
                moveOffset[(int)moveStatus] = e.X - startMoveAtX;
                Invalidate();
            }
        }

	    protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (moveStatus != TimeLineMoveStatus.TIMELINE_MOVE_NONE)
            {
                startTime[(int)moveStatus] = startTime[(int)moveStatus] + (moveOffset[(int)moveStatus] * 30000) / thirtySecondPixels;
                startTimeCache[(int)moveStatus] = startTime[(int)moveStatus];
                if (startTime[(int)moveStatus] > 0)
                {
                    StartTimeChanged(this, new TimeLineEventArgs((int)moveStatus, (((cachedTime.Minute * 60) + cachedTime.Second) * 1000) +
                                                                                cachedTime.Millisecond +
                                                                                startTime[(int)moveStatus]));
                }
            }
            moveStatus = TimeLineMoveStatus.TIMELINE_MOVE_NONE;
            for (int i = 0; i < 3; i++)
            {
                trackDuration[i] = trackDurationCache[i];
                trackPosition[i] = trackPositionCache[i];
                startTime[i] = startTimeCache[i];
                moveOffset[i] = 0;
            }
            Invalidate();
        }

    	public void Tick()
        {
            if (moveStatus == TimeLineMoveStatus.TIMELINE_MOVE_NONE)
            {
                cachedTime = DateTime.Now;
                Invalidate();
            }
        }

        public bool Lock(ushort channelID) => locked[channelID] = true;
        public bool Unlock(ushort channelID) => locked[channelID] = false;

        private bool[] locked;
        private bool dragEnabled = true;
        private int[] trackDuration;
		private int[] trackPosition;
		private int[] startTime;
		private int[] trackDurationCache;
		private int[] trackPositionCache;
		private int[] startTimeCache;
		private int[] moveOffset;
		private Rectangle[] boundingBox;
		private DateTime cachedTime;


        private Brush stoppedColour = Brushes.DeepSkyBlue;
        private Brush runningColour = Brushes.Orchid;

		private TimeLineMoveStatus moveStatus;
        private int startMoveAtX;
        private const int thirtySecondPixels = 60;
    }
}
