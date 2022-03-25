﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ARMMordanizerService
{
    public partial class ArmMordanizerService : ServiceBase
    {
        //private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ArmConnection"].ConnectionString;
        //private readonly string _armFilePath = @"" + ConfigurationManager.AppSettings["armFilePath"];
        //private readonly string _armFileCompletePath = @"" + ConfigurationManager.AppSettings["armFileCompletePath"];
        //private SqlConnection con;
        private readonly IArmService _armService;
        readonly System.Timers.Timer _timer = new System.Timers.Timer();

        static void Main()
        {
#if (DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ArmMordanizerService()
            };
            ServiceBase.Run(ServicesToRun);
#else
            var winService = new FileParser();
            winService.FileParse();
#endif
        }
        public ArmMordanizerService()
        {

            //InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            InitializeComponents();
        }
        private void InitializeComponents()
        {
            var timerInterVal = int.Parse(ConfigurationManager.AppSettings["timeInterVal"]);
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
        }

    }
}
