using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    class ToolsRenderer : ToolStripRenderer
    {
        private Brush BackBrush;
        private bool IsDarkBg;

        public ToolsRenderer(Color BackColor)
        {
            BackBrush = new SolidBrush(BackColor);
            IsDarkBg = (BackColor.R + BackColor.G + BackColor.B) / 3 < 0x80;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(BackBrush, e.AffectedBounds);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.sui_ticked, new Rectangle(8, 2, 20, 20));
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            Pen Pen = new Pen(Color.FromArgb(0x3f, IsDarkBg ? Color.White : Color.Black));

            if (e.Vertical)
                e.Graphics.DrawLine(Pen,
                    new Point(2, 4),
                    new Point(2, e.Item.Height - 5));
            else
                e.Graphics.DrawLine(Pen,
                    new Point(4, 2),
                    new Point(e.Item.Width - 5, 2));
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripButton Item = (ToolStripButton)e.Item;

            if (Item.Checked || Item.Selected)
            {
                RenderItemBg(e.Graphics, e.Item.Size, Item.Checked ? 0xbf : 0x3f);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripMenuItem Item = (ToolStripMenuItem)e.Item;

            e.Item.ForeColor = IsDarkBg ? Color.White : Color.Black;

            e.Graphics.FillRectangle(BackBrush, new Rectangle(Point.Empty, Item.Size));

            if (Item.Selected || Item.DropDown.Visible) RenderItemBg(e.Graphics, e.Item.Size, 0x7f);
        }

        private void RenderItemBg(Graphics g, Size Sz, int Alpha)
        {
            Brush HoverBrush = new SolidBrush(Color.FromArgb(Alpha, Color.Black));
            Rectangle Bounds = new Rectangle(
                new Point(0, 1),
                new Size(Sz.Width, Sz.Height - 1));

            g.FillRectangle(HoverBrush, Bounds);
        }
    }
}
