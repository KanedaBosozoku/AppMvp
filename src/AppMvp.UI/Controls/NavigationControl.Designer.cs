namespace AppMvp.UI.Controls
{
    partial class NavigationControl
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
            tsNavigation = new ToolStrip();
            SuspendLayout();
            // 
            // tsNavigation
            // 
            tsNavigation.Location = new Point(0, 0);
            tsNavigation.Name = "tsNavigation";
            tsNavigation.Size = new Size(150, 25);
            tsNavigation.TabIndex = 0;
            tsNavigation.Text = "toolStrip1";
            // 
            // NavigationControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tsNavigation);
            Name = "NavigationControl";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip tsNavigation;
    }
}
