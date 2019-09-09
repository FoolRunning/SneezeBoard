using System;
using System.Windows.Forms;
using SneezeBoardClient.Properties;

namespace SneezeBoardClient
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            lbl_backColor.BackColor = Settings.Default.BoardBackColor;
            cmb_dateRange.SelectedIndex = (int)Settings.Default.DateRange;
        }

        private void lbl_backColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = lbl_backColor.BackColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                lbl_backColor.BackColor = colorDialog.Color;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            Settings.Default.BoardBackColor = lbl_backColor.BackColor;
            Settings.Default.DateRange = (DateRangeType)cmb_dateRange.SelectedIndex;
            Settings.Default.Save();
        }
    }
}
