namespace SPICA.WinForms {
	partial class FrmGFTFormat {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.formatCombo = new System.Windows.Forms.ComboBox();
			this.saveBut = new System.Windows.Forms.Button();
			this.cancelBut = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.formatCombo);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(138, 54);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "GFTexture Format";
			// 
			// formatCombo
			// 
			this.formatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.formatCombo.FormattingEnabled = true;
			this.formatCombo.Items.AddRange(new object[] {
            "PC",
            "Raw"});
			this.formatCombo.Location = new System.Drawing.Point(6, 19);
			this.formatCombo.Name = "formatCombo";
			this.formatCombo.Size = new System.Drawing.Size(121, 21);
			this.formatCombo.TabIndex = 0;
			// 
			// saveBut
			// 
			this.saveBut.Location = new System.Drawing.Point(12, 72);
			this.saveBut.Name = "saveBut";
			this.saveBut.Size = new System.Drawing.Size(69, 24);
			this.saveBut.TabIndex = 2;
			this.saveBut.Text = "Save";
			this.saveBut.UseVisualStyleBackColor = true;
			this.saveBut.Click += new System.EventHandler(this.saveBut_Click);
			// 
			// cancelBut
			// 
			this.cancelBut.Location = new System.Drawing.Point(87, 72);
			this.cancelBut.Name = "cancelBut";
			this.cancelBut.Size = new System.Drawing.Size(63, 24);
			this.cancelBut.TabIndex = 3;
			this.cancelBut.Text = "Cancel";
			this.cancelBut.UseVisualStyleBackColor = true;
			this.cancelBut.Click += new System.EventHandler(this.cancelBut_Click);
			// 
			// FrmGFTFormat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(162, 107);
			this.Controls.Add(this.cancelBut);
			this.Controls.Add(this.saveBut);
			this.Controls.Add(this.groupBox1);
			this.Name = "FrmGFTFormat";
			this.Text = "Export";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox formatCombo;
		private System.Windows.Forms.Button saveBut;
		private System.Windows.Forms.Button cancelBut;
	}
}