using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
	class Program
	{
		static void Main(string[] args)
		{
			CertValidator certValidator = new CertValidator();
			X509Certificate2 cmsCert = null;

			string path = Path.GetFullPath("../../../Certificates");
			Process p = new Process();
			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = "cmd.exe";
			info.RedirectStandardInput = true;
			info.UseShellExecute = false;
			p.StartInfo = info;
			p.Start();

			using (StreamWriter sw = p.StandardInput)
			{
				if (sw.BaseStream.CanWrite)
				{
					sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");
					sw.WriteLine("makecert -n \"CN = CMS\" -r -sv CMS.pvk CMS.cer");
					sw.WriteLine(" pvk2pfx.exe /pvk CMS.pvk /pi cms /spc CMS.cer /pfx CMS.pfx");
					sw.WriteLine(@"copy CMS.cer {0}", path);
					sw.WriteLine(@"copy CMS.pfx {0}", path);
					sw.WriteLine(@"copy CMS.pvk {0}", path);
					sw.WriteLine("del CMS.cer");
					sw.WriteLine("del CMS.pfx");
					sw.WriteLine("del CMS.pvk");
				}
			}

			do
			{
				cmsCert = certValidator.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "CMS");
			} while (cmsCert == null);

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
