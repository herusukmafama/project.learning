using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using project.learning.Libs;
using System.Net;
using System.IO;
using Npgsql;
using System.Data;
using System.Globalization;
using System.Text;
using System.Dynamic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace project.learning.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        private dbConn sqlConn = new dbConn();
        private lConvert lc = new lConvert();
        private int timeout = 5;



        public string execExtAPIPost(string api, string path, string json)
        {
            var WebAPIURL = sqlConn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var serviceProvider = new ServiceCollection().AddHttpClient()
            .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                    builder.PrimaryHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                    }))
            .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();

            

            return result;
        }

        public string execExtAPIGetWithToken(string api, string path, string json, string credential)
        {
            var WebAPIURL = sqlConn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);

            var serviceProvider = new ServiceCollection().AddHttpClient()
           .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
               options.HttpMessageHandlerBuilderActions.Add(builder =>
                   builder.PrimaryHandler = new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                   }))
           .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            HttpResponseMessage response = client.GetAsync(requestStr).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            client.Dispose();

            

            return result;
        }

        public string execExtAPIPostWithToken(string api, string path, string json, string credential)
        {
            string result = "";
            var WebAPIURL = sqlConn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            sqlConn.CallAPiRequestLogs("", api, "POST", path, "", json);


            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            //var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            ////contentData.Headers.Add("Authorization", credential);   

            //HttpResponseMessage response = client.PostAsync(requestStr, contentData).Result;
            //string result = response.Content.ReadAsStringAsync().Result;

            result = execExtAPIPostWithTokenAwait(api, path, json, credential).Result;

            sqlConn.CallAPiResponseLogs("", api, "POST", path, "", json, result);

            

            return result;
        }

        //public string getConnection(string spname, params string[] list)
        //{
        //    //var retObject = new List<dynamic>();
        //    string retObject = "";
        //    var parameter = spname;
        //    if (list != null && list.Count() > 0)
        //    {
        //        for (int i = 0; i < list.Count(); i++)
        //        {
        //            parameter += ";" + list[i];
        //        }
        //    }
        //    retObject = getDataFromApi(parameter);
        //    return retObject;
        //}

        //public string getDataFromApi(string parameter)
        //{
        //    var conn = sqlConn.domainGetApi();
        //    WebRequest request = WebRequest.Create(conn + parameter);
        //    WebResponse response = request.GetResponseAsync().Result;
        //    Stream dataStream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(dataStream);
        //    string responseFromServer = reader.ReadToEnd();
        //    return responseFromServer;
        //}

        public string postConnection(string spname, params string[] list)
        {
            //var retObject = new List<dynamic>();
            string retObject = "";
            var parameter = spname;
            if (list != null && list.Count() > 0)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    parameter += ";" + list[i];
                }
            }
            retObject = postDataFromApi(parameter);

            

            return retObject;
        }

        public string postDataFromApi(string parameter)
        {
            var conn = sqlConn.domainPostApi();
            WebRequest request = WebRequest.Create(conn + parameter);
            WebResponse response = request.GetResponseAsync().Result;
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            

            return responseFromServer;
        }

        public void execSqlWithExecption(string spname, params string[] list)
        {
            var conn = sqlConn.conString();
            string message = "";
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            //add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else if (pars[2] == "b")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }
                cmd.ExecuteNonQuery();
                message = "success";
                nconn.Close();
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            finally
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            //return message;
        }

        public string execSqlWithExecptionResult(string spname, params string[] list)
        {
            var conn = sqlConn.conString();
            string message = "";
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            //add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else if (pars[2] == "b")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                cmd.ExecuteNonQuery();
                message = "success";
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
            }
            finally
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return message;
        }

        public string execSqlWithExecptionResultEN(string spname, string split, params string[] list)
        {
            var conn = sqlConn.conStringEN();
            string message = "";
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            //add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(split);

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else if (pars[2] == "b")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                cmd.ExecuteNonQuery();
                message = "success";
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
            }
            finally
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return message;
        }

        public List<dynamic> getDataToObjectCust(string spname, params string[] list)
        {
            var conn = sqlConn.conStringCust();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {

                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split('|');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;

        }

        public List<dynamic> getDataToObjectCustWithOutSplit(string spname, params string[] list)
        {
            var conn = sqlConn.conStringCust();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {

                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;

        }

        public List<dynamic> getDataToObject(string spname, params string[] list)
        {
            var conn = sqlConn.conString();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {

                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;

        }

        public List<dynamic> getDataToObject2(string spname, params string[] list)
        {
            var conn = sqlConn.conString();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {

                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
                retObject = new List<dynamic>();
                dynamic row = new ExpandoObject();
                row.status = "Success";
                row.message = "Success";
                retObject.Add((ExpandoObject)row);
            }
            catch (Exception ex)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
                retObject = new List<dynamic>();
                dynamic row = new ExpandoObject();
                row.status = "Invalid";
                row.message = "Invalid (" + ex.Message + ").";
                retObject.Add((ExpandoObject)row);
            }

            

            return retObject;

        }

        public List<dynamic> getDataToObjectLMS(string spname, params string[] list)
        {
            var conn = sqlConn.conStringLMS();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;
        }

        public List<dynamic> getDataToObjectEN(string spname, params string[] list)
        {
            var conn = sqlConn.conStringEN();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;
        }

        public List<dynamic> getDataToObjectEN2(string spname, string splitStr, params string[] list)
        {
            var conn = sqlConn.conStringEN();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;

            // add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(splitStr);

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
                retObject = new List<dynamic>();
                dynamic row = new ExpandoObject();
                row.status = "Success";
                row.message = "Success";
                retObject.Add((ExpandoObject)row);
            }
            catch (Exception e)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
                retObject = new List<dynamic>();
                dynamic row = new ExpandoObject();
                row.status = "Invalid";
                row.message = "Invalid (" + e.Message + ").";
                retObject.Add((ExpandoObject)row);
            }

            

            return retObject;
        }

        public List<dynamic> GetDataObj(NpgsqlDataReader dr)
        {
            var retObject = new List<dynamic>();
            while (dr.Read())
            {
                var dataRow = new ExpandoObject() as IDictionary<string, object>;
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    dataRow.Add(
                           dr.GetName(i),
                           dr.IsDBNull(i) ? null : dr[i] // use null instead of {}
                   );
                }
                retObject.Add((ExpandoObject)dataRow);
            }

            

            return retObject;
        }

        public void execSqlWithSplitSemicolon(string spname, params string[] list)
        {
            var conn = sqlConn.conStringLog();
            string message = "";
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            nconn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            // add try catch
            try
            {

                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(';');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "b")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }

                cmd.ExecuteNonQuery();
                message = "success";
                nconn.Close();
            }
            catch (NpgsqlException e)
            {
                message = e.Message;
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            finally
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            //return message;
        }

        public List<dynamic> getDataToObjectEtl(string spname, params string[] list)
        {
            var conn = sqlConn.conStringEtl();
            StringBuilder sb = new StringBuilder();
            NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            nconn.Open();
            //NpgsqlTransaction tran = nconn.BeginTransaction();
            NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
            cmd.CommandType = CommandType.StoredProcedure;
            //add try catch
            try
            {
                if (list != null && list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var pars = item.Split(',');

                        if (pars.Count() > 2)
                        {
                            if (pars[2] == "i")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                            }
                            else if (pars[2] == "s")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                            }
                            else if (pars[2] == "d")
                            {
                                cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                            }
                            else if (pars[2] == "dt")
                            {
                                cmd.Parameters.AddWithValue(pars[0], DateTime.ParseExact(pars[1], "yyyy-MM-dd", CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                        }
                        else if (pars.Count() > 1)
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[1]);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue(pars[0], pars[0]);
                        }
                    }
                }


                NpgsqlDataReader dr = cmd.ExecuteReader();

                if (dr == null || dr.FieldCount == 0)
                {
                    if (nconn.State.Equals(ConnectionState.Open))
                    {
                        nconn.Close();
                    }
                    NpgsqlConnection.ClearPool(nconn);

                    

                    return retObject;
                }

                retObject = GetDataObj(dr);

                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }
            catch (Exception)
            {
                if (nconn.State.Equals(ConnectionState.Open))
                {
                    nconn.Close();
                }
                NpgsqlConnection.ClearPool(nconn);
            }

            

            return retObject;
        }

        public async Task<string> execExtAPIPostWithTokenAwait(string api, string path, string json, string credential)
        {
            #region call others api version : v.3
            string result = "";
            var WebAPIURL = sqlConn.domainGetApi(api);
            string requestStr = WebAPIURL + path;

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Authorization", credential);
            var serviceProvider = new ServiceCollection().AddHttpClient()
           .Configure<HttpClientFactoryOptions>("HttpClientWithSSLUntrusted", options =>
               options.HttpMessageHandlerBuilderActions.Add(builder =>
                   builder.PrimaryHandler = new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback = (m, crt, chn, e) => true
                   }))
           .BuildServiceProvider();
            var _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            HttpClient client = _httpClientFactory.CreateClient("HttpClientWithSSLUntrusted");
            client.BaseAddress = new Uri(requestStr);
            client.Timeout = TimeSpan.FromMinutes(timeout);
            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }
            else
            {
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", credential);
            }

            var contentData = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(requestStr, contentData);
            result = await response.Content.ReadAsStringAsync();
            client.Dispose();
            #endregion

            

            return result;
        }

        public string ExecuteQueryScriptEN(string dbprv, string qry)
        {
            string strout = "";
            StringBuilder sb = new StringBuilder();
            string conn = sqlConn.conStringEN();

            if (dbprv.ToLower() == "postgresql")
            {
                NpgsqlConnection nconn = new NpgsqlConnection(conn);
                nconn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(qry, nconn);
                cmd.CommandType = CommandType.Text;
                //add try catch
                try
                {
                    cmd.ExecuteNonQuery();
                    strout = "success";
                    
                    nconn.Close();
                    NpgsqlConnection.ClearPool(nconn);
                }
                catch(Exception ex)
                {
                    strout = ex.Message;
                    nconn.Close();
                    NpgsqlConnection.ClearPool(nconn);
                }
            }

            

            return strout;
        }

        public List<dynamic> ExecuteQueryScriptWithReturnEN(string dbprv, string qry)
        {
            List<dynamic> retObject = new List<dynamic>();
            StringBuilder sb = new StringBuilder();
            string conn = sqlConn.conStringEN();

            if (dbprv.ToLower() == "postgresql")
            {
                NpgsqlConnection nconn = new NpgsqlConnection(conn);
                nconn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(qry, nconn);
                cmd.CommandType = CommandType.Text;
                //add try catch
                try
                {
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    if (dr == null || dr.FieldCount == 0)
                    {
                        nconn.Close();
                        NpgsqlConnection.ClearPool(nconn);

                        

                        return retObject;
                    }

                    retObject = GetDataObj(dr);
                    nconn.Close();
                    NpgsqlConnection.ClearPool(nconn);
                }
                catch
                {
                    nconn.Close();
                    NpgsqlConnection.ClearPool(nconn);
                }
            }

            

            return retObject;
        }

        internal String getSingleValueEN(string table, string column_name, string clause)
        {
            string strVal = "", provider = "postgresql";
            JArray jaReturn = new JArray();
            JObject joReturn = new JObject();
            List<dynamic> retObject = new List<dynamic>();

            string strQry = "SELECT " + column_name + " FROM " + table + " WHERE " + clause + " ;";

            retObject = ExecuteQueryScriptWithReturnEN(provider, strQry);
            jaReturn = lc.convertDynamicToJArray(retObject);
            if (jaReturn.Count > 0)
            {
                strVal = jaReturn[0][column_name].ToString();
            }

            

            return strVal;
        }

        public List<dynamic> getDataToObjectDynamicSplit(string split, string dbname, string spname, params string[] list)
        {
            var conn = sqlConn.conString();
            //StringBuilder sb = new StringBuilder();
            //NpgsqlConnection nconn = new NpgsqlConnection(conn);
            var retObject = new List<dynamic>();

            using (var nconn = new NpgsqlConnection(conn))
            {
                try
                {
                    nconn.Open();
                    //NpgsqlTransaction tran = nconn.BeginTransaction();
                    NpgsqlCommand cmd = new NpgsqlCommand(spname, nconn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            var pars = item.Split(split);

                            if (pars.Count() > 2)
                            {
                                if (pars[2] == "i")
                                {
                                    cmd.Parameters.AddWithValue(pars[0], Convert.ToInt32(pars[1]));
                                }
                                else if (pars[2] == "s")
                                {
                                    cmd.Parameters.AddWithValue(pars[0], Convert.ToString(pars[1]));
                                }
                                else if (pars[2] == "d")
                                {
                                    cmd.Parameters.AddWithValue(pars[0], Convert.ToDecimal(pars[1]));
                                }
                                else if (pars[2] == "b")
                                {
                                    cmd.Parameters.AddWithValue(pars[0], Convert.ToBoolean(pars[1]));
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue(pars[0], pars[1]);
                                }
                            }
                            else if (pars.Count() > 1)
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[1]);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(pars[0], pars[0]);
                            }
                        }
                    }

                    NpgsqlDataReader dr = cmd.ExecuteReader();

                    if (dr == null || dr.FieldCount == 0)
                    {
                        //if (nconn.State.Equals(ConnectionState.Open))
                        //{
                        //    nconn.Close();
                        //}
                        NpgsqlConnection.ClearPool(nconn);
                        return retObject;
                    }

                    retObject = GetDataObj(dr);

                    //if (nconn.State.Equals(ConnectionState.Open))
                    //{
                    //    nconn.Close();
                    //}
                    NpgsqlConnection.ClearPool(nconn);
                }
                catch (Exception ex)
                {
                    //if (nconn.State.Equals(ConnectionState.Open))
                    //{
                    //    nconn.Close();
                    //}
                    NpgsqlConnection.ClearPool(nconn);
                    sqlConn.ErrDbLog(spname, conn, list != null ? list.ToString() : "", ex.Message);
                }
            }
            return retObject;
        }

    }
}
