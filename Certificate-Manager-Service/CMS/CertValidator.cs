using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CMS
{
	public class CertValidator : ICMSCommunication
	{
		/// <summary>
		/// Implementation of a custom certificate validation on the client side.
		/// Client should consider certificate valid if the given certifiate is not self-signed.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		public void Validate(X509Certificate2 certificate)
		{
			string[] pom = certificate.Subject.Split('=', ' ');
			string user = pom[1].Remove(pom[1].Length - 1);
			
			if (!CheckValidation(certificate))
			{
				Console.WriteLine("ERROR: Validacija {0} NIJE uspesno!", user);
				throw new Exception("Non valid certificate");
			}
			else
			{
				Console.WriteLine("INFO: Validacija {0} uspesno!", user);
			}
		}

		public string CreateAndSignCertificate(string name, string password)
		{
			string tempCertificate = name;

			string[] parse = tempCertificate.Split('\\');

			string certificate = parse[1];

            X509Certificate2 cer = null;// CertificateExist(StoreName.My, StoreLocation.LocalMachine, certificate);			

			if (cer == null)
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

				string pass = password;

				p.StartInfo = info;
				p.Start();

				using (StreamWriter sw = p.StandardInput)
				{
					if (sw.BaseStream.CanWrite)
					{

						path = Path.GetFullPath("../../../Certificates");
						File.Copy(path + "/CMS.cer", @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86\CMS.cer");
						File.Copy(path + "/CMS.pvk", @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86\CMS.pvk");

						IdentityReferenceCollection list = WindowsIdentity.GetCurrent().Groups;

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

						sw.WriteLine("makecert -sv {0}.pvk -iv CMS.pvk -n \"CN = {1}, OU = {2}\" -pe -ic CMS.cer {3}.cer -sky exchange", certificate, certificate, ou, certificate);

                        do
                        {
							srvCert = GetCertificateFromFile(String.Format(@"C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86\{0}.cer", certificate), password);
                        } while (srvCert == null);

                        sw.WriteLine(" pvk2pfx.exe /pvk {0}.pvk /pi {1} /spc {2}.cer /pfx {3}.pfx", certificate, pass, certificate, certificate);
                        
						sw.WriteLine(@"cd C:\Program Files (x86)\Windows Kits\10\bin\10.0.16299.0\x86");
						sw.WriteLine(@"copy {0}.cer {1}", certificate, path);
						sw.WriteLine(@"copy {0}.pfx {1}", certificate, path);
						sw.WriteLine(@"copy {0}.pvk {1}", certificate, path);

                        sw.WriteLine("del {0}.cer", certificate);
                        sw.WriteLine("del {0}.pfx", certificate);
                        sw.WriteLine("del {0}.pvk", certificate);
                        sw.WriteLine("del CMS.cer");
                        sw.WriteLine("del CMS.pvk");
                    }
                }

				ClientInformation client = new ClientInformation();
				Logger log = new Logger();

				client.LogId++;
				client.TimeStamp = DateTime.Now.ToString();
				string[] parts = srvCert.Subject.Split('=', ' ');
				for (int i = 3; i < parts.Count(); i++)
				{
					client.OU += parts[3];
				}
				client.CN = parts[1].Remove(parts[1].Length - 1);
				string opis = "Sertifikat uspesno kreiran za :" + client.CN + "!";
				log.WriteToEventLog("Application", "Administrator", client, opis);
			}
			else
			{
				Console.WriteLine("Sertifikat postoji.");
			}

			return certificate;

		}

		public bool CheckValidation(X509Certificate2 cert)
		{
			bool ret = false;

			if (DateTime.Parse(cert.GetExpirationDateString()) < DateTime.Now)
			{
				ret = false;
			}
			else
			{
				ret = true;
			}

			if (ret)
			{
				string pathh = Path.GetFullPath("../../../RevocationList.txt");
				string[] lines = System.IO.File.ReadAllLines(pathh);

				foreach (string line in lines)
				{
					if(line.Equals(cert.SubjectName.Name))
					{
						ret = false;
					}
				}
			}

			if (ret)
			{
				if (!cert.Issuer.Equals("CN=CMS"))
				{
					ret = false;
				}
			}

			ClientInformation client = new ClientInformation();
			Logger log = new Logger();

			string[] parts = cert.Subject.Split('=', ' ');
			for(int i = 3; i < parts.Count(); i++)
			{
				client.OU += parts[3];
			}
			
			client.CN = parts[1].Remove(parts[1].Length - 1); 

			string opis = "";
			client.LogId++;
			client.TextId++;
			client.TimeStamp = DateTime.Now.ToString();


			if (ret)
			{
				opis = "Uspesno validirano za " + client.CN +"!";
				string path = Path.GetFullPath("../../../tekst.txt");
			}
			else
			{
				opis = "Sertifikat nije validan za " + client.CN + "!";
			}
			log.WriteToEventLog("Application", "Administrator", client, opis);

			return ret;
		}

		public void Withdrawal(X509Certificate2 certificate, string subjectName, string password)   // POZIVA KLIJENT(ZELI DA POVUCE SERTIFIKAT)
		{
			string pathh = Path.GetFullPath("../../../RevocationList.txt");

			string path = pathh;
			StringBuilder sb = new StringBuilder();

			sb.Append("\n");
			sb.Append(certificate.SubjectName.Name);

			// Add text to the file.
			if (!File.Exists(path))
				File.WriteAllText(path, sb.ToString());
			else
				File.AppendAllText(path, sb.ToString());

			string tempCertificate = subjectName;

			string[] parse = tempCertificate.Split('\\');

			string subjectNameCount = parse[1];

			string subjectNameNew = subjectName + "New" + NumOfCertificates(subjectNameCount);

			CreateAndSignCertificate(subjectNameNew, password);

			//CreateAndSignCertificate(Thread.CurrentPrincipal.Identity.Name, newPass);
			ClientInformation client = new ClientInformation();
			Logger log = new Logger();

			string[] parts = certificate.Subject.Split('=', ' ');
			client.OU = parts[3];
			client.CN = parts[1].Remove(parts[1].Length - 1);

			string opis = "Povucen sertifikat";
			client.LogId++;

			client.TimeStamp = DateTime.Now.ToString();
			log.WriteToEventLog("Application", "Administrator", client, opis);
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

			/// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
			foreach (X509Certificate2 c in certCollection)
			{
				if (c.SubjectName.Name.StartsWith(string.Format("CN={0}", subjectName)))
				{
					return c;
				}
			}

			return null;
		}

		public X509Certificate2 GetCertificateFromFile(string fileName, string pass)
		{
			X509Certificate2 certificate = null;
            			
            string pwd = pass;

			///Convert string to SecureString
			SecureString secPwd = new SecureString();
			foreach (char c in pwd)
			{
				secPwd.AppendChar(c);
			}
			pwd = String.Empty;

			/// try-catch necessary if either the speficied file doesn't exist or password is incorrect
			try
			{
				certificate = new X509Certificate2(fileName, secPwd);
			}
			catch (Exception e)
			{
				//Console.WriteLine("Erroro while trying to GetCertificateFromFile {0}. ERROR = {1}", fileName, e.Message);
			}

			return certificate;
		}

		public X509Certificate2 GetCertificateFromTrustedPeople(StoreName storeName, StoreLocation storeLocation)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates;

			/// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
			foreach (X509Certificate2 c in certCollection)
			{
				return c;
			}

			return null;
		}

		public X509Certificate2 CertificateExist(StoreName storeName, StoreLocation storeLocation, String certificate)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);

			X509Certificate2Collection certCollection = store.Certificates;

			foreach (X509Certificate2 c in certCollection)
			{
				if (c.SubjectName.Name.StartsWith(string.Format("CN={0}", certificate)))
					return c;
			}

			return null;

		}

		public int NumOfCertificates(string subjectName)
		{
			int ret = 0;

			string pathh = Path.GetFullPath("../../../RevocationList.txt");
			string[] lines = System.IO.File.ReadAllLines(pathh);


			foreach (string line in lines)
			{
				string[] parse = line.Split('=', ',');
				if (!line.Equals(""))
				{
					if (parse[1].Equals(subjectName) || parse[1].Contains(subjectName + "New"))
					{
						ret++;
					}
				}

			}

			return ret;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
}
