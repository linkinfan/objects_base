using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BestWorker
{
    public partial class changeAppliance : Form
    {
        server server = new server();
        private MySqlConnection conn;
        dL dL = new dL();
        int appid = Properties.Settings.Default.applianceID;
        appliances apliaForm = new appliances();
        public changeAppliance()
        {
            InitializeComponent();
            Size res = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
            this.Location = new Point(res.Width / 3, res.Height/3);
            conn = new MySqlConnection(server.get());
            dateTimePicker1.Value = DateTime.Today.AddMonths(1);
            comboLoad();
            loadInfo();
        }


        void comboLoad()
        {
            comboBox1.DataSource = null;
            DataSet ds = dL.get("select idobjects, concat(name, '  ( ', address, ' )') as nameaddress from objects order by name");
            comboBox1.DataSource = ds.Tables[0];
            comboBox1.ValueMember = "idobjects";
            comboBox1.DisplayMember = "nameaddress";

            comboBox2.DataSource = null;
            DataSet ds2 = dL.get("select idobjectgroup, nameGroup from objectgroup");
            comboBox2.DataSource = ds2.Tables[0];
            comboBox2.ValueMember = "idobjectgroup";
            comboBox2.DisplayMember = "nameGroup";

        }

        void loadInfo()
        {
            DataSet ds = dL.get("select idappliances, nameapp, DATE_FORMAT(date, '%d-%m-%Y'), object from appliances where idappliances='" + appid + "';");
            comboBox1.SelectedValue = ds.Tables[0].Rows[0][3].ToString();
            applianceText.Text = ds.Tables[0].Rows[0][1].ToString();
            if (ds.Tables[0].Rows[0][2].ToString() != "")
            {
                dateTimePicker1.Value = Convert.ToDateTime(ds.Tables[0].Rows[0][2].ToString());
            }
            else
            {
                label1.Visible = true;
                dateTimePicker1.Value = DateTime.Today.AddMonths(1);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //string into = (textBox1.Text != "") ? ", archive" : "";
                //string values = (textBox1.Text != "") ? ", @archive" : "";
                using (conn)
                    if (conn.State == ConnectionState.Closed)
                    {
                        MySqlCommand cmd = new MySqlCommand("insert into objects(name, address, groupOb" + /*into + */")" + " values(@name, @address, @group" + /*values + */"');", conn);
                        conn.Open();
                        if (panel2.Visible == true && objectName.Text != "" && objectAddress.Text != "" && objectName.Text != " " && objectName.Text != "  ")
                        {
                            cmd.Parameters.AddWithValue("@name", objectName.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@address", objectAddress.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@group", comboBox2.SelectedValue);

                            //if (textBox1.Text != "") { cmd.Parameters.AddWithValue("@archive", textBox1.Text); }
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Добавлен объект");
                        }
                        MySqlCommand cmd2 = new MySqlCommand("update appliances set" +
                            " nameapp=@nameapp,date=@date,object=@object,changeDate=NOW(),userChange=@userChange where idappliances=@idapp", conn);
                        cmd2.Parameters.AddWithValue("@nameapp", applianceText.Text);
                        cmd2.Parameters.AddWithValue("@date", dateTimePicker1.Value);
                        cmd2.Parameters.AddWithValue("@object", ((panel2.Visible == true) ? cmd.LastInsertedId : comboBox1.SelectedValue));
                        cmd2.Parameters.AddWithValue("@idapp", appid); 
                        cmd2.Parameters.AddWithValue("@userChange", Environment.UserName);
                        cmd2.ExecuteNonQuery();
                    }

                MessageBox.Show("Данные оборудования изменены");
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("В пункте \"Оповестить о невыполнении за:\" введите число");
            }
        }

        //private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        textBox1.Text = openFileDialog1.FileName;
        //        addArchive.Text = "Изменить архив";
        //    }
        //}

        //private void textBox1_DoubleClick(object sender, EventArgs e)
        //{
        //    textBox1.SelectAll();
        //}

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


        //bool change = false;
        //private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (change == false || (e.KeyChar <= 47 && e.KeyChar >= 58))
        //    {
        //        e.Handled = true;
        //    }
        //    else if (change == true && (e.KeyChar >= 48 && e.KeyChar <= 57))
        //    {
        //        //неправильно срабатывают условия, вводятся не только цифры
        //        e.Handled = false;
        //    }
        //}



        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel2.Visible = true;
            label2.Visible = false;
            comboBox1.Visible = false;
            linkLabel2.Visible = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }
    }
}
