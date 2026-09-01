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
            this.pnlHeaderRegion = new System.Windows.Forms.Panel();
            this.pnlSidebarRegion = new System.Windows.Forms.Panel();
            this.pnlContentRegion = new System.Windows.Forms.Panel();

            // Header region
            this.pnlHeaderRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderRegion.Height = 60;

            // Sidebar region
            this.pnlSidebarRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebarRegion.Width = 200;

            // Content region
            this.pnlContentRegion.Dock = System.Windows.Forms.DockStyle.Fill;

            // Add to form
            this.Controls.Add(this.pnlContentRegion);
            this.Controls.Add(this.pnlSidebarRegion);
            this.Controls.Add(this.pnlHeaderRegion);

            this.Text = "Main Shell";
            this.WindowState = FormWindowState.Maximized;
        }

        private System.Windows.Forms.Panel pnlHeaderRegion;
        private System.Windows.Forms.Panel pnlSidebarRegion;
        private System.Windows.Forms.Panel pnlContentRegion;
        #endregion
    }
}