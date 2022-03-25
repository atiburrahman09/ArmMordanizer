using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{


    public class ArmRepository : IArmRepository
    {
        private ConnectionDb _connectionDB;
        private string temTableNamePrefix = "TEMP_RAW_";
        //private readonly string _dbName = ConfigurationManager.AppSettings["dbName"];

        public ArmRepository()
        {
            _connectionDB = new ConnectionDb();
        }

        public int AddBulkData(DataTable dt, string tableName)
        {
            try
            {
                _connectionDB.con.Open();
                using (var Tra = _connectionDB.con.BeginTransaction())
                {

                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_connectionDB.con))
                    {
                        //Set the database table name.  
                        sqlBulkCopy.DestinationTableName = temTableNamePrefix + tableName;
                        //con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        //con.Close();
                    }


                    Tra.Commit();
                }
                _connectionDB.con.Close();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        public int CheckTableExists(string Tablename)
        {
            string strCmd = null;
            SqlCommand sqlCmd = null;

            try
            {
                //strCmd = "select case when exists((select '['+SCHEMA_NAME(schema_id)+'].['+name+']' As name FROM [" + _dbName + "].sys.tables WHERE name = '" + Tablename + "')) then 1 else 0 end";


                SqlCommand check_Table_Exists = new SqlCommand("SELECT COUNT(*) FROM [FileStore] WHERE ([FileName] = @TableName)", _connectionDB.con);
                check_Table_Exists.Parameters.AddWithValue("@TableName", Tablename);
                int UserExist = (int)check_Table_Exists.ExecuteScalar();
                if (UserExist > 0)
                    return 1;
                else return 0;

            }
            catch { return 0; }
        }

        public int SaveFile(FileStore file)
        {
            string query = "INSERT INTO FileStore (FileName, ExecutionTime, Status) " +
                   "VALUES (@FileName, @ExecutionTime, @Status) ";

            // create connection and command
            //using (SqlConnection cn = new SqlConnection(connectionString))
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    // define parameters and their values
                    //cmd.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = file.FileName;
                    //cmd.Parameters.Add("@ExecutionTime", SqlDbType.DateTime, 50).Value = file.ExecutionTime;
                    //cmd.Parameters.Add("@Status", SqlDbType.Bit, 50).Value = file.Status;
                    cmd.Parameters.AddWithValue("@FileName", file.FileName);
                    cmd.Parameters.AddWithValue("@ExecutionTime", file.ExecutionTime);
                    cmd.Parameters.AddWithValue("@Status", file.Status);


                    // open connection, execute INSERT, close connection
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex) { return 0; }
        }

        public int SchemeCreate(string schema)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(schema, _connectionDB.con))
                {

                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex) { return 0; }
        }

        public int TruncateTable(string TableName)
        {
            string query = "TRUNCATE TABLE @TableName  ";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    //cmd.Parameters.Add("@TableName", SqlDbType.DateTime, 50).Value = TableName;
                    cmd.Parameters.AddWithValue("@TableName", TableName);


                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex) { return 0; }
        }
    }
}
