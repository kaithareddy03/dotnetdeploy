using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RSign.Common;
using RSign.Models.Interfaces;
using RSign.Models.Models;
using RSign.Models.Repository;
using System.Security.Cryptography;
using System.Text;
using Constants = RSign.Common.Helpers.Constants;

namespace RSign.Models.Helpers
{
    public class ModelHelper : IModelHelper
    {
        public static IOptions<AppSettingsConfig> _configuration;
        private static IConfiguration _appConfiguration;
        public ModelHelper(IOptions<AppSettingsConfig> configuration, IConfiguration appConfiguration)
        {
            _configuration = configuration;
            _appConfiguration = appConfiguration;
        }
        public List<UserRoleType> GetUserRole(Guid? userTypeId)
        {
            List<UserRoleType> userRoleType = new List<UserRoleType>();
            try
            {
                string cultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    userRoleType = dbContext.UserType.Select(u => new UserRoleType()
                    {
                        Key = u.ID,
                        Value = u.UserTypeName == "User" ? "User" : u.UserTypeName == "Admin" ? "Admin" : u.UserTypeName == Constants.UserConstants.RSignSupport ? Constants.UserConstants.RSignSupport : "Superuser",
                        IsActive = (u.ID == userTypeId || (userTypeId == null && u.ID == Constants.UserType.USER)) ? true : false
                    }).ToList();
                    return userRoleType;
                }
            }
            catch (Exception ex)
            {
                return userRoleType;
            }
        }
        //public string GetEnvelopeDirectory(Guid envelopeId)
        //{
        //    var path = Convert.ToString(_appConfiguration["TempDirectory"]);
        //    var tempDirectory = Path.Combine(path, envelopeId.ToString());
        //    if (!Directory.Exists(tempDirectory))
        //    {
        //        path = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]);
        //        tempDirectory = Path.Combine(path, envelopeId.ToString());
        //        if (!Directory.Exists(tempDirectory))
        //        {
        //            path = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]);
        //            tempDirectory = Path.Combine(path, envelopeId.ToString());
        //            if (!Directory.Exists(tempDirectory))
        //            {
        //                path = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]);
        //                tempDirectory = Path.Combine(path, envelopeId.ToString());
        //                if (!Directory.Exists(tempDirectory))
        //                {
        //                    path = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]);
        //                    tempDirectory = Path.Combine(path, envelopeId.ToString());
        //                    if (!Directory.Exists(tempDirectory))
        //                    {
        //                        path = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]);
        //                        tempDirectory = Path.Combine(path, envelopeId.ToString());
        //                        if (!Directory.Exists(tempDirectory))
        //                        {
        //                            path = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]);
        //                            tempDirectory = Path.Combine(path, envelopeId.ToString());
        //                            if (!Directory.Exists(tempDirectory))
        //                            {
        //                                path = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]);
        //                                tempDirectory = Path.Combine(path, envelopeId.ToString());
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return path;
        //}
        public static string Decrypt(string encryptedText, string completeEncodedKey, int keySize)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText))
                    return null;

                var aesEncryption = new RijndaelManaged
                {
                    KeySize = keySize,
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    IV = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[0]),
                    Key = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[1])
                };

                ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64CharArray(encryptedText.ToCharArray(), 0, encryptedText.Length);
                return Encoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetRecipientTypeName(string recipientType)
        {
            switch (recipientType)
            {
                case "C20C350E-1C6D-4C03-9154-2CC688C099CB"://Check
                    return "Signer";
                case "63EA73C2-4B64-4974-A7D5-0312B49D29D0"://Date
                    return "CC";
                case "26E35C91-5EE1-4ABF-B421-3B631A34F677"://Company
                    return "Sender";
                case "712F1A0B-1AC6-4013-8D74-AAC4A9BF5568"://Name
                    return "Prefill";
            }
            return "Signer";
        }
        public static string GenerateKey(int keySize)
        {
            var aesEncryption = new RijndaelManaged
            {
                KeySize = keySize,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            // Generates a unique encryption vector
            aesEncryption.GenerateIV();
            var iVectorStr = Convert.ToBase64String(aesEncryption.IV);

            // Generates an encryption key.
            aesEncryption.GenerateKey();
            var iKeyStr = Convert.ToBase64String(aesEncryption.Key);

            string completeKey = iVectorStr + "," + iKeyStr;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(completeKey));
        }
        public static string Encrypt(string plainStr, string completeEncodedKey, int keySize)
        {
            if (string.IsNullOrEmpty(plainStr))
                return null;

            var aesEncryption = new RijndaelManaged
            {
                KeySize = keySize,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[0]),
                Key = Convert.FromBase64String(Encoding.UTF8.GetString(Convert.FromBase64String(completeEncodedKey)).Split(',')[1])
            };

            ICryptoTransform encrypto = aesEncryption.CreateEncryptor();

            // The result of the encryption and decryption
            byte[] plainText = Encoding.UTF8.GetBytes(plainStr);
            byte[] cipherText = encrypto.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherText);
        }
        public string GetEnvelopeDirectoryNew(Guid envelopeId, string UNCPath)
        {
            string defaultPath = Convert.ToString(_appConfiguration["TempDirectory"]);
            var path = Convert.ToString(_appConfiguration["TempDirectory"]);
            var tempDirectory = Path.Combine(path, envelopeId.ToString());

            if (!string.IsNullOrEmpty(UNCPath) && Directory.Exists(Path.Combine(UNCPath, envelopeId.ToString())))
            {
                return UNCPath;
            }
            else if (Directory.Exists(tempDirectory))
            {
                return path;
            }
            else
            {
                UNCPath = GetEnvelopeFolderUNCPath(envelopeId);
                if (!string.IsNullOrEmpty(UNCPath) && Directory.Exists(Path.Combine(UNCPath, envelopeId.ToString())))
                {
                    return UNCPath;
                }
            }

            if (!Directory.Exists(tempDirectory))
            {
                path = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]);
                tempDirectory = Path.Combine(path, envelopeId.ToString());
                if (!Directory.Exists(tempDirectory))
                {
                    path = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]);
                    tempDirectory = Path.Combine(path, envelopeId.ToString());
                    if (!Directory.Exists(tempDirectory))
                    {
                        path = Convert.ToString(_appConfiguration["TemplateDirectory"]);
                        tempDirectory = Path.Combine(path, envelopeId.ToString());
                        if (!Directory.Exists(tempDirectory))
                        {
                            path = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]);
                            tempDirectory = Path.Combine(path, envelopeId.ToString());
                            if (!Directory.Exists(tempDirectory))
                            {
                                path = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]);
                                tempDirectory = Path.Combine(path, envelopeId.ToString());
                                if (!Directory.Exists(tempDirectory))
                                {
                                    path = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]);
                                    tempDirectory = Path.Combine(path, envelopeId.ToString());
                                    if (!Directory.Exists(tempDirectory))
                                    {
                                        path = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]);
                                        tempDirectory = Path.Combine(path, envelopeId.ToString());
                                        if (!Directory.Exists(tempDirectory))
                                        {
                                            path = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]);
                                            tempDirectory = Path.Combine(path, envelopeId.ToString());
                                            if (!Directory.Exists(tempDirectory))
                                            {
                                                path = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]);
                                                tempDirectory = Path.Combine(path, envelopeId.ToString());
                                                if (!Directory.Exists(tempDirectory))
                                                {
                                                    path = defaultPath;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return path;
        }

        public string GetEnvelopeFolderUNCPath(Guid envelopeId)
        {
            try
            {
                string UncPath = string.Empty;
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    EnvelopeFolderMapping envelopeFolderMapping = dbContext.EnvelopeFolderMapping.Where(d => d.EnvelopeId == envelopeId).FirstOrDefault();

                    if (envelopeFolderMapping != null)
                        return UncPath = envelopeFolderMapping.UNCPath;
                    else
                        return UncPath;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetEnvelopeDefaultDirectory()
        {
            return _appConfiguration["TempDirectory"];
        }
        public string GetTemplateDefaultDirectory()
        {
            return _appConfiguration["TemplateDirectory"];
        }
        public string GetTemplateFolderUNCPath(Guid templateId)
        {
            try
            {
                string UncPath = string.Empty;
                using (var dbContext = new RSignDbContext(_configuration))
                {
                    TemplateFolderMapping templateFolderMapping = dbContext.TemplateFolderMapping.Where(d => d.TemplateId == templateId).FirstOrDefault();
                    if (templateFolderMapping != null)
                        return UncPath = templateFolderMapping.UNCPath;
                    else
                        return UncPath;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetTemplateDirectoryName(string UNCPathValue)
        {

            string path = Convert.ToString(_appConfiguration["TemplateDirectory"]),
                tempdirecotry = Convert.ToString(_appConfiguration["TempDirectory"]),
                AlternativeTempDirectory = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]),
                SecondaryAlternativeDirectory = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]),
                TertiaryAlternativeDirectory = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]),
                TetraAlternativeDirectory = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]),
                fifthAlternativeDirectory = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]),
                SixthAlternativeDirectory = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]),
                SeventhAlternativeDirectory = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]),
                 EighthAlternativeDirectory = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]),
                SecondaryTemplateDirectory = Convert.ToString(_appConfiguration["SecondaryTemplateDirectory"]);
            if (UNCPathValue == "1")
            {
                return path;
            }
            else if (UNCPathValue == "2")
            {
                return tempdirecotry;
            }
            else if (UNCPathValue == "3")
            {
                return AlternativeTempDirectory;
            }
            else if (UNCPathValue == "4")
            {
                return SecondaryAlternativeDirectory;
            }
            else if (UNCPathValue == "5")
            {
                return TertiaryAlternativeDirectory;
            }
            else if (UNCPathValue == "6")
            {
                return TetraAlternativeDirectory;
            }
            else if (UNCPathValue == "7")
            {
                return fifthAlternativeDirectory;
            }
            else if (UNCPathValue == "8")
            {
                return SixthAlternativeDirectory;
            }
            else if (UNCPathValue == "9")
            {
                return SeventhAlternativeDirectory;
            }
            else if (UNCPathValue == "10")
            {
                return EighthAlternativeDirectory;
            }
            else if (UNCPathValue == "11")
            {
                return SecondaryTemplateDirectory;
            }
            return "0";
        }

        public string GetIdTemplateDirectory(string UNCPath)
        {
            UNCPath = UNCPath.ToLower();
            string path = Convert.ToString(_appConfiguration["TemplateDirectory"]).ToLower();
            string
                   tempdirecotry = Convert.ToString(_appConfiguration["TempDirectory"]).ToLower(),
                   AlternativeTempDirectory = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]).ToLower(),
                   SecondaryAlternativeDirectory = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]).ToLower(),
                   TertiaryAlternativeDirectory = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]).ToLower(),
                   TetraAlternativeDirectory = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]).ToLower(),
                   fifthAlternativeDirectory = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]).ToLower(),
                   SixthAlternativeDirectory = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]).ToLower(),
                   SeventhAlternativeDirectory = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]).ToLower(),
                     EighthAlternativeDirectory = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]),
                SecondaryTemplateDirectory = Convert.ToString(_appConfiguration["SecondaryTemplateDirectory"]);
            if (path == UNCPath || path == UNCPath + @"\")
            {
                return "1";
            }
            else if (tempdirecotry == UNCPath || tempdirecotry == UNCPath + @"\")
            {
                return "2";
            }
            else if (AlternativeTempDirectory == UNCPath || AlternativeTempDirectory == UNCPath + @"\")
            {
                return "3";
            }
            else if (SecondaryAlternativeDirectory == UNCPath || SecondaryAlternativeDirectory == UNCPath + @"\")
            {
                return "4";
            }
            else if (TertiaryAlternativeDirectory == UNCPath || TertiaryAlternativeDirectory == UNCPath + @"\")
            {
                return "5";
            }
            else if (TetraAlternativeDirectory == UNCPath || TetraAlternativeDirectory == UNCPath + @"\")
            {
                return "6";
            }
            else if (fifthAlternativeDirectory == UNCPath || fifthAlternativeDirectory == UNCPath + @"\")
            {
                return "7";
            }
            else if (SixthAlternativeDirectory == UNCPath || SixthAlternativeDirectory == UNCPath + @"\")
            {
                return "8";
            }
            else if (SeventhAlternativeDirectory == UNCPath || SeventhAlternativeDirectory == UNCPath + @"\")
            {
                return "9";
            }
            else if (EighthAlternativeDirectory == UNCPath || EighthAlternativeDirectory == UNCPath + @"\")
            {
                return "10";
            }
            else if (SecondaryTemplateDirectory == UNCPath || SecondaryTemplateDirectory == UNCPath + @"\")
            {
                return "11";
            }
            return "0";
        }
        public string GetIdEnvelopeDirectory(string UNCPath)
        {
            UNCPath = UNCPath.ToLower();
            string path = Convert.ToString(_appConfiguration["TempDirectory"]);
            string AlternativeTempDirectory = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]).ToLower(),
                SecondaryAlternativeDirectory = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]).ToLower(),
                TertiaryAlternativeDirectory = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]).ToLower(),
                TetraAlternativeDirectory = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]).ToLower(),
                fifthAlternativeDirectory = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]).ToLower(),
                SixthAlternativeDirectory = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]).ToLower(),
                SeventhAlternativeDirectory = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]).ToLower(),
                EighthAlternativeDirectory = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]).ToLower();
            if (path.ToLower() == UNCPath || path.ToLower() == UNCPath + @"\")
            {
                return "1";
            }
            else if (AlternativeTempDirectory == UNCPath || AlternativeTempDirectory == UNCPath + @"\")
            {
                return "2";
            }
            else if (SecondaryAlternativeDirectory == UNCPath || SecondaryAlternativeDirectory == UNCPath + @"\")
            {
                return "3";
            }
            else if (TertiaryAlternativeDirectory == UNCPath || TertiaryAlternativeDirectory == UNCPath + @"\")
            {
                return "4";
            }
            else if (TetraAlternativeDirectory == UNCPath || TetraAlternativeDirectory == UNCPath + @"\")
            {
                return "5";
            }
            else if (fifthAlternativeDirectory == UNCPath || fifthAlternativeDirectory == UNCPath + @"\")
            {
                return "6";
            }
            else if (SixthAlternativeDirectory == UNCPath || SixthAlternativeDirectory == UNCPath + @"\")
            {
                return "7";
            }
            else if (SeventhAlternativeDirectory == UNCPath || SeventhAlternativeDirectory == UNCPath + @"\")
            {
                return "8";
            }
            else if (EighthAlternativeDirectory == UNCPath || EighthAlternativeDirectory == UNCPath + @"\")
            {
                return "9";
            }
            return "0";
        }
        public string GetTemplateDirectory(Guid templateId, string UNCPath)
        {
            if (!string.IsNullOrEmpty(UNCPath) && Directory.Exists(Path.Combine(UNCPath, templateId.ToString())))
            {
                return UNCPath;
            }
            else
            {
                UNCPath = GetTemplateFolderUNCPath(templateId);
                if (!string.IsNullOrEmpty(UNCPath) && Directory.Exists(Path.Combine(UNCPath, templateId.ToString())))
                {
                    return UNCPath;
                }
            }

            string defaultPath = Convert.ToString(_appConfiguration["TemplateDirectory"]);
            var path = Convert.ToString(_appConfiguration["TemplateDirectory"]);
            var tempDirectory = Path.Combine(path, templateId.ToString());

            if (!Directory.Exists(tempDirectory))
            {
                path = Convert.ToString(_appConfiguration["TempDirectory"]);
                tempDirectory = Path.Combine(path, templateId.ToString());
                if (!Directory.Exists(tempDirectory))
                {
                    path = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]);
                    tempDirectory = Path.Combine(path, templateId.ToString());
                    if (!Directory.Exists(tempDirectory))
                    {
                        path = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]);
                        tempDirectory = Path.Combine(path, templateId.ToString());
                        if (!Directory.Exists(tempDirectory))
                        {
                            path = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]);
                            tempDirectory = Path.Combine(path, templateId.ToString());
                            if (!Directory.Exists(tempDirectory))
                            {
                                path = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]);
                                tempDirectory = Path.Combine(path, templateId.ToString());
                                if (!Directory.Exists(tempDirectory))
                                {
                                    path = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]);
                                    tempDirectory = Path.Combine(path, templateId.ToString());
                                    if (!Directory.Exists(tempDirectory))
                                    {
                                        path = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]);
                                        tempDirectory = Path.Combine(path, templateId.ToString());
                                        if (!Directory.Exists(tempDirectory))
                                        {
                                            path = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]);
                                            tempDirectory = Path.Combine(path, templateId.ToString());
                                            if (!Directory.Exists(tempDirectory))
                                            {
                                                path = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]);
                                                tempDirectory = Path.Combine(path, templateId.ToString());
                                                if (!Directory.Exists(tempDirectory))
                                                {
                                                    path = defaultPath;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return path;
        }
        public string GetEnvelopeDirectoryByName(string UNCPathValue)
        {

            string path = Convert.ToString(_appConfiguration["TempDirectory"]);
            string AlternativeTempDirectory = Convert.ToString(_appConfiguration["AlternativeTempDirectory"]),
                SecondaryAlternativeDirectory = Convert.ToString(_appConfiguration["SecondaryAlternativeDirectory"]),
                TertiaryAlternativeDirectory = Convert.ToString(_appConfiguration["TertiaryAlternativeDirectory"]).ToLower(),
                TetraAlternativeDirectory = Convert.ToString(_appConfiguration["TetraAlternativeDirectory"]).ToLower(),
                fifthAlternativeDirectory = Convert.ToString(_appConfiguration["fifthAlternativeDirectory"]).ToLower(),
                SixthAlternativeDirectory = Convert.ToString(_appConfiguration["SixthAlternativeDirectory"]).ToLower(),
                SeventhAlternativeDirectory = Convert.ToString(_appConfiguration["SeventhAlternativeDirectory"]).ToLower(),
                EighthAlternativeDirectory = Convert.ToString(_appConfiguration["EighthAlternativeDirectory"]).ToLower();
            if (UNCPathValue == "1")
            {
                return Convert.ToString(_appConfiguration["TempDirectory"]);
            }
            else if (UNCPathValue == "2")
            {
                return AlternativeTempDirectory;
            }
            else if (UNCPathValue == "3")
            {
                return SecondaryAlternativeDirectory;
            }
            else if (UNCPathValue == "4")
            {
                return TertiaryAlternativeDirectory;
            }
            else if (UNCPathValue == "5")
            {
                return TetraAlternativeDirectory;
            }
            else if (UNCPathValue == "6")
            {
                return fifthAlternativeDirectory;
            }
            else if (UNCPathValue == "7")
            {
                return SixthAlternativeDirectory;
            }
            else if (UNCPathValue == "8")
            {
                return SeventhAlternativeDirectory;
            }
            else if (UNCPathValue == "9")
            {
                return EighthAlternativeDirectory;
            }
            return "0";
        }
        public string CalculateMD5HashForShortURL(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            if (hash.Length >= 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
            }

            return sb.ToString();
        }
    }
}
