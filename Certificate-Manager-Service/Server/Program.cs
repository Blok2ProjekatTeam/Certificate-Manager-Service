using CMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
			CertificateManager c = new CertificateManager();
			c.CreateAndSignCertificate("s");
			Console.ReadLine();
        }
    }
}
