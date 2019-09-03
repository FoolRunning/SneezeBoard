using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
