using System;
using System.Collections.Generic;
using System.Configuration;
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

        public Dictionary<string,Stream> FileRead()
        {
            var files =  File.ReadAllBytes(_armFilePath);

            var stramList= new Dictionary<string,Stream>();  
            foreach (string txtName in Directory.GetFiles(_armFilePath))
            {
                stramList.Add(txtName, new StreamReader(txtName).BaseStream);
            }
            return  stramList;
        }
        private static readonly object Mylock = new object();
        public void FileParse(object sender, System.Timers.ElapsedEventArgs e)
        {
            var strinData = FileRead();
          
        }
    }
}
