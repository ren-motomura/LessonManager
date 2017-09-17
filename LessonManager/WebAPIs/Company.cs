using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Models;
using Protobufs;
using Google.Protobuf;
using LessonManager.Utils;

namespace LessonManager.WebAPIs
{
    class Company
    {
        public static async Task<Result<Models.Company>> Create(string name, string emailAddress, string password)
        {
            var req = new CreateCompanyRequest();
            req.Name = name;
            req.EmailAddress = emailAddress;
            req.Password = password;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateCompany", reqData).ConfigureAwait(false);
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

            var res = CreateCompanyResponse.Parser.ParseFrom(responseDataStream.ToArray());

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

        public static async Task<Result<Models.Company>> SetImageLink(string imageLink)
        {
            var req = new SetCompanyImageLinkRequest();
            req.ImageLink = imageLink;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("SetCompanyImageLink", reqData).ConfigureAwait(false);
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

            var res = CreateCompanyResponse.Parser.ParseFrom(responseDataStream.ToArray());

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

        public static async Task<Result<Models.Company>> SetPassword(string password)
        {
            var req = new SetCompanyPasswordRequest();
            req.Password = password;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("SetCompanyPassword", reqData).ConfigureAwait(false);
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

            var res = SetCompanyPasswordResponse.Parser.ParseFrom(responseDataStream.ToArray());

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
    }
}
