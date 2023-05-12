using RestSharp;
using Senjyouhara.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Senjyouhara.Common.Utils
{

    public class Progress
    {
        public double Total { get; set; }

        public double Loading { get; set; }

        public double Speed { get; set; }
        public double Percent { get; set; }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("Progress { " + nameof(Total) + " ")
                .Append(Total + " ,")
                .Append(nameof(Loading) + " ")
                .Append(Loading + " ,")
                .Append(nameof(Speed) + " ")
                .Append(Speed + " ,")
                .Append(nameof(Percent) + " ")
                .Append(Percent + " }");
            return s.ToString();
        }
    }

    public delegate void CancelToken();

    public class HttpClientService
    {
        private string domain = "http://localhost:8080/";
        private RestClient client;
        private HttpClientOptions clientOptions = new HttpClientOptions();

        private CancelToken cancelToken = () => {
            throw new Exception("中断操作");
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
                string jsonObject = JSONUtil.ToJSON(baseRequest.Data);
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
            baseRequest.Method = Method.GET;
            return ExecuteAsync<T>(baseRequest);
        }


        public Task<ResBase<T>> PostExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.POST;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }
        public Task<ResBase<T>> PutExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.PUT;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }

        public Task<ResBase<T>> DeleteExecuteAsync<T>(BaseRequest baseRequest)
        {
            baseRequest.Method = Method.DELETE;
            baseRequest.Headers.Add("Content-Type", "application/json");
            return ExecuteAsync<T>(baseRequest);
        }

        public async Task<ResBase<object>> DownloadExecuteAsync(BaseRequest baseRequest, string savePath, Action<Progress, CancelToken> cb)
        {
            return  await Task.Run(() => {
                var request = new RestRequest(baseRequest.Url, baseRequest.Method);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Content-Type", "application/json");
                ParamsHandler(request, baseRequest);

                try
                {
                    request.AdvancedResponseWriter = (input, response) => ReadAsBytes(input, response, savePath, cb);
                    client.DownloadData(request, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }

                var res = new ResBase<object>();
                res.Success = true;
                res.Msg = "下载成功";
                return res;
            });
        }

        private void ReadAsBytes(Stream input,IHttpResponse response, string savePath, Action<Progress, CancelToken> cb)
        {
            double size = response.ContentLength;
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                try
                {
                    double progressBarValue = 0;
                    double speed = 0;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                        progressBarValue += read;
                        double percent = (Math.Round(progressBarValue / size, 6) * 100);
                        double DownloadFileSize = Math.Round(progressBarValue / 1024 / 1024, 2);
                        speed = read;
                        Thread.Sleep(10);
                        var p = new Progress();
                        p.Total = size;
                        p.Percent = percent;
                        p.Speed = speed;
                        p.Loading = progressBarValue;
                        cb?.Invoke(p, cancelToken);
                    }
                    File.WriteAllBytes(savePath, ms.ToArray());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw ex;
                }
            };



            //using (var ms = new MemoryStream())
            //{
            //    int read;
            //    try
            //    {
            //        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            //        { ms.Write(buffer, 0, read); }

            //        return ms.ToArray();
            //    }
            //    catch (WebException ex)
            //    { return Encoding.UTF8.GetBytes(ex.Message); }
            //};

        }


        private ResBase<T> ResultHandler<T>(IRestResponse resp, BaseRequest baseRequest)
        {
            var res = new ResBase<T>();
            if (resp.StatusCode.Equals(HttpStatusCode.OK))
            {

                var obj = JSONUtil.ToData<ResBase<T>>(resp.Content);
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
