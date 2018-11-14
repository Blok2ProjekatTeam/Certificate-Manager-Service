using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class Logger
	{
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
