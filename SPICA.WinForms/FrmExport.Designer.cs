namespace SPICA.WinForms
{
    partial class FrmExport
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
            this.GrpInput = new System.Windows.Forms.GroupBox();
            this.GrpAnimationInput = new System.Windows.Forms.GroupBox();
            this.BtnBrowseIn = new System.Windows.Forms.Button();
            this.TxtInputFolder = new System.Windows.Forms.TextBox();
            this.BtnBrowseAnimationIn = new System.Windows.Forms.Button();
            this.TxtInputAnimationFolder = new System.Windows.Forms.TextBox();
            this.GrpOutput = new System.Windows.Forms.GroupBox();
            this.ChkPrefixNames = new System.Windows.Forms.CheckBox();
            this.ChkExportModels = new System.Windows.Forms.CheckBox();
            this.ChkExportAnimations = new System.Windows.Forms.CheckBox();
            this.ChkExportTextures = new System.Windows.Forms.CheckBox();
            this.CmbFormat = new System.Windows.Forms.ComboBox();
            this.BtnBrowseOut = new System.Windows.Forms.Button();
            this.TxtOutFolder = new System.Windows.Forms.TextBox();
            this.BtnConvert = new System.Windows.Forms.Button();
            this.ProgressConv = new System.Windows.Forms.ProgressBar();
            this.GrpInput.SuspendLayout();
            this.GrpAnimationInput.SuspendLayout();
            this.GrpOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // GrpInput
            // 
            this.GrpInput.Controls.Add(this.BtnBrowseIn);
            this.GrpInput.Controls.Add(this.TxtInputFolder);
            this.GrpInput.Location = new System.Drawing.Point(12, 12);
            this.GrpInput.Name = "GrpInput";
            this.GrpInput.Size = new System.Drawing.Size(360, 51);
            this.GrpInput.TabIndex = 0;
            this.GrpInput.TabStop = false;
            this.GrpInput.Text = "Input folder";
            // 
            // GrpAnimationInput
            // 
            this.GrpAnimationInput.Controls.Add(this.BtnBrowseAnimationIn);
            this.GrpAnimationInput.Controls.Add(this.TxtInputAnimationFolder);
            this.GrpAnimationInput.Location = new System.Drawing.Point(12, 63);
            this.GrpAnimationInput.Name = "GrpAnimationInput";
            this.GrpAnimationInput.Size = new System.Drawing.Size(360, 51);
            this.GrpAnimationInput.TabIndex = 0;
            this.GrpAnimationInput.TabStop = false;
            this.GrpAnimationInput.Text = "Input animation folder";
            // 
            // BtnBrowseIn
            // 
            this.BtnBrowseIn.Location = new System.Drawing.Point(322, 21);
            this.BtnBrowseIn.Name = "BtnBrowseIn";
            this.BtnBrowseIn.Size = new System.Drawing.Size(32, 24);
            this.BtnBrowseIn.TabIndex = 1;
            this.BtnBrowseIn.Text = "...";
            this.BtnBrowseIn.UseVisualStyleBackColor = true;
            this.BtnBrowseIn.Click += new System.EventHandler(this.BtnBrowseIn_Click);
            // 
            // TxtInputFolder
            // 
            this.TxtInputFolder.Location = new System.Drawing.Point(6, 22);
            this.TxtInputFolder.Name = "TxtInputFolder";
            this.TxtInputFolder.Size = new System.Drawing.Size(310, 22);
            this.TxtInputFolder.TabIndex = 0;
            // 
            // TxtInputAnimationFolder
            // 
            this.TxtInputAnimationFolder.Location = new System.Drawing.Point(6, 22);
            this.TxtInputAnimationFolder.Name = "TxtInputAnimationFolder";
            this.TxtInputAnimationFolder.Size = new System.Drawing.Size(310, 22);
            this.TxtInputAnimationFolder.TabIndex = 0;
            // 
            // BtnBrowseAnimationIn
            // 
            this.BtnBrowseAnimationIn.Location = new System.Drawing.Point(322, 21);
            this.BtnBrowseAnimationIn.Name = "BtnBrowseAnimationIn";
            this.BtnBrowseAnimationIn.Size = new System.Drawing.Size(32, 24);
            this.BtnBrowseAnimationIn.TabIndex = 1;
            this.BtnBrowseAnimationIn.Text = "...";
            this.BtnBrowseAnimationIn.UseVisualStyleBackColor = true;
            this.BtnBrowseAnimationIn.Click += new System.EventHandler(this.BtnBrowseAnimationIn_Click);
            // 
            // GrpOutput
            // 
            this.GrpOutput.Controls.Add(this.ChkPrefixNames);
            this.GrpOutput.Controls.Add(this.ChkExportModels);
            this.GrpOutput.Controls.Add(this.ChkExportAnimations);
            this.GrpOutput.Controls.Add(this.ChkExportTextures);
            this.GrpOutput.Controls.Add(this.CmbFormat);
            this.GrpOutput.Controls.Add(this.BtnBrowseOut);
            this.GrpOutput.Controls.Add(this.TxtOutFolder);
            this.GrpOutput.Location = new System.Drawing.Point(12, 114);
            this.GrpOutput.Name = "GrpOutput";
            this.GrpOutput.Size = new System.Drawing.Size(360, 140);
            this.GrpOutput.TabIndex = 0;
            this.GrpOutput.TabStop = false;
            this.GrpOutput.Text = "Output folder";
            // 
            // ChkPrefixNames
            // 
            this.ChkPrefixNames.AutoSize = true;
            this.ChkPrefixNames.Location = new System.Drawing.Point(6, 117);
            this.ChkPrefixNames.Name = "ChkPrefixNames";
            this.ChkPrefixNames.Size = new System.Drawing.Size(167, 17);
            this.ChkPrefixNames.TabIndex = 7;
            this.ChkPrefixNames.Text = "Add original name as prefix";
            this.ChkPrefixNames.UseVisualStyleBackColor = true;
            // 
            // ChkExportModels
            // 
            this.ChkExportModels.AutoSize = true;
            this.ChkExportModels.Location = new System.Drawing.Point(6, 48);
            this.ChkExportModels.Name = "ChkExportModels";
            this.ChkExportModels.Size = new System.Drawing.Size(99, 17);
            this.ChkExportModels.TabIndex = 4;
            this.ChkExportModels.Text = "Export models";
            this.ChkExportModels.UseVisualStyleBackColor = true;
            // 
            // ChkExportAnimations
            // 
            this.ChkExportAnimations.AutoSize = true;
            this.ChkExportAnimations.Location = new System.Drawing.Point(6, 94);
            this.ChkExportAnimations.Name = "ChkExportAnimations";
            this.ChkExportAnimations.Size = new System.Drawing.Size(119, 17);
            this.ChkExportAnimations.TabIndex = 6;
            this.ChkExportAnimations.Text = "Export animations";
            this.ChkExportAnimations.UseVisualStyleBackColor = true;
            // 
            // ChkExportTextures
            // 
            this.ChkExportTextures.AutoSize = true;
            this.ChkExportTextures.Location = new System.Drawing.Point(6, 71);
            this.ChkExportTextures.Name = "ChkExportTextures";
            this.ChkExportTextures.Size = new System.Drawing.Size(103, 17);
            this.ChkExportTextures.TabIndex = 5;
            this.ChkExportTextures.Text = "Export textures";
            this.ChkExportTextures.UseVisualStyleBackColor = true;
            // 
            // CmbFormat
            // 
            this.CmbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CmbFormat.FormattingEnabled = true;
            this.CmbFormat.Items.AddRange(new object[] {
            "COLLADA 1.4.1 (*.dae)",
            "Valve StudioMdl (*.smd)"});
            this.CmbFormat.Location = new System.Drawing.Point(162, 49);
            this.CmbFormat.Name = "CmbFormat";
            this.CmbFormat.Size = new System.Drawing.Size(192, 21);
            this.CmbFormat.TabIndex = 8;
            // 
            // BtnBrowseOut
            // 
            this.BtnBrowseOut.Location = new System.Drawing.Point(322, 19);
            this.BtnBrowseOut.Name = "BtnBrowseOut";
            this.BtnBrowseOut.Size = new System.Drawing.Size(32, 24);
            this.BtnBrowseOut.TabIndex = 3;
            this.BtnBrowseOut.Text = "...";
            this.BtnBrowseOut.UseVisualStyleBackColor = true;
            this.BtnBrowseOut.Click += new System.EventHandler(this.BtnBrowseOut_Click);
            // 
            // TxtOutFolder
            // 
            this.TxtOutFolder.Location = new System.Drawing.Point(6, 20);
            this.TxtOutFolder.Name = "TxtOutFolder";
            this.TxtOutFolder.Size = new System.Drawing.Size(310, 22);
            this.TxtOutFolder.TabIndex = 2;
            // 
            // BtnConvert
            // 
            this.BtnConvert.Location = new System.Drawing.Point(276, 329);
            this.BtnConvert.Name = "BtnConvert";
            this.BtnConvert.Size = new System.Drawing.Size(96, 24);
            this.BtnConvert.TabIndex = 9;
            this.BtnConvert.Text = "Convert";
            this.BtnConvert.UseVisualStyleBackColor = true;
            this.BtnConvert.Click += new System.EventHandler(this.BtnConvert_Click);
            // 
            // ProgressConv
            // 
            this.ProgressConv.Location = new System.Drawing.Point(12, 329);
            this.ProgressConv.Name = "ProgressConv";
            this.ProgressConv.Size = new System.Drawing.Size(258, 24);
            this.ProgressConv.TabIndex = 0;
            // 
            // FrmExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.ProgressConv);
            this.Controls.Add(this.BtnConvert);
            this.Controls.Add(this.GrpOutput);
            this.Controls.Add(this.GrpInput);
            this.Controls.Add(this.GrpAnimationInput);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmExport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Batch Exporter";
            this.Load += new System.EventHandler(this.FrmExport_Load);
            this.GrpInput.ResumeLayout(false);
            this.GrpInput.PerformLayout();
            this.GrpAnimationInput.ResumeLayout(false);
            this.GrpAnimationInput.PerformLayout();
            this.GrpOutput.ResumeLayout(false);
            this.GrpOutput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GrpInput;
        private System.Windows.Forms.GroupBox GrpAnimationInput;
        private System.Windows.Forms.Button BtnBrowseIn;
        private System.Windows.Forms.TextBox TxtInputFolder;
        private System.Windows.Forms.Button BtnBrowseAnimationIn;
        private System.Windows.Forms.TextBox TxtInputAnimationFolder;
        private System.Windows.Forms.GroupBox GrpOutput;
        private System.Windows.Forms.CheckBox ChkPrefixNames;
        private System.Windows.Forms.CheckBox ChkExportModels;
        private System.Windows.Forms.CheckBox ChkExportAnimations;
        private System.Windows.Forms.CheckBox ChkExportTextures;
        private System.Windows.Forms.ComboBox CmbFormat;
        private System.Windows.Forms.Button BtnBrowseOut;
        private System.Windows.Forms.TextBox TxtOutFolder;
        private System.Windows.Forms.Button BtnConvert;
        private System.Windows.Forms.ProgressBar ProgressConv;
    }
}