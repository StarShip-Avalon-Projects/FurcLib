using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Furcadia.Net.Web
{
    /// <summary>
    /// Basic Webpage Task interface.
    /// </summary>
    public sealed class NetWeb
    {
        private Uri Url;

        /// <summary>
        /// Initialize the Class with a default URL
        /// </summary>
        /// <param name="url"></param>
        public NetWeb(string url)
        {
            Url = new Uri(url);
        }

        private Action<int> ReportProgress;

        private async Task<List<string>> GetWebPageAsync(Uri Url, Progress<int> progressIndicator, CancellationToken CancelTime)
        {
            var request = await Task.Run(() =>
            {
                // WebRequest.Create freezes??
                var wr = WebRequest.Create(Url);
                wr.Method = "POST";
                return wr;
            });

            // ...

            using (var ss = await request.GetRequestStreamAsync())
            {
                int offset = 0;
                int count = 0;
                CancellationToken token = default(CancellationToken);
                byte[] buffer = null;
                await ss.WriteAsync(buffer, offset, count, token);
            }
            var message = new List<string>();
            using (var rr = await request.GetResponseAsync())
                try //If we did things right, we'll see XML data
                {
                    using (var response = rr.GetResponseStream())

                    {
                        StreamReader reader = new StreamReader(response);

                        message.AddRange(reader.ReadToEnd().ToString().Split('\n'));
                    }
                }
                catch (WebException ex)
                {
                    message.Add("Logon Failed");
                    message.Add("Exception Message :" + ex.Message);
                    if (ex.Response != null)
                        for (int i = 0; i < ex.Response.Headers.Count; ++i)
                            message.Add(string.Format("Header Name:{0}, Header value :{1}", ex.Response.Headers.Keys[i], ex.Response.Headers[i]));

                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        message.Add(string.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                        message.Add(string.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                        using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                        {
                            message.Add(reader.ReadToEnd().Replace("\n", "\r\n"));
                        }
                    }
                }
            return message;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> PostData()
        {
            var progressIndicator = new Progress<int>(ReportProgress);
            var cts = new CancellationTokenSource(30000); // cancel in 30s (optional)
            var result = await Task.Run(() =>
     GetWebPageAsync(Url, progressIndicator, cts.Token), cts.Token);
            return result;
        }
    }
}