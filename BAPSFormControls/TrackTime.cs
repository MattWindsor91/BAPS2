using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BAPSFormControls
{
    enum TrackTimeMovingType
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

        const int BASE_Y_LINE = 44;
        const int BASE_Y_INTRO_ARROW = 8;
        const int WIDTH_INTRO_ARROW = 8;
        const int HEIGHT_INTRO_ARROW = 8;
        const int HEIGHT_INTRO_ARROW_HALF = 4;
        const int BASE_Y_CUE_ARROW = 12;
        const int WIDTH_CUE_ARROW = 8;
        const int HEIGHT_CUE_ARROW = 8;
        const int HEIGHT_CUE_ARROW_HALF = 4;

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
        private int position = 0;

        public int SilencePosition
        {
            get => silencePosition;
            set
            {
                silencePosition = value;
                var silencePoint = (int)(division * silencePosition);
                silencePath = new GraphicsPath();

                var rect = new Rectangle(0, 0, silencePoint, ClientRectangle.Height);
                silencePath.AddRectangle(rect);
                Invalidate();
            }
        }
        private int silencePosition = 0;

        public int CuePosition
        {
            get => cuePosition;
            set
            {
                cuePosition = value;
                var cuePoint = (int)(division * cuePosition);
                cuePath = new GraphicsPath();

                var rect = new Rectangle(0, 0, cuePoint, ClientRectangle.Height);
                cuePath.AddRectangle(rect);
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
                var introPoint = (int)(division * introPosition);
                introPath = new GraphicsPath();

                var rect = new Rectangle(0, 0, introPoint, ClientRectangle.Height);
                introPath.AddRectangle(rect);
                Invalidate();
            }
        }
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
            //Paint the Text property on the control

            if (duration != 0)
            {
                float division = ClientRectangle.Width / (float)duration;
                int positionPoint = (int)(division * position);
                int silencePoint = (int)(division * silencePosition);
                int cuePoint = (int)(division * cuePosition);
                int introPoint = (int)(division * introPosition);

                if (introPoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(introBrush, 0, curveWidth / 2, introPoint, ClientRectangle.Height - curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(introBrush, introPath);
                }
                if (cuePoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(cueBrush, 0, curveWidth, cuePoint, ClientRectangle.Height - (int)1.5 * curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(cueBrush, cuePath);
                }

                if (silencePoint < curveWidth / 2)
                {
                    e.Graphics.FillRectangle(Brushes.Black, 0, curveWidth / 2, silencePoint, ClientRectangle.Height - curveWidth);
                }
                else
                {
                    e.Graphics.FillPath(Brushes.Black, silencePath);
                }

                e.Graphics.FillRectangle(Brushes.Red, new Rectangle(0, BASE_Y_LINE - 2, positionPoint, 2));

                int markerCount = (duration / 10000);
                int i = 0;
                for (i = 10000; i < duration; i += 10000)
                {
                    int markerHeight = 2;
                    if (i % 60000 == 0)
                    {
                        markerHeight = 4;
                    }
                    e.Graphics.DrawLine(Pens.Black,
                                        (int)(division * i),
                                        BASE_Y_LINE - markerHeight,
                                        (int)(division * i),
                                        BASE_Y_LINE);
                }
                /** Draw the cue arrow and text **/
                e.Graphics.DrawLine(Pens.Brown, cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW, cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW);
                e.Graphics.DrawLine(Pens.Brown, cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW_HALF, cuePoint + 4, BASE_Y_LINE + BASE_Y_CUE_ARROW);
                e.Graphics.DrawLine(Pens.Brown, cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW_HALF, cuePoint + 4, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW);
                e.Graphics.DrawLine(Pens.Brown, cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW_HALF, cuePoint + 8, BASE_Y_LINE + BASE_Y_CUE_ARROW + HEIGHT_CUE_ARROW_HALF);
                e.Graphics.DrawString(Utils.MillisecondsToTimeString(cuePosition),
                                        Font,
                                        new SolidBrush(ForeColor),
                                        new Rectangle(cuePoint + 12, BASE_Y_LINE + BASE_Y_CUE_ARROW - 2, 50, 12));
                /** Draw the intro arrow and text **/
                e.Graphics.DrawLine(Pens.Brown, introPoint, BASE_Y_INTRO_ARROW, introPoint, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW);
                e.Graphics.DrawLine(Pens.Brown, introPoint, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW_HALF, introPoint + 4, BASE_Y_INTRO_ARROW);
                e.Graphics.DrawLine(Pens.Brown, introPoint, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW_HALF, introPoint + 4, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW);
                e.Graphics.DrawLine(Pens.Brown, introPoint, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW_HALF, introPoint + WIDTH_INTRO_ARROW, BASE_Y_INTRO_ARROW + HEIGHT_INTRO_ARROW_HALF);
                e.Graphics.DrawString(Utils.MillisecondsToTimeString(introPosition),
                                        Font,
                                        new SolidBrush(ForeColor),
                                        new Rectangle(introPoint + 12, BASE_Y_INTRO_ARROW - 2, 50, 12));
                /** Draw the position arrow **/
                var positionPointer = new Point[5]
        
                                               {
                    new Point(positionPoint, BASE_Y_LINE - 2),
												new Point(positionPoint + 5, BASE_Y_LINE - 7),
												new Point(positionPoint + 5, BASE_Y_LINE - 22),
												new Point(positionPoint - 5, BASE_Y_LINE - 22),
												new Point(positionPoint - 5, BASE_Y_LINE - 7)};
                e.Graphics.FillPolygon(Brushes.Black, positionPointer, FillMode.Alternate);
            }
            e.Graphics.DrawLine(Pens.Black, 0, BASE_Y_LINE, ClientRectangle.Width, BASE_Y_LINE);
            e.Graphics.DrawLine(Pens.Black, 0, BASE_Y_LINE - 5, 0, BASE_Y_LINE + 5);
            e.Graphics.DrawLine(Pens.Black, ClientRectangle.Width - 1, BASE_Y_LINE + 5, ClientRectangle.Width - 1, BASE_Y_LINE - 5);

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
            var rect2 = new Rectangle(cuePoint, BASE_Y_LINE + BASE_Y_CUE_ARROW, WIDTH_CUE_ARROW, HEIGHT_CUE_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }
        private bool IntersectsWithIntroMarker(Point p)
        {
            int introPoint = (int)(division * introPosition);
            var rect = new Rectangle(introPoint - 2, 0, 4, ClientRectangle.Height);
            var rect2 = new Rectangle(introPoint, BASE_Y_INTRO_ARROW, WIDTH_INTRO_ARROW, HEIGHT_INTRO_ARROW);
            return rect.Contains(p) || rect2.Contains(p);
        }
    }
}
