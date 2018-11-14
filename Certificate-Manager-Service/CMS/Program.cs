using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
	class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();

			string address = "net.tcp://localhost:1324/CMS";
			ServiceHost host = new ServiceHost(typeof(CertValidator));
			host.AddServiceEndpoint(typeof(ICMSCommunication), binding, address);

			try
			{
				host.Open();
				Console.WriteLine("CMS is started.\nPress <enter> to stop ...");

				Console.ReadLine();

			}
			catch (Exception e)
			{
				Console.WriteLine("[ERROR] {0}", e.Message);
				Console.WriteLine("[StackTrace] {0}", e.StackTrace);
				Console.ReadLine();
			}
			finally
			{
				host.Close();
			}

			Console.ReadLine();
		}
	}
}
