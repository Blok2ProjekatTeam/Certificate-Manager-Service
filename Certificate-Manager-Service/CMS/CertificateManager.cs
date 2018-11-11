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
using Common;

namespace CMS
{
	public class CertificateManager
	{
		
		public static string CreateAndSignCertificate(string signCertificate)
		{
			bool hasKey = false;
			Process p = new Process();
			X509Certificate2 srvCert = null;
			X509Certificate2 srvCert2 = null;

            string path = String.Empty;

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

                    IdentityReferenceCollection list = WindowsIdentity.GetCurrent().Groups;
                    //list.Translate(typeof(NTAccount)).Value;

                    string ou = string.Empty;

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list.Count != 0)
                        {
                            ou += list[i].Translate(typeof(NTAccount)).Value;
                        }

                        if ((i + 1) != list.Count)
                        {
                            ou += " ";
                        }
                    }


                    sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");

					sw.WriteLine("makecert -sv {0}.pvk -iv CMS.pvk -n \"CN = {1} OU = {2}\" -pe -ic CMS.cer {3}.cer -sr localmachine -ss My -sky exchange", certificate, certificate.ToLower(), ou, certificate);

					do
                    { 
						srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);
						
					} while (srvCert == null);

					sw.WriteLine(" pvk2pfx.exe /pvk {0}.pvk /pi {1} /spc {2}.cer /pfx {3}.pfx",certificate,pass, certificate, certificate);

					//	COPY CERTIFICATES

                    path = Path.GetFullPath("../../../Certificates");

                    sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");
					sw.WriteLine(@"copy {0}.cer {1}", certificate, path);
					sw.WriteLine(@"copy {0}.pfx {1}", certificate, path);
					sw.WriteLine(@"copy {0}.pvk {1}", certificate, path);

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

            ClientInformation client = new ClientInformation();
            Logger log = new Logger();

            string opis = "Sertifikat uspesno kreiran";
            client.LogId++;
            //client.OU = "petra";
            //client.CN = srvCert.GetNameInfo(X509NameType.SimpleName, false);
            client.TimeStamp = DateTime.Now.ToString();
            //client.OU = srvCert.GetNameInfo(X509NameType.DnsName, false);
            string[] parts = srvCert.Subject.Split('=', ' ');
            client.OU = parts[3];
            client.CN = parts[1];
            log.WriteToEventLog("Application", client.CN, client, opis);
            
			return certificate;

		}

        public static bool CheckValidation(X509Certificate2 cert)
        {
            bool ret = false;

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

            ClientInformation client = new ClientInformation();
            Logger log = new Logger();

            string[] parts = cert.Subject.Split('=', ' ');
            client.OU = parts[3];
            client.CN = parts[1];

            string opis = "";
            client.LogId++;
            client.TextId++;
            //client.OU = "petra";
           // client.CN = cert.GetNameInfo(X509NameType.SimpleName, false);
            client.TimeStamp = DateTime.Now.ToString();
           // client.OU = cert.GetNameInfo(X509NameType.DnsName, false);
            

            if(ret)
            {
                opis = "Uspesno validirano!";
                string path = Path.GetFullPath("../../../tekst.txt");
                log.WriteToTxt(path, client);
            }else
            {
                opis = "Sertifikat nije validan!";
            }
            log.WriteToEventLog("Application", client.CN, client, opis);
            
            return ret;
        }

        public static void Withdrawal(string certificate)   // POZIVA KLIJENT(ZELI DA POVUCE SERTIFIKAT)
        {
            X509Certificate2 cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, certificate);

            RevocationList.List.Add(cert);

            CreateAndSignCertificate("");
            ClientInformation client = new ClientInformation();
            Logger log = new Logger();

            string[] parts = cert.Subject.Split('=', ' ');
            client.OU = parts[3];
            client.CN = parts[1];

            string opis = "Povucen sertifikat";
            client.LogId++;

            //client.CN = cert.GetNameInfo(X509NameType.SimpleName, false);
            client.TimeStamp = DateTime.Now.ToString();
            //client.OU = cert.GetNameInfo(X509NameType.DnsName, false);
            log.WriteToEventLog("Application", client.CN, client, opis);
        }

    }
}
