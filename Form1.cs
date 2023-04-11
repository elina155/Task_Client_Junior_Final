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
using System.Collections.ObjectModel;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        static string PathUser = "";
        static string PathUserType = Path.Combine(Environment.CurrentDirectory, "UserTypes.json");
        private static List<User> users;
        private static List<User_type> user_types;
        bool Access = false;
        private Form _newFormSignIn;

        public static class Buffer
        {
            public static String current_login = String.Empty;
            public static String current_password = String.Empty;
            public static String current_path = String.Empty;
        }

        public async Task Sign_in()
        {
            Buffer.current_path = PathUser;
            user_types = ReadTypeOfUserFromDB();
            try
            {
                users = ReadUsersFromDB();
            }
            catch 
            {
                MessageBox.Show("Не выбран файл");
                button5.PerformClick();
            }
            _newFormSignIn = new Form2(this);
            _newFormSignIn.ShowDialog();
            var Current_User = users.Where(User => User.login.Equals(Buffer.current_login));
            if (user_types.Where(User_type => User_type.id.Equals(Current_User.First().type_id)).First().allow_edit == true)
                Access = true;
        }

        static List<User> ReadUsersFromDB()
        
        {
            string UserDB = File.ReadAllText(PathUser);
            users = JsonConvert.DeserializeObject<List<User>>(UserDB);
            return users;
        }

        private async Task SelectUsers()
        {
            progressBar1.Show();
            user_types = ReadTypeOfUserFromDB();
            user_types = ReadTypeOfUserFromDB();
            try
            {
                users = ReadUsersFromDB(); 
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
            }
            catch
            {
                MessageBox.Show("Не выбран файл");
            }
            progressBar1.Hide();
        }

        static List<User_type> ReadTypeOfUserFromDB()
        {
            string User_typeDB = File.ReadAllText(PathUserType);
            user_types = JsonConvert.DeserializeObject<List<User_type>>(User_typeDB);
            return user_types;
        }

        static void SaveToDB(User new_user) 
        {
            string UserDB = File.ReadAllText(PathUser);
            users = JsonConvert.DeserializeObject<List<User>>(UserDB);
            users.Add(new_user);
            UserDB = JsonConvert.SerializeObject(users);
            File.WriteAllText(PathUser, UserDB);
            
        }

        static void DeleteFromDB(int ID)
        {
            string UserDB = File.ReadAllText(PathUser);
            users = JsonConvert.DeserializeObject<List<User>>(UserDB);
            var Delete_user = users.Where(User => User.id.Equals(ID));
            users.Remove(Delete_user.First());
            UserDB = JsonConvert.SerializeObject(users);
            File.WriteAllText(PathUser, UserDB);

        }

        public Form1()
        {
            
            InitializeComponent();
            
            progressBar1.Hide();
            user_types = ReadTypeOfUserFromDB();
            foreach (var User_type in user_types)
            {
                comboBox1.Items.Add(User_type.name);
                comboBox2.Items.Add(User_type.name);
            }

            gd.Columns.Add("id", "id");
            gd.Columns.Add("name", "Имя");
            gd.Columns.Add("login", "Логин");
            gd.Columns.Add("user_type", "Тип пользователя");
            gd.Columns.Add("last_visit_date", "Последнее посещение");
            gd.Columns["id"].ReadOnly = true;
            gd.Columns["last_visit_date"].ReadOnly = true;
            
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
            public bool allow_edit { get; set; }

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
            public string password { get; set; }
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
            if (Access)
            {
                button3.Visible = true;
                button4.Visible = true;
                button3.Enabled = true;
                button4.Enabled = true;
                textBox2.Visible = true;
                textBox3.Visible = true;
                textBox4.Visible = true;
                comboBox2.Visible = true;
            }
            else 
            {
                button3.Visible = false;
                button4.Visible = false;
                button3.Enabled = false;
                button4.Enabled = false;
                textBox2.Visible = false;
                textBox3.Visible = false;
                textBox4.Visible = false;
                comboBox2.Visible = false;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            user_types = ReadTypeOfUserFromDB();
            user_types = ReadTypeOfUserFromDB();
            try
            {
                users = ReadUsersFromDB();
                var Usertemp = users.GroupBy(User => User.id).Last();
                int new_id = Usertemp.First().id + 1;

                if ((textBox2.Text != "") && (textBox3.Text != "") && (textBox4.Text != "") && (comboBox2.SelectedIndex != -1))
                {
                    var array_login = users.Select(t => t.login).ToList();
                    if (array_login.Contains(textBox2.Text))
                    {
                        label5.Text = "Логин занят";
                        textBox2.Text = "";
                    }
                    else
                    {
                        User NEW_USER = new User(
                            new_id,
                            textBox2.Text,
                            textBox3.Text,
                            textBox4.Text,
                            user_types.Where(User_type => User_type.name.Equals(comboBox2.SelectedItem)).First().id,
                            DateTime.Now.AddMinutes(-10)
                            );

                        SaveToDB(NEW_USER);
                        textBox2.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        comboBox2.SelectedIndex = -1;
                        label3.Text = "";
                    }
                }

                button1.PerformClick();
            }
            catch
            {
                MessageBox.Show("Не выбран файл");
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            PathUser = openFileDialog1.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            user_types = ReadTypeOfUserFromDB();
            try
            {
                users = ReadUsersFromDB();
                foreach (DataGridViewRow row in gd.SelectedRows)
                {
                    User DELETE_USER = users.Where(User => User.id.Equals(row.Cells[0].Value)).First();
                    DeleteFromDB(DELETE_USER.id);
                    gd.Rows.Remove(row);
                }
            }
            catch
            {
                MessageBox.Show("Не выбран файл");
            }
            
            
        }
    }
}
