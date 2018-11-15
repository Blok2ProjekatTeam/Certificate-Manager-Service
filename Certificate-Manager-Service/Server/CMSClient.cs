using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Server
{
	public class CMSClient : ChannelFactory<ICMSCommunication>, ICMSCommunication
	{
		ICMSCommunication factory;

		public CMSClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
		{
			factory = this.CreateChannel();			
		}

		public void Validate(X509Certificate2 certificate)
		{
			try
			{
				factory.Validate(certificate);
			}
			catch(Exception e)
			{
				Console.WriteLine("[Validate] ERROR = {0}", e.Message);
			}
		}

		public string CreateAndSignCertificate(string name, string password)
		{
			try
			{
				return factory.CreateAndSignCertificate(name, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("[CreateAndSignCertificate] ERROR = {0}", e.Message);
			}

			return null;
		}

		public bool CheckValidation(X509Certificate2 cert)
		{
			try
			{
				return factory.CheckValidation(cert);
			}
			catch (Exception e)
			{
				Console.WriteLine("[CheckValidation] ERROR = {0}", e.Message);
			}

			return false;
		}

		public void Withdrawal(X509Certificate2 certificate, string subjectName, string pass)
		{
			try
			{
				factory.Withdrawal(certificate, subjectName, pass);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Withdrawal] ERROR = {0}", e.Message);
			}
		}

		public X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
		{
			try
			{
				return factory.GetCertificateFromStorage(storeName, storeLocation, subjectName);
			}
			catch (Exception e)
			{
				Console.WriteLine("[GetCertificateFromStorage] ERROR = {0}", e.Message);
			}

			return null;
		}

		public X509Certificate2 GetCertificateFromFile(string fileName, string password)
		{
			try
			{
				return factory.GetCertificateFromFile(fileName, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("[GetCertificateFromFile] ERROR = {0}", e.Message);
			}

			return null;
		}

		public X509Certificate2 GetCertificateFromTrustedPeople(StoreName storeName, StoreLocation storeLocation)
		{
			try
			{
				return factory.GetCertificateFromTrustedPeople(storeName, storeLocation);
			}
			catch (Exception e)
			{
				Console.WriteLine("[GetCertificateFromTrustedPeople] ERROR = {0}", e.Message);
			}

			return null;
		}

		public X509Certificate2 CertificateExist(StoreName storeName, StoreLocation storeLocation,string certificate)
		{
			try
			{
				return factory.CertificateExist(storeName, storeLocation, certificate);
			}
			catch (Exception e)
			{
				Console.WriteLine("[CertificateExist] ERROR = {0}", e.Message);
			}

			return null;
		}

		public int NumOfCertificates(string subjectName)
		{
			try
			{
				return factory.NumOfCertificates(subjectName);
			}
			catch (Exception e)
			{
				Console.WriteLine("[CertificateExist] ERROR = {0}", e.Message);
			}

			return -1;
		}
	}
}
