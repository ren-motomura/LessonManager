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
                var customer = new Models.Customer();
                customer.ID = c.Id;
                customer.Name = c.Name;
                customer.Description = c.Description;
                if (c.Card != null)
                {
                    customer.CardId = c.Card.Id;
                    customer.Credit = c.Card.Credit;
                }
                return customer;
            }).ToList();

            return new Result<List<Models.Customer>>(
                true,
                customers,
                null
            );

        }

        public static async Task<Result<Models.Customer>> Create(string name, string description)
        {
            var req = new CreateCustomerRequest();
            req.Name = name;
            req.Description = description;

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

            var customer = new Models.Customer();
            customer.ID = res.Customer.Id;
            customer.Name = res.Customer.Name;
            customer.Description = res.Customer.Description;
            customer.CardId = res.Customer.Card != null ? res.Customer.Card.Id : "";
            customer.Credit = res.Customer.Card != null ? res.Customer.Card.Credit : 0;

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

            var customer = new Models.Customer();
            customer.ID = res.Customer.Id;
            customer.Name = res.Customer.Name;
            customer.Description = res.Customer.Description;
            customer.CardId = res.Customer.Card != null ? res.Customer.Card.Id : "";
            customer.Credit = res.Customer.Card != null ? res.Customer.Card.Credit : 0;

            return new Result<Models.Customer>(
                true,
                customer,
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

            var customer = new Models.Customer();
            customer.ID = res.Customer.Id;
            customer.Name = res.Customer.Name;
            customer.Description = res.Customer.Description;
            customer.CardId = res.Customer.Card != null ? res.Customer.Card.Id : "";
            customer.Credit = res.Customer.Card != null ? res.Customer.Card.Credit : 0;

            return new Result<Models.Customer>(
                true,
                customer,
                null
            );
        }
    }
}
