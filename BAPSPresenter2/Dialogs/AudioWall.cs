using BAPSCommon;
using BAPSPresenter;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BAPSPresenter2
{
    public partial class AudioWall : Form
    {
        /// <summary>
        /// Forwards key-down events that aren't handled by this dialog.
        /// </summary>
        public event KeyEventHandler KeyDownForward;

        public AudioWall(System.Collections.Queue msgQueue, TrackList tl)
        {
            this.msgQueue = msgQueue;
            this.tl = tl;

            InitializeComponent();

            // Don't initialise this array _before_ initialising the component;
            // else, OnResize will trip a null-pointer exception.
            buttons = new BAPSFormControls.BAPSButton[20];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var bapsButton = new BAPSFormControls.BAPSButton
                    {
                        BackColor = Color.Transparent,
                        DialogResult = DialogResult.None,
                        Font = new Font("Microsoft Sans Serif", 14.25f, FontStyle.Regular, GraphicsUnit.Point,
                        0),
                        HighlightColor = Color.Red,
                        Highlighted = false,
                        Location = new Point(((i + 1) * 12) + (i * 190), ((j + 1) * 12) + (j * 170)),
                        Name = string.Concat("bapsButton", ((5 * j) + i).ToString()),
                        Size = new Size(190, 170),
                        TabIndex = 0,
                        Text = string.Concat("bapsButton", ((5 * j) + i).ToString()),
                        Tag = 1,
                        Enabled = false
                    };
                    bapsButton.TabIndex = (5 * j) + i;
                    bapsButton.Click += audioWallClick;
                    buttons[(5 * j) + i] = bapsButton;
                    Controls.Add(bapsButton);
                }
            }
            refreshWall();
        }

        public void setChannel(TrackList _tl)
        {
            tl = _tl;
        }

        public void refreshWall()
        {
            Text = string.Concat("Audio Wall for Channel ", tl.Channel.ToString());
            int walli = 0;
            for (int i = 0; i < tl.TrackCount && walli < 20; i++)
            {
                var ei = tl.GetTrack(i);
                if (ei.type != Command.TEXTITEM)
                {
                    buttons[walli].Text = ei.description;
                    buttons[walli].Tag = i;
                    buttons[walli].Enabled = true;
                    if (i == tl.LoadedIndex)
                    {
                        buttons[walli].Highlighted = true;
                    }
                    else
                    {
                        buttons[walli].Highlighted = false;
                    }
                    walli++;
                }
            }
            for (; walli < 20; walli++)
            {
                buttons[walli].Text = "[NONE]";
                buttons[walli].Tag = -1;
                buttons[walli].Enabled = false;
                buttons[walli].Highlighted = false;
            }
        }

        #region Events

        protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

            if (buttons == null) return;
  			int xoffset = (ClientRectangle.Width - ((5 * 202) + 12)) / 2;
            int yoffset = (ClientRectangle.Height - ((4 * 182) + 12)) / 2;
            for (int i = 0 ; i < 5 ; i++)
	        {
		        for (int j = 0 ; j< 4 ; j++)
		        {
			        buttons[(5 * j) + i].Location = new Point(xoffset+((i+1)*12)+(i*190), yoffset+((j+1)*12)+(j*170));
		        }
               }
		}

        private void audioWallClick(object o, EventArgs e)
        {
            var bb = (BAPSFormControls.BAPSButton)o;
            var index = (uint)(int)bb.Tag;  // These two casts are deliberate.
            if (!bb.Highlighted)
            {
                var lcmd = Command.PLAYBACK | Command.LOAD | (Command)tl.Channel;
                msgQueue.Enqueue(new ActionMessageU32int((ushort)lcmd, index));
            }
            var pcmd = Command.PLAYBACK | Command.PLAY | (Command)tl.Channel;
            msgQueue.Enqueue(new ActionMessage((ushort)pcmd));
        }

        void AudioWall_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == KeyShortcuts.AlterWindow)
            {
                AlterWindow();
                e.Handled = true;
                return;
            }

            KeyDownForward?.Invoke(sender, e);
        }

        private void AlterWindow()
        {
            /** Ctrl+Alt+W Alter Window **/
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

        #endregion Events

        System.Collections.Queue msgQueue = null;
        private TrackList tl = null;

        private BAPSFormControls.BAPSButton[] buttons = null;
    }
}
