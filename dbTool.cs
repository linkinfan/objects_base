using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections.Generic;

namespace BestWorker
{
    public partial class dbTool : Form
    {
        MySqlConnection conn;
        server server = new server();
        string Server = Properties.Settings.Default.server;
        string user = Properties.Settings.Default.user;
        string password = Properties.Settings.Default.password;
        string port = Properties.Settings.Default.port;
        string database = Properties.Settings.Default.database;

        public dbTool()
        {
            InitializeComponent();
        }
        private void dbTool_Load(object sender, EventArgs e)
        {
            List<string> serverList = server.getServer();
            List<string> list = server.getList();
            numericUpDown1.Value = Convert.ToInt16(serverList[0]);
            numericUpDown2.Value = Convert.ToInt16(serverList[1]);
            numericUpDown3.Value = Convert.ToInt16(serverList[2]);
            numericUpDown4.Value = Convert.ToInt16(serverList[3]);
            textBox7.Text = list[0];
            textBox8.Text = list[1];
            textBox5.Text = list[3];
            maskedTextBox1.Text = list[2];


        }
        string string1;

        private void button2_Click(object sender, EventArgs e)
        {
            string address, namedb, port, username, userpass;
            address = numericUpDown1.Value + "." + numericUpDown2.Value + "." + numericUpDown3.Value + "." + numericUpDown4.Value;
            namedb = textBox5.Text;
            int portI = Convert.ToInt16(maskedTextBox1.Text);
            port = portI.ToString();
            username = textBox7.Text;
            userpass = textBox8.Text;
            string1 = "server=" + address + ";user=" + username + ";password=" + userpass + ";port=" + port + ";database=" + namedb + ";ConvertZeroDatetime=True;AllowZeroDateTime=True;";
            conn = new MySqlConnection(string1);
            try
            {
                conn.Open();
                conn.Close();
                MessageBox.Show("Проверка выполнена успешно");
                Properties.Settings.Default.server = address;
                Properties.Settings.Default.user = username;
                Properties.Settings.Default.password = userpass;
                Properties.Settings.Default.port = port;
                Properties.Settings.Default.database = namedb;
                Properties.Settings.Default.Save();
                this.Close();
            }
            catch { MessageBox.Show("Соединение настроено неправильно, либо отсутствует связь"); }
        }
    }
}
