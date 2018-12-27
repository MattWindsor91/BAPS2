using System;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSFormControls
{
    public partial class BAPSButton : UserControl, IButtonControl
    {
        public BAPSButton()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.Selectable |
                ControlStyles.StandardClick |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserMouse,
                true);
        }

        public bool Highlighted
        {
            get => isHighlighted;
            set
            {
                isHighlighted = value;
                HighlightChanged();
            }
        }
    
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
                Invalidate();
            }
        }

        public DialogResult DialogResult
        {
            get => myDialogResult;
            set
            {
                if (Enum.IsDefined(typeof(DialogResult), value))
                {
                    myDialogResult = value;
                }
            }
        }

        public void NotifyDefault(bool value)
        {
            if (IsDefault != value) IsDefault = value;
        }

        public void PerformClick()
        {
            if (CanSelect) OnClick(EventArgs.Empty);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
	    {
		    base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
    	{
            Invalidate();
        }

        private void SetupBackBrush(Color color)
        {
            backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle,
                        color,
                        Color.Snow,
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            backBrush.SetBlendTriangularShape(0.5f);
        }

        protected override void OnResize(EventArgs e)
        {
            SetupBackBrush(isHighlighted ? highlightColor : Color.Tan);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                backBrush.SetBlendTriangularShape(0.55f);
                voffset = (int)(0.1 * ClientRectangle.Height);
                Invalidate();
            }
            base.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SetupBackBrush(Color.Chocolate);
                voffset = 0;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            SetupBackBrush(Color.Chocolate);
            voffset = 0;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            SetupBackBrush(isHighlighted ? highlightColor : Color.Tan);
            voffset = 0;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            int curveWidth = (ClientRectangle.Height > 30) ? 20 : ClientRectangle.Height / 2;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), 0, curveWidth, curveWidth, 270, 90);
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 0, 90);
            gp.AddArc(0, ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 90, 90);
            gp.AddArc(0, 0, curveWidth, curveWidth, 180, 90);
            gp.CloseFigure();
            e.Graphics.FillPath(backBrush, gp);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Rectangle rect = ClientRectangle;
            rect.Y += voffset;
            rect.Height -= voffset;
            e.Graphics.DrawString(Text, Font, Brushes.Black, rect, sf);
            if (Focused)
            {
                e.Graphics.DrawPath(Pens.DarkOrange, gp);
            }
            if (!Enabled)
            {
                var col = Color.FromArgb(100, 50, 50, 50);
                var disabledBrush = new SolidBrush(col);
                e.Graphics.FillPath(disabledBrush, gp);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            int curveWidth = (ClientRectangle.Height > 30) ? 20 : ClientRectangle.Height / 2;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), 0, curveWidth, curveWidth, 270, 90);
            gp.AddArc(ClientRectangle.Width - (curveWidth + 1), ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 0, 90);
            gp.AddArc(0, ClientRectangle.Height - (curveWidth + 1), curveWidth, curveWidth, 90, 90);
            gp.AddArc(0, 0, curveWidth, curveWidth, 180, 90);
            gp.CloseFigure();
            e.Graphics.DrawPath(Pens.LightGray, gp);

        }

        private void HighlightChanged()
        {
            if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;
            SetupBackBrush(isHighlighted ? highlightColor : Color.Tan);
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                base.OnClick(EventArgs.Empty);
                e.Handled = true;
            }
        }

        private bool IsDefault = false;
        private int voffset = 0;
        private bool isHighlighted = false;

        private Color highlightColor = Color.Red;
        private DialogResult myDialogResult = DialogResult.None;
        private System.Drawing.Drawing2D.LinearGradientBrush backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, 10, 10), Color.Tan, Color.Snow, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
    }
}
