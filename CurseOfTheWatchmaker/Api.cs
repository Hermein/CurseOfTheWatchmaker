/*Copyright Hermein Developer
 License CC BY-SA 4.0 
 23.04.2018 */
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CurseOfTheWatchmaker
{
    public static class Api
    {
        public static string Post(string uri, string post)
        {
            var html = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "POST";
                request.Headers.Add("X-Requested-With", "ShockwaveFlash/29.0.0.140");
                request.Headers.Add("Origin", "https://static3.belka-games.com");
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36 OPR/52.0.2871.64";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                request.Referer = "https://static3.belka-games.com/wcm/vk/mcmwebloader.6a07a5fe.swf";
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate,sdch");
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                request.ServicePoint.Expect100Continue = false;
                request.AllowAutoRedirect = true;
                var encodedPostParams = Encoding.UTF8.GetBytes(post);
                request.ContentLength = encodedPostParams.Length;
                request.GetRequestStream().Write(encodedPostParams, 0, encodedPostParams.Length);
                request.GetRequestStream().Close();
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    if (responseStream != null)
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    if (responseStream != null)
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                if (responseStream != null)
                {
                    var sRead = new StreamReader(responseStream, Encoding.UTF8);
                    html = sRead.ReadToEnd();
                    sRead.Close();
                }
            }
            catch
            {

            }
            return html;
        }
        public static string Md5(string input)
        {
            return Md5(Encoding.UTF8.GetBytes(input));
        }
        private static string Md5(byte[] input)
        {
            return MD5.Create().ComputeHash(input).Aggregate("", (current, a) => current + a.ToString("x2"));
        }
        public static string Get(string url)
        {
            var html = string.Empty;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "*/*";
                request.UserAgent =
                    "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.204 Safari/534.16";
                request.ServicePoint.Expect100Continue = false;
                request.Headers.Add(HttpRequestHeader.AcceptCharset, "windows-1251,utf-8;q=0.7,*;q=0.3");
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate,gzip");
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                request.AllowAutoRedirect = true;
                request.Timeout = 60000;
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                    var sRead = new StreamReader(responseStream, Encoding.UTF8);
                    html = sRead.ReadToEnd();
                    sRead.Close();
                    sRead.Dispose();
                    responseStream.Close();
                    responseStream.Dispose();
                }
            }
            catch
            {

            }
            return html;
        }
        public static string Parse(string maska, string text)
        {
            try
            {
                var result = new Regex(maska).Match(text).Groups[1].Value;
                return result == "" ? "" : result;
            }
            catch
            {
                return "";
            }
        }

    }
}
