using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSFormControls
{
    public partial class BAPSLabel : UserControl
    {
        public Font InfoFont
        {
            get => infoTextLabel.Font;
            set => infoTextLabel.Font = value;
        }

        private bool isHighlighted = false;
        public bool Highlighted
        {
            get => isHighlighted;
            set
            {
                if (isHighlighted == value) return;

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
                if (highlightColor == value) return;

                highlightColor = value;
                HighlightChanged();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text
        {
            get => mainTextLabel.Text;
            set => mainTextLabel.Text = value;
        }

        public string InfoText
        {
            get => infoTextLabel.Text;
            set => infoTextLabel.Text = value;
        }

        public BAPSLabel()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        private void HighlightChanged()
        {
            BackColor = isHighlighted ? highlightColor : SystemColors.Control;
        }

        /// <summary>
        /// Event handler for child controls' mouse-down events, forwarding them
        /// to the parent control's mouse-down handler with translated locations.
        /// </summary>
        /// <param name="sender">The original sender.</param>
        /// <param name="e">The original mouse event.</param>
        private void ChildControl_MouseDown(object sender, MouseEventArgs e)
        {
            var s = (Control)sender;
            var e2 = new MouseEventArgs(e.Button, e.Clicks, e.X + s.Location.X, e.Y + s.Location.Y, e.Delta);
            OnMouseDown(e2);
        }
    }
}
