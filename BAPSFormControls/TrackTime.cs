using BAPSCommon;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BAPSFormControls
{
    internal enum TrackTimeMovingType
    {
        NONE,
        POSITION,
        INTROPOSITION,
        CUEPOSITION
    }

    public partial class TrackTime : Control
    {
        public event PositionRequestChangeEventHandler PositionChanged;

        private const int BASE_Y_LINE = 44;
        private const int BASE_Y_INTRO_ARROW = 8;
        private const int BASE_Y_CUE_ARROW = 12;
        private const int WIDTH_ARROW = 8;
        private const int HEIGHT_ARROW = 8;
        private const int HEIGHT_ARROW_HALF = 4;

        public int Channel { get; set; }

        public int Duration
        {
            get => duration;
            set
            {
                if (value == duration) return;
                duration = value;
                Invalidate();
            }
        }

        private int duration = 0;

        public int Position
        {
            get => position;
            set
            {
                if (position == duration) return;
                position = value;
                Invalidate();
            }
        }

        private int position = 0;

        public int CuePosition
        {
            get => cuePosition;
            set
            {
                if (cuePosition == duration) return;
                cuePosition = value;
                Invalidate();
            }
        }

        private int cuePosition = 0;

        public int IntroPosition
        {
            get => introPosition;
            set
            {
                introPosition = value;
                Invalidate();
            }
        }

        private float Division => ClientRectangle.Width / (float)duration;

        private int introPosition = 0;

        private TrackTimeMovingType movingItem = TrackTimeMovingType.NONE;

        private Brush backBrush = SystemBrushes.Control;
        private Brush cueBrush = Brushes.Crimson;
        private Brush introBrush = Brushes.ForestGreen;

        private ToolTip tooltip;

        public TrackTime()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserMouse,
                     true);
            DoubleBuffered = true;
            tooltip = new ToolTip
            {
                // Set up the delays for the ToolTip.
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                // Force the ToolTip text to be displayed whether or not the form is active.
                ShowAlways = true
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillRectangle(backBrush, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));

            if (duration != 0)
            {
                DrawSecondaryPosition(e, introBrush, IntroPosition, BASE_Y_INTRO_ARROW);
                DrawSecondaryPosition(e, cueBrush, CuePosition, BASE_Y_LINE + BASE_Y_CUE_ARROW);
                DrawPrimaryPosition(e);
                DrawMarkers(e);
            }
            DrawMainLine(e);
        }

        private int XToPos(int x)
        {
            if (x < 0) return 0;
            if (ClientRectangle.Width <= x) return duration;
            return (int)(x / Division);
        }

        private int PosToX(int position) => (int)(Division * position);

        private void DrawSecondaryPosition(PaintEventArgs e, Brush brush, int position, int arrowY)
        {
            var posX = PosToX(position);
            e.Graphics.FillRectangle(brush, new Rectangle(0, 0, posX, ClientRectangle.Height));
            DrawMiniArrow(e, posX, arrowY);
            DrawTime(e, position, posX, arrowY);
        }

        private void DrawPrimaryPosition(PaintEventArgs e)
        {
            var positionPoint = PosToX(position);
            e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, BASE_Y_LINE - 2, positionPoint, 2));
            DrawPositionArrow(e, positionPoint, BASE_Y_LINE - 2);
        }

        private void DrawMainLine(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Black, 0, BASE_Y_LINE, ClientRectangle.Width, BASE_Y_LINE);
            e.Graphics.DrawLine(Pens.Black, 0, BASE_Y_LINE - 5, 0, BASE_Y_LINE + 5);
            e.Graphics.DrawLine(Pens.Black, ClientRectangle.Width - 1, BASE_Y_LINE + 5, ClientRectangle.Width - 1, BASE_Y_LINE - 5);
        }

        private const int MarkerDivisor = 10_000;
        private const int MarkerMajorTick = 6;
        private const int MarkerMajorHeight = 4;
        private const int MarkerMinorHeight = 2;

        private static bool IsMajorTick(int i) => i % (MarkerDivisor * MarkerMajorTick) == 0;

        private void DrawMarkers(PaintEventArgs e)
        {
            for (int i = MarkerDivisor; i < duration; i += MarkerDivisor)
            {
                var x = PosToX(i);
                int markerHeight = IsMajorTick(i) ? MarkerMajorHeight : MarkerMinorHeight;
                e.Graphics.DrawLine(Pens.Black,
                                    x,
                                    BASE_Y_LINE - markerHeight,
                                    x,
                                    BASE_Y_LINE);
            }
        }

        private static void DrawPositionArrow(PaintEventArgs e, int x, int y)
        {
            var positionPointer = new Point[5]
            {
                new Point(x, y),
                new Point(x + 5, y - 5),
                new Point(x + 5, y - 20),
                new Point(x - 5, y - 20),
                new Point(x - 5, y - 5)
            };
            e.Graphics.FillPolygon(Brushes.Black, positionPointer, FillMode.Alternate);
        }

        private static void DrawMiniArrow(PaintEventArgs e, int x, int y)
        {
            e.Graphics.DrawLine(Pens.Brown, x, y, x, y + HEIGHT_ARROW);
            e.Graphics.DrawLine(Pens.Brown, x, y + HEIGHT_ARROW_HALF, x + 4, y);
            e.Graphics.DrawLine(Pens.Brown, x, y + HEIGHT_ARROW_HALF, x + 4, y + HEIGHT_ARROW);
            e.Graphics.DrawLine(Pens.Brown, x, y + HEIGHT_ARROW_HALF, x + 8, y + HEIGHT_ARROW_HALF);
        }

        private void DrawTime(PaintEventArgs e, int time, int x, int y)
        {
            e.Graphics.DrawString(Utils.MillisecondsToTimeString(time),
                        Font,
                        new SolidBrush(ForeColor),
                        new Rectangle(x + 12, y - 2, 50, 12));
        }

        private void SetMouseCursor()
        {
            Cursor.Current = movingItem == TrackTimeMovingType.NONE ? Cursors.Default : Cursors.SizeWE;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            SetMouseCursor();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                OnPossibleMouseDrag(e);
            }
            else
            {
                OnPossibleMouseOver(e);
            }
        }

        private void OnPossibleMouseDrag(MouseEventArgs e)
        {
            int newPosition = XToPos(e.X);
            switch (movingItem)
            {
                case TrackTimeMovingType.POSITION:
                    HandlePositionDrag(newPosition);
                    break;

                case TrackTimeMovingType.CUEPOSITION:
                    HandleCueDrag(newPosition);
                    break;

                case TrackTimeMovingType.INTROPOSITION:
                    HandleIntroDrag(newPosition);
                    break;
            }
        }

        private void SetPositionAndNotify(int newPosition)
        {
            position = newPosition;
            PositionChanged.Invoke(this, new PositionRequestChangeEventArgs((ushort)Channel, PositionType.Position, position));
        }

        private void HandlePositionDrag(int newPosition)
        {
            if (position == newPosition) return;
            SetPositionAndNotify(Math.Max(newPosition, cuePosition));
        }

        private void HandleCueDrag(int newPosition)
        {
            if (cuePosition == newPosition) return;

            cuePosition = newPosition;
            PositionChanged.Invoke(this, new PositionRequestChangeEventArgs((ushort)Channel, PositionType.Cue, cuePosition));

            if (cuePosition > position) SetPositionAndNotify(cuePosition);
        }

        private void HandleIntroDrag(int newPosition)
        {
            if (introPosition == newPosition) return;
            introPosition = newPosition;
            PositionChanged.Invoke(this, new PositionRequestChangeEventArgs((ushort)Channel, PositionType.Intro, introPosition));
        }

        private void OnPossibleMouseOver(MouseEventArgs e)
        {
            var pt = e.Location;
            if (IntersectsWithPositionMarker(pt))
            {
                movingItem = TrackTimeMovingType.POSITION;
            }
            else if (IntersectsWithIntroMarker(pt))
            {
                movingItem = TrackTimeMovingType.INTROPOSITION;
                tooltip.SetToolTip(this, "Intro Position");
            }
            else if (IntersectsWithCueMarker(pt))
            {
                movingItem = TrackTimeMovingType.CUEPOSITION;
                tooltip.SetToolTip(this, "Cue Position");
            }
            else
            {
                movingItem = TrackTimeMovingType.NONE;
                tooltip.SetToolTip(this, null);
            }
            SetMouseCursor();
        }

        private bool IntersectsWithPositionMarker(Point p)
        {
            int positionPoint = PosToX(position);
            var rect = new Rectangle(positionPoint - 5, BASE_Y_LINE - 22, 10, 20);
            return rect.Contains(p);
        }

        private bool IntersectsWithCueMarker(Point p)
        {
            int cuePoint = PosToX(cuePosition);
            var rect = new Rectangle(cuePoint - 2, 0, 4, ClientRectangle.Height);
            var rect2 = new Rectangle(cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW, WIDTH_ARROW, HEIGHT_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }

        private bool IntersectsWithIntroMarker(Point p)
        {
            int introPoint = PosToX(introPosition);
            var rect = new Rectangle(introPoint - 2, 0, 4, ClientRectangle.Height);
            var rect2 = new Rectangle(introPoint, BASE_Y_INTRO_ARROW, WIDTH_ARROW, HEIGHT_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }
    }
}
