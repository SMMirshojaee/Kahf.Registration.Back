using System.Text.Json;
using RestSharp;
using SMS.Models.PayamResan;

namespace SMS;

public class PayamResan
{
	private readonly RestClient _client = new("http://api.sms-webservice.com/api/V3/");
	private readonly string _apiKey = "205800-2F7A822589BB468288AA4DF0AD130590";
	private readonly long _sender = 9998623595;

	public async Task<SMSOutputGenericModel<List<SendSMSOutput>>> SendSmsAsync(string text,
		string recipients)
	{
		RestRequest request = new RestRequest($"Send").AddQueryParameter("ApiKey", _apiKey, true);
		request.AddQueryParameter("Text", text, true);
		request.AddQueryParameter("Sender", _sender);
		request.AddQueryParameter("Recipients", recipients);

		RestResponse response = await _client.ExecuteGetAsync(request);
		SMSOutputGenericModel<List<SendSMSOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SendSMSOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SendSMSOutput>>> SendBulkSmsAsync(SendBulkSMSInput input)
	{
		RestRequest request = new RestRequest($"SendBulk").AddJsonBody(input);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<SendSMSOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SendSMSOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SendSMSOutput>>> SendMultiPleSmsAsync(SendMultipleSMSInput input)
	{
		RestRequest request = new RestRequest($"SendMultiple").AddJsonBody(input);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<SendSMSOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SendSMSOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SendTokenOutput>>> SendTokenSingle(string templateKey,
		long destination, string p1, string p2, string p3)
	{
		RestRequest request = new RestRequest($"SendTokenSingle").AddQueryParameter("ApiKey", _apiKey, true);
		request.AddQueryParameter("TemplateKey", templateKey);
		request.AddQueryParameter("Destination", destination);
		if (p1.Length > 0) request.AddQueryParameter("p1", p1);
		if (p1.Length > 0) request.AddQueryParameter("p2", p2);
		if (p1.Length > 0) request.AddQueryParameter("p3", p3);

		RestResponse response = await _client.ExecuteGetAsync(request);
		SMSOutputGenericModel<List<SendTokenOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SendTokenOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SendTokenOutput>>> SendMultiPleTokenAsync(SendMultipleTokenInput input)
	{
		RestRequest request = new RestRequest($"SendTokenMulti").AddJsonBody(input);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<SendTokenOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SendTokenOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<TokenListOutput>>> TokenList(BaseUser user)
	{
		RestRequest request = new RestRequest($"TokenList").AddJsonBody(user);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<TokenListOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<TokenListOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SMSStatusOutput>>> StatusById(GetStatusByIdInput input)
	{
		input.ApiKey = _apiKey;
		RestRequest request = new RestRequest($"StatusById").AddJsonBody(input);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<SMSStatusOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SMSStatusOutput>>>(response.Content);
		return finalresult;
	}

	public async Task<SMSOutputGenericModel<List<SMSStatusOutput>>> StatusByUserTraceId(
		GetStatusByUserTraceIdsInput input)
	{
		RestRequest request = new RestRequest($"StatusByUserTraceId").AddJsonBody(input);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<List<SMSStatusOutput>> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<List<SMSStatusOutput>>>(response.Content);
		return finalresult;
	}


	public async Task<SMSOutputGenericModel<SMSAcountIfoOutput>> AccountInfo(BaseUser user)
	{
		RestRequest request = new RestRequest($"AccountInfo").AddJsonBody(user);
		RestResponse response = await _client.ExecutePostAsync(request);
		SMSOutputGenericModel<SMSAcountIfoOutput> finalresult =
			JsonSerializer.Deserialize<SMSOutputGenericModel<SMSAcountIfoOutput>>(response.Content);
		return finalresult;
	}
}