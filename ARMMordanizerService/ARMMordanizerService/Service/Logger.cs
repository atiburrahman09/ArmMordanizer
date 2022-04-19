using System;
using System.Configuration;
using System.IO;

namespace ARMMordanizerService
{
    public sealed class Logger:ILogger
    {
        private static readonly object MyLock = new object();
        private readonly string _logFile = @"" +ConfigurationManager.AppSettings["logFile"];
        private Logger()
        {
            if (!File.Exists(_logFile))
                File.Create(_logFile);
        }
        private static readonly Lazy<Logger> LoggerInstance=new Lazy<Logger>(()=>new Logger());
        public static Logger GetInstance => LoggerInstance.Value;

 
        public void Log(string message)
        {
            lock (MyLock)
            {
                try
                {
                    using (var file=File.AppendText(_logFile))
                    {
                        file.Write(message+" at "+DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")+Environment.NewLine);
                    }

                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
