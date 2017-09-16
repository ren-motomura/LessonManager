using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using LessonManager.Utils;
using Protobufs;
using Google.Protobuf;
using System.Linq;

namespace LessonManager.WebAPIs
{
    class Studio
    {
        public static async Task<Result<List<Models.Studio>>> GetAll()
        {
            var responseMessage = await Client.Instance.Request("SelectStudios", new byte[] { }).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<List<Models.Studio>>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = SelectStudiosResponse.Parser.ParseFrom(responseDataStream.ToArray());
            var studios = res.Studios.Select(s =>
            {
                var studio = new Models.Studio();
                studio.ID = s.Id;
                studio.Name = s.Name;
                studio.Address = s.Address;
                studio.PhoneNumber = s.PhoneNumber;
                return studio;
            }).ToList();

            return new Result<List<Models.Studio>>(
                true,
                studios,
                null
            );
        }

        public static async Task<Result<Models.Studio>> Create(string name, string address, string phoneNumber)
        {
            var req = new CreateStudioRequest();
            req.Name = name;
            req.Address = address;
            req.PhoneNumber = phoneNumber;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateStudio", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Studio>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateStudioResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var studio = new Models.Studio();
            studio.ID = res.Studio.Id;
            studio.Name = res.Studio.Name;
            studio.Address = res.Studio.Address;
            studio.PhoneNumber = res.Studio.PhoneNumber;

            return new Result<Models.Studio>(
                true,
                studio,
                null
            );
        }
    }
}
