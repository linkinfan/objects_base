using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace BestWorker
{
    public partial class appliances : Form
    {
        dL dL = new dL();
        server server = new server();
        MySqlConnection conn;
        private BindingSource bindingSource1 = new BindingSource();
        private DataTable table = new DataTable();
        DataGridView.HitTestInfo hti;
        ContextMenu m = new ContextMenu();
        string query = "SELECT idappliances, o.idobjects, o.name as 'Имя объекта', og.nameGroup as 'Группа', o.address as 'Адрес', nameapp as 'Название оборудования', if(date_add(date, interval -30 day) > now(),'ПОВЕРКА НЕ ТРЕБУЕТСЯ', concat(datediff(date,now()), ' дней')) as 'До окончания поверки осталось', DATE_FORMAT(date, '%d-%m-%Y') as 'Дата следующей поверки' FROM appliances a inner join objects o on o.idobjects=a.object inner join objectgroup og on og.idobjectgroup=o.groupOb ";
        public appliances()
        {
            InitializeComponent();
            conn = new MySqlConnection(server.get());
            m.MenuItems.Add(new MenuItem("Изменить", MenuClick));
            m.MenuItems.Add(new MenuItem("Удалить", MenuClick));
            //Clipboard.SetText(query + "order by date;");
            table = dL.get(query + "order by date;").Tables[0];
            bindingSource1.DataSource = table;
            loadGroups();
            loadObjects(bindingSource1);
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
        void loadObjects(BindingSource datas)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = datas;
            dataGridView1.Columns[6].DefaultCellStyle.ForeColor = Color.Red;
            datagridSize();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[6].Value.ToString().Contains("ПОВЕРКА НЕ ТРЕБУЕТСЯ"))
                {
                    dataGridView1.Rows[i].Cells[6].Style.ForeColor = Color.Black;
                }
            }
            label1.Text = "Всего" + dataGridView1.Rows.Count.ToString() + " записей";
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

        BindingSource searchDataLoad()
        {
            string search = textBox1.Text.ToLower();
            string q = "";
            string whereGroupQuery = "where o.groupOb='" + comboBox1.SelectedValue.ToString() + "' ";
            bool searchText = textBox1.Text != "" && textBox1.Text != "Искать по объекту или адресу...";
            bool groupSearch = comboBox1.SelectedValue.ToString() != "1";
            if (comboBox1.Visible == false)
            {
                q = query + (searchText ? "where (o.name like LOWER('%" + search + "%') or o.address like LOWER('%" + search + "%')) " : "") + "order by date;";
                bindingSource1.DataSource = dL.get(q).Tables[0];
            }
            else if (comboBox1.Visible == true)
            {
                q = query + (groupSearch ? whereGroupQuery : "") + (searchText ? (groupSearch ? " and " : "where ") + "(o.name like LOWER('%" + search + "%') or o.address like LOWER('%" + search + "%')) " : "") + "order by date;";

                bindingSource1.DataSource = dL.get(q).Tables[0];
            }
            //Clipboard.SetText(q);
            return bindingSource1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBox1.SelectedValue.ToString());
            loadObjects(searchDataLoad());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            comboBox1.Visible = false;
            textBox1.Text = "";
            linkLabel1.Visible = false;
            loadObjects(searchDataLoad());            
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
            datagridSize();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            comboBox1.Visible = true;
            linkLabel1.Visible = true;
            loadObjects(searchDataLoad());
        }


        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            loadObjects(searchDataLoad());
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
                                MySqlCommand cmd = new MySqlCommand("delete from appliances where idappliances='" + appid + "';", conn);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                                MessageBox.Show("Прибор <" + appname + "> удален");
                            }

                        loadObjects(searchDataLoad());

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

                    loadObjects(searchDataLoad());

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
                loadObjects(searchDataLoad());

                dataGridView1.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            addSnapApp add = new addSnapApp();
            add.ShowDialog();
        }
    }
}
