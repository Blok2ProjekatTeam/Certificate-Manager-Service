using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding2 = new NetTcpBinding();
			ICMSCommunication proxy2 = new ChannelFactory<ICMSCommunication>(binding2, new EndpointAddress("net.tcp://localhost:1324/CMS")).CreateChannel();

			Console.WriteLine("Enter new certificate password: ");
			string password = Console.ReadLine();

            string tempCertificate = WindowsIdentity.GetCurrent().Name;

            string[] parse = tempCertificate.Split('\\');

            string clientCertificateName = parse[1];

            bool temp = false;

            X509Certificate2 clientCertificate = ClientGet.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertificateName);

            if(clientCertificate == null)
            {
                clientCertificateName = proxy2.CreateAndSignCertificate(WindowsIdentity.GetCurrent().Name, password);

                do
                {
                    clientCertificate = ClientGet.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertificateName);
                    if (clientCertificate == null)
                    {
                        temp = false;
                    }
                    else if (!clientCertificate.HasPrivateKey)
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
                Console.WriteLine("Sertifikat postoji.");
            }            

			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

			X509Certificate2 srvCert = ClientGet.GetCertificateFromTrustedPeople(StoreName.TrustedPeople, StoreLocation.LocalMachine);

			EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:2222/Receiver"), new X509CertificateEndpointIdentity(srvCert));

			Thread.Sleep(3000);
			using (WCFClient proxy = new WCFClient(binding, address, clientCertificateName, proxy2))
			{
				Random r = new Random();
				/*Console.WriteLine("*********	Menu	**********");
				Console.WriteLine("* 1. Withdrawal certificate.	 *");
				Console.WriteLine("* 2. Establish communication.    *");
				Console.WriteLine("* 3. Exit                        *");
				Console.WriteLine("**********************************");
				Console.WriteLine("Select option: ");
				int select = int.Parse(Console.ReadLine());

				if(select == 1)
				{
					CertificateManager.Withdrawal(clientCertificate);
				}
				else if(select == 2)
				{*/
				do
				{
					proxy.TestCommunication(clientCertificate);
					Thread.Sleep(r.Next(1, 10) * 1000);
				} while (true);
				/*}
				else if(select == 3)
				{
					
				}
				else
				{
					Console.WriteLine("ERROR: Selected option error!");
				} */
			}

			Console.ReadKey();
		}
	}
}
