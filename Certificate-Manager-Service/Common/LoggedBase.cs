using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public class LoggedBase
	{
		public static Dictionary<string, DateTime> LoggedUsers = new Dictionary<string, DateTime>();

		public async static void CheckIsAlive()
		{
			do
			{
				if (LoggedUsers.Count > 0)
				{
					DateTime trenutno = DateTime.Now;

					foreach (string key in LoggedUsers.Keys)
					{
						DateTime user = LoggedUsers[key];
						double result = (trenutno - user).TotalSeconds;

						if (result > 10)
						{
							LoggedUsers.Remove(key);
							Console.WriteLine("Connection with client: " + key + " is dead!!!"); // ispisi u event log
							Logger l = new Logger();
							ClientInformation c = new ClientInformation();
							c.CN = "client";
							c.LogId = 1;
							c.TextId = 1;
							c.TimeStamp = DateTime.Now.ToString();
							c.OU = "mile zorz";
							l.WriteToEventLog("Application", "Application", c, "Connection with client: " + key + " is dead!!!");
							break;
						}
						else
						{
							Console.WriteLine("Connection with client: " + key + " is alive");
							Logger l = new Logger();
							ClientInformation c = new ClientInformation();
							c.CN = "client";
							c.LogId = 1;
							c.TextId = 1;
							c.TimeStamp = DateTime.Now.ToString();
							c.OU = "mile zorz";
							l.WriteToEventLog("Application", "Application", c, "Connection with client: "+ key +" is alive");
						}
					}
				}

				await Task.Delay(3000);
			} while (true);
		}
	}
}
