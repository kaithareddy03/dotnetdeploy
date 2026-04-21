using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;

namespace RSign.ManageDocument.Models.iTextHelper
{
    public class DigitalCertIN
    {
        public Org.BouncyCastle.X509.X509Certificate[] chain;
        public ICipherParameters pk;
        public String Certificate;
        public char[] Password;
        public string DigestAlgorithm;
        public DigitalCertIN(string _certificate, string _password)
        {
            Certificate = _certificate;
            Password = _password.ToCharArray();
            var tmpCertificate = new X509Certificate2(Certificate, _password,
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet);

            DigestAlgorithm = iText.Signatures.DigestAlgorithms.GetDigest(tmpCertificate.SignatureAlgorithm.Value);
            pk = Pkcs12FileHelper.ReadFirstKey(Certificate, Password, Password);
            chain = Pkcs12FileHelper.ReadFirstChain(Certificate, Password);
        }
    }
}
