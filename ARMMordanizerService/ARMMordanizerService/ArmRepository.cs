using ARMMordanizerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    

    public class ArmRepository:IArmRepository
    {
        private ConnectionDb _connectionDB;
        public ArmRepository()
        {
            _connectionDB = new ConnectionDb();
        }

        public int AddBulkData(object Data)
        {
            _connectionDB.con.Open();
            using ( var Tra = _connectionDB.con.BeginTransaction())
            {


                Tra.Commit();
            }
                throw new NotImplementedException();
        }

        public int CheckTableExists(string Tablename)
        {
            throw new NotImplementedException();
        }

        public int SaveFile(FileStore file)
        {
            throw new NotImplementedException();
        }

        public int SchemeCreate(string schema)
        {
            throw new NotImplementedException();
        }
    }
}
