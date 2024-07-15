using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using project.learning.Libs;
using System.Text;
using project.learning.Model;

namespace project.learning.Libs
{
    public class dbConn
    {
        private lConvert lc = new lConvert();

        #region GSM
        public string GetAppConfigGSM(string cstr)
        {
            lGSM lg = new lGSM();
            var config = lg.execExtAPIPost("urlAPI_idcconfig", "GSM/Secret", "master_appsettings");

            return "" + config.GetSection(cstr).Value.ToString();
        }
        public string ConfigGSM(string cstr)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

            var config = builder.Build();
            return "" + config.GetSection("Secret:" + cstr).Value.ToString();
        }

        public string GetHdrConfigGSM(string cstr)
        {
            lGSM lg = new lGSM();
            var config = lg.execExtAPIPost("urlAPI_idcconfig", "GSM/Secret", "headerconfig");

            return "" + config.GetSection(cstr).Value.ToString();
        }
        #endregion GSM
        public string ConfigKey(string cstr)
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM("KeyConvert:" + cstr);
            }
            else
            {
                //var builder = new ConfigurationBuilder()
                //       .SetBasePath(Directory.GetCurrentDirectory())
                //       .AddJsonFile("appsettings.json");

                //var config = builder.Build();
                //return "" + config.GetSection("KeyConvert:" + cstr).Value.ToString();

                return "idxpartners";
            }
        }

        public string conString()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_lms");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_lms").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string geIntegrationSetting(string module)
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM("IntegrationSetting:" + module);
            }
            else
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var config = builder.Build();
                return "" + config.GetSection("IntegrationSetting:" + module).Value.ToString();
            }
        }

        public string conStringLMS()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_lms");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_lms").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string conStringCust()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_cust");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_cust").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string conStringEN()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_en");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_en").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string conStringLog()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_core");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_core").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string conStringEtl()
        {
            var repPass = string.Empty;
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                var configPass = lc.decrypt(GetAppConfigGSM("configPass:passwordDB"));
                var configDB = GetAppConfigGSM("DbContextSettings:ConnectionString_reportetl");
                repPass = configDB.Replace("{pass}", configPass);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                repPass = config.GetSection("DbContextSettings:ConnectionString_reportetl").Value.ToString();

                if (repPass.Contains("{usrId}") && repPass.Contains("{pass}") && repPass.Contains("{host}") && repPass.Contains("{port}"))
                {
                    repPass = repPass.Replace("{usrId}", EpvCredentialModel.username_1)
                                       .Replace("{pass}", EpvCredentialModel.password_1)
                                       .Replace("{host}", EpvCredentialModel.host_1)
                                       .Replace("{port}", EpvCredentialModel.port_1);
                }
            }

            return repPass;
        }

        public string getAppSettingParam(string group, string api)
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM(group + ":" + api);
            }
            else
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var config = builder.Build();
                return "" + config.GetSection(group + ":" + api).Value.ToString();
            }
        }

        public string domainGetTokenCredential(string param)
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM("TokenAuthentication:" + param);
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                return config.GetSection("TokenAuthentication:" + param).Value.ToString();
            }
        }
        public string domainGetApi(string api)
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM("APISettings:" + api);
            }
            else
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var config = builder.Build();
                return "" + config.GetSection("APISettings:" + api).Value.ToString();
            }
        }

        //public string domainGetApi()
        //{
        //    var builder = new ConfigurationBuilder()
        //           .SetBasePath(Directory.GetCurrentDirectory())
        //           .AddJsonFile("appsettings.json");

        //    var config = builder.Build();
        //    return "" + config.GetSection("DomainSettings:urlGetDomainAPI").Value.ToString();
        //}

        public string domainPostApi()
        {
            if (Convert.ToBoolean(ConfigGSM("switcher")) == true)
            {
                return GetAppConfigGSM("DomainSettings:urlPostDomainAPI");
            }
            else
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var config = builder.Build();
                return "" + config.GetSection("DomainSettings:urlPostDomainAPI").Value.ToString();
            }
        }

        internal void CallAPiRequestLogs(string module, string code, string method, string path, string header, string body)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            string filename = "req_" + code + ".txt";
            string split = "|";

            string resHeader = "Timestamp" + split + "Method" + split + "Path" + split + "Header" + split + "Body";
            string resBody = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + split + method + split + path + split + (header).Replace("\r\n", "") + split + (body).Replace("\r\n", "");
            string folder = "Files/logs/" + today + "/callotherapi/" + code + "/";

            string txtPath = Path.GetFullPath(folder);

            if (!Directory.Exists(txtPath))
            {
                Directory.CreateDirectory(txtPath);
            }

            string txt = resHeader + Environment.NewLine + resBody;
            if (!File.Exists(txtPath + filename))
            {
                txt = resHeader + Environment.NewLine + resBody;
            }
            else
            {
                txt = resBody;
            }
            //insert txt file
            var logWriter = new System.IO.StreamWriter(txtPath + filename, append: true);
            logWriter.WriteLine(txt);
            logWriter.Dispose();

        }

        public void CallAPiResponseLogs(string module, string code, string method, string path, string header, string body, string result)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            string filename = "res_" + code + ".txt";
            string split = "|";
            string folder = "Files/logs/" + today + "/callotherapi/" + code + "/";

            string resHeader = "Timestamp" + split + "Method" + split + "Path" + split + "Header" + split + "Request" + split + "Response";
            string resBody = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + split + method + split + path + split + (header).Replace("\r\n", "") + split +
                (body).Replace("\r\n", "") + split + (result).Replace("\r\n", "");

            string txtPath = Path.GetFullPath(folder);

            if (!Directory.Exists(txtPath))
            {
                Directory.CreateDirectory(txtPath);
            }

            string txt = resHeader + Environment.NewLine + resBody;
            if (!File.Exists(txtPath + filename))
            {
                txt = resHeader + Environment.NewLine + resBody;
            }
            else
            {
                txt = resBody;
            }
            //insert txt file
            var logWriter = new System.IO.StreamWriter(txtPath + filename, append: true);
            logWriter.WriteLine(txt);
            logWriter.Dispose();

        }


        public string getOtherSetup(string module)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            return "" + config.GetSection("OthersSetup:" + module).Value.ToString();
        }

        public void CallLogRequest(string body, string result)
        {
            try
            {
                var date = DateTime.Now;
                var today = date.ToString("yyyy-MM-dd");
                var today2 = date.ToString("yyyyMMdd_HHmmss");
                string filename = "payment_overbooking_request_log.txt";
                string folder = "Files/logs/" + today + "/overbooking/";
                string split = "|";
                string resHeader = "Timestamp" + split + "Request" + split + "Response";
                string resBody = date.ToString("yyyy-MM-dd HH:mm:ss") + (body).Replace("\r\n", "") + split + (result).Replace("\r\n", "");

                string txtPath = Path.GetFullPath(folder);
                if (!Directory.Exists(txtPath))
                {
                    Directory.CreateDirectory(txtPath);
                }

                string txt = resHeader + Environment.NewLine + resBody;
                if (!File.Exists(txtPath + filename))
                {
                    txt = resHeader + Environment.NewLine + resBody;
                }
                else
                {
                    txt = resBody;
                }
                //insert txt file
                var logWriter = new System.IO.StreamWriter(txtPath + filename, append: true);
                logWriter.WriteLine(txt);
                logWriter.Dispose();

            }
            catch (Exception)
            {



            }


        }

        public void ErrDbLog(string spname, string constring, string parameters, string response)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            string filename = "errdb_log_" + today + ".txt";
            string split = "|";

            string resHeader = "Date" + split + "SP Name" + split + "Constring" + split + "Parameters" + split + "Response";
            string resBody = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + split + spname + split + constring + split + parameters + split + response;

            string txtPath = Path.GetFullPath("Logs/DB/");

            if (!Directory.Exists(txtPath))
            {
                Directory.CreateDirectory(txtPath);
            }

            string txt = resHeader + Environment.NewLine + resBody;
            if (!File.Exists(txtPath + filename))
            {
                txt = resHeader + Environment.NewLine + resBody;
            }
            else
            {
                txt = resBody;
            }
            //insert txt file
            var logWriter = new System.IO.StreamWriter(txtPath + filename, append: true);
            logWriter.WriteLine(txt);
            logWriter.Dispose();

        }
    }
}
