using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protobufs;
using Google.Protobuf;

namespace LessonManager.WebAPIs
{
    class Session
    {
        public static async Task<Result<Boolean>> Create(string emailAddress, string password)
        {
            var req = new CreateSessionRequest();
            req.EmailAddress = emailAddress;
            req.Password = password;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateSession", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Boolean>(
                    false,
                    false,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateSessionResponse.Parser.ParseFrom(responseDataStream.ToArray());

            return new Result<Boolean>(
                true,
                true,
                null
            );
        }
    }
}
