using ARMMordanizerService.Model;
using Aspose.Cells;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public class FileParser
    {
        private readonly string _armFilePath = @"" + ConfigurationManager.AppSettings["armFilePath"];
        private readonly string _armFileCompletePath = @"" + ConfigurationManager.AppSettings["armFileCompletePath"];
        //private readonly string _dbName =  ConfigurationManager.AppSettings["dbName"];

        private IArmService _iArmService;
        private IArmRepository _iArmRepo;

        public Dictionary<string,Stream> FileRead()
        {
            var files =  File.ReadAllBytes(_armFilePath);

            var streamList= new Dictionary<string,Stream>();  
            foreach (string txtName in Directory.GetFiles(_armFilePath))
            {
                streamList.Add(txtName, new StreamReader(txtName).BaseStream);
            }
            return  streamList;
        }
        private static readonly object Mylock = new object();
        public void FileParse(object sender, System.Timers.ElapsedEventArgs e)
        {
            var stringData = FileRead();

            foreach(var file in stringData)
            {
                string isValid = _iArmService.IsValidFile(_armFilePath + file.Key);
                if (isValid == "" || isValid == string.Empty)
                {
                    DataTable dt = GetFileData(file.Key,file.Value);
                   
                    int isExists = _iArmRepo.CheckTableExists(file.Key);
                    if (isExists == 1)
                    {
                        _iArmRepo.TruncateTable(file.Key);
                        _iArmRepo.AddBulkData(dt,file.Key);
                        createFileStore(file);
                    }
                    else
                    {
                        string createTableSQL = BuildCreateTableScript(dt, file.Key);
                        _iArmRepo.SchemeCreate(createTableSQL);
                        _iArmRepo.AddBulkData(dt,file.Key);
                    }
                }
            }
          
        }
        public void FileParse()
        {
            var stringData = FileRead();

            foreach (var file in stringData)
            {
                string isValid = _iArmService.IsValidFile(_armFilePath + file.Key);
                if (isValid == "" || isValid == string.Empty)
                {
                    DataTable dt = GetFileData(file.Key, file.Value);

                    int isExists = _iArmRepo.CheckTableExists(file.Key);
                    if (isExists == 1)
                    {
                        _iArmRepo.TruncateTable(file.Key);
                        _iArmRepo.AddBulkData(dt, file.Key);
                        createFileStore(file);
                    }
                    else
                    {
                        string createTableSQL = BuildCreateTableScript(dt, file.Key);
                        _iArmRepo.SchemeCreate(createTableSQL);
                        _iArmRepo.AddBulkData(dt, file.Key);
                    }
                }
            }

        }

        private void createFileStore(KeyValuePair<string, Stream> file)
        {
            FileStore xFile = new FileStore
            {
                FileName = file.Key,
                ExecutionTime = DateTime.Now,
                Status = true
            } ;
            _iArmRepo.SaveFile(xFile);
        }

        public static string BuildCreateTableScript(DataTable Table,string tableName)
        {
            
            StringBuilder result = new StringBuilder();
            

            result.AppendFormat("CREATE TABLE [{0}] ( ",tableName);

            result.AppendFormat("[{0}] {1} {2} {3} {4}",
                    "ID", // 0
                    "[INT] ", // 1
                    "IDENTITY(1,1)",//2
                    "NOT NULL", // 3
                    Environment.NewLine // 4
                );
            result.Append("   ,");
            bool FirstTime = true;
            foreach (DataColumn column in Table.Columns.OfType<DataColumn>())
            {
                if (FirstTime) FirstTime = false;
                else
                    result.Append("   ,");

                result.AppendFormat("[{0}] {1} {2} {3}",
                    column.ColumnName, // 0
                    GetSQLTypeAsString(column.DataType), // 1
                    column.AllowDBNull ? "NULL" : "NOT NULL", // 2
                    Environment.NewLine // 3
                );
            }
            result.AppendFormat(") ON [PRIMARY]{0}", Environment.NewLine);

            // Build an ALTER TABLE script that adds keys to a table that already exists.
            if (Table.PrimaryKey.Length > 0)
                result.Append(BuildKeysScript(Table));

            return result.ToString();
        }

        /// <summary>
        /// Builds an ALTER TABLE script that adds a primary or composite key to a table that already exists.
        /// </summary>
        private static string BuildKeysScript(DataTable Table)
        {
            // Already checked by public method CreateTable. Un-comment if making the method public
            // if (Helper.IsValidDatatable(Table, IgnoreZeroRows: true)) return string.Empty;
            if (Table.PrimaryKey.Length < 1) return string.Empty;

            StringBuilder result = new StringBuilder();

            if (Table.PrimaryKey.Length == 1)
                result.AppendFormat("ALTER TABLE {1}{0}   ADD PRIMARY KEY ({2}){0}GO{0}{0}", Environment.NewLine, Table.TableName, Table.PrimaryKey[0].ColumnName);
            else
            {
                List<string> compositeKeys = Table.PrimaryKey.OfType<DataColumn>().Select(dc => dc.ColumnName).ToList();
                string keyName = compositeKeys.Aggregate((a, b) => a + b);
                string keys = compositeKeys.Aggregate((a, b) => string.Format("{0}, {1}", a, b));
                result.AppendFormat("ALTER TABLE {1}{0}ADD CONSTRAINT pk_{3} PRIMARY KEY ({2}){0}GO{0}{0}", Environment.NewLine, Table.TableName, keys, keyName);
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the SQL data type equivalent, as a string for use in SQL script generation methods.
        /// </summary>
        private static string GetSQLTypeAsString(Type DataType)
        {
            switch (DataType.Name)
            {
                case "Boolean": return "[bit]";
                case "Char": return "[char]";
                case "SByte": return "[tinyint]";
                case "Int16": return "[smallint]";
                case "Int32": return "[int]";
                case "Int64": return "[bigint]";
                case "Byte": return "[tinyint] UNSIGNED";
                case "UInt16": return "[smallint] UNSIGNED";
                case "UInt32": return "[int] UNSIGNED";
                case "UInt64": return "[bigint] UNSIGNED";
                case "Single": return "[float]";
                case "Double": return "[double]";
                case "Decimal": return "[decimal]";
                case "DateTime": return "[datetime]";
                case "Guid": return "[uniqueidentifier]";
                case "Object": return "[variant]";
                case "String": return "[nvarchar](250)";
                default: return "[nvarchar](MAX)";
            }
        }

        private DataTable GetFileData(string key, Stream value)
        {
            DataTable dt;
            using (var package = new ExcelPackage(value))
            {

                Workbook workbook = new Workbook(value);
                Worksheet worksheet = workbook.Worksheets[0];
                //worksheet
                dt = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxDataRow + 1, worksheet.Cells.MaxDataColumn + 1, true);
                return dt;

            }
        }
    }
}
