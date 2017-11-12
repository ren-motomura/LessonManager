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
    class Lesson
    {
        public static async Task<Result<Models.Lesson>> Register(int studioID, int staffID, int customerID, int fee, Models.Lesson.PType paymentType, DateTime takenAt)
        {
            var req = new RegisterLessonRequest();
            req.StudioId = studioID;
            req.StaffId = staffID;
            req.CustomerId = customerID;
            req.Fee = fee;
            req.PaymentType = paymentType == Models.Lesson.PType.Cash ? PaymentType.ByCash : PaymentType.ByCard;
            req.TakenAt = Utils.Time.DateTimeToTimestamp(takenAt);

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("RegisterLesson", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Lesson>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = RegisterLessonResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var lesson = new Models.Lesson();
            lesson.ID = res.Lesson.Id;
            lesson.StudioID = res.Lesson.StudioId;
            lesson.StaffID = res.Lesson.StaffId;
            lesson.CustomerID = res.Lesson.CustomerId;
            lesson.Fee = res.Lesson.Fee;
            lesson.PaymentType = res.Lesson.PaymentType == PaymentType.ByCash ? Models.Lesson.PType.Cash : Models.Lesson.PType.Card;
            lesson.TakenAt = Utils.Time.TimestampToDateTime(res.Lesson.TakenAt);

            return new Result<Models.Lesson>(
                true,
                lesson,
                null
            );
        }

        public static async Task<Result<List<Models.Lesson>>> Search(Models.Studio studio, Models.Staff staff, Models.Customer customer, DateTime takenAtFrom, DateTime takenAtTo)
        {
            var req = new SearchLessonsRequest();
            req.StudioId = studio != null ? studio.ID : -1;
            req.StaffId = staff != null ? staff.ID : -1;
            req.CustomerId = customer != null ? customer.ID : -1;
            req.TakenAtFrom = Utils.Time.DateTimeToTimestamp(takenAtFrom);
            req.TakenAtTo = Utils.Time.DateTimeToTimestamp(takenAtTo);

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("SearchLessons", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<List<Models.Lesson>>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = SearchLessonsResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var lessons = from lesson in res.Lessons
                          select new Models.Lesson() {
                              ID = lesson.Id,
                              StudioID = lesson.StudioId,
                              StaffID = lesson.StaffId,
                              CustomerID = lesson.CustomerId,
                              Fee = lesson.Fee,
                              PaymentType = lesson.PaymentType == PaymentType.ByCash ? Models.Lesson.PType.Cash : Models.Lesson.PType.Card,
                              TakenAt = Utils.Time.TimestampToDateTime(lesson.TakenAt),
                          };

            return new Result<List<Models.Lesson>>(
                true,
                lessons.ToList(),
                null
            );
        }

        public static async Task<Result<bool>> Delete(int id)
        {
            var req = new DeleteLessonRequest();
            req.Id = id;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("DeleteLesson", reqData).ConfigureAwait(false);
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
