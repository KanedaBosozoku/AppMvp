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
            lstPeople = new ListView();
            colName = new ColumnHeader();
            colEmail = new ColumnHeader();
            SuspendLayout();
            // 
            // lstPeople
            // 
            lstPeople.Columns.AddRange(new ColumnHeader[] { colName, colEmail });
            lstPeople.Dock = DockStyle.Fill;
            lstPeople.FullRowSelect = true;
            lstPeople.Location = new Point(0, 0);
            lstPeople.Name = "lstPeople";
            lstPeople.Size = new Size(400, 300);
            lstPeople.TabIndex = 0;
            lstPeople.UseCompatibleStateImageBehavior = false;
            lstPeople.View = View.Details;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 100;
            // 
            // colEmail
            // 
            colEmail.Text = "Email";
            colEmail.Width = 150;
            // 
            // PeopleView
            // 
            Controls.Add(lstPeople);
            Name = "PeopleView";
            Size = new Size(400, 300);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ListView lstPeople;
        private System.Windows.Forms.ColumnHeader colName;
        private ColumnHeader colEmail;
    }
}
