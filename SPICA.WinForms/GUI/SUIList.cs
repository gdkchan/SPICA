using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    public partial class SUIList : UserControl
    {
        private List<string> Items;

        public int SelectedIndex;
        private int OldIndex;

        [Category("Behavior"), Description("The fixed height of each list Item.")]
        public int ItemHeight;

        [Category("Appearance"), Description("The background color of a selected Item.")]
        public Color SelectionColor { get; set; } = Color.Orange;

        [Category("Appearance"), Description("The normal color of the Scroll Bar (no hover).")]
        public Color BarColor
        {
            get { return ListScroll.BarColor; }
            set { ListScroll.BarColor = value; }
        }

        [Category("Appearance"), Description("The color of the Scroll Bar when the mouse is hovering it.")]
        public Color BarColorHover
        {
            get { return ListScroll.BarColorHover; }
            set { ListScroll.BarColorHover = value; }
        }

        public event EventHandler SelectedIndexChanged;

        public SUIList()
        {
            InitializeComponent();

            Items = new List<string>();

            ItemHeight = 16;

            SelectedIndex = -1;
            OldIndex = -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int Index = 0; Index < Items.Count; Index++)
            {
                int ScrollY = ListScroll.Visible ? -ListScroll.Value : 0;

                int Y = ScrollY + Index * ItemHeight;

                if (Index == SelectedIndex)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SelectionColor), new Rectangle(0, Y, Width, ItemHeight));
                }

                e.Graphics.DrawString(Items[Index], Font, new SolidBrush(ForeColor), new Point(0, Y));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SelectedIndex = (e.Y + (ListScroll.Visible ? ListScroll.Value : 0)) / ItemHeight;

            if (SelectedIndex >= Items.Count) SelectedIndex = -1;

            if (SelectedIndex != OldIndex)
            {
                OldIndex = SelectedIndex;

                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

                Invalidate();
            }

            base.OnMouseDown(e);
        }

        public void AddItem(string Item)
        {
            Items.Add(Item); CalculateScroll();
        }

        public void RemoveItem(string Item)
        {
            Items.Remove(Item); CalculateScroll();
        }

        public void RemoveItem(int Index)
        {
            Items.RemoveAt(Index); CalculateScroll();
        }

        public void Clear()
        {
            Items.Clear();

            CalculateScroll();

            Invalidate();
        }

        private void CalculateScroll()
        {
            ListScroll.Maximum = Math.Max(Items.Count * ItemHeight - Height, 0);

            ListScroll.Visible = ListScroll.Maximum > 0;
        }

        private void ListScroll_ScrollChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
