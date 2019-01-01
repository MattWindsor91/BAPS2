namespace BAPSFormControls
{
    partial class BAPSLabel
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.infoTextLabel = new System.Windows.Forms.Label();
            this.mainTextLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoTextLabel
            // 
            this.infoTextLabel.AutoSize = true;
            this.infoTextLabel.Location = new System.Drawing.Point(3, 0);
            this.infoTextLabel.Name = "infoTextLabel";
            this.infoTextLabel.Size = new System.Drawing.Size(35, 13);
            this.infoTextLabel.TabIndex = 0;
            this.infoTextLabel.Text = "label1";
            this.infoTextLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            // 
            // mainTextLabel
            // 
            this.mainTextLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mainTextLabel.AutoSize = true;
            this.mainTextLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mainTextLabel.Location = new System.Drawing.Point(56, 74);
            this.mainTextLabel.Name = "mainTextLabel";
            this.mainTextLabel.Size = new System.Drawing.Size(35, 13);
            this.mainTextLabel.TabIndex = 1;
            this.mainTextLabel.Text = "label2";
            this.mainTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainTextLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.infoTextLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.mainTextLabel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(148, 148);
            this.tableLayoutPanel1.TabIndex = 2;
            this.tableLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            // 
            // BAPSLabel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "BAPSLabel";
            this.Size = new System.Drawing.Size(148, 148);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label infoTextLabel;
        private System.Windows.Forms.Label mainTextLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
