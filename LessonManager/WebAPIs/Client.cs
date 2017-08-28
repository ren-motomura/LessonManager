using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private HttpClient client;

        private Client()
        {
            client = new HttpClient();
        }
    }
}
