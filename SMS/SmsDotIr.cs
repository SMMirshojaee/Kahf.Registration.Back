using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SMS;

public class SmsDotIr //: ISmsService
{
	private readonly string _username = "09128486146";

	private readonly string
		_apiKey =
			"LZMad0h8hJkptO2XUsoQBqPoLra0SplfED7AeNNbOBdcJSO6"; //sandbox="gNHd4YqkswMpgFbkDDYc3K9raScATVt6rtXsQJDMQNm3T4XK"

	private readonly string _lineNumebr = "30007487123465";

	private readonly string _sendApi = "https://api.sms.ir/v1/send"; //ارسال
	private readonly string _buckApi = "https://api.sms.ir/v1/send/bulk"; //ارسال گروهی
	private readonly string _likeToLikeApi = "https://api.sms.ir/v1/send/likeToLike"; //ارسال نظیر به نظیر
	private readonly string _otp = "https://api.sms.ir/v1/send/verify"; //ارسال verify

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
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
			HttpResponseMessage response = await httpClient.GetAsync($"https://api.sms.ir/v1/send/{messageId}");
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
			//TODO:log
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
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
			HttpResponseMessage response = await httpClient.GetAsync($"https://api.sms.ir/v1/send/pack/{packId}");
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
			//TODO:log
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
			HttpClient httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.GetAsync(
				$"https://api.sms.ir/v1/send?username={_username}&password={_apiKey}&mobile={mobile}&line={_lineNumebr}&text={message}"
			);
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
			//TODO:log
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
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
			BulkRequest request = new()
			{
				LineNumber = _lineNumebr,
				MessageText = message,
				Mobiles = mobiles,
				SendDateTime = null
			};
			HttpContent content = JsonContent.Create(request);
			HttpResponseMessage response = await httpClient.PostAsync("https://api.sms.ir/v1/send/bulk", content);
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
			//TODO:log
			return new SmsIrResponse<BulkData>
			{
				Status = 0,
				Message = e.Message
			};
		}
	}

	public async Task SendVerify(string message, string mobiles)
	{
		HttpClient httpClient = new HttpClient();

		httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

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
			await httpClient.PostAsync("https://api.sms.ir/v1/send/verify", stringContent);
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

	public int DeliveryState { get; set; }

	public long DeliveryDateTime { get; set; }
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