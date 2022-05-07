using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ARMMordanizerService
{
    public partial class ArmMordanizerService : ServiceBase
    {
        private readonly ILogger _logger;

        readonly System.Timers.Timer _timer = new System.Timers.Timer();
        private string UploadLogFile = "";
        private System.Timers.Timer timer;


        ArmRepository _iArmRepo = new ArmRepository();
        public ArmMordanizerService()
        {
            UploadLogFile = _iArmRepo.GetFileLocation(3);

            //InitializeComponents();
            _logger = Logger.GetInstance;

        }

        protected override void OnStart(string[] args)
        {
            _logger.Log("Service started", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

            InitializeComponents();
        }


        private void InitializeComponents()
        {

            var timerInterVal = Convert.ToInt32(_iArmRepo.GetFileLocation(0));// int.Parse(ConfigurationManager.AppSettings["timeInterVal"]);
            _timer.AutoReset = true;
            _timer.Interval = timerInterVal;
            _timer.Enabled = true;
            _timer.Start();
            _timer.Elapsed += (new FileParser()).FileParse;
        }


        protected override void OnStop()
        {
            _timer.Enabled = false;
            _timer.Stop();
            _logger.Log("Service stopped", @"" + UploadLogFile.Replace("DDMMYY", DateTime.Now.ToString("ddMMyy")));

        }

    }
}
