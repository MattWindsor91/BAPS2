using System;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSFormControls
{
    public partial class BAPSLabel : Control
    {
        private System.Drawing.Drawing2D.LinearGradientBrush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, 10, 10), Color.Tan, Color.Snow, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
        private Bitmap offScreen = new Bitmap(1, 1);

        private bool isHighlighted = false;
        public bool Highlighted
        {
            get => isHighlighted;
            set
            {
                isHighlighted = value;
                HighlightChanged();
            }
        }

        private Color highlightColor = Color.Red;
        public Color HighlightColor
        {
            get => highlightColor;
            set
            {
                highlightColor = value;
                HighlightChanged();
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                prepareGraphics();
                Invalidate();
            }
        }

        private string infoText = "";
        public string InfoText
        {
            get => infoText;
            set
            {
                infoText = value;
                prepareGraphics();
                Invalidate();
            }
        }

        public BAPSLabel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            TabStop = false;
            backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, 10, 10), Color.Tan, Color.Snow, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(offScreen, 0, 0);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            var rect = ClientRectangle;
            if (string.Compare(infoText, "") != 0)
            {
                var font = new Font(Font.FontFamily, 8, FontStyle.Regular, GraphicsUnit.Point);
                e.Graphics.DrawString(infoText, font, new SolidBrush(ForeColor), 5.0f, 1.0f);
                // have info text to display
                rect.Y += 12;
                rect.Height -= 12;
            }

            e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), rect, sf);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateBackBrush();
	        offScreen = new Bitmap(ClientSize.Width, ClientSize.Height);
	        prepareGraphics();
        }

        private bool NoClientRectangle => ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0;

        private void UpdateBackBrush()
        {
            backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle,
                        isHighlighted ? highlightColor : Color.Snow,
                        Color.AntiqueWhite,
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            backBrush.SetBlendTriangularShape(0.5f);
        }

        private void prepareGraphics()
        {
            if (NoClientRectangle) return;

            var gOffScreen = Graphics.FromImage(offScreen);

            int curveWidth = (ClientRectangle.Height > 30) ? 20 : ClientRectangle.Height / 2;
            gOffScreen.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), 0, curveWidth, curveWidth, 270, 90);
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 0, 90);
            gp.AddArc(0, ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 90, 90);
            gp.AddArc(0, 0, curveWidth, curveWidth, 180, 90);
            gp.CloseFigure();
            gOffScreen.FillPath(backBrush, gp);
            gOffScreen.DrawPath(Pens.LightGray, gp);
        }

        private void HighlightChanged()
        {
            if (NoClientRectangle) return;
            UpdateBackBrush();
            prepareGraphics();
            Invalidate();
        }
    }
}
