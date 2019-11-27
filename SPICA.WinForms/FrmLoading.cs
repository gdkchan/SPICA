using System.Drawing;
using System.Windows.Forms;

namespace SPICA.WinForms
{
    public class FrmLoading : Form
    {
        private ProgressBar m_Bar;
        private Label m_Label;

        private int m_Length;
        private int m_Counter;

        public FrmLoading(int Length)
        {
            m_Bar = new ProgressBar {Dock = DockStyle.Bottom, Maximum = Length - 1, Step = 1};
            m_Label = new Label {Dock = DockStyle.Top, AutoSize = true, MaximumSize = new Size(400, 0)};
            m_Length = Length;

            Padding = new Padding(10);
            Size = new Size(400, 120);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Controls.AddRange(new Control[] {m_Label, m_Bar});

            LostFocus += (sender, args) => BringToFront();
            Closing += (sender, args) => args.Cancel = true;

            CenterToScreen();
            Show();
        }

        public void Proceed(string FileName)
        {
            Text = "Loading Files... (" + ++m_Counter + "/" + m_Length + ")";
            m_Label.Text = "Loading \"" + FileName + "\"";
            m_Bar.PerformStep();

            // Prevents UI from not responding
            Application.DoEvents();
        }
    }
}