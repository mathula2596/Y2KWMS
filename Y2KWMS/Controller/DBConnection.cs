using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Y2KWMS.Controller
{
    class DBConnection
    {
        public SqlConnection connection;
        private string connectionString;
        public DBConnection()
        {
            connectionString = connectionStringReader();
            try
            {
                connection = new SqlConnection(connectionString);

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public string connectionStringReader()
        {
            int counter = 0;
            foreach (string line in File.ReadLines("../../dataBase.txt"))
            {
                if(line.Length >0)
                {
                    connectionString = line;
                }
                counter++;
            }

            return connectionString;
        }
    }
}
