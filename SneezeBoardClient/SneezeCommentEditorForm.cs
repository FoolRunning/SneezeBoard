using System.Windows.Forms;

namespace SneezeBoardClient
{
    public partial class SneezeCommentEditorForm : Form
    {
        public SneezeCommentEditorForm(string currentText)
        {
            InitializeComponent();
            txtBx_update.Text = currentText;
        }

        public string UpdatedText => txtBx_update.Text;
    }
}
