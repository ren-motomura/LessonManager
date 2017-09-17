using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Protobufs;
using Google.Protobuf;
using LessonManager.Utils;

namespace LessonManager.WebAPIs
{
    class Session
    {
        public static async Task<Result<Models.Company>> Create(string emailAddress, string password)
        {
            var req = new CreateSessionRequest();
            req.EmailAddress = emailAddress;
            req.Password = password;

            var reqData = req.ToByteArray();

            HttpResponseMessage responseMessage;
            try
            {
                responseMessage = await Client.Instance.Request("CreateSession", reqData).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                return new Result<Models.Company>(
                    false,
                    null,
                    new FailData( // TODO: エラーの作り方を考え直したほうが良いかも
                        System.Net.HttpStatusCode.InternalServerError, new ErrorResponse()
                    )
                );
            }

            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Company>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateSessionResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var company = new Models.Company();
            company.Id = res.Company.Id;
            company.Name = res.Company.Name;
            company.EmailAddress = res.Company.EmailAddress;
            company.CreatedAt = Time.TimestampToDateTime(res.Company.CreatedAt);
            company.ImageLink = res.Company.ImageLInk;

            return new Result<Models.Company>(
                true,
                company,
                null
            );
        }

        public static void Remove()
        {
            Client.Instance.RemoveSession();
        }
    }
}
