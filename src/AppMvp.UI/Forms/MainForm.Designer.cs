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
            lblPlaceHolder1 = new Label();
            pnlHeaderRegion.SuspendLayout();
            pnlSidebarRegion.SuspendLayout();
            pnlContentRegion.SuspendLayout();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            SuspendLayout();
            // statusStrip1
            //
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1 });
            statusStrip1.Dock = DockStyle.Bottom;
            statusStrip1.Location = new Point(0, 374 - statusStrip1.Height);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";

            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Text = "Ready";

            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            //
            // toolStripCancelButton
            //
            // add cancel button to the status strip (created in code-behind)

            //
            // pnlHeaderRegion
            // 
            pnlHeaderRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlHeaderRegion.Controls.Add(lblPlaceHolderHeader);
            pnlHeaderRegion.Dock = DockStyle.Top;
            pnlHeaderRegion.Location = new Point(0, 0);
            pnlHeaderRegion.Name = "pnlHeaderRegion";
            pnlHeaderRegion.Size = new Size(505, 34);
            pnlHeaderRegion.TabIndex = 2;
            // 
            // lblPlaceHolderHeader
            // 
            lblPlaceHolderHeader.Dock = DockStyle.Fill;
            lblPlaceHolderHeader.Location = new Point(0, 0);
            lblPlaceHolderHeader.Name = "lblPlaceHolderHeader";
            lblPlaceHolderHeader.Size = new Size(503, 32);
            lblPlaceHolderHeader.TabIndex = 0;
            lblPlaceHolderHeader.Text = "Header Preview";
            lblPlaceHolderHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlSidebarRegion
            // 
            pnlSidebarRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlSidebarRegion.Controls.Add(lblPlaceHolderSide);
            pnlSidebarRegion.Dock = DockStyle.Left;
            pnlSidebarRegion.Location = new Point(0, 34);
            pnlSidebarRegion.Name = "pnlSidebarRegion";
            pnlSidebarRegion.Size = new Size(148, 340);
            pnlSidebarRegion.TabIndex = 1;
            // 
            // lblPlaceHolderSide
            // 
            lblPlaceHolderSide.Dock = DockStyle.Fill;
            lblPlaceHolderSide.Location = new Point(0, 0);
            lblPlaceHolderSide.Name = "lblPlaceHolderSide";
            lblPlaceHolderSide.Size = new Size(146, 338);
            lblPlaceHolderSide.TabIndex = 0;
            lblPlaceHolderSide.Text = "Sidebar Preview";
            lblPlaceHolderSide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlContentRegion
            // 
            pnlContentRegion.BorderStyle = BorderStyle.FixedSingle;
            pnlContentRegion.Controls.Add(lblPlaceHolderContent);
            pnlContentRegion.Controls.Add(lblPlaceHolder1);
            pnlContentRegion.Dock = DockStyle.Fill;
            pnlContentRegion.Location = new Point(148, 34);
            pnlContentRegion.Name = "pnlContentRegion";
            pnlContentRegion.Size = new Size(357, 340);
            pnlContentRegion.TabIndex = 0;
            // 
            // lblPlaceHolderContent
            // 
            lblPlaceHolderContent.Dock = DockStyle.Fill;
            lblPlaceHolderContent.Location = new Point(0, 0);
            lblPlaceHolderContent.Name = "lblPlaceHolderContent";
            lblPlaceHolderContent.Size = new Size(355, 338);
            lblPlaceHolderContent.TabIndex = 1;
            lblPlaceHolderContent.Text = "Content Preview";
            lblPlaceHolderContent.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblPlaceHolder1
            // 
            lblPlaceHolder1.Location = new Point(0, 0);
            lblPlaceHolder1.Name = "lblPlaceHolder1";
            lblPlaceHolder1.Size = new Size(0, 0);
            lblPlaceHolder1.TabIndex = 0;
            lblPlaceHolder1.TextAlign = ContentAlignment.MiddleCenter;
            lblPlaceHolder1.Visible = false;
            // 
            // MainForm
            // 
            ClientSize = new Size(505, 374);
            Controls.Add(pnlContentRegion);
            Controls.Add(pnlSidebarRegion);
            Controls.Add(pnlHeaderRegion);
            Controls.Add(statusStrip1);
            Name = "MainForm";
            Text = "Main Shell";
            WindowState = FormWindowState.Maximized;
            pnlHeaderRegion.ResumeLayout(false);
            pnlSidebarRegion.ResumeLayout(false);
            pnlContentRegion.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlHeaderRegion;
        private System.Windows.Forms.Panel pnlSidebarRegion;
        private System.Windows.Forms.Panel pnlContentRegion;
        #endregion

        private Label lblPlaceHolder1;
        private Label lblPlaceHolderContent;
        private Label lblPlaceHolderHeader;
        private Label lblPlaceHolderSide;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripButton toolStripCancelButton;
    }
}