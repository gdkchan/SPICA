using SPICA.Formats.CtrH3D;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms.GUI
{
    public partial class SUIList : UserControl
    {
        private List<string> Items;

        private int OldIndex;

        private int _SelectedIndex;

        private int _ItemHeight = 16;

        private Color _SelectionColor = Color.Orange;

        private Action UnsubscribeBinding;

        [Browsable(false)]
        public bool IsBound { get; private set; }

        [Browsable(false)]
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                _SelectedIndex = value;

                Invalidate();
            }
        }

        [Category("Behavior"), Description("The fixed height of each list Item.")]
        public int ItemHeight
        {
            get
            {
                return _ItemHeight;
            }
            set
            {
                _ItemHeight = value;

                Invalidate();
            }
        }

        [Category("Appearance"), Description("The background color of a selected Item.")]
        public Color SelectionColor
        {
            get
            {
                return _SelectionColor;
            }
            set
            {
                _SelectionColor = value;

                Invalidate();
            }
        }

        [Category("Appearance"), Description("The normal color of the Scroll Bar.")]
        public Color BarColor
        {
            get
            {
                return ListScroll.BarColor;
            }
            set
            {
                ListScroll.BarColor = value;
            }
        }

        [Category("Appearance"), Description("The color of the Scroll Bar on mouse hover.")]
        public Color BarColorHover
        {
            get
            {
                return ListScroll.BarColorHover;
            }
            set
            {
                ListScroll.BarColorHover = value;
            }
        }

        public event EventHandler SelectedIndexChanged;

        public SUIList()
        {
            InitializeComponent();

            Items = new List<string>();

            _SelectedIndex = -1;
            OldIndex = -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int ScrollY = ListScroll.Visible ? -ListScroll.Value : 0;

            int StartIndex = -ScrollY / ItemHeight;
            int EndIndex = Math.Min(StartIndex + Height / ItemHeight + 2, Items.Count);

            for (int Index = StartIndex; Index < EndIndex; Index++)
            {
                int Y = ScrollY + Index * ItemHeight;

                if (Index == _SelectedIndex)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SelectionColor), new Rectangle(0, Y, Width, ItemHeight));
                }

                e.Graphics.DrawString(
                    Items[Index], 
                    Font,
                    new SolidBrush(ForeColor), 
                    new Rectangle(0, Y, Width, ItemHeight),
                    new StringFormat { LineAlignment = StringAlignment.Center });
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Select((e.Y + (ListScroll.Visible ? ListScroll.Value : 0)) / ItemHeight);

            base.OnMouseDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ListScroll.Visible)
            {
                int Disp = Math.Max(Height >> 4, ItemHeight >> 1);

                ListScroll.Value = e.Delta > 0
                    ? Math.Max(ListScroll.Value - Disp, 0)
                    : Math.Min(ListScroll.Value + Disp, ListScroll.Maximum);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            CalculateScroll();

            base.OnLayout(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up: SelectUp(); break;
                case Keys.Down: SelectDown(); break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void SelectUp()
        {
            if (_SelectedIndex > 0)
            {
                Select(_SelectedIndex - 1);

                if (_SelectedIndex * ItemHeight < ListScroll.Value)
                {
                    ListScroll.Value = Math.Max(_SelectedIndex * ItemHeight, 0);
                }
            }
        }

        public void SelectDown()
        {
            if (_SelectedIndex + 1 < Items.Count)
            {
                Select(_SelectedIndex + 1);

                if (_SelectedIndex * ItemHeight > ListScroll.Value + Height - ItemHeight)
                {
                    ListScroll.Value = Math.Min(_SelectedIndex * ItemHeight - Height + ItemHeight, ListScroll.Maximum);
                }
            }
        }

        public void Select(int Index)
        {
            if (Index >= Items.Count || Index < 0)
                _SelectedIndex = -1;
            else
                _SelectedIndex = Index;

            if (_SelectedIndex != OldIndex)
            {
                OldIndex = _SelectedIndex;

                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public void AddItem(string Item)
        {
            Items.Add(Item); CalculateScroll();
        }

        public void RemoveItem(string Item)
        {
            Items.Remove(Item); CalculateScroll(); Select(-1);
        }

        public void RemoveItem(int Index)
        {
            Items.RemoveAt(Index); CalculateScroll(); Select(-1);
        }

        public void Clear()
        {
            Items.Clear();

            CalculateScroll();

            Select(-1);

            Invalidate();
        }

        public void Bind<T>(PatriciaList<T> Source) where T : INamed
        {
            if (IsBound) Unbind();

            Items.Clear();

            Select(-1);

            foreach (INamed Item in Source)
            {
                Items.Add(Item.Name);
            }

            CalculateScroll();

            Invalidate();

            Source.CollectionChanged += Source_CollectionChanged;

            UnsubscribeBinding = delegate ()
            {
                Source.CollectionChanged -= Source_CollectionChanged;
            };

            IsBound = true;
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsBound)
            {
                int Index = e.NewStartingIndex;
                string Item = ((INamed)e.NewItems[0]).Name;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:     Items.Add(Item); break;
                    case NotifyCollectionChangedAction.Remove:  Items.Remove(Item); break;
                    case NotifyCollectionChangedAction.Replace: Items.Insert(Index, Item); break;
                    case NotifyCollectionChangedAction.Reset:   Items.Clear(); break;
                }

                if (e.Action == NotifyCollectionChangedAction.Remove ||
                    e.Action == NotifyCollectionChangedAction.Reset)
                    Select(-1);

                CalculateScroll();

                Invalidate();
            }
        }

        public void Unbind()
        {
            if (IsBound)
            {
                UnsubscribeBinding();

                IsBound = false;
            }
        }

        private void CalculateScroll()
        {
            ListScroll.Maximum = Math.Max(Items.Count * ItemHeight - Height, 0);

            ListScroll.Visible = ListScroll.Maximum > 0;
        }

        private void ListScroll_ScrollChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
