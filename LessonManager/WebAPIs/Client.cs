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
            client = new HttpClient();
            client.BaseAddress = new Uri(host);
        }

        public Task<HttpResponseMessage> Request(string funcName, byte[] data)
        {
            var cli = client;
            var req = new HttpRequestMessage(HttpMethod.Post, "");
            req.Headers.Add(funcNameHeader, funcName);
            // TODO: session
            req.Content = new ByteArrayContent(data);

            return cli.SendAsync(req)
                .ContinueWith((t) =>
                {
                    return t.Result;
                });

        }
    }
}
