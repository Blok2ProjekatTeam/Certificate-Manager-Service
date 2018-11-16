using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract
	{
		IWCFContract factory;
		public WCFClient(NetTcpBinding binding, EndpointAddress address, string clientCertificateName, ICMSCommunication proxy)
			: base(binding, address)
		{

			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCostumValidator(proxy);
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = ClientGet.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertificateName);

			factory = this.CreateChannel();

		}

		public void TestCommunication(X509Certificate2 certificate)
		{
			try
			{
				factory.TestCommunication(certificate);
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
                Thread.Sleep(5000);
                Process.GetCurrentProcess().Kill();
			}
		}

		//public void WriteToEventLog(string LogName, string SourceName, ClientInformation client, string opis)
		//{
		//    try
		//    {
		//        factory.WriteToEventLog(LogName, SourceName, client, opis);
		//    }
		//    catch (Exception e)
		//    {
		//        Console.WriteLine("[Event Log] ERROR = {0}", e.Message);
		//    }
		//}

		//public void WriteToTxt(string path, ClientInformation client, string opis)
		//{
		//    try
		//    {
		//        factory.WriteToTxt(path, client, opis);
		//    }
		//    catch (Exception e)
		//    {
		//        Console.WriteLine("[WriteToTxt] ERROR = {0}", e.Message);
		//    }
		//}
	}
}
