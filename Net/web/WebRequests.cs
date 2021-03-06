﻿using System.IO;
using System.Net;
using System.Text;

namespace Furcadia.Net.Web

{
    /// <summary>
    ///
    /// </summary>
    public class WebRequests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequests"/> class.
        /// </summary>
        public WebRequests()
        {
            oCookies = null;
        }

        #region Private Fields

        private static CookieCollection oCookies;

        #endregion Private Fields

        private static string userAgent = "FurcaddiaFramework";

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public static string UserAgent { get => userAgent; set => userAgent = value; }

        #region Public Methods

        /// <summary>
        /// Sends the post data request to a web server.
        /// </summary>
        /// <param name="PostData">The post data.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string SendPostRequest(string PostData, string url)
        {
            var message = new StringBuilder();
            var LoginWebRequest = (HttpWebRequest)WebRequest.Create(url);
            var postData = new StringBuilder();
            var PostDataEncoding = Encoding.GetEncoding(1252);

            message.AppendLine("postData:");
            message.AppendLine(PostData);

            byte[] PostArray = PostDataEncoding.GetBytes(postData.ToString());

            // *** Set any header related and operational properties
            LoginWebRequest.Method = "POST";
            LoginWebRequest.UserAgent = UserAgent;
            LoginWebRequest.ContentType = "application/x-www-form-urlencoded";

            // Is this how we handle PostData KeyValues?
            Stream PostDataStream = LoginWebRequest.GetRequestStream();
            PostDataStream.Write(PostArray, 0, PostArray.Length);
            PostDataStream.Close();

            // *** reuse cookies if available
            LoginWebRequest.CookieContainer = new CookieContainer();

            if (oCookies != null && oCookies.Count > 0)
            {
                LoginWebRequest.CookieContainer.Add(oCookies);
            }
            try //If we did things right, we'll see XML data
            {
                using (HttpWebResponse response = LoginWebRequest.GetResponse() as HttpWebResponse)

                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    message.Append(reader.ReadToEnd());
                }
            }
            catch (WebException ex)
            {
                message.AppendLine("Logon Failed");
                message.AppendLine("Exception Message :" + ex.Message);
                if (ex.Response != null)
                    for (int i = 0; i < ex.Response.Headers.Count; ++i)
                        message.AppendLine(string.Format("Header Name:{0}, Header value :{1}", ex.Response.Headers.Keys[i], ex.Response.Headers[i]));

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    message.AppendLine(string.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                    message.AppendLine(string.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        message.Append(reader.ReadToEnd().Replace("\n", "\r\n"));
                    }
                }
            }
            return message.ToString();
        }

        #endregion Public Methods
    }
}