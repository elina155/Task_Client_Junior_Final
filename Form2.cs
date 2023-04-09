using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form2 : Form
    {
        private Form1 _form1;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;

        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            if ((login.Text != null) && (password.Text != null))
            { 
            
            }
        }
    }
}
