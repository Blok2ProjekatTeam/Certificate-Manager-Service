using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	[ServiceContract]
	public interface ICMSCommunication
	{
		[OperationContract]
		void Validate(X509Certificate2 certificate);

		[OperationContract]
		string CreateAndSignCertificate(string name, string password);

		[OperationContract]
		bool CheckValidation(X509Certificate2 cert);

		[OperationContract]
		void Withdrawal(X509Certificate2 certificate);

		[OperationContract]
		X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName);

		[OperationContract]
		X509Certificate2 GetCertificateFromFile(string fileName, string pass);

		[OperationContract]
		X509Certificate2 GetCertificateFromTrustedPeople(StoreName storeName, StoreLocation storeLocation);

		[OperationContract]
		X509Certificate2 CertificateExist(StoreName storeName, StoreLocation storeLocation, string certificate);
	}
}
