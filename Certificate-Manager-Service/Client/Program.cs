using CMS;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            string clientCertificateName = CertificateManager.CreateAndSignCertificate("");
            X509Certificate2 clientCertificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientCertificateName);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                        
            X509Certificate2 srvCert = CertManager.GetCertificateFromTrustedPeople(StoreName.TrustedPeople, StoreLocation.LocalMachine);
            
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:1234/Receiver"), new X509CertificateEndpointIdentity(srvCert));

            using (WCFClient proxy = new WCFClient(binding, address, clientCertificateName))
            {
                Random r = new Random();
                do
                {
                    proxy.TestCommunication(clientCertificate);
                    Thread.Sleep(r.Next(1, 10)*1000);
                } while (true);
                        
            }                  

            Console.ReadKey();
        }
    }
}
