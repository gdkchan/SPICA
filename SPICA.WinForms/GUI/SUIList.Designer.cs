namespace SPICA.WinForms.GUI
{
    partial class SUIList
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
            this.ListScroll = new SPICA.WinForms.GUI.SUIVScroll();
            this.SuspendLayout();
            // 
            // ListScroll
            // 
            this.ListScroll.BackColor = System.Drawing.Color.Transparent;
            this.ListScroll.BarColor = System.Drawing.Color.White;
            this.ListScroll.BarColorHover = System.Drawing.Color.Gray;
            this.ListScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.ListScroll.Location = new System.Drawing.Point(248, 0);
            this.ListScroll.Maximum = 100;
            this.ListScroll.Name = "ListScroll";
            this.ListScroll.Size = new System.Drawing.Size(8, 256);
            this.ListScroll.TabIndex = 0;
            this.ListScroll.Text = null;
            this.ListScroll.Value = 0;
            this.ListScroll.Visible = false;
            this.ListScroll.ScrollChanged += new System.EventHandler(this.ListScroll_ScrollChanged);
            // 
            // SUIList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.ListScroll);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "SUIList";
            this.Size = new System.Drawing.Size(256, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private SUIVScroll ListScroll;
    }
}
