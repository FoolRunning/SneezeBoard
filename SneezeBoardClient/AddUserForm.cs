using System.Windows.Forms;

namespace SneezeBoardClient
{
    public partial class AddUserForm : Form
    {
        public AddUserForm()
        {
            InitializeComponent();
        }
        public string UserName => txtbx_name.Text;
    }
}
