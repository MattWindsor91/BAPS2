namespace BAPSPresenter2
{
    partial class BAPSDirectory
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
            this.LayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.Listing = new System.Windows.Forms.ListBox();
            this.LayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // LayoutPanel
            // 
            this.LayoutPanel.AutoSize = true;
            this.LayoutPanel.Controls.Add(this.RefreshButton);
            this.LayoutPanel.Controls.Add(this.Listing);
            this.LayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.LayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.LayoutPanel.Name = "LayoutPanel";
            this.LayoutPanel.Size = new System.Drawing.Size(234, 155);
            this.LayoutPanel.TabIndex = 0;
            // 
            // RefreshButton
            // 
            this.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RefreshButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefreshButton.Location = new System.Drawing.Point(0, 0);
            this.RefreshButton.Margin = new System.Windows.Forms.Padding(0);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(234, 23);
            this.RefreshButton.TabIndex = 222;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // Listing
            // 
            this.Listing.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Listing.Location = new System.Drawing.Point(0, 23);
            this.Listing.Margin = new System.Windows.Forms.Padding(0);
            this.Listing.Name = "Listing";
            this.Listing.Size = new System.Drawing.Size(234, 121);
            this.Listing.TabIndex = 221;
            this.Listing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Listing_MouseDown);
            // 
            // BAPSDirectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.LayoutPanel);
            this.Name = "BAPSDirectory";
            this.Size = new System.Drawing.Size(234, 155);
            this.LayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel LayoutPanel;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.ListBox Listing;
    }
}
