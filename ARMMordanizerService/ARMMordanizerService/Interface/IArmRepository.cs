﻿using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public interface IArmRepository
    {
        int SchemeCreate(string schema);
        int AddBulkData(DataTable dt,string tableName);
        int SaveFile(FileStore file);
        int CheckTableExists(string Tablename);
        int TruncateTable(string TableName);
    }
}