using NAPS2.Automation;
using NAPS2.Automation.Service;
using NAPS2.Config;
using NAPS2.DI.Modules;
using NAPS2.Worker;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAPS2.DI.EntryPoints
{
    /// <summary>
    /// The entry point for NAPS2.Service.exe, the NAPS2 Service.
    /// </summary>
    public static class ServiceEntryPoint
    {
        public static void Run(string serviceExecutable)
        {
            var log = new EventLog(ServiceErrorOutput.EventLogName);
            log.Source = ServiceErrorOutput.EventSourceName;
            try
            {
                log.WriteEntry("Starting service...", EventLogEntryType.Information);
                // Initialize Ninject (the DI framework)
                var kernel = new StandardKernel(new CommonModule(), new ServiceModule());

                Paths.ClearTemp();

                // Parse the command-line arguments (and display help text if appropriate)
                var configManager = kernel.Get<AutomatedScanningConfigManager>();

                // Start a pending worker process
                WorkerManager.Init();

                // Run the scan automation logic
                var scanning = kernel.Get<AutomatedScanning>(new ConstructorArgument("options", configManager.Options), new ConstructorArgument("serviceExecutable", serviceExecutable));
                scanning.Execute().Wait();
                log.WriteEntry("Service completed successfully", EventLogEntryType.Information);
            }
            catch (ThreadAbortException)
            {
                log.WriteEntry("Service was stopped", EventLogEntryType.Information);
            }
            catch (Exception e)
            {
                log.WriteEntry("Fatal error!\r\n\r\n" + e.ToString(), EventLogEntryType.Error);
            }
            finally
            {
                log.Close();
            }
        }
    }
}
