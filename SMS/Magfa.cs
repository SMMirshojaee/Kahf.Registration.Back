using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace SMS
{
    public class Magfa
    {
        private static readonly string _sendAddress = "https://sms.magfa.com/api/http/sms/v2/send";
        private static readonly string _getStatusAddress = "https://sms.magfa.com/api/http/sms/v2/statuses/";
        private static readonly string username = "beh_saman";
        private static readonly string password = "_Beh$$a@man_";
        private static readonly string domain = "behsamanco";
        private static readonly string number = "30007279";

        public async Task<Response?> Send(string message, params (int messageId, string mobile)[] idsAndMobiles)
        {
            try
            {

                RestClientOptions options = new RestClientOptions(_sendAddress)
                {
                    Authenticator = new HttpBasicAuthenticator(username + "/" + domain, password),
                };

                RestClient client = new RestClient(options);
                RestRequest request = new()
                {
                    Method = Method.Post
                };
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "application/json");
                request.AddBody(new
                {
                    senders = new[] { number },
                    messages = new[] { message },
                    uids = idsAndMobiles.Select(e => e.messageId),
                    recipients = idsAndMobiles.Select(e => e.mobile)
                });

                RestResponse<Response> response = await client.ExecuteAsync<Response>(request);
                if (response is { IsSuccessful: true, Data.Status: 0 })
                {
                    return response.Data;

                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<StatusResponse?> GetStatuses(IEnumerable<long> messageIds)
        {
            try
            {
                RestClientOptions options = new RestClientOptions(_getStatusAddress + string.Join(",", messageIds))
                {
                    Authenticator = new HttpBasicAuthenticator(username + "/" + domain, password),
                };

                RestClient client = new RestClient(options);
                RestRequest request = new()
                {
                    Method = Method.Get
                };
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("accept", "application/json");

                var response = await client.ExecuteAsync<StatusResponse>(request);
                if (response.IsSuccessful)
                    return response.Data;
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public class StatusResponse
    {
        public int Status { get; set; }
        public MessageStatus[] Dlrs { get; set; } = null!;
    }

    public class MessageStatus
    {
        public long Mid { get; set; }
        public short Status { get; set; }
        //public DateTime Date { get; set; }
    }

    public class Response
    {
        public int Status { get; set; }
        public MagfaMessage[] Messages { get; set; } = null!;
    }

    public class MagfaMessage
    {
        public int Status { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Parts { get; set; }
        public float Tariff { get; set; }
        public string Alphabet { get; set; } = null!;
        public string Recipient { get; set; } = null!;

    }
}
