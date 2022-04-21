﻿using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{


    public class ArmRepository : IArmRepository
    {
        private ConnectionDb _connectionDB;
        private readonly ILogger _logger;
        private string temTableNamePrefix = "TMP_RAW_";

        public ArmRepository()
        {
            _logger = Logger.GetInstance;

            _connectionDB = new ConnectionDb();
        }

        public int AddBulkData(DataTable dt, string tableName)
        {
            try
            {
                DataTable dtSource = new DataTable();
                string sourceTableQuery = "Select top 1 * from " + temTableNamePrefix + tableName;
                using (SqlCommand cmd = new SqlCommand(sourceTableQuery, _connectionDB.con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtSource);
                    }
                }
                using (SqlBulkCopy bulk = new SqlBulkCopy(_connectionDB.con, SqlBulkCopyOptions.KeepIdentity, null) { DestinationTableName = temTableNamePrefix + tableName, BulkCopyTimeout = 0 })
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
                _logger.Log("AddBulkData Exception: " + ex.Message);
                throw ex;
                //return 0;
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
                _logger.Log("CheckTableExists Exception: " + ex.Message);
                throw ex;
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
                _logger.Log("SaveFile Exception: " + ex.Message);
                throw ex;
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
                _logger.Log("SchemeCreate Exception: " + ex.Message);
                throw ex;
            }
        }

        public int TruncateTable(string TableName)
        {
            //string query = "truncate table @TableName";
            string tableName = temTableNamePrefix + TableName;

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
                _logger.Log("TruncateTable Exception: " + ex.Message);
                throw ex;
            }
        }
    }
}
