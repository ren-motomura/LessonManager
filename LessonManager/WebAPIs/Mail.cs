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
    class Mail
    {
        public static async Task<Result<bool>> Send(Models.Mail mail)
        {
            var req = new SendEmailRequest();
            req.ToAddresses.Add(mail.ToAddresses);
            req.Subject = mail.Subject;
            req.Body = mail.Body;
            req.Attachments.Add(
                from at in mail.Attachments
                select new MailAttachment() { Name = at.Name, Data = ByteString.CopyFrom(at.Data) }
            );
                              ;
            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("SendMail", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<bool>(
                    false,
                    false,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            return new Result<bool>(
                true,
                true,
                null
            );
        }
    }
}
