using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestWorker
{
    public class server
    {
        string Server = Properties.Settings.Default.server;
        string user = Properties.Settings.Default.user;
        string password = Properties.Settings.Default.password;
        string port = Properties.Settings.Default.port;
        string database = Properties.Settings.Default.database;
        string endConn = Properties.Settings.Default.endConn;
        public string get()
        {
            return "server=" + Server + ";user=" + user + ";password=" + password + ";port=" + port + ";database=" + database + ";" + endConn;
        }

        public List<string> getServer()
        {
            List<string> myList = Server.Split('.').ToList();
            return myList;
        }     
        public List<string> getList()
        {
            List<string> list = new List<string>();
            list.Add(user);
            list.Add(password);
            list.Add(port);
            list.Add(database);
            return list;
        }
    }
}
