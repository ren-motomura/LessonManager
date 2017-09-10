using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;

namespace LessonManager.WebAPIs
{
    class Client
    {
        private static Client instance = new Client();

        public static Client Instance
        {
            get { return instance; }
        }

        //private const string host = "https://third-being-175805.appspot.com/"; // 設定ファイルに切り出す？
        private const string host = "http://localhost:8080/"; // 設定ファイルに切り出す？
        private const string funcNameHeader = "X-Lessonmanager-Funcname";

        private HttpClient client;

        private Client()
        {
            client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(5000) };
            client.BaseAddress = new Uri(host);
        }

        public async Task<HttpResponseMessage> Request(string funcName, byte[] data)
        {
            var cli = client;
            var req = new HttpRequestMessage(HttpMethod.Post, "");
            req.Headers.Add(funcNameHeader, funcName);
            // TODO: session
            req.Content = new ByteArrayContent(data);

            var responseMessage = await cli.SendAsync(req);

            IEnumerable<string> cookies;
            if (responseMessage.Headers.TryGetValues("Set-Cookie", out cookies))
            {
                client.DefaultRequestHeaders.Remove("Cookie");
                client.DefaultRequestHeaders.Add("Cookie", cookies.First());
            }

            return responseMessage;
        }

        public void RemoveSession()
        {
            client.DefaultRequestHeaders.Remove("Cookie");
        }
    }
}
