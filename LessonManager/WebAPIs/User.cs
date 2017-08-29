using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Models;
using Protobufs;
using Google.Protobuf;

namespace LessonManager.WebAPIs
{
    class User
    {
        public static async Task<Models.User> Create(string name, string emailAddress, string password)
        {
            var req = new CreateUserRequest();
            req.Name = name;
            req.EmailAddress = emailAddress;
            req.Password = password;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateUser", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);
            var res = CreateUserResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var user = new Models.User();
            user.Id = res.Id;
            user.Name = res.Name;
            user.EmailAddress = res.EmailAddress;

            return user;
        }
    }
}
