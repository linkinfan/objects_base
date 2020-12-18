using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;

namespace BestWorker
{
    public partial class tasksAdmin : Form
    {
        server server = new server();
        MySqlConnection conn;
        string nick = Properties.Settings.Default.nick;
        string path = Environment.CurrentDirectory + "\\screenshots\\";
        string firstname = Properties.Settings.Default.firstname;
        string surname = Properties.Settings.Default.surname;
        string patronymic = Properties.Settings.Default.patronymic;
        string query = "select * from tasks ";
        string archiveLink = "";
        Timer tm = new Timer();
        public bool addOrChange = true; //переключатель в режимов работы с объектом

        List<string> dates = new List<string>();
        dL dL = new dL();
        DataSet NoDate, mainDS = null;
        public tasksAdmin()
        {
            InitializeComponent();
            conn = new MySqlConnection(server.get());
            this.Text += " " + " " + firstname + " " + surname + " " + patronymic;
            NoDate = dL.get(query);
            dates = dateSQLConverter(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(1).AddSeconds(-1));
            mainDS = dL.get("select * from tasks where whensend between '" + dates[0] + "' and '" + dates[1] + "' and deleteT=0;");

            loadTreeView(mainDS, true, dates);
            // folderSet();            



        }
        //void folderSet()
        //{
        //    using (conn)
        //    {
        //        if (conn.State == ConnectionState.Closed)
        //        {
        //            MySqlCommand cmd = new MySqlCommand("update settings set screenshotFolder=@path where idsettings=0;", conn);
        //            cmd.Parameters.AddWithValue("@path", path);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            conn.Close();
        //        }
        //    }
        //}

        void loadTreeView(DataSet ds, bool enableDate, List<string> datesLoad)
        {
            treeView1.Nodes.Clear();
            label4.Text = "";
            if (enableDate == true)
            {
                label4.Text = "C " + Convert.ToDateTime(datesLoad[0]).ToShortDateString() + " по " + Convert.ToDateTime(datesLoad[1]).ToShortDateString();
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode node = new TreeNode();
                node.Text = row["Name"].ToString();
                node.Tag = row["idtasks"];

                node.Nodes.Add("Когда отправлено: " +
                Convert.ToDateTime(row["whensend"]));

                node.Nodes.Add("Когда начать: " + Convert.ToDateTime(row["whenstart"]).ToShortDateString());

                node.Nodes.Add("Когда закончить: " + Convert.ToDateTime(row["whenstop"]).ToShortDateString());
                treeView1.Nodes.Add(node);

            }
            if (treeView1.Nodes.Count > 0) treeView1.SelectedNode = treeView1.Nodes[0];
        }

        //конвертер даты datetime в удобоваримый для mysql формат
        List<string> dateSQLConverter(DateTime dt1, DateTime dt2)
        {
            string start = dt1.Year + "-" + (dt1.Month.ToString().Length > 1 ? dt1.Month.ToString() : "0" + dt1.Month.ToString()) + "-" + (dt1.Day.ToString().Length > 1 ? dt1.Day.ToString() : "0" + dt1.Day.ToString()) + " " + dt1.TimeOfDay;

            string end = dt2.Year + "-" + (dt2.Month.ToString().Length > 1 ? dt2.Month.ToString() : "0" + dt2.Month.ToString()) + "-" + (dt2.Day.ToString().Length > 1 ? dt2.Day.ToString() : "0" + dt2.Day.ToString()) + " " + dt2.TimeOfDay;
            return new List<string>() { start, end };
        }

        void getInfo()
        {
            listBox1.DataSource = null;
            listBox2.DataSource = null;
            DataSet info = dL.get("select note, archive, name from tasks where idtasks = '" + treeView1.SelectedNode.Tag + "' and deleteT=0");
            textBox1.Text = info.Tables[0].Rows[0][0].ToString();
            textBox3.Text = info.Tables[0].Rows[0][2].ToString();
            if (info.Tables[0].Rows[0][1].ToString() == "")
            {
                linkLabel1.Visible = false;
            }
            else
            {
                linkLabel1.Visible = true;
                archiveLink = info.Tables[0].Rows[0][1].ToString();
            }
            if (treeView1.Nodes.Count > 0)
            {
                DataSet ds1 = dL.get("SELECT idwhodo, concat(r.firstname, ' ', r.surname, ' ', patronymic) as fio from whodo w inner join regdata r on w.user = r.idregdata where task='" + treeView1.SelectedNode.Tag + "'");
                listBox1.DataSource = ds1.Tables[0];
                listBox1.ValueMember = "idwhodo";
                listBox1.DisplayMember = "fio";

                DataSet ds2 = dL.get("SELECT idwhodid, concat(r.firstname, ' ', r.surname, ' ', patronymic) as fio from whodid w inner join regdata r on w.user = r.idregdata where task='" + treeView1.SelectedNode.Tag + "'");
                listBox2.DataSource = ds2.Tables[0];
                listBox2.ValueMember = "idwhodid";
                listBox2.DisplayMember = "fio";

            }
        }
        //private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (dataGridView1.SelectedRows.Count != 0)
        //    {
        //        button1.Text = "Изменить";
        //        linkLabel1.Visible = true;
        //        linkLabel3.Visible = true;
        //        groupBox2.Text = "Изменить задачу";
        //        taskNameBox.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
        //        label3.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
        //        taskBoxText.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();

        //   }
        //}

        //кнопка архив
        /*string archive = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            if (File.Exists(archive)) Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + archive));*/


        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (monthCalendar1.Visible == false) monthCalendar1.Visible = true;
            else { monthCalendar1.Visible = false; }
        }

        private void monthCalendar1_MouseLeave(object sender, EventArgs e)
        {
            monthCalendar1.Visible = false;
        }

        //при нажатии на показать все необходимо выводить текущий месяц

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            List<string> selectDate = dateSQLConverter(monthCalendar1.SelectionStart, monthCalendar1.SelectionEnd);
            string between = "where whensend between '" + selectDate[0] + "' and '" + selectDate[1] + "';";
            string monthChanger = query + between;
            DataSet ds = dL.get(monthChanger);
            loadTreeView(ds, true, new List<string> { selectDate[0], selectDate[1] });
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadTreeView(mainDS, true, dates);
        }

        private void tasksAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            using (conn)
                if (conn.State == ConnectionState.Closed)
                {
                    MySqlCommand updateStatus = new MySqlCommand("update regdata set status=0 where idregdata=" + nick + ";", conn);
                    conn.Open();
                    updateStatus.ExecuteNonQuery();

                    reg reg = new reg(); reg.Visible = true;
                }
        }

        //Открыть архив
        //private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{

        //}


        private void tasksAdmin_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size == new System.Drawing.Size(1076, 498))
            {
                label3.Anchor = AnchorStyles.None;
                listBox1.Anchor = AnchorStyles.None;
                label3.Anchor = AnchorStyles.Bottom;
                listBox1.Anchor = AnchorStyles.Bottom;
            }
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            employees empl = new BestWorker.employees();
            empl.ShowDialog();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Parent == null) getInfo();
        }

        private void yearLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<string> datesY = dateSQLConverter(DateTime.Today.AddYears(-1), DateTime.Today.AddDays(1).AddSeconds(-1));
            DataSet ds = dL.get("select * from tasks where whensend between '" + datesY[0] + "' and '" + datesY[1] + "';");
            loadTreeView(ds, true, datesY);
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(@archiveLink))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", " /select, " + @archiveLink));
            }
            else { MessageBox.Show("Файл не может быть запущен", "Ошибка"); }
        }


        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            string search = textBox2.Text.ToLower();
            //if (e.KeyChar == '/')
            //{
            //    toolStripStatusLabel1.Text = "Для выполнения команды нажмите Enter"; //здесь я спрятал вывод уведомления о командах поиска
            //}
            //else if(!search.Contains("/"))
            //{
            //    toolStripStatusLabel1.Text = "";
            //}
            //textBox2.SelectionStart = textBox2.Text.Length;
            
            DataSet searchData = new DataSet();
            searchData = dL.get("select * from tasks t where note like LOWER('%" + search + "%') or name like LOWER('%" + search + "%')");
            loadTreeView(searchData, false, new List<string>());
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.Text = "Поиск...";
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(label1.Visible == false) label1.Visible = true;
            else if(label1.Visible == true) label1.Visible = false;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            string search = textBox2.Text.ToLower();
            DataSet searchData = new DataSet();
            if (search.Contains("/"))
            {
                MessageBox.Show("Поиск при помощи команд в данный момент не работает.");  
                //if (e.KeyCode == Keys.Enter)
                //{
                //    if (search.Contains("/отпр") || search.Contains("/начвып") || search.Contains("/оквып") || search.Contains("/вып"))
                //    {
                //        DateTime notConvertSearch = Convert.ToDateTime(search.Substring(search.IndexOf(' ') + 1, 10));
                //        //searchQuery = search.Replace('.', '-').Substring(search.IndexOf(' '), 10);
                //        string searchQuery = notConvertSearch.ToString("yyyy-MM-dd HH:mm:ss");//хорошая вещь для перевода в читаемый mysql формат

                //        if (search.Contains("/отпр "))
                //        {
                //            searchData = dL.get("select * from tasks where whensend like '%" + searchQuery + "%';");
                //        }
                //        else if (search.Contains("/начвып "))
                //        {
                //            searchData = dL.get("select * from tasks where whenstart like '%" + searchQuery + "%';");
                //        }
                //        else if (search.Contains("/оквып "))
                //        {
                //            searchData = dL.get("select * from tasks where whenstop like '%" + searchQuery + "%';");
                //        }
                //        else if (search.Contains("/вып "))
                //        {
                //            searchData = dL.get("select * from tasks t inner join sessions s on s.idtasks=t.idtasks where datetimeEnd like '%" + searchQuery + "%';");
                //        }
                //    }
                //    else if (search.Contains("/уч "))
                //    {
                //        string searchQuery = search.Substring(search.IndexOf(' '), search.Length-search.IndexOf(' '));
                //        searchData = dL.get("select t.idtasks, t.name, t.whensend, t.whenstart, t.whenstop, t.note, t.archive from work.tasks t inner join whodo w on w.task=t.idtasks inner join regdata r on r.idregdata = w.user where concat(r.firstname, ' ', substr(r.surname, 1, 1), '.', substr(r.patronymic, 1, 1), '.') like LOWER('%" + searchQuery + "%')");
                //    }

                //    else
                //    {
                //        MessageBox.Show("Такой команды не существует");
                //        textBox2.Text = "";
                //    }
                //    loadTreeView(searchData, false, new List<string>());
                //    toolStripStatusLabel1.Text = "Выполнено";
                    
                //    tm.Interval = 1000;
                //    tm.Start();
                //    tm.Tick += Tm_Tick;
                //}
            }

            //костыль для поиска
            if (e.KeyCode == Keys.Back)
            {
                if (textBox2.Text.Length == 1)
                {
                    textBox2.Clear();
                }
            }
        }

        private void Tm_Tick(object sender, EventArgs e)
        {
            tm.Stop();
            toolStripStatusLabel1.Text = "";
        }

        private void добавитьЗадачуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addTask addTask = new addTask();
            addTask.ShowDialog();
        }

        private void удалитьЗадачуToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Вы точно хотите удалить данную задачу?", "\"" + treeView1.SelectedNode.Text + "\"", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes && treeView1.Nodes.Count > 0)
            {
                using (conn)
                    if (conn.State == ConnectionState.Closed)
                    {
                        MySqlCommand cmd = new MySqlCommand("update tasks set deleteT = 1 where idtasks='" + treeView1.SelectedNode.Tag + "';", conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                loadTreeView(mainDS, true, dates);
            }
        }

        private void изменитьПриборыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addObject add = new addObject();
            add.ShowDialog();
            addOrChange = false; //режим изменения объекта
        }

        private void добавитьОбъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addObject add = new addObject();
            add.ShowDialog();
            addOrChange = true; //режим добавления объекта
        }

        private void статусОбъектовToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node != treeView1.SelectedNode)
                {
                    node.Collapse();
                }
            }
        }
    }
}
