using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics.Eventing.Reader;

namespace ClientApp
{
    public partial class Form2 : Form
    {
        static string PathUser = Path.Combine(Environment.CurrentDirectory, "Users.json");
        public Form1 _form1;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;

        }

        static List<User_OnlyLoginPassword> ReadUsersFromDB()
        {
            string UserDB = File.ReadAllText(PathUser);
            List<User_OnlyLoginPassword> users = JsonConvert.DeserializeObject<List<User_OnlyLoginPassword>>(UserDB);
            return users;
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            List<User_OnlyLoginPassword> users = ReadUsersFromDB();
            if ((login.Text != "") && (password.Text != ""))
            {
                var CurrentUser = users.
                    Where(User_OnlyLoginPassword => User_OnlyLoginPassword.login.Equals(login.Text)
                        && User_OnlyLoginPassword.password.Equals(password.Text));
                if (CurrentUser.Count() != 1)
                    label3.Text = "Пользователь не найден";
                else
                {
                    Form1.Buffer.current_login = CurrentUser.First().login;
                    this.Close();
                }
            }
            else label3.Text = "Есть незаполненные поля";
           
        }

        class User_OnlyLoginPassword
        {
            public string login { get; private set; }
            public string password { get; set; }

            public User_OnlyLoginPassword(string login, string password)
            {
                this.login = login;
                this.password = password;
            }
        }

    }
}
