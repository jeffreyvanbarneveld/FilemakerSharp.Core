using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using XmlTextReader = System.Xml.XmlReader;


namespace FilemakerSharp.Core
{
    internal class FMHttpHelper
    {
        public static int Timeout = 100000;

        /// <summary>
        /// Post XML to url
        /// </summary>
        /// <param name="url">The url</param>
        /// <param name="data">Post data</param>
        /// <param name="username">FM Username</param>
        /// <param name="password">FM Password</param>
        /// <returns></returns>
        public static async Task<XmlNode> PostXML(string url, string data, string username, string password)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);

            HttpClient cli = new HttpClient();


            cli.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)));


            var res = await cli.PostAsync(url, new StringContent(data));
            
            

            return TransformResponse(url, res.Content.ReadAsStreamAsync().Result);
        }

        private static XmlNode TransformResponse(string url, Stream input)
        {
            XmlTextReader reader = XmlTextReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });
            
            try
            {

                XmlDocument xml = new XmlDocument();
                xml.Load(reader);
                return xml.DocumentElement;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }
        }
    }
}
