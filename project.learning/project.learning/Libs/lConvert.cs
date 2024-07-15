using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace project.learning.Libs
{
    public class lConvert
    {
        public JArray convertDynamicToJArray(List<dynamic> list)
        {
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
            foreach (dynamic dr in list)
            {
                detail = new JObject();
                foreach (var pair in dr)
                {
                    detail.Add(pair.Key, pair.Value);
                }
                data.Lists.Add(detail);
            }
            return data.Lists;
        }
        public JArray convertDynamicToJArrayStr(List<dynamic> list)
        {
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
            foreach (dynamic dr in list)
            {
                detail = new JObject();
                foreach (var pair in dr)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(pair.Value)))
                    {
                        detail.Add(pair.Key, Convert.ToString(pair.Value).Trim());
                    }
                    else
                    {
                        detail.Add(pair.Key, "");
                    }
                }
                data.Lists.Add(detail);
            }
            return data.Lists;
        }

        public JArray convertDynamicToJArrayWith2Combination(List<dynamic> list, List<dynamic> jproduct, string fieldidentified, string keyidentified, string val)
        {
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
            foreach (dynamic dr in list)
            {
                detail = new JObject();
                foreach (var pair in dr)
                {
                    detail.Add(pair.Key, pair.Value);
                    if (pair.Key == fieldidentified)
                    {
                        foreach (JObject jp in jproduct)
                        {
                            if (jp[keyidentified] == pair.Value)
                            {
                                detail.Add(val, jp[val]);
                            }
                        }
                    }
                }
                data.Lists.Add(detail);
            }
            return data.Lists;
        }

        public JObject convertDynamicToSingleJObject(List<dynamic> list)
        {
            var jsonObject = new JObject();
            foreach (dynamic dr in list)
            {
                jsonObject.Add(dr.p_key, dr.p_val);
            }
            return jsonObject;
        }
        public JArray convertDynamicObjToJArray(dynamic dr)
        {
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
           
            detail = new JObject();
            foreach (var pair in dr)
            {
                detail.Add(pair.Key, pair.Value);
            }
            data.Lists.Add(detail);
            
            return data.Lists;
        }

        public JArray convertToOneDynamic(List<dynamic> list)
        {
            //key tidak ada yg sama
            var jsonObject = new JObject();
            dynamic data = jsonObject;
            data.Lists = new JArray() as dynamic;
            dynamic detail = new JObject();
            List<string> str = new List<string>();
            foreach (dynamic dr in list)
            {
                foreach (var pair in dr)
                {
                    if (!str.Where(o => string.Equals(pair.Key, o, StringComparison.OrdinalIgnoreCase)).Any()) {
                        detail.Add(pair.Key, pair.Value);
                        str.Add(pair.Key);
                    }
                }
            }
            data.Lists.Add(detail);
            return data.Lists;
        }

        public string EncryptString(string encryptString, string EncryptionKey)
        {
            //string EncryptionKey = "idxp@rtn3rs";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string DecryptString(string cipherText, string EncryptionKey)
        {
            //string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public string encrypt(string str)
        {
            dbConn ldb = new dbConn();
            var key = ldb.ConfigKey("EncryptionKey");
            //var key = "idxp@rtn3rs";
            //var key = "idxpartners";
            string encrypted = EncryptString(str, key);
            return encrypted;
        }

        public string decrypt(string str)
        {
            dbConn ldb = new dbConn();
            var key = ldb.ConfigKey("DecryptionKey");
            //var key = "idxp@rtn3rs";
            //var key = "idxpartners";
            string decrypted = DecryptString(str, key);
            return decrypted;
        }

    }
}
