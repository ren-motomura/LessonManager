using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protobufs;

namespace LessonManager.WebAPIs
{
    class FailData
    {
        public HttpStatusCode Status;
        public ErrorResponse Body;

        public FailData(HttpStatusCode s, ErrorResponse b)
        {
            Status = s;
            Body = b;
        }
    }
}
