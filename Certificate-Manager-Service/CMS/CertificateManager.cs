using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Security.Principal;

namespace CMS
{
	public class CertificateManager
	{
		
		public static bool CreateAndSignCertificate(string signCertificate)
		{
			bool ret = false;
			bool hasKey = false;
			Process p = new Process();
			X509Certificate2 srvCert = null;
			X509Certificate2 srvCert2 = null;

			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = "cmd.exe";
			info.RedirectStandardInput = true;
			info.UseShellExecute = false;
			Console.WriteLine("Enter name for new certificate: ");
			string certificate = Console.ReadLine();

			Console.WriteLine("Enter password for new certificate: ");
			string pass = Console.ReadLine();

			p.StartInfo = info;
			p.Start();

			using (StreamWriter sw = p.StandardInput)
			{
				if (sw.BaseStream.CanWrite)
				{
                    //sw.WriteLine(@"cd C:\Users\Administrator\Documents\git\CMS\Certificate-Manager-Service-master\Certificate-Manager-Service\Certificates");
                    //sw.WriteLine(@"copy CMS.cer C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");
                    //sw.WriteLine(@"copy CMS.pvk C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");

                    sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");

					sw.WriteLine("makecert -sv {0}.pvk -iv CMS.pvk -n \"CN = {1}\" -pe -ic CMS.cer {2}.cer -sr localmachine -ss My -sky exchange", certificate, certificate.ToLower(), certificate);

					do
                    { 
						srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);
						
					} while (srvCert == null);

					sw.WriteLine(" pvk2pfx.exe /pvk {0}.pvk /pi {1} /spc {2}.cer /pfx {3}.pfx",certificate,pass, certificate, certificate);

					//	COPY CERTIFICATES
					sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");
					sw.WriteLine(@"copy {0}.cer C:\Users\Administrator\Documents\git\CMS\Certificate-Manager-Service-master\Certificate-Manager-Service\Certificates", certificate);
					sw.WriteLine(@"copy {0}.pfx C:\Users\Administrator\Documents\git\CMS\Certificate-Manager-Service-master\Certificate-Manager-Service\Certificates", certificate);
					sw.WriteLine(@"copy {0}.pvk C:\Users\Administrator\Documents\git\CMS\Certificate-Manager-Service-master\Certificate-Manager-Service\Certificates", certificate);

                    sw.WriteLine("del {0}.cer", certificate);
                    sw.WriteLine("del {0}.pfx", certificate);
                    sw.WriteLine("del {0}.pvk", certificate);
                    //sw.WriteLine("del CMS.cer");
                    //sw.WriteLine("del CMS.pvk");

                    do
                    {
						srvCert2 = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);

						if(srvCert2.HasPrivateKey)
						{
							hasKey = true;
						}
					} while (hasKey == false);

					sw.WriteLine(@"winhttpcertcfg -g -c LOCAL_MACHINE\My -s {0} -a {1}",certificate, WindowsIdentity.GetCurrent().Name);
				}
			}		

			return ret;

		}

        public static bool CheckValidation(string certificate)
        {
            bool ret = false;
            X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);

            if(DateTime.Parse(cert.GetExpirationDateString()) < DateTime.Now)
            {
                ret = false;
            }
            else
            {
                ret = true;
            }

            if (ret)
            {
                if (RevocationList.List.Contains(cert))
                {
                    ret = false;
                }
            }

            if (ret)
            {
                if(!cert.Issuer.Equals("CN=CMS"))
                {
                    ret = false;
                }
            }

            return ret;
        }

        public static void Withdrawal(string certificate)   // POZIVA KLIJENT(ZELI DA POVUCE SERTIFIKAT)
        {
            X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);

            RevocationList.List.Add(cert);

            CreateAndSignCertificate("");
        }

    }
}
