using SPICA.Formats.Common;

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

        private List<int> _SelectedIndices;

        private int _ItemHeight = 16;

        private Color _SelectionColor = Color.Orange;

        private Action UnsubscribeBinding;

        [Browsable(false)] public string this[int Index] { get { return Items[Index]; } }

        [Browsable(false)] public int Count => Items.Count;

        [Browsable(false)] public bool IsBound { get; private set; }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                OldIndex = _SelectedIndex = value;

                Invalidate();
            }
        }

        [Browsable(false)] public int[] SelectedIndices => _SelectedIndices.ToArray();

        [Category("Appearance"), Description("The normal color of the Scroll Bar.")]
        public Color BarColor
        {
            get => ListScroll.BarColor;
            set => ListScroll.BarColor = value;
        }

        [Category("Appearance"), Description("The color of the Scroll Bar on mouse hover.")]
        public Color BarColorHover
        {
            get => ListScroll.BarColorHover;
            set => ListScroll.BarColorHover = value;
        }

        [Category("Behavior"), Description("The fixed height of each list Item.")]
        public int ItemHeight
        {
            get => _ItemHeight;
            set
            {
                _ItemHeight = value;

                Invalidate();
            }
        }

        [Category("Appearance"), Description("The background color of a selected Item.")]
        public Color SelectionColor
        {
            get => _SelectionColor;
            set
            {
                _SelectionColor = value;

                Invalidate();
            }
        }

        [Category("Behavior"), Description("Allows multiple items to be selected at once.")]
        public bool MultiSelect { get; set; } = false;

        public event EventHandler Selected;
        public event EventHandler SelectedIndexChanged;

        public SUIList()
        {
            InitializeComponent();

            Items = new List<string>();

            _SelectedIndices = new List<int>();

            OldIndex = _SelectedIndex = -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int ScrollY = ListScroll.Visible ? -ListScroll.Value : 0;

            int StartIndex = -ScrollY / ItemHeight;

            int EndIndex = Math.Min(StartIndex + Height / ItemHeight + 2, Items.Count);

            for (int Index = StartIndex; Index < EndIndex; Index++)
            {
                int Y = ScrollY + Index * ItemHeight;

                if (_SelectedIndices.Contains(Index))
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
            int ClickIndex = (e.Y + (ListScroll.Visible ? ListScroll.Value : 0)) / ItemHeight;

            Select(ClickIndex, MultiSelect && ModifierKeys.HasFlag(Keys.Control));

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
                case Keys.Up:     SelectUp();   break;
                case Keys.Down:   SelectDown(); break;
                case Keys.Escape: Select(-1);   break;
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

        public void Select(int Index, bool Multi = false)
        {
            if (Index >= Items.Count || Index < 0) Index = -1;

            if (Multi)
            {
                if (_SelectedIndices.Contains(Index))
                {
                    _SelectedIndices.Remove(Index);
                }
                else if (Index != -1)
                {
                    _SelectedIndices.Add(Index);
                }
            }
            else
            {
                _SelectedIndices.Clear();

                if (Index != -1)
                {
                    _SelectedIndices.Add(Index);
                }
            }

            _SelectedIndex = _SelectedIndices.Count > 0 ? _SelectedIndices[0] : -1;

            if (Index != OldIndex || Multi)
            {
                OldIndex = Index;

                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);

                Invalidate();
            }

            Selected?.Invoke(this, EventArgs.Empty);
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

        public void Bind<T>(IPatriciaDict<T> Source) where T : INamed
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

                string Item = null;

                if (e.Action != NotifyCollectionChangedAction.Reset)
                {
                    Item = ((INamed)e.NewItems[0]).Name;
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:     Items.Add(Item);           break;
                    case NotifyCollectionChangedAction.Remove:  Items.Remove(Item);        break;
                    case NotifyCollectionChangedAction.Replace: Items.Insert(Index, Item); break;
                    case NotifyCollectionChangedAction.Reset:   Items.Clear();             break;
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
