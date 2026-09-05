namespace AppMvp.UI.Forms
{
    partial class MainForm
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
            pnlHeaderRegion = new Panel();
            lblPlaceHolderHeader = new Label();
            pnlSidebarRegion = new Panel();
            lblPlaceHolderSide = new Label();
            pnlContentRegion = new Panel();
            lblPlaceHolderContent = new Label();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            pnlNavigationRegion = new Panel();
            lblPlaceHolderNav = new Label();
            pnlHeaderRegion.SuspendLayout();
            pnlSidebarRegion.SuspendLayout();
            pnlContentRegion.SuspendLayout();
            statusStrip1.SuspendLayout();
            pnlNavigationRegion.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeaderRegion
            // 
            pnlHeaderRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlHeaderRegion.Controls.Add(lblPlaceHolderHeader);
            pnlHeaderRegion.Dock = DockStyle.Top;
            pnlHeaderRegion.Location = new Point(148, 0);
            pnlHeaderRegion.Name = "pnlHeaderRegion";
            pnlHeaderRegion.Size = new Size(357, 34);
            pnlHeaderRegion.TabIndex = 2;
            // 
            // lblPlaceHolderHeader
            // 
            lblPlaceHolderHeader.Dock = DockStyle.Fill;
            lblPlaceHolderHeader.Location = new Point(0, 0);
            lblPlaceHolderHeader.Name = "lblPlaceHolderHeader";
            lblPlaceHolderHeader.Size = new Size(355, 32);
            lblPlaceHolderHeader.TabIndex = 0;
            lblPlaceHolderHeader.Text = "Header Preview";
            lblPlaceHolderHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlSidebarRegion
            // 
            pnlSidebarRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlSidebarRegion.Controls.Add(lblPlaceHolderSide);
            pnlSidebarRegion.Dock = DockStyle.Left;
            pnlSidebarRegion.Location = new Point(0, 0);
            pnlSidebarRegion.Name = "pnlSidebarRegion";
            pnlSidebarRegion.Size = new Size(148, 352);
            pnlSidebarRegion.TabIndex = 1;
            // 
            // lblPlaceHolderSide
            // 
            lblPlaceHolderSide.Dock = DockStyle.Fill;
            lblPlaceHolderSide.Location = new Point(0, 0);
            lblPlaceHolderSide.Name = "lblPlaceHolderSide";
            lblPlaceHolderSide.Size = new Size(146, 350);
            lblPlaceHolderSide.TabIndex = 0;
            lblPlaceHolderSide.Text = "Sidebar Preview";
            lblPlaceHolderSide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlContentRegion
            // 
            pnlContentRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlContentRegion.Controls.Add(lblPlaceHolderContent);
            pnlContentRegion.Dock = DockStyle.Fill;
            pnlContentRegion.Location = new Point(148, 62);
            pnlContentRegion.Name = "pnlContentRegion";
            pnlContentRegion.Size = new Size(357, 290);
            pnlContentRegion.TabIndex = 0;
            // 
            // lblPlaceHolderContent
            // 
            lblPlaceHolderContent.Dock = DockStyle.Fill;
            lblPlaceHolderContent.Location = new Point(0, 0);
            lblPlaceHolderContent.Name = "lblPlaceHolderContent";
            lblPlaceHolderContent.Size = new Size(355, 288);
            lblPlaceHolderContent.TabIndex = 1;
            lblPlaceHolderContent.Text = "Content Preview";
            lblPlaceHolderContent.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 352);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(505, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(39, 17);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.Visible = false;
            // 
            // pnlNavigationRegion
            // 
            pnlNavigationRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlNavigationRegion.Controls.Add(lblPlaceHolderNav);
            pnlNavigationRegion.Dock = DockStyle.Top;
            pnlNavigationRegion.Location = new Point(148, 34);
            pnlNavigationRegion.Name = "pnlNavigationRegion";
            pnlNavigationRegion.Size = new Size(357, 28);
            pnlNavigationRegion.TabIndex = 1;
            // 
            // lblPlaceHolderNav
            // 
            lblPlaceHolderNav.Dock = DockStyle.Fill;
            lblPlaceHolderNav.Location = new Point(0, 0);
            lblPlaceHolderNav.Name = "lblPlaceHolderNav";
            lblPlaceHolderNav.Size = new Size(355, 26);
            lblPlaceHolderNav.TabIndex = 0;
            lblPlaceHolderNav.Text = "Navigation Preview";
            lblPlaceHolderNav.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            ClientSize = new Size(505, 374);
            Controls.Add(pnlContentRegion);
            Controls.Add(pnlNavigationRegion);
            Controls.Add(pnlHeaderRegion);
            Controls.Add(pnlSidebarRegion);
            Controls.Add(statusStrip1);
            Name = "MainForm";
            Text = "Main Shell";
            WindowState = FormWindowState.Maximized;
            pnlHeaderRegion.ResumeLayout(false);
            pnlSidebarRegion.ResumeLayout(false);
            pnlContentRegion.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            pnlNavigationRegion.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Panel pnlHeaderRegion;
        private System.Windows.Forms.Panel pnlSidebarRegion;
        private System.Windows.Forms.Panel pnlContentRegion;

        #endregion
        private Label lblPlaceHolderContent;
        private Label lblPlaceHolderHeader;
        private Label lblPlaceHolderSide;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripButton toolStripCancelButton;
        private Panel pnlNavigationRegion;
        private Label lblPlaceHolderNav;
    }
}