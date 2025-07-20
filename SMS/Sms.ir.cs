using RestSharp;

namespace SMS
{
    public class SmsIr
    {
        private readonly string _apiKey = "LZMad0h8hJkptO2XUsoQBqPoLra0SplfED7AeNNbOBdcJSO6";
        private readonly string _lineNumebr = "30007487123465";

        private readonly string _buckApi = "https://api.sms.ir/v1/send/bulk";//ارسال گروهی
        private readonly string _likeToLikeApi = "https://api.sms.ir/v1/send/likeToLike";//ارسال نظیر به نظیر

        public async Task SendBuck(string message, string[] mobiles)
        {
            RestRequest request = new RestRequest(_buckApi, Method.Post);
            RestClient client = new RestClient();

            request.AddHeader("x-api-key", _apiKey);
            BuckBody body = new()
            {
                LineNumber = _lineNumebr,
                MessageText = message,
                Mobiles = mobiles
            };
            request.AddBody(body, ContentType.Json);

            RestResponse<BuckResponse> response = await client.ExecuteAsync<BuckResponse>(request);

            if (response.IsSuccessful)
            {
                ResponseData? data = response.Data?.Data;
            }
        }

        private class BuckBody
        {
            public string LineNumber { get; set; } = null!;
            public string MessageText { get; set; } = null!;
            public string[] Mobiles { get; set; } = null!;
        }

        private class BuckResponse
        {
            public int Status { get; set; }
            public string Message { get; set; } = null!;
            public ResponseData Data { get; set; }

        }
        private class ResponseData
        {
            public string PackId { get; set; } = null!;
            public int[] MessageIds { get; set; } = null!;
            public decimal Cost { get; set; }
        }
    }
}
