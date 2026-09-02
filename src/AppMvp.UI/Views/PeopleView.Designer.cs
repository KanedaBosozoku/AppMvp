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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeopleView));
            dgvPeople = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            ColumnEmail = new DataGridViewTextBoxColumn();
            toolStrip1 = new ToolStrip();
            tsBtnEdit = new ToolStripButton();
            tsBtnRefresh = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)dgvPeople).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvPeople
            // 
            dgvPeople.AllowUserToAddRows = false;
            dgvPeople.AllowUserToDeleteRows = false;
            dgvPeople.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPeople.Columns.AddRange(new DataGridViewColumn[] { colId, colName, ColumnEmail });
            dgvPeople.Dock = DockStyle.Fill;
            dgvPeople.Location = new Point(0, 0);
            dgvPeople.Name = "dgvPeople";
            dgvPeople.ReadOnly = true;
            dgvPeople.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPeople.Size = new Size(400, 300);
            dgvPeople.TabIndex = 0;
            // 
            // colId
            // 
            colId.HeaderText = "Id";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Visible = false;
            // 
            // colName
            // 
            colName.HeaderText = "Name";
            colName.Name = "colName";
            colName.ReadOnly = true;
            colName.Width = 120;
            // 
            // ColumnEmail
            // 
            ColumnEmail.HeaderText = "Email";
            ColumnEmail.Name = "ColumnEmail";
            ColumnEmail.ReadOnly = true;
            ColumnEmail.Width = 150;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsBtnEdit, tsBtnRefresh });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(400, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsBtnEdit
            // 
            tsBtnEdit.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsBtnEdit.Image = (Image)resources.GetObject("tsBtnEdit.Image");
            tsBtnEdit.ImageTransparentColor = Color.Magenta;
            tsBtnEdit.Name = "tsBtnEdit";
            tsBtnEdit.Size = new Size(31, 22);
            tsBtnEdit.Text = "Edit";
            // 
            // tsBtnRefresh
            // 
            tsBtnRefresh.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsBtnRefresh.Image = (Image)resources.GetObject("tsBtnRefresh.Image");
            tsBtnRefresh.ImageTransparentColor = Color.Magenta;
            tsBtnRefresh.Name = "tsBtnRefresh";
            tsBtnRefresh.Size = new Size(50, 22);
            tsBtnRefresh.Text = "Refresh";
            // 
            // PeopleView
            // 
            Controls.Add(toolStrip1);
            Controls.Add(dgvPeople);
            Name = "PeopleView";
            Size = new Size(400, 300);
            ((System.ComponentModel.ISupportInitialize)dgvPeople).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.DataGridView dgvPeople;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn ColumnEmail;
        private ToolStrip toolStrip1;
        private ToolStripButton tsBtnEdit;
        private ToolStripButton tsBtnRefresh;
    }
}
