using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Logger
    {
        public void WriteToEventLog(string LogName, string SourceName, ClientInformation client, string opis)
        {
            //string user = WindowsIdentity.GetCurrent().User.ToString();

            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, LogName);
            }
            EventLog newLog = new EventLog(LogName, Environment.MachineName, SourceName);
            newLog.WriteEntry(opis, EventLogEntryType.Information, client.LogId);
            

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
                string putanja = Path.GetFullPath("../../../tekst.txt");
                String last = File.ReadLines(putanja).Last();
                string[] splitovano = last.Split(' ');
                int id = Int32.Parse(splitovano[0]);
                id++;

                using (StreamWriter writetext = File.AppendText(path))
                {
                    writetext.WriteLine(id + " " + client.TimeStamp + " " + client.OU + " " + client.CN);
                }
            }

        }
    }
}
