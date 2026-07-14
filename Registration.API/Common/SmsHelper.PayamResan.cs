using System.Net;
using Registration.API.Business;
using SMS;
using SMS.Models.PayamResan;

namespace Registration.API.Common
{
	public class SmsHelper(PayamResan smsService, MessageBusiness messageBusiness)
	{
		//public static string ReplaceTemplates(this string smsText, string? firstName = null, string? lastName = null,
		//    string? nationalNumber = null, string? trackingCode = null)
		//{
		//    if (!string.IsNullOrEmpty(firstName))
		//        smsText = smsText.Replace("{fn}", firstName);
		//    if (!string.IsNullOrEmpty(lastName))
		//        smsText = smsText.Replace("{ln}", lastName);
		//    if (!string.IsNullOrEmpty(nationalNumber))
		//        smsText = smsText.Replace("{nn}", nationalNumber);
		//    if (!string.IsNullOrEmpty(trackingCode))
		//        smsText = smsText.Replace("{tc}", trackingCode);
		//    return smsText;
		//}

		public async Task<ActionReport> Send(int applicantId, string nationalNumber, string mobile, string text,
			int? userId)
		{
			Message message = new()
			{
				ApplicantId = applicantId,
				NationalNumber = nationalNumber,
				Mobile = mobile,
				Text = text,
				UserId = userId,
			};

			ActionReport report = await messageBusiness.Add(message);
			if (!report.Successful)
				return ActionReport.Error(report);

			SMSOutputGenericModel<List<SendSMSOutput>> response = await smsService.SendSmsAsync(text, mobile);
			if (!response.Success)
				return ActionReport.Error(HttpStatusCode.InternalServerError);

			await Task.Delay(2000);

			var statusReport = await smsService.StatusById(new GetStatusByIdInput
			{
				Ids = response.Result.Select(e => e.Id).ToArray()
			});
			if (statusReport.Success && statusReport.Result.Any())
			{
				message.Status = (int)statusReport.Result.First().StatusCode;
			}

			return await messageBusiness.SaveChanges();
		}

		public async Task<ActionReport> Send(List<Applicant> applicants, string text, int? userId)
		{
			List<Message> messages = applicants.Select(e => new Message
			{
				ApplicantId = e.Id,
				NationalNumber = e.NationalNumber,
				Mobile = e.PhoneNumber,
				Text = text,
				UserId = userId,
				Status = 0
			}).ToList();

			ActionReport report = await messageBusiness.AddRange(messages);
			if (!report.Successful)
				return ActionReport.Error(report);

			var sendResponse =
				await smsService.SendSmsAsync(text, string.Join(",", messages.Select(e => e.Mobile)));
			if (!sendResponse.Success)
				return ActionReport.Error(HttpStatusCode.InternalServerError);

			List<(Message Message, long Id)> messageAndIds = new();
			await Task.WhenAll(
				Task.Run(() =>
				{
					messageAndIds = messages.Zip(sendResponse.Result.Select(r => r.Id), (message, id) => (message, id))
						.ToList();
				}),
				Task.Delay(2000));
			SMSOutputGenericModel<List<SMSStatusOutput>> statusReport = await smsService.StatusById(
				new GetStatusByIdInput
				{
					Ids = sendResponse.Result.Select(e => e.Id).ToArray()
				});
			if (statusReport.Success)
			{
				foreach (SMSStatusOutput status in statusReport.Result)
				{
					(Message Message, long Id)? messageAndId =
						messageAndIds.FirstOrDefault(e => e.Id == status.Id);
					if (messageAndId is null) continue;
					messageAndId!.Value.Message.Status = (int)status.StatusCode;
				}
			}

			return await messageBusiness.SaveChanges();
		}
	}
}