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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.TopMenu = new System.Windows.Forms.MenuStrip();
            this.MenuFileRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMergeFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gFBMDLSwitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuBatchExport = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOptionsRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuRenderer = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuShowGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuShowAxis = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuShowSkeleton = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuShowInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuWireframeMode = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUserInterface = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuShowSide = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gFPAKExtractorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelpRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.AnimControlsPanel = new System.Windows.Forms.Panel();
            this.LblAnimSpeed = new System.Windows.Forms.Label();
            this.LblAnimLoopMode = new System.Windows.Forms.Label();
            this.Animator = new System.Windows.Forms.Timer(this.components);
            this.TabIcons = new System.Windows.Forms.ImageList(this.components);
            this.MainContainer = new System.Windows.Forms.SplitContainer();
            this.SideIcons = new System.Windows.Forms.ToolStrip();
            this.ToolButtonExport = new System.Windows.Forms.ToolStripButton();
            this.ToolButtonImport = new System.Windows.Forms.ToolStripButton();
            this.TBtnOpen = new System.Windows.Forms.ToolStripButton();
            this.TBtnMerge = new System.Windows.Forms.ToolStripButton();
            this.TBtnSave = new System.Windows.Forms.ToolStripButton();
            this.MenuSeparatorShowHideJustIgnore = new System.Windows.Forms.ToolStripSeparator();
            this.TBtnShowGrid = new System.Windows.Forms.ToolStripButton();
            this.TBtnShowAxis = new System.Windows.Forms.ToolStripButton();
            this.TBtnShowBones = new System.Windows.Forms.ToolStripButton();
            this.TBtnShowInfo = new System.Windows.Forms.ToolStripButton();
            this.TBtnShowSide = new System.Windows.Forms.ToolStripButton();
            this.TopIcons = new System.Windows.Forms.ToolStrip();
            this.SideTabs = new SPICA.WinForms.GUI.SUITabControl();
            this.TabPageModels = new System.Windows.Forms.TabPage();
            this.ModelsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageTextures = new System.Windows.Forms.TabPage();
            this.TexturesList = new SPICA.WinForms.GUI.SUIList();
            this.TexturePreview = new System.Windows.Forms.PictureBox();
            this.TextureInfo = new System.Windows.Forms.Label();
            this.TabPageCameras = new System.Windows.Forms.TabPage();
            this.CamerasList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageLights = new System.Windows.Forms.TabPage();
            this.LightsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageSklAnims = new System.Windows.Forms.TabPage();
            this.SklAnimsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageMatAnims = new System.Windows.Forms.TabPage();
            this.MatAnimsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageVisAnims = new System.Windows.Forms.TabPage();
            this.VisAnimsList = new SPICA.WinForms.GUI.SUIList();
            this.TabPageCamAnims = new System.Windows.Forms.TabPage();
            this.CamAnimsList = new SPICA.WinForms.GUI.SUIList();
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
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
            this.MainContainer.Panel2.SuspendLayout();
            this.MainContainer.SuspendLayout();
            this.SideIcons.SuspendLayout();
            this.TopIcons.SuspendLayout();
            this.SideTabs.SuspendLayout();
            this.TabPageModels.SuspendLayout();
            this.TabPageTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TexturePreview)).BeginInit();
            this.TabPageCameras.SuspendLayout();
            this.TabPageLights.SuspendLayout();
            this.TabPageSklAnims.SuspendLayout();
            this.TabPageMatAnims.SuspendLayout();
            this.TabPageVisAnims.SuspendLayout();
            this.TabPageCamAnims.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopMenu
            // 
            this.TopMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.TopMenu.ForeColor = System.Drawing.Color.White;
            this.TopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFileRoot,
            this.MenuOptionsRoot,
            this.toolsToolStripMenuItem,
            this.MenuHelpRoot});
            this.TopMenu.Location = new System.Drawing.Point(0, 0);
            this.TopMenu.Name = "TopMenu";
            this.TopMenu.Size = new System.Drawing.Size(944, 24);
            this.TopMenu.TabIndex = 1;
            // 
            // MenuFileRoot
            // 
            this.MenuFileRoot.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuOpenFile,
            this.MenuMergeFiles,
            this.exportToolStripMenuItem,
            this.MenuBatchExport});
            this.MenuFileRoot.Name = "MenuFileRoot";
            this.MenuFileRoot.Size = new System.Drawing.Size(37, 20);
            this.MenuFileRoot.Text = "&File";
            // 
            // MenuOpenFile
            // 
            this.MenuOpenFile.Name = "MenuOpenFile";
            this.MenuOpenFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuOpenFile.Size = new System.Drawing.Size(162, 22);
            this.MenuOpenFile.Text = "&Open...";
            this.MenuOpenFile.Click += new System.EventHandler(this.MenuOpenFile_Click);
            // 
            // MenuMergeFiles
            // 
            this.MenuMergeFiles.Name = "MenuMergeFiles";
            this.MenuMergeFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.MenuMergeFiles.Size = new System.Drawing.Size(162, 22);
            this.MenuMergeFiles.Text = "&Merge...";
            this.MenuMergeFiles.Click += new System.EventHandler(this.MenuMergeFiles_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gFBMDLSwitchToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.exportToolStripMenuItem.Text = "Export...";
            // 
            // gFBMDLSwitchToolStripMenuItem
            // 
            this.gFBMDLSwitchToolStripMenuItem.Name = "gFBMDLSwitchToolStripMenuItem";
            this.gFBMDLSwitchToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.gFBMDLSwitchToolStripMenuItem.Text = "GFBMDL (Switch)";
            this.gFBMDLSwitchToolStripMenuItem.Click += new System.EventHandler(this.gFBMDLSwitchToolStripMenuItem_Click);
            // 
            // MenuBatchExport
            // 
            this.MenuBatchExport.Name = "MenuBatchExport";
            this.MenuBatchExport.Size = new System.Drawing.Size(162, 22);
            this.MenuBatchExport.Text = "&Batch export...";
            this.MenuBatchExport.Click += new System.EventHandler(this.MenuBatchExport_Click);
            // 
            // MenuOptionsRoot
            // 
            this.MenuOptionsRoot.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuRenderer,
            this.MenuUserInterface});
            this.MenuOptionsRoot.Name = "MenuOptionsRoot";
            this.MenuOptionsRoot.Size = new System.Drawing.Size(61, 20);
            this.MenuOptionsRoot.Text = "&Options";
            // 
            // MenuRenderer
            // 
            this.MenuRenderer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuShowGrid,
            this.MenuShowAxis,
            this.MenuShowSkeleton,
            this.MenuShowInfo,
            this.MenuWireframeMode});
            this.MenuRenderer.Name = "MenuRenderer";
            this.MenuRenderer.Size = new System.Drawing.Size(146, 22);
            this.MenuRenderer.Text = "&Renderer";
            // 
            // MenuShowGrid
            // 
            this.MenuShowGrid.Name = "MenuShowGrid";
            this.MenuShowGrid.Size = new System.Drawing.Size(167, 22);
            this.MenuShowGrid.Text = "Show &grid";
            this.MenuShowGrid.Click += new System.EventHandler(this.MenuShowGrid_Click);
            // 
            // MenuShowAxis
            // 
            this.MenuShowAxis.Name = "MenuShowAxis";
            this.MenuShowAxis.Size = new System.Drawing.Size(167, 22);
            this.MenuShowAxis.Text = "Show &axis";
            this.MenuShowAxis.Click += new System.EventHandler(this.MenuShowAxis_Click);
            // 
            // MenuShowSkeleton
            // 
            this.MenuShowSkeleton.Name = "MenuShowSkeleton";
            this.MenuShowSkeleton.Size = new System.Drawing.Size(167, 22);
            this.MenuShowSkeleton.Text = "Show &skeleton";
            // 
            // MenuShowInfo
            // 
            this.MenuShowInfo.Name = "MenuShowInfo";
            this.MenuShowInfo.Size = new System.Drawing.Size(167, 22);
            this.MenuShowInfo.Text = "Show &model info.";
            // 
            // MenuWireframeMode
            // 
            this.MenuWireframeMode.Name = "MenuWireframeMode";
            this.MenuWireframeMode.Size = new System.Drawing.Size(167, 22);
            this.MenuWireframeMode.Text = "&Wireframe mode";
            // 
            // MenuUserInterface
            // 
            this.MenuUserInterface.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuShowSide});
            this.MenuUserInterface.Name = "MenuUserInterface";
            this.MenuUserInterface.Size = new System.Drawing.Size(146, 22);
            this.MenuUserInterface.Text = "&User interface";
            // 
            // MenuShowSide
            // 
            this.MenuShowSide.Name = "MenuShowSide";
            this.MenuShowSide.Size = new System.Drawing.Size(161, 22);
            this.MenuShowSide.Text = "Show &side menu";
            this.MenuShowSide.Click += new System.EventHandler(this.MenuShowSide_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gFPAKExtractorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // gFPAKExtractorToolStripMenuItem
            // 
            this.gFPAKExtractorToolStripMenuItem.Name = "gFPAKExtractorToolStripMenuItem";
            this.gFPAKExtractorToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.gFPAKExtractorToolStripMenuItem.Text = "GFLXPack extractor";
            this.gFPAKExtractorToolStripMenuItem.Click += new System.EventHandler(this.GFPAKExtractorToolStripMenuItem_Click);
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
            this.AnimControlsPanel.Size = new System.Drawing.Size(944, 30);
            this.AnimControlsPanel.TabIndex = 4;
            // 
            // LblAnimSpeed
            // 
            this.LblAnimSpeed.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblAnimSpeed.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnimSpeed.Location = new System.Drawing.Point(855, 1);
            this.LblAnimSpeed.Name = "LblAnimSpeed";
            this.LblAnimSpeed.Size = new System.Drawing.Size(44, 27);
            this.LblAnimSpeed.TabIndex = 1;
            this.LblAnimSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblAnimLoopMode
            // 
            this.LblAnimLoopMode.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblAnimLoopMode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblAnimLoopMode.Location = new System.Drawing.Point(899, 1);
            this.LblAnimLoopMode.Name = "LblAnimLoopMode";
            this.LblAnimLoopMode.Size = new System.Drawing.Size(44, 27);
            this.LblAnimLoopMode.TabIndex = 0;
            this.LblAnimLoopMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Animator
            // 
            this.Animator.Interval = 16;
            this.Animator.Tick += new System.EventHandler(this.Animator_Tick);
            // 
            // TabIcons
            // 
            this.TabIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TabIcons.ImageStream")));
            this.TabIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.TabIcons.Images.SetKeyName(0, "sui_cube.png");
            this.TabIcons.Images.SetKeyName(1, "sui_eye.png");
            this.TabIcons.Images.SetKeyName(2, "sui_camera.png");
            this.TabIcons.Images.SetKeyName(3, "sui_lightbulb.png");
            this.TabIcons.Images.SetKeyName(4, "sui_bone_film.png");
            this.TabIcons.Images.SetKeyName(5, "sui_rainbow_film.png");
            this.TabIcons.Images.SetKeyName(6, "sui_transparent_film.png");
            this.TabIcons.Images.SetKeyName(7, "sui_cam_film.png");
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
            this.MainContainer.Panel2.Controls.Add(this.SideIcons);
            this.MainContainer.Size = new System.Drawing.Size(944, 477);
            this.MainContainer.SplitterDistance = 698;
            this.MainContainer.TabIndex = 0;
            // 
            // SideIcons
            // 
            this.SideIcons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.SideIcons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolButtonExport,
            this.ToolButtonImport});
            this.SideIcons.Location = new System.Drawing.Point(0, 0);
            this.SideIcons.Name = "SideIcons";
            this.SideIcons.Size = new System.Drawing.Size(242, 31);
            this.SideIcons.TabIndex = 1;
            // 
            // ToolButtonExport
            // 
            this.ToolButtonExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonExport.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonExport.Image")));
            this.ToolButtonExport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonExport.Name = "ToolButtonExport";
            this.ToolButtonExport.Size = new System.Drawing.Size(28, 28);
            this.ToolButtonExport.ToolTipText = "Export...";
            this.ToolButtonExport.Click += new System.EventHandler(this.ToolButtonExport_Click);
            // 
            // ToolButtonImport
            // 
            this.ToolButtonImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolButtonImport.Image = ((System.Drawing.Image)(resources.GetObject("ToolButtonImport.Image")));
            this.ToolButtonImport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.ToolButtonImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolButtonImport.Name = "ToolButtonImport";
            this.ToolButtonImport.Size = new System.Drawing.Size(28, 28);
            this.ToolButtonImport.ToolTipText = "Import...";
            this.ToolButtonImport.Click += new System.EventHandler(this.ToolButtonImport_Click);
            // 
            // TBtnOpen
            // 
            this.TBtnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnOpen.Image = ((System.Drawing.Image)(resources.GetObject("TBtnOpen.Image")));
            this.TBtnOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnOpen.Name = "TBtnOpen";
            this.TBtnOpen.Size = new System.Drawing.Size(28, 27);
            this.TBtnOpen.ToolTipText = "Open...";
            this.TBtnOpen.Click += new System.EventHandler(this.TBtnOpen_Click);
            // 
            // TBtnMerge
            // 
            this.TBtnMerge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnMerge.Image = ((System.Drawing.Image)(resources.GetObject("TBtnMerge.Image")));
            this.TBtnMerge.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnMerge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnMerge.Name = "TBtnMerge";
            this.TBtnMerge.Size = new System.Drawing.Size(28, 27);
            this.TBtnMerge.ToolTipText = "Merge...";
            this.TBtnMerge.Click += new System.EventHandler(this.TBtnMerge_Click);
            // 
            // TBtnSave
            // 
            this.TBtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnSave.Image = ((System.Drawing.Image)(resources.GetObject("TBtnSave.Image")));
            this.TBtnSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnSave.Name = "TBtnSave";
            this.TBtnSave.Size = new System.Drawing.Size(28, 27);
            this.TBtnSave.ToolTipText = "Save...";
            this.TBtnSave.Click += new System.EventHandler(this.TBtnSave_Click);
            // 
            // MenuSeparatorShowHideJustIgnore
            // 
            this.MenuSeparatorShowHideJustIgnore.Name = "MenuSeparatorShowHideJustIgnore";
            this.MenuSeparatorShowHideJustIgnore.Size = new System.Drawing.Size(6, 30);
            // 
            // TBtnShowGrid
            // 
            this.TBtnShowGrid.CheckOnClick = true;
            this.TBtnShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnShowGrid.Image = ((System.Drawing.Image)(resources.GetObject("TBtnShowGrid.Image")));
            this.TBtnShowGrid.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnShowGrid.Name = "TBtnShowGrid";
            this.TBtnShowGrid.Size = new System.Drawing.Size(28, 27);
            this.TBtnShowGrid.ToolTipText = "Toggle grid";
            this.TBtnShowGrid.Click += new System.EventHandler(this.TBtnShowGrid_Click);
            // 
            // TBtnShowAxis
            // 
            this.TBtnShowAxis.CheckOnClick = true;
            this.TBtnShowAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnShowAxis.Image = ((System.Drawing.Image)(resources.GetObject("TBtnShowAxis.Image")));
            this.TBtnShowAxis.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnShowAxis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnShowAxis.Name = "TBtnShowAxis";
            this.TBtnShowAxis.Size = new System.Drawing.Size(28, 27);
            this.TBtnShowAxis.ToolTipText = "Toggle axis";
            this.TBtnShowAxis.Click += new System.EventHandler(this.TBtnShowAxis_Click);
            // 
            // TBtnShowBones
            // 
            this.TBtnShowBones.CheckOnClick = true;
            this.TBtnShowBones.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnShowBones.Image = ((System.Drawing.Image)(resources.GetObject("TBtnShowBones.Image")));
            this.TBtnShowBones.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnShowBones.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnShowBones.Name = "TBtnShowBones";
            this.TBtnShowBones.Size = new System.Drawing.Size(28, 27);
            this.TBtnShowBones.ToolTipText = "Toggle skeleton";
            // 
            // TBtnShowInfo
            // 
            this.TBtnShowInfo.CheckOnClick = true;
            this.TBtnShowInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnShowInfo.Image = ((System.Drawing.Image)(resources.GetObject("TBtnShowInfo.Image")));
            this.TBtnShowInfo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnShowInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnShowInfo.Name = "TBtnShowInfo";
            this.TBtnShowInfo.Size = new System.Drawing.Size(28, 27);
            this.TBtnShowInfo.ToolTipText = "Toggle model info.";
            // 
            // TBtnShowSide
            // 
            this.TBtnShowSide.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.TBtnShowSide.CheckOnClick = true;
            this.TBtnShowSide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TBtnShowSide.Image = ((System.Drawing.Image)(resources.GetObject("TBtnShowSide.Image")));
            this.TBtnShowSide.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TBtnShowSide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TBtnShowSide.Name = "TBtnShowSide";
            this.TBtnShowSide.Size = new System.Drawing.Size(28, 27);
            this.TBtnShowSide.ToolTipText = "Toggle side menu";
            this.TBtnShowSide.Click += new System.EventHandler(this.TBtnShowSide_Click);
            // 
            // TopIcons
            // 
            this.TopIcons.AutoSize = false;
            this.TopIcons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.TopIcons.ForeColor = System.Drawing.Color.White;
            this.TopIcons.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TBtnOpen,
            this.TBtnMerge,
            this.TBtnSave,
            this.MenuSeparatorShowHideJustIgnore,
            this.TBtnShowGrid,
            this.TBtnShowAxis,
            this.TBtnShowBones,
            this.TBtnShowInfo,
            this.TBtnShowSide});
            this.TopIcons.Location = new System.Drawing.Point(0, 24);
            this.TopIcons.Name = "TopIcons";
            this.TopIcons.Size = new System.Drawing.Size(944, 30);
            this.TopIcons.TabIndex = 3;
            // 
            // SideTabs
            // 
            this.SideTabs.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.SideTabs.Controls.Add(this.TabPageModels);
            this.SideTabs.Controls.Add(this.TabPageTextures);
            this.SideTabs.Controls.Add(this.TabPageCameras);
            this.SideTabs.Controls.Add(this.TabPageLights);
            this.SideTabs.Controls.Add(this.TabPageSklAnims);
            this.SideTabs.Controls.Add(this.TabPageMatAnims);
            this.SideTabs.Controls.Add(this.TabPageVisAnims);
            this.SideTabs.Controls.Add(this.TabPageCamAnims);
            this.SideTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SideTabs.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SideTabs.ForegroundColor = System.Drawing.Color.White;
            this.SideTabs.ImageList = this.TabIcons;
            this.SideTabs.ItemSize = new System.Drawing.Size(24, 24);
            this.SideTabs.Location = new System.Drawing.Point(0, 31);
            this.SideTabs.Multiline = true;
            this.SideTabs.Name = "SideTabs";
            this.SideTabs.SelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.SideTabs.SelectedIndex = 0;
            this.SideTabs.ShowToolTips = true;
            this.SideTabs.Size = new System.Drawing.Size(242, 446);
            this.SideTabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.SideTabs.TabIndex = 0;
            // 
            // TabPageModels
            // 
            this.TabPageModels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageModels.Controls.Add(this.ModelsList);
            this.TabPageModels.ImageKey = "sui_cube.png";
            this.TabPageModels.Location = new System.Drawing.Point(4, 28);
            this.TabPageModels.Name = "TabPageModels";
            this.TabPageModels.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageModels.Size = new System.Drawing.Size(234, 414);
            this.TabPageModels.TabIndex = 0;
            this.TabPageModels.ToolTipText = "Models";
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
            this.ModelsList.MultiSelect = true;
            this.ModelsList.Name = "ModelsList";
            this.ModelsList.SelectedIndex = -1;
            this.ModelsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.ModelsList.Size = new System.Drawing.Size(228, 408);
            this.ModelsList.TabIndex = 0;
            this.ModelsList.SelectedIndexChanged += new System.EventHandler(this.ModelsList_SelectedIndexChanged);
            // 
            // TabPageTextures
            // 
            this.TabPageTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageTextures.Controls.Add(this.TexturesList);
            this.TabPageTextures.Controls.Add(this.TexturePreview);
            this.TabPageTextures.Controls.Add(this.TextureInfo);
            this.TabPageTextures.ImageKey = "sui_eye.png";
            this.TabPageTextures.Location = new System.Drawing.Point(4, 28);
            this.TabPageTextures.Name = "TabPageTextures";
            this.TabPageTextures.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageTextures.Size = new System.Drawing.Size(234, 414);
            this.TabPageTextures.TabIndex = 1;
            this.TabPageTextures.ToolTipText = "Textures";
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
            this.TexturesList.MultiSelect = false;
            this.TexturesList.Name = "TexturesList";
            this.TexturesList.SelectedIndex = -1;
            this.TexturesList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.TexturesList.Size = new System.Drawing.Size(228, 199);
            this.TexturesList.TabIndex = 2;
            this.TexturesList.SelectedIndexChanged += new System.EventHandler(this.TexturesList_SelectedIndexChanged);
            // 
            // TexturePreview
            // 
            this.TexturePreview.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TexturePreview.Location = new System.Drawing.Point(3, 202);
            this.TexturePreview.Name = "TexturePreview";
            this.TexturePreview.Size = new System.Drawing.Size(228, 192);
            this.TexturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TexturePreview.TabIndex = 1;
            this.TexturePreview.TabStop = false;
            // 
            // TextureInfo
            // 
            this.TextureInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TextureInfo.Location = new System.Drawing.Point(3, 394);
            this.TextureInfo.Name = "TextureInfo";
            this.TextureInfo.Size = new System.Drawing.Size(228, 17);
            this.TextureInfo.TabIndex = 0;
            // 
            // TabPageCameras
            // 
            this.TabPageCameras.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageCameras.Controls.Add(this.CamerasList);
            this.TabPageCameras.ImageKey = "sui_camera.png";
            this.TabPageCameras.Location = new System.Drawing.Point(4, 28);
            this.TabPageCameras.Name = "TabPageCameras";
            this.TabPageCameras.Size = new System.Drawing.Size(234, 414);
            this.TabPageCameras.TabIndex = 4;
            this.TabPageCameras.ToolTipText = "Cameras";
            // 
            // CamerasList
            // 
            this.CamerasList.BackColor = System.Drawing.Color.Transparent;
            this.CamerasList.BarColor = System.Drawing.Color.White;
            this.CamerasList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.CamerasList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CamerasList.ForeColor = System.Drawing.Color.White;
            this.CamerasList.ItemHeight = 16;
            this.CamerasList.Location = new System.Drawing.Point(0, 0);
            this.CamerasList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CamerasList.MultiSelect = false;
            this.CamerasList.Name = "CamerasList";
            this.CamerasList.SelectedIndex = -1;
            this.CamerasList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.CamerasList.Size = new System.Drawing.Size(234, 414);
            this.CamerasList.TabIndex = 0;
            this.CamerasList.Selected += new System.EventHandler(this.CamerasList_Selected);
            // 
            // TabPageLights
            // 
            this.TabPageLights.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageLights.Controls.Add(this.LightsList);
            this.TabPageLights.ImageIndex = 3;
            this.TabPageLights.Location = new System.Drawing.Point(4, 28);
            this.TabPageLights.Name = "TabPageLights";
            this.TabPageLights.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageLights.Size = new System.Drawing.Size(234, 414);
            this.TabPageLights.TabIndex = 7;
            this.TabPageLights.ToolTipText = "Lights";
            // 
            // LightsList
            // 
            this.LightsList.BackColor = System.Drawing.Color.Transparent;
            this.LightsList.BarColor = System.Drawing.Color.White;
            this.LightsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.LightsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LightsList.ForeColor = System.Drawing.Color.White;
            this.LightsList.ItemHeight = 16;
            this.LightsList.Location = new System.Drawing.Point(3, 3);
            this.LightsList.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.LightsList.MultiSelect = true;
            this.LightsList.Name = "LightsList";
            this.LightsList.SelectedIndex = -1;
            this.LightsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.LightsList.Size = new System.Drawing.Size(228, 408);
            this.LightsList.TabIndex = 1;
            this.LightsList.Selected += new System.EventHandler(this.LightsList_Selected);
            // 
            // TabPageSklAnims
            // 
            this.TabPageSklAnims.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageSklAnims.Controls.Add(this.SklAnimsList);
            this.TabPageSklAnims.ImageKey = "sui_bone_film.png";
            this.TabPageSklAnims.Location = new System.Drawing.Point(4, 28);
            this.TabPageSklAnims.Name = "TabPageSklAnims";
            this.TabPageSklAnims.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageSklAnims.Size = new System.Drawing.Size(234, 414);
            this.TabPageSklAnims.TabIndex = 2;
            this.TabPageSklAnims.ToolTipText = "Skeletal animations";
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
            this.SklAnimsList.MultiSelect = true;
            this.SklAnimsList.Name = "SklAnimsList";
            this.SklAnimsList.SelectedIndex = -1;
            this.SklAnimsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.SklAnimsList.Size = new System.Drawing.Size(228, 408);
            this.SklAnimsList.TabIndex = 3;
            this.SklAnimsList.Selected += new System.EventHandler(this.SklAnimsList_Selected);
            // 
            // TabPageMatAnims
            // 
            this.TabPageMatAnims.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageMatAnims.Controls.Add(this.MatAnimsList);
            this.TabPageMatAnims.ImageKey = "sui_rainbow_film.png";
            this.TabPageMatAnims.Location = new System.Drawing.Point(4, 28);
            this.TabPageMatAnims.Name = "TabPageMatAnims";
            this.TabPageMatAnims.Padding = new System.Windows.Forms.Padding(3);
            this.TabPageMatAnims.Size = new System.Drawing.Size(234, 414);
            this.TabPageMatAnims.TabIndex = 3;
            this.TabPageMatAnims.ToolTipText = "Material animations";
            // 
            // MatAnimsList
            // 
            this.MatAnimsList.BackColor = System.Drawing.Color.Transparent;
            this.MatAnimsList.BarColor = System.Drawing.Color.White;
            this.MatAnimsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.MatAnimsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MatAnimsList.ForeColor = System.Drawing.Color.White;
            this.MatAnimsList.ItemHeight = 16;
            this.MatAnimsList.Location = new System.Drawing.Point(3, 3);
            this.MatAnimsList.Margin = new System.Windows.Forms.Padding(3, 9, 3, 9);
            this.MatAnimsList.MultiSelect = true;
            this.MatAnimsList.Name = "MatAnimsList";
            this.MatAnimsList.SelectedIndex = -1;
            this.MatAnimsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.MatAnimsList.Size = new System.Drawing.Size(228, 408);
            this.MatAnimsList.TabIndex = 4;
            this.MatAnimsList.Selected += new System.EventHandler(this.MatAnimsList_Selected);
            // 
            // TabPageVisAnims
            // 
            this.TabPageVisAnims.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageVisAnims.Controls.Add(this.VisAnimsList);
            this.TabPageVisAnims.ImageKey = "sui_transparent_film.png";
            this.TabPageVisAnims.Location = new System.Drawing.Point(4, 28);
            this.TabPageVisAnims.Name = "TabPageVisAnims";
            this.TabPageVisAnims.Size = new System.Drawing.Size(234, 414);
            this.TabPageVisAnims.TabIndex = 6;
            this.TabPageVisAnims.ToolTipText = "Visibility animations";
            // 
            // VisAnimsList
            // 
            this.VisAnimsList.BackColor = System.Drawing.Color.Transparent;
            this.VisAnimsList.BarColor = System.Drawing.Color.White;
            this.VisAnimsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.VisAnimsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisAnimsList.ForeColor = System.Drawing.Color.White;
            this.VisAnimsList.ItemHeight = 16;
            this.VisAnimsList.Location = new System.Drawing.Point(0, 0);
            this.VisAnimsList.Margin = new System.Windows.Forms.Padding(3, 12, 3, 12);
            this.VisAnimsList.MultiSelect = true;
            this.VisAnimsList.Name = "VisAnimsList";
            this.VisAnimsList.SelectedIndex = -1;
            this.VisAnimsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.VisAnimsList.Size = new System.Drawing.Size(234, 414);
            this.VisAnimsList.TabIndex = 5;
            this.VisAnimsList.Selected += new System.EventHandler(this.VisAnimsList_Selected);
            // 
            // TabPageCamAnims
            // 
            this.TabPageCamAnims.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.TabPageCamAnims.Controls.Add(this.CamAnimsList);
            this.TabPageCamAnims.ImageKey = "sui_cam_film.png";
            this.TabPageCamAnims.Location = new System.Drawing.Point(4, 28);
            this.TabPageCamAnims.Name = "TabPageCamAnims";
            this.TabPageCamAnims.Size = new System.Drawing.Size(234, 414);
            this.TabPageCamAnims.TabIndex = 5;
            this.TabPageCamAnims.ToolTipText = "Camera animations";
            // 
            // CamAnimsList
            // 
            this.CamAnimsList.BackColor = System.Drawing.Color.Transparent;
            this.CamAnimsList.BarColor = System.Drawing.Color.White;
            this.CamAnimsList.BarColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(183)))), ((int)(((byte)(123)))));
            this.CamAnimsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CamAnimsList.ForeColor = System.Drawing.Color.White;
            this.CamAnimsList.ItemHeight = 16;
            this.CamAnimsList.Location = new System.Drawing.Point(0, 0);
            this.CamAnimsList.Margin = new System.Windows.Forms.Padding(3, 12, 3, 12);
            this.CamAnimsList.MultiSelect = false;
            this.CamAnimsList.Name = "CamAnimsList";
            this.CamAnimsList.SelectedIndex = -1;
            this.CamAnimsList.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(134)))), ((int)(((byte)(106)))));
            this.CamAnimsList.Size = new System.Drawing.Size(234, 414);
            this.CamAnimsList.TabIndex = 5;
            this.CamAnimsList.Selected += new System.EventHandler(this.CamAnimsList_Selected);
            // 
            // AnimSeekBar
            // 
            this.AnimSeekBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(53)))), ((int)(((byte)(48)))));
            this.AnimSeekBar.CursorColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(209)))), ((int)(((byte)(134)))));
            this.AnimSeekBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnimSeekBar.ForeColor = System.Drawing.Color.Gainsboro;
            this.AnimSeekBar.Location = new System.Drawing.Point(1, 1);
            this.AnimSeekBar.Maximum = 0F;
            this.AnimSeekBar.Name = "AnimSeekBar";
            this.AnimSeekBar.Size = new System.Drawing.Size(630, 27);
            this.AnimSeekBar.TabIndex = 8;
            this.AnimSeekBar.Value = 0F;
            this.AnimSeekBar.Seek += new System.EventHandler(this.AnimSeekBar_Seek);
            this.AnimSeekBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnimSeekBar_MouseUp);
            // 
            // AnimButtonPrev
            // 
            this.AnimButtonPrev.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPrev.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPrev.Icon")));
            this.AnimButtonPrev.Location = new System.Drawing.Point(631, 1);
            this.AnimButtonPrev.Name = "AnimButtonPrev";
            this.AnimButtonPrev.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPrev.TabIndex = 9;
            this.AnimButtonPrev.Click += new System.EventHandler(this.AnimButtonPrev_Click);
            // 
            // AnimButtonSlowDown
            // 
            this.AnimButtonSlowDown.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonSlowDown.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonSlowDown.Icon")));
            this.AnimButtonSlowDown.Location = new System.Drawing.Point(659, 1);
            this.AnimButtonSlowDown.Name = "AnimButtonSlowDown";
            this.AnimButtonSlowDown.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonSlowDown.TabIndex = 10;
            this.AnimButtonSlowDown.Click += new System.EventHandler(this.AnimButtonSlowDown_Click);
            // 
            // AnimButtonPlayBackward
            // 
            this.AnimButtonPlayBackward.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPlayBackward.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPlayBackward.Icon")));
            this.AnimButtonPlayBackward.Location = new System.Drawing.Point(687, 1);
            this.AnimButtonPlayBackward.Name = "AnimButtonPlayBackward";
            this.AnimButtonPlayBackward.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPlayBackward.TabIndex = 7;
            this.AnimButtonPlayBackward.Click += new System.EventHandler(this.AnimButtonPlayBackward_Click);
            // 
            // AnimButtonPlayForward
            // 
            this.AnimButtonPlayForward.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPlayForward.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPlayForward.Icon")));
            this.AnimButtonPlayForward.Location = new System.Drawing.Point(715, 1);
            this.AnimButtonPlayForward.Name = "AnimButtonPlayForward";
            this.AnimButtonPlayForward.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPlayForward.TabIndex = 6;
            this.AnimButtonPlayForward.Click += new System.EventHandler(this.AnimButtonPlayForward_Click);
            // 
            // AnimButtonPause
            // 
            this.AnimButtonPause.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonPause.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonPause.Icon")));
            this.AnimButtonPause.Location = new System.Drawing.Point(743, 1);
            this.AnimButtonPause.Name = "AnimButtonPause";
            this.AnimButtonPause.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonPause.TabIndex = 5;
            this.AnimButtonPause.Click += new System.EventHandler(this.AnimButtonPause_Click);
            // 
            // AnimButtonStop
            // 
            this.AnimButtonStop.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonStop.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonStop.Icon")));
            this.AnimButtonStop.Location = new System.Drawing.Point(771, 1);
            this.AnimButtonStop.Name = "AnimButtonStop";
            this.AnimButtonStop.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonStop.TabIndex = 4;
            this.AnimButtonStop.Click += new System.EventHandler(this.AnimButtonStop_Click);
            // 
            // AnimButtonSpeedUp
            // 
            this.AnimButtonSpeedUp.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonSpeedUp.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonSpeedUp.Icon")));
            this.AnimButtonSpeedUp.Location = new System.Drawing.Point(799, 1);
            this.AnimButtonSpeedUp.Name = "AnimButtonSpeedUp";
            this.AnimButtonSpeedUp.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonSpeedUp.TabIndex = 3;
            this.AnimButtonSpeedUp.Click += new System.EventHandler(this.AnimButtonSpeedUp_Click);
            // 
            // AnimButtonNext
            // 
            this.AnimButtonNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.AnimButtonNext.Icon = ((System.Drawing.Bitmap)(resources.GetObject("AnimButtonNext.Icon")));
            this.AnimButtonNext.Location = new System.Drawing.Point(827, 1);
            this.AnimButtonNext.Name = "AnimButtonNext";
            this.AnimButtonNext.Size = new System.Drawing.Size(28, 27);
            this.AnimButtonNext.TabIndex = 2;
            this.AnimButtonNext.Click += new System.EventHandler(this.AnimButtonNext_Click);
            // 
            // FrmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(66)))), ((int)(((byte)(61)))));
            this.ClientSize = new System.Drawing.Size(944, 561);
            this.Controls.Add(this.MainContainer);
            this.Controls.Add(this.AnimControlsPanel);
            this.Controls.Add(this.TopIcons);
            this.Controls.Add(this.TopMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SPICA";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragEnter);
            this.TopMenu.ResumeLayout(false);
            this.TopMenu.PerformLayout();
            this.AnimControlsPanel.ResumeLayout(false);
            this.MainContainer.Panel2.ResumeLayout(false);
            this.MainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
            this.MainContainer.ResumeLayout(false);
            this.SideIcons.ResumeLayout(false);
            this.SideIcons.PerformLayout();
            this.TopIcons.ResumeLayout(false);
            this.TopIcons.PerformLayout();
            this.SideTabs.ResumeLayout(false);
            this.TabPageModels.ResumeLayout(false);
            this.TabPageTextures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TexturePreview)).EndInit();
            this.TabPageCameras.ResumeLayout(false);
            this.TabPageLights.ResumeLayout(false);
            this.TabPageSklAnims.ResumeLayout(false);
            this.TabPageMatAnims.ResumeLayout(false);
            this.TabPageVisAnims.ResumeLayout(false);
            this.TabPageCamAnims.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage TabPageSklAnims;
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
        private GUI.SUIIconButton AnimButtonSlowDown;
        private GUI.SUIIconButton AnimButtonPrev;
        private System.Windows.Forms.Timer Animator;
        private System.Windows.Forms.ImageList TabIcons;
        private System.Windows.Forms.TabPage TabPageMatAnims;
        private GUI.SUIList MatAnimsList;
        private System.Windows.Forms.ToolStripMenuItem MenuMergeFiles;
        private System.Windows.Forms.ToolStripMenuItem MenuBatchExport;
        private System.Windows.Forms.ToolStripMenuItem MenuOptionsRoot;
        private System.Windows.Forms.ToolStripMenuItem MenuRenderer;
        private System.Windows.Forms.ToolStripButton TBtnOpen;
        private System.Windows.Forms.ToolStripButton TBtnMerge;
        private System.Windows.Forms.ToolStripButton TBtnSave;
        private System.Windows.Forms.ToolStripSeparator MenuSeparatorShowHideJustIgnore;
        private System.Windows.Forms.ToolStripButton TBtnShowGrid;
        private System.Windows.Forms.ToolStripButton TBtnShowAxis;
        private System.Windows.Forms.ToolStripButton TBtnShowBones;
        private System.Windows.Forms.ToolStripButton TBtnShowInfo;
        private System.Windows.Forms.ToolStripButton TBtnShowSide;
        private System.Windows.Forms.ToolStrip TopIcons;
        private System.Windows.Forms.ToolStripMenuItem MenuShowGrid;
        private System.Windows.Forms.ToolStripMenuItem MenuShowAxis;
        private System.Windows.Forms.ToolStripMenuItem MenuShowSkeleton;
        private System.Windows.Forms.ToolStripMenuItem MenuShowInfo;
        private System.Windows.Forms.ToolStripMenuItem MenuWireframeMode;
        private System.Windows.Forms.ToolStripMenuItem MenuUserInterface;
        private System.Windows.Forms.ToolStripMenuItem MenuShowSide;
        private System.Windows.Forms.ToolStrip SideIcons;
        private System.Windows.Forms.ToolStripButton ToolButtonExport;
        private System.Windows.Forms.ToolStripButton ToolButtonImport;
        private System.Windows.Forms.TabPage TabPageCameras;
        private GUI.SUIList CamerasList;
        private System.Windows.Forms.TabPage TabPageCamAnims;
        private GUI.SUIList CamAnimsList;
        private System.Windows.Forms.TabPage TabPageVisAnims;
        private GUI.SUIList VisAnimsList;
        private System.Windows.Forms.TabPage TabPageLights;
        private GUI.SUIList LightsList;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gFPAKExtractorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gFBMDLSwitchToolStripMenuItem;
    }
}