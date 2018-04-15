using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Protobufs;
using Google.Protobuf;

namespace LessonManager.WebAPIs
{
    class Customer
    {
        public static async Task<Result<List<Models.Customer>>> GetAll()
        {
            var responseMessage = await Client.Instance.Request("SelectCustomers", new byte[] { }).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<List<Models.Customer>>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = SelectCustomersResponse.Parser.ParseFrom(responseDataStream.ToArray());
            var customers = res.Customers.Select(c =>
            {
                return ConvertCustomer(c);
            }).ToList();

            return new Result<List<Models.Customer>>(
                true,
                customers,
                null
            );

        }

        public static async Task<Result<Models.Customer>> Create(
            string name,
            string kana,
            DateTime birthday,
            int gender,
            string postalCode1,
            string postalCode2,
            string address,
            string phoneNumber,
            DateTime joinDate,
            string emailAddress,
            bool canMail,
            bool canEmail,
            bool canCall,
            string description,
            string cardID
        )
        {
            var req = new CreateCustomerRequest();
            req.Name = name;
            req.Kana = kana;
            req.Birthday = Utils.Time.DateTimeToTimestamp(birthday);
            req.Gender = gender;
            req.PostalCode1 = postalCode1;
            req.PostalCode2 = postalCode2;
            req.Address = address;
            req.PhoneNumber = phoneNumber;
            req.JoinDate = Utils.Time.DateTimeToTimestamp(joinDate);
            req.EmailAddress = emailAddress;
            req.CanMail = canMail;
            req.CanEmail = canEmail;
            req.CanCall = canCall;
            req.Description = description;
            req.Card = new Card() { Id = cardID, Credit = 0 };

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("CreateCustomer", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Customer>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateCustomerResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var customer = ConvertCustomer(res.Customer);

            return new Result<Models.Customer>(
                true,
                customer,
                null
            );
        }

        public static async Task<Result<Models.Customer>> Update(int id, string name, string description)
        {
            var req = new UpdateCustomerRequest();
            req.Customer = new Protobufs.Customer
            {
                Id = id,
                Name = name,
                Description = description,
            };

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("UpdateCustomer", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Customer>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateCustomerResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var customer = ConvertCustomer(res.Customer);

            return new Result<Models.Customer>(
                true,
                customer,
                null
            );
        }

        public static async Task<Result<bool>> Delete(int id)
        {
            var req = new DeleteCustomerRequest();
            req.Id = id;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("DeleteCustomer", reqData).ConfigureAwait(false);
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

            var res = CreateCustomerResponse.Parser.ParseFrom(responseDataStream.ToArray());

            return new Result<bool>(
                true,
                true,
                null
            );
        }

        public static async Task<Result<Models.Customer>> SetCard(int customerID, string cardID, int credit)
        {
            var req = new SetCardOnCustomerRequest();
            req.CustomerId = customerID;
            req.Card = new Card() { Id = cardID, Credit = credit };

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("SetCardOnCustomer", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Customer>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = CreateCustomerResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var customer = ConvertCustomer(res.Customer);

            return new Result<Models.Customer>(
                true,
                customer,
                null
            );
        }

        public static async Task<Result<Models.Customer>> AddCredit(int customerID, int amount)
        {
            var req = new AddCreditRequest();
            req.CustomerId = customerID;
            req.Amount = amount;

            var reqData = req.ToByteArray();

            var responseMessage = await Client.Instance.Request("AddCredit", reqData).ConfigureAwait(false);
            var responseDataStream = new MemoryStream((int)responseMessage.Content.Headers.ContentLength); // long から int への cast は避けるべきだが...
            await responseMessage.Content.CopyToAsync(responseDataStream).ConfigureAwait(false);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return new Result<Models.Customer>(
                    false,
                    null,
                    new FailData(
                        responseMessage.StatusCode, ErrorResponse.Parser.ParseFrom(responseDataStream.ToArray())
                    )
                );
            }

            var res = AddCreditResponse.Parser.ParseFrom(responseDataStream.ToArray());

            var customer = ConvertCustomer(res.Customer);

            return new Result<Models.Customer>(
                true,
                customer,
                null
            );
        }

        private static Models.Customer ConvertCustomer(Protobufs.Customer c)
        {
            var customer = new Models.Customer();
            customer.ID = c.Id;
            customer.Name = c.Name;
            customer.Kana = c.Kana;
            customer.Birthday = Utils.Time.TimestampToDateTime(c.Birthday);
            customer.Gender = c.Gender;
            customer.PostalCode1 = c.PostalCode1;
            customer.PostalCode2 = c.PostalCode2;
            customer.Address = c.Address;
            customer.PhoneNumber = c.PhoneNumber;
            customer.JoinDate = Utils.Time.TimestampToDateTime(c.JoinDate);
            customer.EmailAddress = c.EmailAddress;
            customer.CanMail = c.CanMail;
            customer.CanEmail = c.CanEmail;
            customer.CanCall = c.CanCall;
            customer.Description = c.Description;
            if (c.Card != null)
            {
                customer.CardId = c.Card.Id;
                customer.Credit = c.Card.Credit;
            }
            return customer;
        }
    }
}
