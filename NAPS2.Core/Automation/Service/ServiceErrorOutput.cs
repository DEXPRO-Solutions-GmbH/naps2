using NAPS2.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAPS2.Automation.Service
{
    public class ServiceErrorOutput : IErrorOutput
    {
        public static string EventLogName { get; } = "DOCUMENTS Scanner";
        public static string EventSourceName { get; } = "DOCUMENTS Scanner";
        private readonly EventLog log;

        public ServiceErrorOutput()
        {
            //Log is created by installer in NAPS2.Service project
            log = new EventLog(EventLogName);
            log.Source = EventSourceName;
        }

        public void DisplayError(string errorMessage)
        {
            log.WriteEntry(errorMessage, EventLogEntryType.Error);
        }

        public void DisplayError(string errorMessage, string details)
        {
            log.WriteEntry(errorMessage + "\r\n\r\n" + details, EventLogEntryType.Error);
        }

        public void DisplayError(string errorMessage, Exception exception)
        {
            log.WriteEntry(errorMessage + "\r\n\r\n" + exception.ToString(), EventLogEntryType.Error);
        }
    }
}
