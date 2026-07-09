using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SMS;

public class SmsDotIr //: ISmsService
{
	private readonly ILogger<SmsDotIr> _logger;
	private readonly HttpClient _httpClient;
	private readonly string _username = "09128486146";

	private readonly string
		_apiKey =
			"LZMad0h8hJkptO2XUsoQBqPoLra0SplfED7AeNNbOBdcJSO6"; //sandbox="gNHd4YqkswMpgFbkDDYc3K9raScATVt6rtXsQJDMQNm3T4XK"

	private readonly string _lineNumebr = "30007487123465";
	private readonly string _sendApi = "https://api.sms.ir/v1/send"; //ارسال
	private readonly string _buckApi = "https://api.sms.ir/v1/send/bulk"; //ارسال گروهی
	private readonly string _likeToLikeApi = "https://api.sms.ir/v1/send/likeToLike"; //ارسال نظیر به نظیر
	private readonly string _otp = "https://api.sms.ir/v1/send/verify"; //ارسال verify

	public SmsDotIr(ILogger<SmsDotIr> logger, IHttpClientFactory httpClientFactory)
	{
		_logger = logger;
		_httpClient = httpClientFactory.CreateClient();
		_httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
	}

	public async Task<SmsIrResponse<StatusData>> GetStatus(int messageId)
	{
		/*
			1	رسیده
			2	نرسیده به گوشی
			3	رسیده به مخابرات
			4	نرسیده به مخابرات
			5	رسیده به اپراتور
			6	ناموفق
			7	لیست سیاه
			8	نامشخص
		 */
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"https://api.sms.ir/v1/send/{messageId}");
			string responseString = await response.Content.ReadAsStringAsync();
			_logger.LogInformation("GetStatus: messageId={messageId} responseString={responseString}", messageId,
				responseString);
			SmsIrResponse<StatusData>? result = await response.Content.ReadFromJsonAsync<SmsIrResponse<StatusData>>();
			if (result is null)
			{
				return new SmsIrResponse<StatusData>
				{
					Status = 0,
					Message = "response is null"
				};
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("GetStatus: messageId={messageId} exception={@exception}", messageId, e);
			return new SmsIrResponse<StatusData>
			{
				Status = 0,
				Message = e.Message
			};
		}
	}

	public async Task<SmsIrResponse<List<StatusesData>>> GetStatuses(string packId)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync($"https://api.sms.ir/v1/send/pack/{packId}");
			string responseString = await response.Content.ReadAsStringAsync();
			_logger.LogInformation("GetStatuses: packId={packId} responseString={responseString}", packId,
				responseString);
			SmsIrResponse<List<StatusesData>>? result =
				await response.Content.ReadFromJsonAsync<SmsIrResponse<List<StatusesData>>>();
			if (result is null)
			{
				return new SmsIrResponse<List<StatusesData>>
				{
					Status = 0,
					Message = "response is null"
				};
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("GetStatuses: packId={packId} exception={@exception}", packId, e);
			return new SmsIrResponse<List<StatusesData>>
			{
				Status = 0,
				Message = e.Message
			};
		}
	}

	public async Task<SmsIrResponse<SendData>> Send(string message, string mobile)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.GetAsync(
				$"https://api.sms.ir/v1/send?username={_username}&password={_apiKey}&mobile={mobile}&line={_lineNumebr}&text={message}"
			);
			string responseString = await response.Content.ReadAsStringAsync();
			_logger.LogInformation("Send: message={message} ,mobile={mobile}, responseString={responseString}", message,
				mobile, responseString);
			SmsIrResponse<SendData>? result =
				await response.Content.ReadFromJsonAsync<SmsIrResponse<SendData>>();
			if (result is null)
			{
				return new SmsIrResponse<SendData>
				{
					Status = 0,
					Message = "response is null"
				};
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("Send: message={message} ,mobile={mobile}, exception={@exception}", message, mobile, e);
			return new SmsIrResponse<SendData>
			{
				Status = 0,
				Message = e.Message
			};
		}
	}

	public async Task<SmsIrResponse<BulkData>> SendBuck(string message, List<string> mobiles)
	{
		try
		{
			BulkRequest request = new()
			{
				LineNumber = _lineNumebr,
				MessageText = message,
				Mobiles = mobiles,
				SendDateTime = null
			};
			HttpContent content = JsonContent.Create(request);
			HttpResponseMessage response = await _httpClient.PostAsync("https://api.sms.ir/v1/send/bulk", content);
			string responseString = await response.Content.ReadAsStringAsync();
			_logger.LogInformation("SendBuck: message={message} ,mobiles={@mobiles}, responseString={responseString}",
				message,
				mobiles, responseString);
			SmsIrResponse<BulkData>? result =
				await response.Content.ReadFromJsonAsync<SmsIrResponse<BulkData>>();
			if (result is null)
			{
				return new SmsIrResponse<BulkData>
				{
					Status = 0,
					Message = "response is null"
				};
			}

			return result;
		}
		catch (Exception e)
		{
			_logger.LogError("SendBuck: message={message} ,mobiles={@mobiles}, exception={@exception}", message,
				mobiles,
				e);
			return new SmsIrResponse<BulkData>
			{
				Status = 0,
				Message = e.Message
			};
		}
	}

	public async Task SendVerify(string message, string mobiles)
	{
		VerifySendModel model = new VerifySendModel
		{
			Mobile = mobiles,
			TemplateId = 776008,
			Parameters = new VerifySendParameterModel[]
			{
				new VerifySendParameterModel
				{
					Name = "Message", Value = "message"
				}
			}
		};

		string payload = JsonSerializer.Serialize(model);
		StringContent stringContent = new(payload, Encoding.UTF8, "application/json");

		HttpResponseMessage response =
			await _httpClient.PostAsync("https://api.sms.ir/v1/send/verify", stringContent);
	}
}

public class SmsIrResponse<T>
{
	public int Status { get; set; }
	public bool IsSuccessful => Status == 1;
	public string Message { get; set; } = string.Empty;
	public T Data { get; set; }
}

public class VerifySendParameterModel
{
	public string Name { get; set; }
	public string Value { get; set; }
}

public sealed class StatusData
{
	public long MessageId { get; set; }

	public long Mobile { get; set; }

	public string MessageText { get; set; } = string.Empty;

	public long SendDateTime { get; set; }

	public long LineNumber { get; set; }

	public decimal Cost { get; set; }

	public int? DeliveryState { get; set; }

	public long? DeliveryDateTime { get; set; }
}

public class VerifySendModel
{
	public string Mobile { get; set; }

	public int TemplateId { get; set; }

	public VerifySendParameterModel[] Parameters { get; set; }
}

public sealed class BulkRequest
{
	public string LineNumber { get; set; } = string.Empty;

	public string MessageText { get; set; } = string.Empty;

	public List<string> Mobiles { get; set; } = [];

	public DateTime? SendDateTime { get; set; }
}

public class SendData
{
	public int MessageId { get; set; }
	public decimal Cost { get; set; }
}

public class BulkData
{
	public string PackId { get; set; } = null!;
	public int[] MessageIds { get; set; } = null!;
	public decimal Cost { get; set; }
}

public sealed class StatusesData
{
	public int MessageId { get; set; }

	public long Mobile { get; set; }

	public string MessageText { get; set; } = string.Empty;

	public int SendDateTime { get; set; }

	public long LineNumber { get; set; }

	public decimal Cost { get; set; }

	public byte? DeliveryState { get; set; }

	public int? DeliveryDateTime { get; set; }
}