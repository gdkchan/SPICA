using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPICA.WinForms {
	public partial class FrmGFTFormat : Form {

		public int Format = 0;

		public FrmGFTFormat() {
			InitializeComponent();
			this.formatCombo.SelectedIndex = 0;
		}

		private void cancelBut_Click(object sender, EventArgs e) {
			this.Format = -1;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void saveBut_Click(object sender, EventArgs e) {
			this.Format = this.formatCombo.SelectedIndex;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
