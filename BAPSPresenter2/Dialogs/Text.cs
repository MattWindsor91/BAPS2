﻿using System.Drawing;
using System.Windows.Forms;

namespace BAPSPresenter2.Dialogs
{
    public partial class Text : Form
    {
        /// <summary>
        /// Forwards key-down events that aren't handled by this dialog.
        /// </summary>
        public event KeyEventHandler KeyDownForward;

        public Text(string text)
        {
            InitializeComponent();

            Text = "News Stories / Long Links";
            textText.Text = text;
        }

        public void scroll(int updown)
        {
            bool isDown = updown == 0;
            var y = isDown ? textText.ClientSize.Height - 1 : 0;
            var pnt = new Point(0, y);
            var charIndex = textText.GetCharIndexFromPosition(pnt);
            textText.SelectionStart = charIndex;
            textText.SelectionLength = 0;
            textText.ScrollToCaret();
        }

        private float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (max < value) return max;
            return value;
        }

        public void textSize(int updown)
        {
            bool isSmaller = updown == 0;
            var size = Clamp(textText.Font.Size + (isSmaller ? -1 : 1), 12, 40);
            textText.Font = new Font(textText.Font.FontFamily, size);
        }

        public void toggleMaximize()
        {
            if (WindowState == FormWindowState.Normal)
            {
                var bounds = Screen.GetWorkingArea(this);
                bounds.X = 0;
                bounds.Y = 0;
                MaximizedBounds = bounds;
                MaximumSize = bounds.Size;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Normal;
            }
        }

        public void updateText(string text)
        {
            textText.Text = text;
        }

        #region Events

        private void TextDialog_KeyDown(object sender, KeyEventArgs e)
        {
            KeyDownForward?.Invoke(sender, e);
        }

        #endregion Events
    }
}
