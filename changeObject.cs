using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BestWorker
{
    public partial class changeObject : Form
    {
        SQLiteConnection conn;
        dL dL = new dL();
        string idOb = Properties.Settings.Default.idOb;
        public changeObject()
        {
            InitializeComponent();
            Size res = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(res.Width / 3, res.Height / 3);
            conn = new SQLiteConnection(dL.GetServ());
            loadInfo();
        }



        void loadInfo()
        {
            DataSet ds = dL.get("select idobjects, name, address, archive, groupOb from appliances where idobjects='" +
                idOb + "';");
            objectName.Text = ds.Tables[0].Rows[0][1].ToString();
            objectAddress.Text = ds.Tables[0].Rows[0][2].ToString();
            textBox1.Text = ds.Tables[0].Rows[0][3].ToString();
            folderBrowserDialog1.SelectedPath = ds.Tables[0].Rows[0][3].ToString();
            comboBox2.SelectedValue = ds.Tables[0].Rows[0][4].ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string into = (textBox1.Text != "") ? ", archive" : "";
                string values = (textBox1.Text != "") ? ", @archive" : "";
                using (conn)
                    if (conn.State == ConnectionState.Closed)
                    {
                        if (objectName.Text != "" && objectAddress.Text != "")
                        {
                            SQLiteCommand cmd = new SQLiteCommand("insert into objects(name, address, groupOb" + into + ")" + " values(@name, @address, @group" + values + "');", conn);
                            conn.Open();
                            cmd.Parameters.AddWithValue("@name", objectName.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@address", objectAddress.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@group", comboBox2.SelectedValue);
                            if (textBox1.Text != "") { cmd.Parameters.AddWithValue("@archive", textBox1.Text); }
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Добавлен объект");

                        }
                    }

            }
            catch (FormatException)
            {
                MessageBox.Show("В пункте \"Оповестить о невыполнении за:\" введите число");
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Regex regex1 = new Regex(@"\s+\s+");
            MatchCollection matches = regex1.Matches(objectName.Text);

            if (matches.Count > 0)
            {
                objectName.Text = objectName.Text.Substring(0, objectName.Text.Length - 1);
                objectName.SelectionStart = objectName.Text.Length;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
