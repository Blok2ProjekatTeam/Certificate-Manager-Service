using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
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
            newLog.WriteEntry(client.LogId + " " + client.TimeStamp + " " + client.OU + " " + client.CN + " " + opis, EventLogEntryType.Information, client.LogId);
            

        }

        public void WriteToTxt(string path, ClientInformation client)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine(client.TextId + " " + client.TimeStamp + " " + client.OU + " " + client.CN);
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter writetext = File.AppendText(path))
                {
                    writetext.WriteLine(client.TextId + " " + client.TimeStamp + " " + client.OU + " " + client.CN);
                }
            }

        }
    }
}
