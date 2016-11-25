namespace SPICA.WinForms
{
    partial class FrmMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.TopMenu = new System.Windows.Forms.MenuStrip();
            this.MenuFileRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelpRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.TopIcons = new System.Windows.Forms.ToolStrip();
            this.ToolButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonSave = new System.Windows.Forms.ToolStripButton();
            this.MenuSeparatorShowHideJustIgnore = new System.Windows.Forms.ToolStripSeparator();
            this.MenuButtonWireframeMode = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonShowGrid = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonShowAxis = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowBones = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowInfo = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowSideMenu = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.SuspendLayout();
            this.TopMenu.SuspendLayout();
            this.TopIcons.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 54);
            this.MainContainer.Name = "MainContainer";
            this.MainContainer.Size = new System.Drawing.Size(784, 507);
            this.MainContainer.SplitterDistance = 580;
            this.MainContainer.TabIndex = 0;
            // 
            // TopMenu
            // 
            this.TopMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TopMenu.ForeColor = System.Drawing.Color.White;
            this.TopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFileRoot,
            this.MenuHelpRoot});
            this.TopMenu.Location = new System.Drawing.Point(0, 0);
            this.TopMenu.Name = "TopMenu";
            this.TopMenu.Size = new System.Drawing.Size(784, 24);
            this.TopMenu.TabIndex = 1;
            // 
            // MenuFileRoot
            // 
            this.MenuFileRoot.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuOpenFile});
            this.MenuFileRoot.Name = "MenuFileRoot";
            this.MenuFileRoot.Size = new System.Drawing.Size(37, 20);
            this.MenuFileRoot.Text = "&File";
            // 
            // MenuOpenFile
            // 
            this.MenuOpenFile.Name = "MenuOpenFile";
            this.MenuOpenFile.Size = new System.Drawing.Size(152, 22);
            this.MenuOpenFile.Text = "&Open";
            this.MenuOpenFile.Click += new System.EventHandler(this.MenuOpenFile_Click);
            // 
            // MenuHelpRoot
            // 
            this.MenuHelpRoot.Name = "MenuHelpRoot";
            this.MenuHelpRoot.Size = new System.Drawing.Size(44, 20);
            this.MenuHelpRoot.Text = "&Help";
            // 
            // TopIcons
            // 
            this.TopIcons.AutoSize = false;
            this.TopIcons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.TopIcons.ForeColor = System.Drawing.Color.White;
            this.TopIcons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolButtonOpen,
            this.ToolButtonSave,
            this.MenuSeparatorShowHideJustIgnore,
            this.MenuButtonWireframeMode,
            this.ToolButtonShowGrid,
            this.ToolButtonShowAxis,
            this.MenuButtonShowBones,
            this.MenuButtonShowInfo,
            this.MenuButtonShowSideMenu});
            this.TopIcons.Location = new System.Drawing.Point(0, 24);
            this.TopIcons.Name = "TopIcons";
            this.TopIcons.Size = new System.Drawing.Size(784, 30);
            this.TopIcons.TabIndex = 3;
            this.TopIcons.Text = "toolStrip1";
            // 
            // ToolButtonOpen
            // 
            this.ToolButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonOpen.Image")));
            this.ToolButtonOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonOpen.Name = "ToolButtonOpen";
            this.ToolButtonOpen.Size = new System.Drawing.Size(28, 27);
            this.ToolButtonOpen.ToolTipText = "Open";
            this.ToolButtonOpen.Click += new System.EventHandler(this.ToolButtonOpen_Click);
            // 
            // ToolButtonSave
            // 
            this.ToolButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonSave.Image")));
            this.ToolButtonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonSave.Name = "ToolButtonSave";
            this.ToolButtonSave.Size = new System.Drawing.Size(28, 27);
            this.ToolButtonSave.ToolTipText = "Save";
            // 
            // MenuSeparatorShowHideJustIgnore
            // 
            this.MenuSeparatorShowHideJustIgnore.Name = "MenuSeparatorShowHideJustIgnore";
            this.MenuSeparatorShowHideJustIgnore.Size = new System.Drawing.Size(6, 30);
            // 
            // MenuButtonWireframeMode
            // 
            this.MenuButtonWireframeMode.CheckOnClick = true;
            this.MenuButtonWireframeMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuButtonWireframeMode.Image = ((System.Drawing.Image)(resources.GetObject("MenuButtonWireframeMode.Image")));
            this.MenuButtonWireframeMode.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuButtonWireframeMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuButtonWireframeMode.Name = "MenuButtonWireframeMode";
            this.MenuButtonWireframeMode.Size = new System.Drawing.Size(28, 27);
            this.MenuButtonWireframeMode.ToolTipText = "Enable/Disable wireframe";
            // 
            // ToolButtonShowGrid
            // 
            this.ToolButtonShowGrid.CheckOnClick = true;
            this.ToolButtonShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonShowGrid.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonShowGrid.Image")));
            this.ToolButtonShowGrid.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonShowGrid.Name = "ToolButtonShowGrid";
            this.ToolButtonShowGrid.Size = new System.Drawing.Size(28, 27);
            this.ToolButtonShowGrid.ToolTipText = "Show/Hide grid";
            // 
            // ToolButtonShowAxis
            // 
            this.ToolButtonShowAxis.CheckOnClick = true;
            this.ToolButtonShowAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonShowAxis.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonShowAxis.Image")));
            this.ToolButtonShowAxis.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonShowAxis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonShowAxis.Name = "ToolButtonShowAxis";
            this.ToolButtonShowAxis.Size = new System.Drawing.Size(28, 27);
            this.ToolButtonShowAxis.ToolTipText = "Show/Hide axis";
            // 
            // MenuButtonShowBones
            // 
            this.MenuButtonShowBones.CheckOnClick = true;
            this.MenuButtonShowBones.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuButtonShowBones.Image = ((System.Drawing.Image)(resources.GetObject("MenuButtonShowBones.Image")));
            this.MenuButtonShowBones.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuButtonShowBones.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuButtonShowBones.Name = "MenuButtonShowBones";
            this.MenuButtonShowBones.Size = new System.Drawing.Size(28, 27);
            this.MenuButtonShowBones.ToolTipText = "Show/Hide skeleton";
            // 
            // MenuButtonShowInfo
            // 
            this.MenuButtonShowInfo.CheckOnClick = true;
            this.MenuButtonShowInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuButtonShowInfo.Image = ((System.Drawing.Image)(resources.GetObject("MenuButtonShowInfo.Image")));
            this.MenuButtonShowInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuButtonShowInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuButtonShowInfo.Name = "MenuButtonShowInfo";
            this.MenuButtonShowInfo.Size = new System.Drawing.Size(28, 27);
            this.MenuButtonShowInfo.ToolTipText = "Show/Hide model info.";
            // 
            // MenuButtonShowSideMenu
            // 
            this.MenuButtonShowSideMenu.CheckOnClick = true;
            this.MenuButtonShowSideMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuButtonShowSideMenu.Image = ((System.Drawing.Image)(resources.GetObject("MenuButtonShowSideMenu.Image")));
            this.MenuButtonShowSideMenu.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuButtonShowSideMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuButtonShowSideMenu.Name = "MenuButtonShowSideMenu";
            this.MenuButtonShowSideMenu.Size = new System.Drawing.Size(28, 27);
            this.MenuButtonShowSideMenu.ToolTipText = "Show/Hide side menu";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.TopIcons);
            this.Controls.Add(this.TopMenu);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICA";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.TopMenu.ResumeLayout(false);
            this.TopMenu.PerformLayout();
            this.TopIcons.ResumeLayout(false);
            this.TopIcons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.MenuStrip TopMenu;
        private System.Windows.Forms.ToolStrip TopIcons;
        private System.Windows.Forms.ToolStripButton ToolButtonOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuFileRoot;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpRoot;
        private System.Windows.Forms.ToolStripMenuItem MenuOpenFile;
        private System.Windows.Forms.ToolStripButton ToolButtonSave;
        private System.Windows.Forms.ToolStripSeparator MenuSeparatorShowHideJustIgnore;
        private System.Windows.Forms.ToolStripButton ToolButtonShowGrid;
        private System.Windows.Forms.ToolStripButton ToolButtonShowAxis;
        private System.Windows.Forms.ToolStripButton MenuButtonShowBones;
        private System.Windows.Forms.ToolStripButton MenuButtonWireframeMode;
        private System.Windows.Forms.ToolStripButton MenuButtonShowInfo;
        private System.Windows.Forms.ToolStripButton MenuButtonShowSideMenu;
    }
}