using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;

namespace BestWorker
{
    public partial class appliances : Form
    {
        dL dL = new dL();
        SQLiteConnection conn;
        private BindingSource bindingSource1 = new BindingSource();
        private DataTable table = new DataTable();
        DataGridView.HitTestInfo hti;
        ContextMenu m = new ContextMenu();
        string query = "SELECT idappliances, o.idobjects, o.name as 'Имя объекта', og.nameGroup as 'Группа', o.address as 'Адрес', nameapp as 'Название оборудования', dates as 'До окончания поверки осталось', strftime('%d-%m-%Y', dates) as 'Дата следующей поверки', del  FROM appliances a inner join objects o on o.idobjects=a.object inner join objectgroup og on og.idobjectgroup=o.groupOb ";
        public appliances()
        {
            InitializeComponent();
            //string s = "";
            //if (File.Exists(s))
            //{

            //}
            conn = new SQLiteConnection(dL.GetServ());

            
            try
            {
                using (conn)
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                        conn.Close();
                    }
            }
            catch (SQLiteException ex) { MessageBox.Show(ex.Message); dbnotfound db = new dbnotfound(); db.ShowDialog(); }

            m.MenuItems.Add(new MenuItem("Изменить", MenuClick));
            m.MenuItems.Add(new MenuItem("Удалить", MenuClick));
            //Clipboard.SetText(query + "order by date;");
            table = dL.get(query + "order by dates;").Tables[0];
            bindingSource1.DataSource = table;
            loadGroups();
            LoadObjects(DataLoad());
            textBox1.Text = "1";
            textBox1.Clear();

            
        }

        void loadGroups()
        {
            comboBox1.DataSource = null;
            DataSet ds = dL.get("select * from objectgroup order by nameGroup");
            comboBox1.DataSource = ds.Tables[0];
            comboBox1.ValueMember = "idobjectgroup";
            comboBox1.DisplayMember = "nameGroup";
        }

        void datagridSize()
        {
            dataGridView1.Columns[0].Width = dataGridView1.Width / 30;
            dataGridView1.Columns[1].Width = dataGridView1.Width / 30;
            dataGridView1.Columns[2].Width = dataGridView1.Width / 6;
            dataGridView1.Columns[3].Width = dataGridView1.Width / 10;
            dataGridView1.Columns[4].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[5].Width = dataGridView1.Width / 5;
            dataGridView1.Columns[6].Width = dataGridView1.Width / 7;
            dataGridView1.Columns[7].Width = dataGridView1.Width - (dataGridView1.Width / 30 + dataGridView1.Width / 30 + dataGridView1.Width / 6 + dataGridView1.Width / 10 + dataGridView1.Width / 4 + dataGridView1.Width / 5 + dataGridView1.Width / 7) - 20;
        }
        void LoadObjects(BindingSource datas)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = datas;
            dataGridView1.Columns[8].Visible = false;
            if (dataGridView1.Columns["search"] != null) dataGridView1.Columns["search"].Visible = false;
            datagridSize();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[8].Value.ToString() == "1")
                {
                    dataGridView1.Rows.Remove(row);
                }
                if (row.Cells[6].Value != DBNull.Value)
                {

                    DateTime val = Convert.ToDateTime(row.Cells[6].Value).AddDays(-30);
                    DateTime date = Convert.ToDateTime(row.Cells[6].Value);
                    if (val > DateTime.Now)
                    {
                        row.Cells[6].Value = "ПОВЕРКА НЕ ТРЕБУЕТСЯ";
                    }
                    if (val < DateTime.Now)
                    {
                        row.Cells[6].Style.ForeColor = Color.Red;
                        row.Cells[6].Value = ((date - DateTime.Now).Days).ToString() + " дней";
                    }
                }
            }
            label1.Text = "Всего " + dataGridView1.Rows.Count.ToString() + " записей";
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Искать по объекту или адресу...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.WindowText;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "Искать по объекту или адресу...";
                textBox1.ForeColor = SystemColors.ButtonShadow;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //костыль для поиска
            if (e.KeyCode == Keys.Back)
            {
                if (textBox1.Text.Length == 1)
                {
                    textBox1.Clear();
                }
            }
        }

        DataTable search(string q, string search)
        {
            bool searchText = textBox1.Text != "" && textBox1.Text != "Искать по объекту или адресу...";
            DataTable dt = dL.get(q + " order by dates").Tables[0];
            if (searchText)
            {
                dt.Columns.Add("search");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["search"] = dt.Rows[i][2].ToString().ToLower() + " " + dt.Rows[i][4].ToString().ToLower();
                }
                foreach (DataRow row in dt.Rows) 
                {
                    if (!row["search"].ToString().Contains(search)) 
                    {
                        row.Delete();
                    }
                }
                return dt;
            }
            else
            {
                return dt;
            }
        }
        BindingSource DataLoad()
        {
            string searchText = textBox1.Text.ToLower();
            string whereGroupQuery = "where o.groupOb='" + comboBox1.SelectedValue.ToString() + "' ";
            bool groupSearch = comboBox1.SelectedValue.ToString() != "1";
            if (comboBox1.Visible == false)
            {
                bindingSource1.DataSource = search(query, searchText);
            }
            else if (comboBox1.Visible == true)
            {
                //q = query + (groupSearch ? whereGroupQuery : "") + (searchText ? (groupSearch ? " and " : "where ") + "name like lower('%" + search + "%') or address like lower('%" + search + "%')" : "") + " order by dates;";
                //Clipboard.SetText(q);
                string q = query + (groupSearch ? whereGroupQuery : "");
                bindingSource1.DataSource = search(q, searchText);
            }
            return bindingSource1;
        }



        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            datagridSize();
        }



        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            LoadObjects(DataLoad());
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && dataGridView1.RowCount > 0)
            {
                hti = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[hti.RowIndex].Selected = true;
                m.Show(dataGridView1, new Point(e.X, e.Y));
            }
        }

        private void MenuClick(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (sender == m.MenuItems[1])
                {
                    string appname = dataGridView1.Rows[hti.RowIndex].Cells[5].Value.ToString();
                    string appid = dataGridView1.Rows[hti.RowIndex].Cells[0].Value.ToString();
                    string message = "Вы уверены, что хотите удалить прибор <" + appname + "> ?";
                    const string caption = "Подтверждение удаления прибора";
                    var result = MessageBox.Show(message, caption,
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        using (conn)
                            if (conn.State == ConnectionState.Closed)
                            {
                                SQLiteCommand cmd = new SQLiteCommand("delete from appliances where idappliances='" + appid + "';", conn);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                MessageBox.Show("Прибор <" + appname + "> удален");
                            }

                        LoadObjects(DataLoad());
                        if (hti.RowIndex == 0) dataGridView1.Rows[hti.RowIndex + 1].Selected = true;
                        else if (hti.RowIndex > 0) dataGridView1.Rows[hti.RowIndex - 1].Selected = true;
                    }


                }
                else if (sender == m.MenuItems[0])
                {
                    int position = Convert.ToInt32(dataGridView1.Rows[hti.RowIndex].Cells[0].Value.ToString());
                    Properties.Settings.Default.applianceID = position;
                    changeAppliance cha = new changeAppliance();
                    cha.ShowDialog();

                    LoadObjects(DataLoad());
                    dataGridView1.Rows[hti.RowIndex].Selected = true;
                }


            }
        }


        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dataGridView1.RowCount > 0)
            {
                hti = dataGridView1.HitTest(e.X, e.Y);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[hti.RowIndex].Selected = true;
                int position = Convert.ToInt32(dataGridView1.Rows[hti.RowIndex].Cells[0].Value.ToString());
                Properties.Settings.Default.applianceID = position;
                changeAppliance cha = new changeAppliance();
                cha.ShowDialog();
                LoadObjects(DataLoad());
                dataGridView1.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            addSnapApp add = new addSnapApp();
            add.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadObjects(DataLoad());
        }
    }
}
