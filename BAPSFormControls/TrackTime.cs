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
        public event EventHandler PositionChanged;

        public event EventHandler IntroPositionChanged;

        public event EventHandler CuePositionChanged;

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
                duration = value;
                division = ClientRectangle.Width / (float)duration;
                Invalidate();
            }
        }

        private int duration = 0;

        public int Position
        {
            get => position;
            set
            {
                position = value;
                Invalidate();
            }
        }

        public int PositionPoint => (int)(division * position);
        private int position = 0;

        public int SilencePosition
        {
            get => silencePosition;
            set
            {
                silencePosition = value;
                silencePath = new GraphicsPath();

                var rect = new Rectangle(0, 0, SilencePoint, ClientRectangle.Height);
                silencePath.AddRectangle(rect);
                Invalidate();
            }
        }

        public int SilencePoint => (int)(division * silencePosition);
        private int silencePosition = 0;

        public int CuePosition
        {
            get => cuePosition;
            set
            {
                cuePosition = value;
                cuePath = new GraphicsPath();

                var rect = new Rectangle(0, 0, CuePoint, ClientRectangle.Height);
                cuePath.AddRectangle(rect);
                Invalidate();
            }
        }

        int CuePoint => (int)(division * cuePosition);
        private int cuePosition = 0;

        public int IntroPosition
        {
            get => introPosition;
            set
            {
                introPosition = value;
                introPath = new GraphicsPath();

                var rect = new Rectangle(0, 0, IntroPoint, ClientRectangle.Height);
                introPath.AddRectangle(rect);
                Invalidate();
            }
        }

        int IntroPoint => (int)(division * introPosition);
        private int introPosition = 0;

        private TrackTimeMovingType movingItem = TrackTimeMovingType.NONE;
        private float division = 0.0f;

        private Brush backBrush = SystemBrushes.Control;
        private Brush cueBrush = Brushes.Crimson;
        private Brush introBrush = Brushes.ForestGreen;
        private int curveWidth = 20;

        private GraphicsPath backgroundPath;
        private GraphicsPath introPath;
        private GraphicsPath cuePath;
        private GraphicsPath silencePath;

        private ToolTip tooltip;

        public TrackTime()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserMouse,
                     true);
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
            backgroundPath = new GraphicsPath();
            var rect = new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height);
            backgroundPath.AddRectangle(rect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(backBrush, backgroundPath);

            if (duration != 0)
            {
                if (IntroPoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(introBrush, 0, curveWidth / 2, IntroPoint, ClientRectangle.Height - curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(introBrush, introPath);
                }
                if (CuePoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(cueBrush, 0, curveWidth, CuePoint, ClientRectangle.Height - (int)1.5 * curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(cueBrush, cuePath);
                }

                if (SilencePoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(Brushes.Black, 0, curveWidth / 2, SilencePoint, ClientRectangle.Height - curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(Brushes.Black, silencePath);
                }

                e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, BASE_Y_LINE - 2, PositionPoint, 2));
                DrawMarkers(e, division);
                /** Draw the cue arrow and text **/
                DrawMiniArrow(e, CuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW);
                DrawTime(e, cuePosition, CuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW);
                /** Draw the intro arrow and text **/
                DrawMiniArrow(e, IntroPoint, BASE_Y_INTRO_ARROW);
                DrawTime(e, introPosition, IntroPoint, BASE_Y_INTRO_ARROW);
                /** Draw the position arrow **/
                DrawPositionArrow(e, PositionPoint, BASE_Y_LINE - 2);
            }
            DrawMainLine(e);
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

        private void DrawMarkers(PaintEventArgs e, float division)
        {
            int markerCount = duration / MarkerDivisor;
            for (int i = MarkerDivisor; i < duration; i += MarkerDivisor)
            {
                int markerHeight = IsMajorTick(i) ? MarkerMajorHeight : MarkerMinorHeight;
                e.Graphics.DrawLine(Pens.Black,
                                    (int)(division * i),
                                    BASE_Y_LINE - markerHeight,
                                    (int)(division * i),
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
                int newPosition;
                if (e.X < 0)
                {
                    newPosition = 0;
                }
                else if (e.X > ClientRectangle.Width - 1)
                {
                    newPosition = duration;
                }
                else
                {
                    newPosition = (int)((e.X) / division);
                }
                switch (movingItem)
                {
                    case TrackTimeMovingType.POSITION:
                        if (position != newPosition)
                        {
                            if (newPosition >= cuePosition)
                            {
                                position = newPosition;
                            }
                            else
                            {
                                position = cuePosition;
                            }
                            PositionChanged(this, EventArgs.Empty);
                        }
                        break;

                    case TrackTimeMovingType.CUEPOSITION:
                        if (cuePosition != newPosition)
                        {
                            cuePosition = newPosition;
                            CuePositionChanged(this, EventArgs.Empty);
                            if (cuePosition > position)
                            {
                                position = cuePosition;
                                PositionChanged(this, EventArgs.Empty);
                            }
                        }
                        break;

                    case TrackTimeMovingType.INTROPOSITION:
                        if (introPosition != newPosition)
                        {
                            introPosition = newPosition;
                            Invalidate();
                            IntroPositionChanged(this, EventArgs.Empty);
                        }
                        break;
                }
            }
            else
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
        }

        private bool IntersectsWithPositionMarker(Point p)
        {
            int positionPoint = (int)(division * position);
            var rect = new Rectangle(positionPoint - 5, BASE_Y_LINE - 22, 10, 20);
            return rect.Contains(p);
        }

        private bool IntersectsWithCueMarker(Point p)
        {
            int cuePoint = (int)(division * cuePosition);
            var rect = new Rectangle(cuePoint - 2, 0, 4, ClientRectangle.Height);
            var rect2 = new Rectangle(cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW, WIDTH_ARROW, HEIGHT_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }

        private bool IntersectsWithIntroMarker(Point p)
        {
            int introPoint = (int)(division * introPosition);
            var rect = new Rectangle(introPoint - 2, 0, 4, ClientRectangle.Height);
            var rect2 = new Rectangle(introPoint, BASE_Y_INTRO_ARROW, WIDTH_ARROW, HEIGHT_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }
    }
}
