using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Json;
using Senjyouhara.Common.Model;
using Senjyouhara.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Senjyouhara.Common.Utils
{
    public class HttpClientService
    {
        private string domain = "http://localhost:8080/";
        private RestClient client;
        private HttpClientOptions clientOptions = new HttpClientOptions();
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public HttpClientService()
        {
            this.client = new RestClient(domain);
        }

        public HttpClientService(string domain)
        {
            this.domain = domain;
            this.client = new RestClient(domain);
        }

        public Dictionary<string, string> getAuth()
        {
            Dictionary<string, string> HeaderAuth = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(GlobalSession.UserInfo?.Token))
            {
                HeaderAuth.Add("Authorization", "Bearer " + GlobalSession.UserInfo.Token);
            }

            return HeaderAuth;
        }

        private string UrlHandler(string url)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }
            else if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }
            return domain + url;
        }


        private void ParamsHandler(RestRequest request, BaseRequest baseRequest)
        {

            baseRequest.Url = UrlHandler(baseRequest.Url);

            if (baseRequest.QueryParams != null && baseRequest.QueryParams.Count > 0)
            {
                foreach (var q in baseRequest.QueryParams)
                {
                    request.AddQueryParameter(baseRequest.QueryParams[q.Key], baseRequest.QueryParams[q.Value]);
                }
            }

            if (baseRequest.Data != null)
            {
                string jsonObject = JsonConvert.SerializeObject(baseRequest.Data, Formatting.Indented, jsonSerializerSettings);
                request.AddParameter("", jsonObject, ParameterType.RequestBody);
            }

            if (baseRequest.Files != null && baseRequest.Files.Count > 0)
            {
                foreach (var q in baseRequest.Files)
                {
                    request.AddFile(baseRequest.Files[q.Key], baseRequest.Files[q.Value]);
                }
            }

            if (baseRequest.Headers != null && baseRequest.Headers.Count > 0)
            {
                request.AddHeaders(baseRequest.Headers);
            }

            var auth = getAuth();
            if (auth != null && auth.Count > 0)
            {
                request.AddHeaders(auth);

            }
        }

        public async Task<ResBase<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {

            var request = new RestRequest(baseRequest.Url, baseRequest.Method);
            ParamsHandler(request, baseRequest);
            //request.Method = baseRequest.Method;
            //request.Resource = baseRequest.Url;
            var result = await client.ExecuteAsync(request);
            var res = ResultHandler<T>(result, baseRequest);
            return res;

        }
        public Task<ResBase<T>> GetExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.Get;
            return ExecuteAsync<T>(baseRequest);
        }


        public Task<ResBase<T>> PostExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.Post;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }
        public Task<ResBase<T>> PutExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.Put;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }

        public Task<ResBase<T>> DeleteExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.Delete;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }

        public async Task<ResBase<T>> DownloadExecuteAsync<T>(BaseRequest baseRequest, string savePath)
        {
            var request = new RestRequest(baseRequest.Url, baseRequest.Method);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            ParamsHandler(request, baseRequest);

            byte[] bytes = await client.DownloadDataAsync(request);
            var res = new ResBase<T>();
            res.Msg = "下载成功";

            try
            {
                File.WriteAllBytes(savePath, bytes);
            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
                res.Success = false;
            }
            return res;
        }


        private ResBase<T> ResultHandler<T>(RestResponse resp, BaseRequest baseRequest)
        {
            var res = new ResBase<T>();
            if (resp.StatusCode.Equals(HttpStatusCode.OK))
            {

                var obj = JsonConvert.DeserializeObject<ResBase<T>>(resp.Content, jsonSerializerSettings);
                if (obj == null)
                {
                    res.Msg = "服务器出错！";
                    if (baseRequest.Options.IsShowErrorTips)
                    {
                        //MessageBox.Show("消息提示", res.Msg, MessageBoxButton.OK);
                    }
                    return res;
                }

                if (!obj.Success)
                {
                    if (baseRequest.Options.IsShowErrorTips)
                    {
                        //MessageBox.Show("消息提示", res.Msg, MessageBoxButton.OK);
                    }
                }

                return obj;

            }

            res.Msg = "服务器出错！";
            if (baseRequest.Options.IsShowErrorTips)
            {
                //MessageBox.Show("消息提示", res.Msg, MessageBoxButton.OK);
            }
            return res;
        }

    }
}
