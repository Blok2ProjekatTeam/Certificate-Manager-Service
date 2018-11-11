using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class WCFService : IWCFContract
    {
        public void TestCommunication(X509Certificate2 certificate)
        {
            Console.WriteLine("Communication established.");
            ClientInformation client = new ClientInformation();
            client.TextId++;
            client.TimeStamp = DateTime.Now.ToString();
            client.OU = "";
            client.CN = certificate.GetNameInfo(X509NameType.SimpleName, false);            

            Logger log = new Logger();
            string path = Path.GetFullPath("../../../tekst.txt");
            log.WriteToTxt(path, client);
        }

        //public void WriteToEventLog(string LogName, string SourceName, ClientInformation client, string opis)
        //{
        //    if (!EventLog.SourceExists(SourceName))
        //    {
        //        EventLog.CreateEventSource(SourceName, LogName);
        //    }
        //    EventLog newLog = new EventLog(LogName, Environment.MachineName, SourceName);
        //    newLog.WriteEntry(client.Id + " " + client.TimeStamp + " " + client.OU + " " + client.CN + " " + opis, EventLogEntryType.Information, client.Id);


        //}

        //public void WriteToTxt(string path, ClientInformation client, string opis)
        //{
        //    if (!File.Exists(path))
        //    {
        //        using (StreamWriter writetext = new StreamWriter(path))
        //        {
        //            writetext.WriteLine(client.Id + " " + client.TimeStamp + " " + client.OU + " " + client.CN + " " + opis);
        //        }
        //    }
        //    else if (File.Exists(path))
        //    {
        //        using (StreamWriter writetext = File.AppendText(path))
        //        {
        //            writetext.WriteLine(client.Id + " " + client.TimeStamp + " " + client.OU + " " + client.CN + " " + opis);
        //        }
        //    }

        //}
    }
}
