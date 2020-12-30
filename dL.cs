using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;

namespace BestWorker
{

    class dL
    {
        SQLiteConnection conn;

        public string GetServ()
        {
            //MessageBox.Show($"Data Source=" + Properties.Settings.Default.server + ";Version=3;");
            return $"Data Source=" + Properties.Settings.Default.server + ";Version=3;";
        }
        public DataSet get(string adapter)
        {
            conn = new SQLiteConnection(GetServ());
            using (conn)
            {
                DataSet ds = new DataSet();
                if (conn.State == ConnectionState.Closed)
                {
                    SQLiteDataAdapter MyAdapter = new SQLiteDataAdapter(adapter, conn);
                    conn.Open();

                    MyAdapter.AcceptChangesDuringFill = true;
                    MyAdapter.Fill(ds);
                    conn.Close();
                }
                return ds;
            }
        }

        public List<string> listByDataReader(SQLiteCommand command, string whatReturn)
        {
            List<string> innerList = new List<string>();
            conn = new SQLiteConnection(GetServ());
            using (conn)
            {
                conn.Open();

                SQLiteDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        innerList.Add(read[whatReturn].ToString());
                    }
                }
                conn.Close();
            }
            return innerList;
        }

    }
}
