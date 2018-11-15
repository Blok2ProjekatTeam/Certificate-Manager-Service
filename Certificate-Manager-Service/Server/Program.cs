using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ReadKey();

			NetTcpBinding binding2 = new NetTcpBinding();
			EndpointAddress address2 = new EndpointAddress("net.tcp://localhost:1324/CMS");
			
			using (CMSClient proxy2 = new CMSClient(binding2, address2))
			{
                bool temp = false;
                X509Certificate2 serverCertificate = null;

                string tempCertificate = WindowsIdentity.GetCurrent().Name;

                string[] parse = tempCertificate.Split('\\');

                string serverCertificateName = parse[1];

                serverCertificate = ServerGet.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serverCertificateName);
                if (serverCertificate == null)
                {
					Console.WriteLine("Enter new certificate password: ");
					string password = Console.ReadLine();
					string tempCertificate2 = WindowsIdentity.GetCurrent().Name;

					string[] parse2 = tempCertificate2.Split('\\');

					string serverCertificateName2 = parse[1];

					serverCertificateName = proxy2.CreateAndSignCertificate(WindowsIdentity.GetCurrent().Name, password);

                    do
                    {
                        serverCertificate = ServerGet.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, serverCertificateName2);
                        if(serverCertificate == null)
                        {
                            temp = false;
                        }
                        else if(!serverCertificate.HasPrivateKey)
                        {
                            temp = false;
                        }
                        else
                        {
                            temp = true;
                        }
                    } while (!temp);
                }
                else
                {
                    Console.WriteLine("Postoji sertifikat.");
                }
                
				NetTcpBinding binding = new NetTcpBinding();

                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string address = "net.tcp://localhost:1234/Server";

                ServiceHost host = new ServiceHost(typeof(WCFService));

				host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;

				host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCustomValidator(proxy2);

				host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                host.Credentials.ServiceCertificate.Certificate = serverCertificate;

				Task task = new Task(() => LoggedBase.CheckIsAlive());
				task.Start();

				try
				{
					Thread.Sleep(5000);
					host.Open();
					Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");

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
}
