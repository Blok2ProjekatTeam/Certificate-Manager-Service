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
    public interface IWCFContract
    {
        [OperationContract]
        void TestCommunication(X509Certificate2 certificate);

        //[OperationContract]
        //void WriteToTxt(string path, ClientInformation client, string opis);
        //[OperationContract]
        //void WriteToEventLog(string LogName, string SourceName, ClientInformation client, string opis);
    }
}
