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
            //CertificateManager.CreateAndSignCertificate("s");  // STA JE OVAJ PARAMETAR STO PRIMA?
            //  GDE SE RADI VALIDACIJA ZA SERVERSKU A GDE ZA KLIJENTSKU STRANU?
            if (!CertificateManager.CheckValidation("sale"))
            {
                //CertificateManager.CreateAndSignCertificate("s");
            }
			Console.ReadLine();
        }
    }
}
