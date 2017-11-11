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
    }
}
