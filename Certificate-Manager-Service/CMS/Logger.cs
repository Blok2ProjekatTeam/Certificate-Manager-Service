using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
	public class Logger
	{
		public void WriteToEventLog(string LogName, string SourceName, ClientInformation client, string opis)
		{
			if (!EventLog.SourceExists(SourceName))
			{
				EventLog.CreateEventSource(SourceName, LogName);
			}
			EventLog newLog = new EventLog(LogName, Environment.MachineName, SourceName);
			newLog.WriteEntry(opis, EventLogEntryType.Information, client.LogId);
		}
	}
}
