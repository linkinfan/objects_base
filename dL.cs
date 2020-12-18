using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace BestWorker
{

    class dL
    {
        server server = new server();
        MySqlConnection conn;

        public DataSet get(string adapter)
        {
            conn = new MySqlConnection(server.get());
            using (conn)
            {
                DataSet ds = new DataSet();
                if (conn.State == ConnectionState.Closed)
                {
                    MySqlDataAdapter MyAdapter = new MySqlDataAdapter(adapter, conn);
                    conn.Open();
                    MyAdapter.Fill(ds);
                    conn.Close();
                }
                return ds;
            }
        }

        public List<string> listByDataReader(MySqlCommand command, string whatReturn)
        {
            List<string> innerList = new List<string>();
            conn = new MySqlConnection(server.get());
            using (conn)
            {
                conn.Open();

                MySqlDataReader read = command.ExecuteReader();

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
