using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ARMMordanizerService.Model.Keys;

namespace ARMMordanizerService
{


    public class ArmRepository : IArmRepository
    {
        private ConnectionDb _connectionDB;
        private readonly ILogger _logger;
        private string temTableNamePrefix1 = "TMP_RAW_";
        private string temTableNamePrefix2 = "TMP_";
        private string UploadTimeInterval = "";
        private string UploadQueue = "";
        private string UploadCompletePath = "";
        private string UploadLogFile = "";
        public ArmRepository()
        {
            _logger = Logger.GetInstance;

            _connectionDB = new ConnectionDb();
            UploadLogFile = GetFileLocation(3);
        }

        public int AddBulkData(DataTable dt, string tableName)
        {
            try
            {
                DataTable dtSource = new DataTable();
                string sourceTableQuery = "Select top 1 * from " + temTableNamePrefix1 + tableName;
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtSource);
                    }
                }
                using (SqlBulkCopy bulk = new SqlBulkCopy(_connectionDB.con, SqlBulkCopyOptions.KeepIdentity, null) { DestinationTableName = temTableNamePrefix1 + tableName, BulkCopyTimeout = 0 })
                {

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string destinationColumnName = dt.Columns[i].ToString();

                        // check if destination column exists in source table 
                        // Contains method is not case sensitive    
                        if (dtSource.Columns.Contains(destinationColumnName))
                        {
                            //Once column matched get its index
                            int sourceColumnIndex = dtSource.Columns.IndexOf(destinationColumnName);

                            string sourceColumnName = dtSource.Columns[sourceColumnIndex].ToString();

                            // give column name of source table rather then destination table 
                            // so that it would avoid case sensitivity
                            bulk.ColumnMappings.Add(sourceColumnName, sourceColumnName);
                        }
                    }
                    _connectionDB.con.Open();
                    bulk.WriteToServer(dt);
                    _connectionDB.con.Close();
                }
                //using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_connectionDB.con))
                //{
                //    //Set the database table name.  
                //    sqlBulkCopy.DestinationTableName = temTableNamePrefix + tableName;
                //    sqlBulkCopy.BulkCopyTimeout = 0;
                //    _connectionDB.con.Open();
                //    sqlBulkCopy.WriteToServer(dt);
                //    _connectionDB.con.Close();
                //}

                return 1;
            }
            catch (Exception ex)
            {
                _logger.Log("AddBulkData Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }


        }

        public int CheckTableExists(string Tablename)
        {
            int tableExist;
            string query = "SELECT COUNT(*) FROM [FileStore] WHERE [FileName] = @TableName";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@TableName", Tablename);

                    _connectionDB.con.Open();
                    tableExist = (int)cmd.ExecuteScalar();
                    _connectionDB.con.Close();
                }
                return tableExist;
            }
            catch (Exception ex)
            {
                _logger.Log("CheckTableExists Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }

        }

        public int SaveFile(FileStore file)
        {
            string query = "INSERT INTO FileStore (FileName, ExecutionTime, Status) " +
                   "VALUES (@FileName, @ExecutionTime, @Status) ";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@FileName", file.FileName);
                    cmd.Parameters.AddWithValue("@ExecutionTime", file.ExecutionTime);
                    cmd.Parameters.AddWithValue("@Status", file.Status);


                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                _logger.Log("SaveFile Exception: " + ex.Message,UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }
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
            catch (Exception ex)
            {
                _logger.Log("SchemeCreate Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }
        }

        public int TruncateTable(string TableName,string tablePrefix)
        {
            string tableName = tablePrefix + TableName;
            //string query = "truncate table @TableName";
            string strTruncateTable = "TRUNCATE TABLE " + tableName;


            try
            {
                using (SqlCommand cmd = new SqlCommand(strTruncateTable, _connectionDB.con))
                {
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                _logger.Log("TruncateTable Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }
        }

        public string GetFileLocation(int Key)
        {
            //use condition for key and set property 
            string propertyName = "";
            if (Key == 0)
            {
                propertyName = Enum.GetName(typeof(KeyNames), 0);
            }

            else if (Key == 1)
            {
                propertyName = Enum.GetName(typeof(KeyNames), 1);
            }
            else if (Key == 2)
            {
                propertyName = Enum.GetName(typeof(KeyNames), 2);
            }
            else if (Key == 3)
            {
                propertyName = Enum.GetName(typeof(KeyNames), 3);
                
            }


            string location = "";
            string sourceTableQuery = "Select PropertyValue from [SystemGlobalProperties] WHERE [PropertyName] = @propertyName";
            using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
            {
                cmd.Parameters.AddWithValue("@propertyName", propertyName);
                _connectionDB.con.Open();
                var dr = cmd.ExecuteReader();
                if (dr.Read()) // Read() returns TRUE if there are records to read, or FALSE if there is nothing
                {
                    location = dr["PropertyValue"].ToString();
                   
                    //string[] tempValue = location.Split('\\');
                    ////tempValue = tempValue.Take(tempValue.Count() - 1).ToArray();
                    //tempValue = tempValue.Reverse().Skip(1).Reverse().ToArray();
                    //foreach (var obj in tempValue)
                    //{
                    //    location = obj + "\\";
                    //}

                }
                _connectionDB.con.Close();

            }
            return location;
        }

        public string GetSqlFromMappingConfig(string key)
        {
            try
            {

                string sql = "";
                string query = "Select SQL from [MapperConfiguration] WHERE [SourceTable] = @sourceTable AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(query, _connectionDB.con))
                {
                    cmd.Parameters.AddWithValue("@sourceTable", "dbo."+ temTableNamePrefix1 + key);
                    _connectionDB.con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read()) // Read() returns TRUE if there are records to read, or FALSE if there is nothing
                    {
                        sql = dr["SQL"].ToString();

                    }
                    _connectionDB.con.Close();

                }
                return sql;
            }
            catch (Exception ex)
            {
                _logger.Log("GetSqlFromMappingConfig Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

                return "";
            }
            
        }

        public int InsertDestinationTable(string insertSql)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(insertSql, _connectionDB.con))
                {
                    _connectionDB.con.Open();
                    cmd.ExecuteNonQuery();
                    _connectionDB.con.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {
                _logger.Log("InsertDestinationTable Exception: " + ex.Message, UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));
                //throw ex;
                return -1;
            }
        }
    }
}
