using ARMMordanizerService.Model;
using Aspose.Cells;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public class FileParser
    {
        private readonly string _armFilePath = @"" + ConfigurationManager.AppSettings["armFilePath"];
        private readonly string _armFileCompletePath = @"" + ConfigurationManager.AppSettings["armFileCompletePath"];
        private string temTableNamePrefix = "TEMP_RAW_";

        private readonly IArmService _iArmService;
        private IArmRepository _iArmRepo;
        private readonly ILogger _logger;

        public FileParser()
        {
            _iArmService = new ArmService();
            _iArmRepo = new ArmRepository();
            _logger = Logger.GetInstance;
        }
        public Dictionary<string, Stream> FileRead()
        {
            var streamList = new Dictionary<string, Stream>();
            foreach (string txtName in Directory.GetFiles(_armFilePath))
            {
                //StreamReader x = new StreamReader(txtName);
                streamList.Add(Path.GetFileName(txtName), new StreamReader(txtName).BaseStream);
                //x.Close();
            }
            return streamList;
        }
        private static readonly object Mylock = new object();
        public void FileParse(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(Mylock, 0)) return;

            var stringData = FileRead();

            foreach (var file in stringData)
            {
                string path = _armFilePath + file.Key;
                string isValid = _iArmService.IsValidFile(path);
                if (isValid == "" || isValid == string.Empty)
                {
                    DataTable dt = GetFileData(file.Key, file.Value);

                    int isExists = _iArmRepo.CheckTableExists(Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                    if (isExists == 1)
                    {
                        _iArmRepo.TruncateTable(Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                        _iArmRepo.AddBulkData(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                        createFileStore(file);
                    }
                    else
                    {
                        string createTableSQL = BuildCreateTableScript(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key), temTableNamePrefix);
                        _iArmRepo.SchemeCreate(createTableSQL);
                        _iArmRepo.AddBulkData(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                    }
                }
            }
            
            RemoveFilesFromFolder(stringData);
            DeleteFilesFromFolder();

        }
        public void FileParse()
        {
            var stringData = FileRead();

            foreach (var file in stringData)
            {
                string path = _armFilePath + file.Key;
                string isValid = _iArmService.IsValidFile(path);
                if (isValid == "" || isValid == string.Empty)
                {
                    DataTable dt = GetFileData(file.Key, file.Value);

                    int isExists = _iArmRepo.CheckTableExists(Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                    if (isExists == 1)
                    {
                        _iArmRepo.TruncateTable(Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                        _iArmRepo.AddBulkData(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                        createFileStore(file);
                    }
                    else
                    {
                        string createTableSQL = BuildCreateTableScript(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key), temTableNamePrefix);
                        _iArmRepo.SchemeCreate(createTableSQL);
                        _iArmRepo.AddBulkData(dt, Path.GetFileNameWithoutExtension(_armFilePath + file.Key));
                        createFileStore(file);

                    }
                }
            }
            RemoveFilesFromFolder(stringData);
            DeleteFilesFromFolder();

        }

        private void DeleteFilesFromFolder()
        {
            bool errors = false;
            DirectoryInfo dir = new DirectoryInfo(_armFilePath);

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                try
                {

                    FileSecurity fs = File.GetAccessControl(_armFilePath);
                    fi.IsReadOnly = false;
                    fi.Delete();

                    //Wait for the item to disapear (avoid 'dir not empty' error).
                    while (fi.Exists)
                    {
                        System.Threading.Thread.Sleep(10);
                        fi.Refresh();
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.Message);
                    errors = true;
                }
            }

        }

        private void RemoveFilesFromFolder(Dictionary<string, Stream> stringData)
        {

            string[] fileList = System.IO.Directory.GetFiles(_armFilePath);
            foreach (string file in fileList)
            {
                string fileToMove = _armFilePath + Path.GetFileName(file);
                string moveTo = _armFileCompletePath + Path.GetFileNameWithoutExtension(file) + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file);
                //moving file
                File.Copy(fileToMove, moveTo);
            }
        }

        private void createFileStore(KeyValuePair<string, Stream> file)
        {
            FileStore xFile = new FileStore
            {
                FileName = file.Key,
                ExecutionTime = DateTime.Now,
                Status = true
            };
            _iArmRepo.SaveFile(xFile);
        }

        public static string BuildCreateTableScript(DataTable Table, string tableName,string temTableNamePrefix)
        {

            StringBuilder result = new StringBuilder();


            result.AppendFormat("CREATE TABLE [{0}] ( ", temTableNamePrefix + tableName);

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
                value.Close();
                return dt;

            }
        }
    }
}
