using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public class ConnectionDb
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ArmConnection"].ConnectionString;
        public SqlConnection con;
        public ConnectionDb()
        {
            con=new SqlConnection(_connectionString);
            //if(con.State == System.Data.ConnectionState.Open)
            //{
            //    con.Close();
            //}
        }
    }
}
