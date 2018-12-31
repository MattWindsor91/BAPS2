using System.Windows.Forms;

namespace BAPSPresenter2.Dialogs
{
    partial class Text
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textText
            // 
            this.textText.BackColor = System.Drawing.SystemColors.Window;
            this.textText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.textText.Location = new System.Drawing.Point(0, 0);
            this.textText.Multiline = true;
            this.textText.Name = "textText";
            this.textText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textText.Size = new System.Drawing.Size(759, 542);
            this.textText.TabIndex = 2;
            // 
            // TextDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 542);
            this.ControlBox = false;
            this.Controls.Add(this.textText);
            this.Icon = global::BAPSPresenter2.Properties.Resources.Icon;
            this.KeyPreview = true;
            this.Name = "TextDialog";
            this.Text = "Text Display";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextDialog_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        TextBox textText;
    }
}