using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System.Collections;
using CryptoX509 = System.Security.Cryptography.X509Certificates;

namespace RSign.ManageDocument.Models.iTextHelper
{
    public class DigitalCert
    {
        public Org.BouncyCastle.X509.X509Certificate[] chain;
        public ICipherParameters pk;
        public string Certificate;   
        private string _path = "";
        private string _password = "";
        private string _digestAlgorithm = "";
        private AsymmetricKeyParameter _akp;
        private X509Certificate[] _chain;

        public X509Certificate[] Chain
        {
            get { return _chain; }
        }
        /// <summary/>
        public AsymmetricKeyParameter Akp
        {
            get { return _akp; }
        }
        /// <summary/>
        public string Path
        {
            get { return _path; }
        }
        /// <summary/>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary/>
        public string DigestAlgorithm
        {
            get { return _digestAlgorithm; }
            set { _digestAlgorithm = value; }
        }

        public DigitalCert(string cpath, string cpassword, bool usePkcs12 = true)
        {
            _path = cpath;
            _password = cpassword;
            ProcessCert(usePkcs12);
        }

        private void ProcessCert(bool usePkcs12 = true)
        {
            var cert2 = new CryptoX509.X509Certificate2(Path, Password,
                CryptoX509.X509KeyStorageFlags.Exportable | CryptoX509.X509KeyStorageFlags.MachineKeySet | CryptoX509.X509KeyStorageFlags.PersistKeySet);
            _digestAlgorithm = DigestAlgorithms.GetDigest(cert2.SignatureAlgorithm.Value);

            if (usePkcs12)
            {
                string alias = string.Empty;
                var pk12 = new Pkcs12Store(new FileStream(this.Path, FileMode.Open, FileAccess.Read), Password.ToCharArray());
                IEnumerator i = pk12.Aliases.GetEnumerator();
                while (i.MoveNext())
                {
                    alias = ((string)i.Current);
                    if (pk12.IsKeyEntry(alias))
                        break;
                }

                _akp = pk12.GetKey(alias).Key;
                X509CertificateEntry[] ce = pk12.GetCertificateChain(alias);
                _chain = new X509Certificate[ce.Length];
                for (int k = 0; k < ce.Length; ++k)
                    _chain[k] = ce[k].Certificate;
            }
            else
            {
                _akp = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(cert2.PrivateKey).Private;
                var chain = new CryptoX509.X509Chain();
                chain.Build(cert2);
                _chain = new X509Certificate[chain.ChainElements.Count];
                for (int k = 0; k < chain.ChainElements.Count; ++k)
                {
                    CryptoX509.X509ChainElement chainElement = chain.ChainElements[k];
                    _chain[k] = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(chainElement.Certificate);
                }
            }
        }
    }
}
