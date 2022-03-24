using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public interface IArmRepository
    {
        int SchemeCreate(string schema);
        int AddBulkData(object Data);
        int SaveFile(FileStore file);
        int CheckTableExists(string Tablename);
    }
}
