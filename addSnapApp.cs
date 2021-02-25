using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SQLite;

namespace BestWorker
{
    public partial class addSnapApp : Form
    {
        SQLiteConnection conn;
        dL dL = new dL();
        List<string> appliancesNameList = new List<string>();
        List<string> appliancesDateList = new List<string>();

        public addSnapApp()
        {
            InitializeComponent();
            conn = new SQLiteConnection(dL.GetServ());
            dateTimePicker1.Value = DateTime.Today.AddMonths(1);
            comboLoad();
        }

        void loadChange()
        {
            objectName.Text = "";
            objectAddress.Text = "";
            //textBox1.Text = "";
        }

        void comboLoad()
        {
            comboBox1.DataSource = null;
            DataSet ds = dL.get("select idobjects, name || '  ( ' || address || ' )' as nameaddress from objects order by name");
            comboBox1.DataSource = ds.Tables[0];
            comboBox1.ValueMember = "idobjects";
            comboBox1.DisplayMember = "nameaddress";

            comboBox2.DataSource = null;
            DataSet ds2 = dL.get("select idobjectgroup, nameGroup from objectgroup");
            comboBox2.DataSource = ds2.Tables[0];
            comboBox2.ValueMember = "idobjectgroup";
            comboBox2.DisplayMember = "nameGroup";

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 0)
            {
                try
                {
                    string path = folderBrowserDialog1.SelectedPath;
                    string into = (path != "") ? ", archive" : "";
                    string values = (path != "") ? ", @archive" : "";
                    List<List<string>> appliances = new List<List<string>>();
                    if (conn.State == ConnectionState.Closed)
                    {
                        int lastid = 0;
                        SQLiteCommand cmd = new SQLiteCommand("insert into objects(name, address, groupOb" + into + ")" + " values(@name, @address, @group" + values + ");", conn);
                        if (panel2.Visible == true && objectName.Text != "" && objectAddress.Text != "" && objectName.Text != " " && objectName.Text != "  ")
                        {
                            cmd.Parameters.AddWithValue("@name", objectName.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@address", objectAddress.Text.Replace('/', '.'));
                            cmd.Parameters.AddWithValue("@group", comboBox2.SelectedValue);

                            if (path != "") { cmd.Parameters.AddWithValue("@archive", path); }
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Добавлен объект");
                            DataSet ds = dL.get("select * from appliances");
                            if (ds.Tables[0].Rows.Count == 0)
                                lastid = 1;
                            else
                            {
                                SQLiteCommand lastcomm = new SQLiteCommand("select last_insert_rowid() as idobjects from objects", conn);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                System.Object temp = lastcomm.ExecuteScalar();
                                lastid = int.Parse(temp.ToString());
                            }

                            dateTimePicker1.Value = DateTime.Today;
                            folderBrowserDialog1.SelectedPath = null;
                        }



                        SQLiteCommand cmd2 = new SQLiteCommand("insert into appliances(nameapp,dates,object,changeDate,userCreate) values(@nameapp,@date," + ((panel2.Visible == true) ? lastid : comboBox1.SelectedValue) + ",date('now'),@userCreate)", conn);

                        foreach (ListViewItem item in listView2.Items)
                        {
                            appliancesNameList.Add(item.SubItems[0].Text);
                        }

                        foreach (ListViewItem item in listView2.Items)
                        {
                            appliancesDateList.Add(item.SubItems[1].Text);
                        }

                        for (int i = 0; i <= listView2.Items.Count - 1; i++)
                        {
                            cmd2.Parameters.Clear();
                            cmd2.Parameters.AddWithValue("@nameapp", appliancesNameList[i]);
                            cmd2.Parameters.AddWithValue("@date", Convert.ToDateTime(appliancesDateList[i]));
                            cmd2.Parameters.AddWithValue("@userCreate", Environment.UserName);
                            conn.Open();
                            cmd2.ExecuteNonQuery();
                            conn.Close();
                        }
                        MessageBox.Show("Оборудование привязано");
                        objectName.Text = "";
                        objectAddress.Text = "";
                        applianceText.Text = "";
                        listView2.Items.Clear();
                        comboBox2.SelectedIndex = 0;
                        dateTimePicker1.Value = DateTime.Today.AddMonths(1);
                        appliancesDateList = new List<string>();
                        appliancesNameList = new List<string>();
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("В пункте \"Оповестить о невыполнении за:\" введите число");
                }

            }
            else { MessageBox.Show("Не все поля заполнены"); }
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


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (applianceText.Text != "")
            {
                ListViewItem item = new ListViewItem { Text = applianceText.Text.ToString() };// создаём первый уровень равный первой колонке applianceText.Text.ToString());
                item.SubItems.Add(dateTimePicker1.Value.ToString());
                listView2.Items.Add(item);
                applianceText.Text = "";
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count == 1)
            {
                int index = listView2.SelectedItems[0].Index;
                listView2.Items[index].Remove();
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.Items.Count > 0 && listView2.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in listView2.SelectedItems)
                {
                    dateTimePicker1.Value = Convert.ToDateTime(item.SubItems[1].Text);
                }
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            panel2.Visible = true;
            label2.Visible = false;
            comboBox1.Visible = false;
            linkLabel2.Visible = false;
            button.Text = "Добавить объект и привязать оборудование";
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                linkLabel3.Text = "Изменить папку с файлами";
            }
        }

        private void linkLabel3_MouseHover(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.SelectedPath != "") toolTip1.SetToolTip(linkLabel3, folderBrowserDialog1.SelectedPath);
        }
    }
}

//работа с double-click по listview
/* if (positionsListView.SelectedItems.Count == 1)
        {
            ListView.SelectedListViewItemCollection items = positionsListView.SelectedItems;

            ListViewItem lvItem = items[0];
            string what = lvItem.Text;

        }
*/