namespace AppMvp.UI.Views
{
    partial class PeopleView
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
            this.lstPeople = new System.Windows.Forms.ListBox();
            this.SuspendLayout();

            this.lstPeople.Dock = System.Windows.Forms.DockStyle.Fill;

            this.Controls.Add(this.lstPeople);
            this.Name = "PeopleView";
            this.Size = new System.Drawing.Size(400, 300);

            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox lstPeople;
    }
}
