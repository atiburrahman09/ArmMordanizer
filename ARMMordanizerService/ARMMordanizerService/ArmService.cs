using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMMordanizerService
{
    public class ArmService:IArmService
    {
        private readonly IArmRepository _armRepository;

        public ArmService(string connectionString)
        {
            _armRepository = new ArmRepository();
        }
        public string IsValidFile(string physicalFile)
        {
            if (!File.Exists(physicalFile))
                return "File not found";
            var file = new FileInfo(physicalFile);
            if (file.Extension != ".CSV" || file.Extension != ".XSLX")
                return "Invalid file";
            return "";
        }
    }
}
