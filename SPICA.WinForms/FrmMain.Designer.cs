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
            this.TopMenu = new System.Windows.Forms.MenuStrip();
            this.MenuFileRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelpRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.AnimControlsPanel = new System.Windows.Forms.Panel();
            this.LblAnimSpeed = new System.Windows.Forms.Label();
            this.LblAnimLoopMode = new System.Windows.Forms.Label();
            this.ToolButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonSave = new System.Windows.Forms.ToolStripButton();
            this.MenuSeparatorShowHideJustIgnore = new System.Windows.Forms.ToolStripSeparator();
            this.MenuButtonWireframeMode = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonShowGrid = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonShowAxis = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowBones = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowInfo = new System.Windows.Forms.ToolStripButton();
            this.MenuButtonShowSideMenu = new System.Windows.Forms.ToolStripButton();
            this.TopIcons = new System.Windows.Forms.ToolStrip();
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.SideTabs = new SPICA.WinForms.GUI.SUITabControl();
            this.TabPageModels = new System.Windows.Forms.TabPage();
            this.ModelsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageTextures = new System.Windows.Forms.TabPage();
            this.TexturesList = new SPICA.WinForms.GUI.SUIList();
            this.TexturePreview = new System.Windows.Forms.PictureBox();
            this.TextureInfo = new System.Windows.Forms.Label();
            this.TabPageMdlAnims = new System.Windows.Forms.TabPage();
            this.SklAnimsList = new SPICA.WinForms.GUI.SUIList();
            this.AnimSeekBar = new SPICA.WinForms.GUI.SUIAnimSeekBar();
            this.AnimButtonPrev = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonSlowDown = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonPlayBackward = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonPlayForward = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonPause = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonStop = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonSpeedUp = new SPICA.WinForms.GUI.SUIIconButton();
            this.AnimButtonNext = new SPICA.WinForms.GUI.SUIIconButton();
            this.TopMenu.SuspendLayout();
            this.AnimControlsPanel.SuspendLayout();
            this.TopIcons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SideTabs.SuspendLayout();
            this.TabPageModels.SuspendLayout();
            this.TabPageTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TexturePreview)).BeginInit();
            this.TabPageMdlAnims.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopMenu
            // 
            this.TopMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
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
            this.MenuOpenFile.Size = new System.Drawing.Size(103, 22);
            this.MenuOpenFile.Text = "&Open";
            this.MenuOpenFile.Click += new System.EventHandler(this.MenuOpenFile_Click);
            // 
            // MenuHelpRoot
            // 
            this.MenuHelpRoot.Name = "MenuHelpRoot";
            this.MenuHelpRoot.Size = new System.Drawing.Size(44, 20);
            this.MenuHelpRoot.Text = "&Help";
            // 
            // AnimControlsPanel
            // 
            this.AnimControlsPanel.Controls.Add(this.AnimSeekBar);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonPrev);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonSlowDown);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonPlayBackward);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonPlayForward);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonPause);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonStop);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonSpeedUp);
            this.AnimControlsPanel.Controls.Add(this.AnimButtonNext);
            this.AnimControlsPanel.Controls.Add(this.LblAnimSpeed);
            this.AnimControlsPanel.Controls.Add(this.LblAnimLoopMode);
            this.AnimControlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.AnimControlsPanel.Location = new System.Drawing.Point(0, 531);
            this.AnimControlsPanel.Name = "AnimControlsPanel";
            this.AnimControlsPanel.Padding = new System.Windows.Forms.Padding(1, 1, 1, 2);
            this.AnimControlsPanel.Size = new System.Drawing.Size(784, 30);
            this.AnimControlsPanel.TabIndex = 4;
            // 
            // LblAnimSpeed
            // 
            this.LblAnimSpeed.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblAnimSpeed.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnimSpeed.Location = new System.Drawing.Point(703, 1);
            this.LblAnimSpeed.Name = "LblAnimSpeed";
            this.LblAnimSpeed.Size = new System.Drawing.Size(40, 27);
            this.LblAnimSpeed.TabIndex = 1;
            this.LblAnimSpeed.Text = "1.0x";
            this.LblAnimSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblAnimLoopMode
            // 
            this.LblAnimLoopMode.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblAnimLoopMode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnimLoopMode.Location = new System.Drawing.Point(743, 1);
            this.LblAnimLoopMode.Name = "LblAnimLoopMode";
            this.LblAnimLoopMode.Size = new System.Drawing.Size(40, 27);
            this.LblAnimLoopMode.TabIndex = 0;
            this.LblAnimLoopMode.Text = "1 GO";
            this.LblAnimLoopMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // MainContainer
            // 
            this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContainer.Location = new System.Drawing.Point(0, 54);
            this.MainContainer.Name = "MainContainer";
            // 
            // MainContainer.Panel2
            // 
            this.MainContainer.Panel2.Controls.Add(this.SideTabs);
            this.MainContainer.Size = new System.Drawing.Size(784, 477);
            this.MainContainer.SplitterDistance = 580;
            this.MainContainer.TabIndex = 0;
            // 
            // SideTabs
            // 
            this.SideTabs.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.SideTabs.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.SideTabs.Controls.Add(this.TabPageModels);
            this.SideTabs.Controls.Add(this.TabPageTextures);
            this.SideTabs.Controls.Add(this.TabPageMdlAnims);
            this.SideTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SideTabs.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideTabs.ForegroundColor = System.Drawing.Color.White;
            this.SideTabs.Location = new System.Drawing.Point(0, 0);
            this.SideTabs.Multiline = true;
            this.SideTabs.Name = "SideTabs";
            this.SideTabs.SelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.SideTabs.SelectedIndex = 0;
            this.SideTabs.Size = new System.Drawing.Size(200, 477);
            this.SideTabs.TabIndex = 0;
            // 
            // TabPageModels
            // 
            this.TabPageModels.BackColor = System.Drawing.Color.Transparent;
            this.TabPageModels.Controls.Add(this.ModelsList);
            this.TabPageModels.Location = new System.Drawing.Point(4, 4);
            this.TabPageModels.Name = "TabPageModels";
            this.TabPageModels.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageModels.Size = new System.Drawing.Size(169, 469);
            this.TabPageModels.TabIndex = 0;
            this.TabPageModels.Text = "Models";
            // 
            // ModelsList
            // 
            this.ModelsList.BackColor = System.Drawing.Color.Transparent;
            this.ModelsList.BarColor = System.Drawing.Color.White;
            this.ModelsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.ModelsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModelsList.ForeColor = System.Drawing.Color.White;
            this.ModelsList.ItemHeight = 16;
            this.ModelsList.Location = new System.Drawing.Point(3, 3);
            this.ModelsList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ModelsList.Name = "ModelsList";
            this.ModelsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.ModelsList.Size = new System.Drawing.Size(163, 463);
            this.ModelsList.TabIndex = 0;
            this.ModelsList.SelectedIndexChanged += new System.EventHandler(this.ModelsList_SelectedIndexChanged);
            // 
            // TabPageTextures
            // 
            this.TabPageTextures.BackColor = System.Drawing.Color.Transparent;
            this.TabPageTextures.Controls.Add(this.TexturesList);
            this.TabPageTextures.Controls.Add(this.TexturePreview);
            this.TabPageTextures.Controls.Add(this.TextureInfo);
            this.TabPageTextures.Location = new System.Drawing.Point(4, 4);
            this.TabPageTextures.Name = "TabPageTextures";
            this.TabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTextures.Size = new System.Drawing.Size(169, 469);
            this.TabPageTextures.TabIndex = 1;
            this.TabPageTextures.Text = "Textures";
            // 
            // TexturesList
            // 
            this.TexturesList.BackColor = System.Drawing.Color.Transparent;
            this.TexturesList.BarColor = System.Drawing.Color.White;
            this.TexturesList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.TexturesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TexturesList.ForeColor = System.Drawing.Color.White;
            this.TexturesList.ItemHeight = 16;
            this.TexturesList.Location = new System.Drawing.Point(3, 3);
            this.TexturesList.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.TexturesList.Name = "TexturesList";
            this.TexturesList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.TexturesList.Size = new System.Drawing.Size(163, 254);
            this.TexturesList.TabIndex = 2;
            this.TexturesList.SelectedIndexChanged += new System.EventHandler(this.TexturesList_SelectedIndexChanged);
            // 
            // TexturePreview
            // 
            this.TexturePreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TexturePreview.Location = new System.Drawing.Point(3, 257);
            this.TexturePreview.Name = "TexturePreview";
            this.TexturePreview.Size = new System.Drawing.Size(163, 192);
            this.TexturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TexturePreview.TabIndex = 1;
            this.TexturePreview.TabStop = false;
            // 
            // TextureInfo
            // 
            this.TextureInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextureInfo.Location = new System.Drawing.Point(3, 449);
            this.TextureInfo.Name = "TextureInfo";
            this.TextureInfo.Size = new System.Drawing.Size(163, 17);
            this.TextureInfo.TabIndex = 0;
            // 
            // TabPageMdlAnims
            // 
            this.TabPageMdlAnims.BackColor = System.Drawing.Color.Transparent;
            this.TabPageMdlAnims.Controls.Add(this.SklAnimsList);
            this.TabPageMdlAnims.Location = new System.Drawing.Point(4, 4);
            this.TabPageMdlAnims.Name = "TabPageMdlAnims";
            this.TabPageMdlAnims.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageMdlAnims.Size = new System.Drawing.Size(169, 469);
            this.TabPageMdlAnims.TabIndex = 2;
            this.TabPageMdlAnims.Text = "Model anims.";
            // 
            // SklAnimsList
            // 
            this.SklAnimsList.BackColor = System.Drawing.Color.Transparent;
            this.SklAnimsList.BarColor = System.Drawing.Color.White;
            this.SklAnimsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.SklAnimsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SklAnimsList.ForeColor = System.Drawing.Color.White;
            this.SklAnimsList.ItemHeight = 16;
            this.SklAnimsList.Location = new System.Drawing.Point(3, 3);
            this.SklAnimsList.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.SklAnimsList.Name = "SklAnimsList";
            this.SklAnimsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.SklAnimsList.Size = new System.Drawing.Size(163, 463);
            this.SklAnimsList.TabIndex = 3;
            this.SklAnimsList.SelectedIndexChanged += new System.EventHandler(this.SklAnimsList_SelectedIndexChanged);
            // 
            // AnimSeekBar
            // 
            this.AnimSeekBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.AnimSeekBar.CursorColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(209)))), ((int)(((byte)(134)))));
            this.AnimSeekBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnimSeekBar.ForeColor = System.Drawing.Color.Gainsboro;
            this.AnimSeekBar.Location = new System.Drawing.Point(1, 1);
            this.AnimSeekBar.Maximum = 10F;
            this.AnimSeekBar.Name = "AnimSeekBar";
            this.AnimSeekBar.Size = new System.Drawing.Size(478, 27);
            this.AnimSeekBar.TabIndex = 8;
            this.AnimSeekBar.Value = 2F;
            this.AnimSeekBar.Seek += new System.EventHandler(this.AnimSeekBar_Seek);
            this.AnimSeekBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnimSeekBar_MouseUp);
            // 
            // AnimButtonPrev
            // 
            this.AnimButtonPrev.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPrev.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPrev.Icon")));
            this.AnimButtonPrev.Location = new System.Drawing.Point(479, 1);
            this.AnimButtonPrev.Name = "AnimButtonPrev";
            this.AnimButtonPrev.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPrev.TabIndex = 9;
            // 
            // AnimButtonSlowDown
            // 
            this.AnimButtonSlowDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonSlowDown.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonSlowDown.Icon")));
            this.AnimButtonSlowDown.Location = new System.Drawing.Point(507, 1);
            this.AnimButtonSlowDown.Name = "AnimButtonSlowDown";
            this.AnimButtonSlowDown.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonSlowDown.TabIndex = 10;
            this.AnimButtonSlowDown.Click += new System.EventHandler(this.AnimButtonSlowDown_Click);
            // 
            // AnimButtonPlayBackward
            // 
            this.AnimButtonPlayBackward.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPlayBackward.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPlayBackward.Icon")));
            this.AnimButtonPlayBackward.Location = new System.Drawing.Point(535, 1);
            this.AnimButtonPlayBackward.Name = "AnimButtonPlayBackward";
            this.AnimButtonPlayBackward.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPlayBackward.TabIndex = 7;
            this.AnimButtonPlayBackward.Click += new System.EventHandler(this.AnimButtonPlayBackward_Click);
            // 
            // AnimButtonPlayForward
            // 
            this.AnimButtonPlayForward.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPlayForward.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPlayForward.Icon")));
            this.AnimButtonPlayForward.Location = new System.Drawing.Point(563, 1);
            this.AnimButtonPlayForward.Name = "AnimButtonPlayForward";
            this.AnimButtonPlayForward.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPlayForward.TabIndex = 6;
            this.AnimButtonPlayForward.Click += new System.EventHandler(this.AnimButtonPlayForward_Click);
            // 
            // AnimButtonPause
            // 
            this.AnimButtonPause.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPause.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPause.Icon")));
            this.AnimButtonPause.Location = new System.Drawing.Point(591, 1);
            this.AnimButtonPause.Name = "AnimButtonPause";
            this.AnimButtonPause.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPause.TabIndex = 5;
            this.AnimButtonPause.Click += new System.EventHandler(this.AnimButtonPause_Click);
            // 
            // AnimButtonStop
            // 
            this.AnimButtonStop.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonStop.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonStop.Icon")));
            this.AnimButtonStop.Location = new System.Drawing.Point(619, 1);
            this.AnimButtonStop.Name = "AnimButtonStop";
            this.AnimButtonStop.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonStop.TabIndex = 4;
            this.AnimButtonStop.Click += new System.EventHandler(this.AnimButtonStop_Click);
            // 
            // AnimButtonSpeedUp
            // 
            this.AnimButtonSpeedUp.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonSpeedUp.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonSpeedUp.Icon")));
            this.AnimButtonSpeedUp.Location = new System.Drawing.Point(647, 1);
            this.AnimButtonSpeedUp.Name = "AnimButtonSpeedUp";
            this.AnimButtonSpeedUp.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonSpeedUp.TabIndex = 3;
            this.AnimButtonSpeedUp.Click += new System.EventHandler(this.AnimButtonSpeedUp_Click);
            // 
            // AnimButtonNext
            // 
            this.AnimButtonNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonNext.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonNext.Icon")));
            this.AnimButtonNext.Location = new System.Drawing.Point(675, 1);
            this.AnimButtonNext.Name = "AnimButtonNext";
            this.AnimButtonNext.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonNext.TabIndex = 2;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.AnimControlsPanel);
            this.Controls.Add(this.TopIcons);
            this.Controls.Add(this.TopMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICA";
            this.TopMenu.ResumeLayout(false);
            this.TopMenu.PerformLayout();
            this.AnimControlsPanel.ResumeLayout(false);
            this.TopIcons.ResumeLayout(false);
            this.TopIcons.PerformLayout();
            this.MainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.SideTabs.ResumeLayout(false);
            this.TabPageModels.ResumeLayout(false);
            this.TabPageTextures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TexturePreview)).EndInit();
            this.TabPageMdlAnims.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainContainer;
        private System.Windows.Forms.MenuStrip TopMenu;
        private System.Windows.Forms.ToolStripMenuItem MenuFileRoot;
        private System.Windows.Forms.ToolStripMenuItem MenuHelpRoot;
        private System.Windows.Forms.ToolStripMenuItem MenuOpenFile;
        private GUI.SUITabControl SideTabs;
        private System.Windows.Forms.TabPage TabPageModels;
        private System.Windows.Forms.TabPage TabPageTextures;
        private System.Windows.Forms.TabPage TabPageMdlAnims;
        private GUI.SUIList ModelsList;
        private System.Windows.Forms.Label TextureInfo;
        private System.Windows.Forms.PictureBox TexturePreview;
        private GUI.SUIList TexturesList;
        private GUI.SUIList SklAnimsList;
        private System.Windows.Forms.Panel AnimControlsPanel;
        private System.Windows.Forms.Label LblAnimSpeed;
        private System.Windows.Forms.Label LblAnimLoopMode;
        private GUI.SUIAnimSeekBar AnimSeekBar;
        private GUI.SUIIconButton AnimButtonPlayBackward;
        private GUI.SUIIconButton AnimButtonPlayForward;
        private GUI.SUIIconButton AnimButtonPause;
        private GUI.SUIIconButton AnimButtonStop;
        private GUI.SUIIconButton AnimButtonSpeedUp;
        private GUI.SUIIconButton AnimButtonNext;
        private System.Windows.Forms.ToolStripButton ToolButtonOpen;
        private System.Windows.Forms.ToolStripButton ToolButtonSave;
        private System.Windows.Forms.ToolStripSeparator MenuSeparatorShowHideJustIgnore;
        private System.Windows.Forms.ToolStripButton MenuButtonWireframeMode;
        private System.Windows.Forms.ToolStripButton ToolButtonShowGrid;
        private System.Windows.Forms.ToolStripButton ToolButtonShowAxis;
        private System.Windows.Forms.ToolStripButton MenuButtonShowBones;
        private System.Windows.Forms.ToolStripButton MenuButtonShowInfo;
        private System.Windows.Forms.ToolStripButton MenuButtonShowSideMenu;
        private System.Windows.Forms.ToolStrip TopIcons;
        private GUI.SUIIconButton AnimButtonSlowDown;
        private GUI.SUIIconButton AnimButtonPrev;
    }
}