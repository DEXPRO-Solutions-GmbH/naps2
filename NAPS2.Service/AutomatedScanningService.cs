using NAPS2.DI.EntryPoints;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAPS2.Service
{
    public partial class AutomatedScanningService : ServiceBase
    {
        Thread runningService = null;
        bool isStopping = false;

        public AutomatedScanningService()
        {
            InitializeComponent();
        }

        private void Run()
        {
            isStopping = false;
            try
            {
                // Use reflection to avoid antivirus false positives (yes, really)
                typeof(ServiceEntryPoint).GetMethod("Run").Invoke(null, new object[] { ServiceAssemblyLocator.Locate() });
            }
            catch (ThreadAbortException)
            { }
            isStopping = true;
            Stop();
        }

        protected override void OnStart(string[] args)
        {
            if (runningService == null)
            {
                runningService = new Thread(Run);
                runningService.Start();
            }
        }

        protected override void OnStop()
        {
            if (!isStopping)
            {
                try
                {
                    runningService?.Abort();
                } catch {}
            }
            runningService = null;
            isStopping = false;
        }
    }
}
