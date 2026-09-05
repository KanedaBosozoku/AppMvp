using System;
using System.Collections.Generic;
using System.Text;

namespace AppMvp.UI.Controls
{
    public class ModernNavRenderer : ToolStripProfessionalRenderer
    {
        private readonly Color _selectedBack = Color.SteelBlue;
        private readonly Color _hoverBack = Color.FromArgb(60, 120, 200);
        private readonly Color _normalBack = SystemColors.Control;
        private readonly Color _selectedFore = Color.White;
        private readonly Color _normalFore = SystemColors.ControlText;

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var btn = (ToolStripButton)e.Item;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, btn.Bounds.Width - 1, btn.Bounds.Height - 1);

            bool isSelected = btn.Checked;
            bool isHover = btn.Selected && !btn.Checked;

            Color backColor =
                isSelected ? _selectedBack :
                isHover ? _hoverBack :
                _normalBack;

            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(backColor))
            {
                // Rounded rectangle
                int radius = 6;
                var path = RoundedRect(rect, radius);
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Foreground color
            btn.ForeColor = isSelected ? _selectedFore : _normalFore;
        }

        // Helper: rounded rectangle path
        private System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }

}
