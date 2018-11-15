using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	public class WCFService : IWCFContract
	{
		public void TestCommunication(X509Certificate2 certificate)
		{
			string[] pom = certificate.Subject.Split('=', ' ');
			string user = pom[1].Remove(pom[1].Length - 1);
			Console.WriteLine("INFO: Communication established with {0}.", user);

			if (!LoggedBase.LoggedUsers.ContainsKey(user))
				LoggedBase.LoggedUsers.Add(user, DateTime.Now);
			else
				LoggedBase.LoggedUsers[user] = DateTime.Now;


			ClientInformation client = new ClientInformation();

			client.TimeStamp = DateTime.Now.ToString();
			string[] parts = certificate.Subject.Split('=', ' ');
			for (int i = 3; i < parts.Count(); i++)
			{
				client.OU += parts[i];
			}
			client.CN = parts[1].Remove(parts[1].Length - 1);
			client.TextId++;


			Logger log = new Logger();
			string path = Path.GetFullPath("../../../tekst.txt");
			log.WriteToTxt(path, client);
		}
	}
}
