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
    class Staff
    {
        public static async Task<Result<List<Models.Staff>>> GetAll()
        {
            var responseMessage = await Client.Instance.Request("SelectStaffs", new byte[] { }).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<List<Models.Staff>>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = SelectStaffsResponse.Parser.ParseFrom(responseDataStream.ToArray());
            var staffs = res.Staffs.Select(s =>
            {
                var staff = new Models.Staff();
                staff.ID = s.Id;
                staff.Name = s.Name;
                staff.ImageLink = s.ImageLink;
                return staff;
            }).ToList();

            return new Result<List<Models.Staff>>(
                true,
                staffs,
                null
            );
        }

        public static async Task<Result<Models.Staff>> Create(string name, string imageLink)
        {
            var req = new CreateStaffRequest();
            req.Name = name;
            req.ImageLink = imageLink;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateStaff", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Staff>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateStaffResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var staff = new Models.Staff();
            staff.ID = res.Staff.Id;
            staff.Name = res.Staff.Name;
            staff.ImageLink = res.Staff.ImageLink;

            return new Result<Models.Staff>(
                true,
                staff,
                null
            );
        }

        public static async Task<Result<Models.Staff>> Update(int id, string imageLink)
        {
            var req = new UpdateStaffRequest();
            req.Staff = new Protobufs.Staff();
            req.Staff.Id = id;
            req.Staff.ImageLink = imageLink;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("UpdateStaff", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Staff>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = UpdateStaffResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var staff = new Models.Staff();
            staff.ID = res.Staff.Id;
            staff.Name = res.Staff.Name;
            staff.ImageLink = res.Staff.ImageLink;

            return new Result<Models.Staff>(
                true,
                staff,
                null
            );
        }

        public static async Task<Result<bool>> Delete(int id)
        {
            var req = new DeleteStaffRequest();
            req.Id = id;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("DeleteStaff", reqData).ConfigureAwait(false);
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
