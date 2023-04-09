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
    public partial class Form1 : Form
    {
        static string PathUser = Path.Combine(Environment.CurrentDirectory, "Users.json");
        static string PathUserType = Path.Combine(Environment.CurrentDirectory, "UserTypes.json");

        private Form _newFormSignIn;

        public async Task Sign_in()
        {
            _newFormSignIn = new Form2(this);
            _newFormSignIn.Show();
        }

        static List<User> ReadUsersFromDB()
        {
            string UserDB = File.ReadAllText(PathUser);
            List<User> users = JsonConvert.DeserializeObject<List<User>>(UserDB);
            return users;
        }

        private async Task SelectUsers()
        {
            progressBar1.Show();
            List<User_type> user_types = ReadTypeOfUserFromDB();
            List<User> users = ReadUsersFromDB();

            gd.Rows.Clear();

            var possible_type_names = user_types
                .Where(User_type => comboBox1.SelectedItem is null 
                                 || User_type.name.Equals(comboBox1.SelectedItem.ToString()))
                .Select(t => t.id).ToList();
            var Usertemp = users
                .Where(User => User.name.Contains(textBox1.Text) 
                            && possible_type_names.Contains(User.type_id)
                            && User.last_visit_date >= dateTimePicker1.Value 
                            && User.last_visit_date <= dateTimePicker2.Value);
            await Task.Delay(5000);
            foreach (var User in Usertemp)
            {
                var type_name_table = user_types.Where(User_type => User_type.id.Equals(User.type_id));
                gd.Rows.Add(User.id, User.name, User.login, type_name_table.First().name, User.last_visit_date.ToShortDateString());
            }
            
            progressBar1.Hide();
        }

        static List<User_type> ReadTypeOfUserFromDB()
        {
            string User_typeDB = File.ReadAllText(PathUserType);
            List<User_type> user_types = JsonConvert.DeserializeObject<List<User_type>>(User_typeDB);
            return user_types;
        }

        public Form1()
        {
            InitializeComponent(); 
            progressBar1.Hide();
            List<User_type> user_types = ReadTypeOfUserFromDB();

            foreach (var User_type in user_types)
                comboBox1.Items.Add(User_type.name);

            gd.Columns.Add("id", "id");
            gd.Columns.Add("login", "Логин");
            gd.Columns.Add("name", "Имя");
            gd.Columns.Add("user_type", "Тип пользователя");
            gd.Columns.Add("last_visit_date", "Последнее посещение");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            gd.Rows.Clear();
            await SelectUsers();
        }

        class User_type
        {
            public int id { get; private set; }
            public string name { get; private set; }
            private bool allow_edit { get; set; }

            public User_type(int id, string name, bool allow_edit)
            {
                this.id = id;
                this.name = name;
                this.allow_edit = allow_edit;
            }
        }

        class User
        {
            public int id { get; private set; }
            public string login { get; private set; }
            private string password { get; set; }
            public string name { get; private set; }
            public int type_id { get; private set; }
            public DateTime last_visit_date { get; private set; }

            public User(int id, string login, string password, string name, int type_id, DateTime last_visit_date)
            {
                this.id = id;
                this.login = login; 
                this.password = password;   
                this.name = name;   
                this.type_id = type_id; 
                this.last_visit_date = last_visit_date;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            
            this.Enabled = false;
            await Sign_in();
            this.Enabled = true;
        }
    }
}
