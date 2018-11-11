using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CMS
{
    public class ClientCertValidator : X509CertificateValidator
    {
        /// <summary>
        /// Implementation of a custom certificate validation on the client side.
        /// Client should consider certificate valid if the given certifiate is not self-signed.
        /// If validation fails, throw an exception with an adequate message.
        /// </summary>
        /// <param name="certificate"> certificate to be validate </param>
        public override void Validate(X509Certificate2 certificate)
        {
            if (!CertificateManager.CheckValidation(certificate))
            {
                Console.WriteLine("CLIENT VALIDACIJA");

                throw new Exception("Certificate is self-issued.");
            }
        }
    }
}
