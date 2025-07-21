using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;

namespace Payment
{
    public class Zarrinpal(bool isDevelopment)
    {
        private readonly string _requestApi = "https://payment.zarinpal.com/pg/v4/payment/request.json";
        private readonly string _verifyApi = "https://payment.zarinpal.com/pg/v4/payment/verify.json";
        private readonly string _payAddress = "https://payment.zarinpal.com/pg/StartPay/";
        private readonly string _merchantId = "027982b7-5907-40f5-b1c7-ee28303baef9";

        //private string _sandboxRequestApi = "https://sandbox.zarinpal.com/pg/v4/payment/request.json";
        //private string _sandboxVerifyApi = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json";
        //private string _sandboxPayApi = "https://sandbox.zarinpal.com/pg/StartPay/";
        //private string _fakeMerchantId = "0822b19c-c26e-4971-9430-4c5036121b5d";


        private readonly string _localCallbackUrl = "https://localhost:7024/api/Order/callback";
        private readonly string _callbackUrl = "https://api.kahfolhasin.ir/api/Order/callback";

        public async Task<ZarrinpalResponse> Verify(string authority, int amount)
        {
            RestClient client = new RestClient(_verifyApi);
            RestRequest request = new RestRequest
            {
                Method = Method.Post
            };

            request.AddJsonBody(JsonSerializer.Serialize(new
            {
                merchant_id = _merchantId,
                amount = amount,
                authority = authority
            }), ContentType.Json);
            RestResponse<ResponseData> response = await client.ExecuteAsync<ResponseData>(request);

            if (response is { IsSuccessful: true, Data.Data.Code: 100 or 101 })
            {
                return new ZarrinpalResponse
                {
                    Successful = true,
                    Code = response.Data.Data.Code,
                    Content = response.Content,
                    RefId = response.Data.Data.RefId,
                };
            }

            return new ZarrinpalResponse
            {
                Successful = false,
                Code = response.Data?.Data.Code,
                Content = response.Content,
            }; ;
        }
        public async Task<ZarrinpalResponse> SendRequest(string firstName, string lastName, int id, int amount, int orderId, string mobile)
        {
            RestClient client = new RestClient(_requestApi);
            RestRequest request = new RestRequest
            {
                Method = Method.Post
            };

            request.AddJsonBody(JsonSerializer.Serialize(new
            {
                merchant_id = _merchantId,
                amount = amount,
                callback_url = isDevelopment ? _localCallbackUrl : _callbackUrl,
                description = $"{id}/{firstName}-{lastName}/{amount}",
                metadata = new { mobile = mobile },
                currency = "IRR",
                order_id = orderId
            }), ContentType.Json);

            RestResponse<ResponseData> response = await client.ExecuteAsync<ResponseData>(request);

            if (response is { IsSuccessful: true, Data.Data.Code: 100 or 101 })
            {
                return new ZarrinpalResponse
                {
                    Successful = true,
                    Code = response.Data.Data.Code,
                    Content = response.Content,
                    Authority = response.Data.Data.Authority,
                    RedirectUrl = _payAddress + response.Data.Data.Authority
                };
            }

            return new ZarrinpalResponse
            {
                Successful = false,
                Code = response.Data?.Data.Code,
                Content = response.Content,
            }; ;
        }

    }

    public class ZarrinpalResponse
    {
        public bool Successful { get; set; }
        public int? Code { get; set; }
        public string? Content { get; set; }
        public string? Authority { get; set; }
        public long? RefId { get; set; }
        public string? RedirectUrl { get; set; }
    }

    public class ResponseData
    {
        public RequestResponseDto Data { get; set; }
        public List<string> Errors { get; set; }
    }

    public class RequestResponseDto
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Authority { get; set; }
        [JsonPropertyName("card_hash")]
        public string CardHash { get; set; }
        [JsonPropertyName("card_pan")]
        public string CardPan { get; set; }
        [JsonPropertyName("ref_id")]
        public long RefId { get; set; }
        public int Fee { get; set; }
        [JsonPropertyName("fee_type")]
        public string FeeType { get; set; }
    }
}

