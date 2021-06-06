using GeoAuth_Location;
using HRLogixMobileLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HRLogixMobileLib
{

    public class objTimeLog
    {
        public string ReferenceId { get; set; }
        public DateTime timeLog { get; set; }
        public string action { get; set; }
        public int areaId { get; set; }
        public string areaName { get; set; }

        public tblEmployee_TimeLog TimeLog()
        {
            tblEmployee_TimeLog ret = new tblEmployee_TimeLog
            {
                LogId = -1,
                TimeLog = timeLog,
                EntryType = "GeoLocation",
                Mode = action == "in" ? "In" : "Out",
                DeviceReference = String.Format(Constants.GEO_REFERENCE_FORMAT, areaId, areaName)
            };

            return ret;
        }
    }

    public class LoginResult : queryResult
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenIssued { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        public string ErrType { get; set; }
    }

    public static class APIManager
    {
        public static LoginResult Login(string url, string userName, string password)
        {
            LoginResult result = new LoginResult();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", userName);
            parameters.Add("password", password);
            parameters.Add("grant_type", "password");

            try
            {
                string content = GetString(url, HttpMethod.Post, null, parameters);

                if (!string.IsNullOrEmpty(content))
                {
                    JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
                    result.AccessToken = jwtDynamic.Value<string>("access_token");

                    if (!string.IsNullOrEmpty(result.AccessToken))
                    {
                        result.IsSuccessful = true;
                        result.AccessTokenIssued = jwtDynamic.Value<DateTime>(".issued");
                        result.AccessTokenExpiry = jwtDynamic.Value<DateTime>(".expires");
                        result.ServerTime = result.AccessTokenIssued;
                    }
                    else
                    {
                        result.Err = jwtDynamic.Value<string>("error_description");
                        result.ErrType = jwtDynamic.Value<string>("error");
                    }
                }
                else
                {
                    result.Err = "Incorrect data format returned";
                }
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static queryResult PostMultipartContent(string url, MultipartFormDataContent mContent, string accessToken = null)
        {
            queryResult result = new queryResult();

            try
            {
                string content = GetString(url, HttpMethod.Post, accessToken, null, mContent);
                result = parseContent(content);
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static queryResult GetQueryResult(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null)
        {
            queryResult result = new queryResult();

            try
            {
                string content = GetString(url, httpMethod, accessToken, parameters);
                result = parseContent(content);
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static string GetString(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null, MultipartFormDataContent mContent = null)
        {
            try
            {
                var response = GetResponse(url, httpMethod, accessToken, parameters, mContent);

                var t = Task.Run(() => response.Content.ReadAsStringAsync());
                return t.Result;
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0} request to {1} thrown an error: {2}",
                    httpMethod == HttpMethod.Post ? "POST" : "GET",
                    url,
                    ex.Message);

                throw new Exception(msg);
            }
        }

        public static HttpResponseMessage GetResponse(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null, 
            MultipartFormDataContent mContent = null, bool useGetAsync = false)
        {
            var client = new HttpClient();

            if (accessToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var request = new HttpRequestMessage(httpMethod, url);
            if (mContent != null)
            {
                request.Content = mContent;
            }
            else if (parameters != null)
            {
                request.Content = new FormUrlEncodedContent(parameters.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
            }

            Task<HttpResponseMessage> t;

            if (useGetAsync)
            {
                if (parameters != null)
                {
                    string p = string.Join("&", parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
                    url += (string.IsNullOrEmpty(p) ? "" : "?" + p);
                }

                t = Task.Run(() => client.GetAsync(url));
            }
            else
            {
                t = Task.Run(() => client.SendAsync(request));               
            }

            t.Wait();
            return t.Result;
        }

        public static async Task<LoginResult> LoginAsync(string url, string userName, string password)
        {
            LoginResult result = new LoginResult();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", userName);
            parameters.Add("password", password);
            parameters.Add("grant_type", "password");

            try
            {
                string content = await GetStringAsync(url, HttpMethod.Post, null, parameters);

                if (!string.IsNullOrEmpty(content))
                {
                    JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
                    result.AccessToken = jwtDynamic.Value<string>("access_token");

                    if (!string.IsNullOrEmpty(result.AccessToken))
                    {
                        result.IsSuccessful = true;
                        result.AccessTokenIssued = jwtDynamic.Value<DateTime>(".issued");
                        result.AccessTokenExpiry = jwtDynamic.Value<DateTime>(".expires");
                        result.ServerTime = result.AccessTokenIssued;
                    }
                    else
                    {
                        result.Err = jwtDynamic.Value<string>("error_description");
                        result.ErrType = jwtDynamic.Value<string>("error");
                    }
                }
                else
                {
                    result.Err = "Incorrect data format returned";
                }
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static async Task<queryResult> GetQueryResultAsync(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null)
        {
            queryResult result = new queryResult();

            try
            {
                string content = await GetStringAsync(url, httpMethod, accessToken, parameters);
                result = parseContent(content);
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static async Task<string> GetStringAsync(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null, MultipartFormDataContent mContent = null)
        {
            try
            {
                var response = await GetResponseAsync(url, httpMethod, accessToken, parameters, mContent);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                string msg = string.Format("{0} request to {1} thrown an error: {2}",
                    httpMethod == HttpMethod.Post ? "POST" : "GET",
                    url,
                    ex.Message);

                throw new Exception(msg);
            }
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(string url, HttpMethod httpMethod, string accessToken = null, Dictionary<string, string> parameters = null,
            MultipartFormDataContent mContent = null, bool useGetAsync = false)
        {
            var client = new HttpClient();

            if (accessToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var request = new HttpRequestMessage(httpMethod, url);
            if (mContent != null)
            {
                request.Content = mContent;
            }
            else if (parameters != null)
            {
                request.Content = new FormUrlEncodedContent(parameters.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
            }

            HttpResponseMessage t;

            if (useGetAsync)
            {
                if (parameters != null)
                {
                    string p = string.Join("&", parameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
                    url += (string.IsNullOrEmpty(p) ? "" : "?" + p);
                }

                t = await client.GetAsync(url);
            }
            else
            {
                t = await client.SendAsync(request);
            }

            return t;
        }

        public static async Task<queryResult> PostMultipartContentAsync(string url, MultipartFormDataContent mContent, string accessToken = null)
        {
            queryResult result = new queryResult();

            try
            {
                string content = await GetStringAsync(url, HttpMethod.Post, accessToken, null, mContent);
                result = parseContent(content);
            }
            catch (Exception ex)
            {
                result.IsUnreachable = true;
                result.Err = ex.Message;
            }

            return result;
        }

        public static queryResult parseContent(string content)
        {
            queryResult result = new queryResult();

            if (Procs.IsJson(content))
            {
                result = JsonConvert.DeserializeObject<queryResult>(content, new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Local });
            }
            else
            {
                result = new queryResult
                {
                    Err = "Incorrect data format returned"
                };
            }

            return result;
        }
    }

    public class InfoData
    {
        public bool IsOfflineData { get; set; }
    }

    public class UserInfo : InfoData
    {
        public tblEmployee Employee { get; set; }
        public tblEmployee_Career Career { get; set; }
        public tblEmployee_Info Info { get; set; }
        public bool AllowGeoLocation { get; set; }
        public List<tblGeoAuth_Area> Areas { get; set; }
    }

    public class CompanySettings : InfoData
    {
        public byte[] imageArray { get; set; }
        public int width { get; set; } = 150;
        public int height { get; set; } = 150;
        public int x { get; set; } = -1;
        public int y { get; set; } = -1;
        public int aspect { get; set; } = 1;
        public string bgColor { get; set; }
        public int bgImageHeight { get; set; } = 100;
        public string companyName { get; set; }
        public bool showCompanyName { get; set; }
    }

    public class UserPhoto : InfoData
    {
        public byte[] imageArray { get; set; }
    }

    public class TimeLogsInfo : InfoData
    {
        public DateTime sDt { get; set; }
        public DateTime eDt { get; set; }
        public List<tblEmployee_TimeLog> ServerTimeLogs { get; set; }
        public List<tblEmployee_TimeLog> OfflineTimeLogs { get; set; }

        public List<tblEmployee_TimeLog> TimeLogs
        {
            get
            {
                List<tblEmployee_TimeLog> tmp = new List<tblEmployee_TimeLog>();

                if (ServerTimeLogs != null)
                {
                    tmp.AddRange(ServerTimeLogs);
                }

                if (OfflineTimeLogs != null)
                {
                    tmp.AddRange(OfflineTimeLogs);
                }
                

                return tmp;
            }
        }
    }

    public class OnlineApplicationsModel : InfoData
    {
        public int Leave { get; set; }
        public int OT { get; set; }
        public int Travel { get; set; }

        public int Total
        {
            get
            {
                return Leave + OT + Travel;
            }
        }

        public string Greetings { get; set; }

        public Dictionary<string, string> Greetings_Details { get; set; } = new Dictionary<string, string>();

        public string GetString(int count, string format = "{0} submitted online application{1}")
        {
            return string.Format(format,
                count == 0 ? "No" : count.ToString(),
                count <= 1 ? "" : "s"
                );
        }

        public OnlineApplicationsModel()
        {
            Leave = 0;
            OT = 0;
            Travel = 0;
        }
    }

    public class queryResult
    {
        public bool IsSuccessful { get; set; }
        public object Data { get; set; }
        public string Remarks { get; set; }
        public string Err { get; set; }
        public DateTime ServerTime { get; set; } = default(DateTime);
        public bool IsUnreachable { get; set; }
    }
}
