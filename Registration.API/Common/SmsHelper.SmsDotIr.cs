using System.Net;
using Registration.API.Business;
using SMS;

namespace Registration.API.Common
{
	public class SmsHelper(SmsDotIr smsService, MessageBusiness messageBusiness)
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

			SmsIrResponse<SendData> response = await smsService.Send(text, mobile);
			if (!response.IsSuccessful)
				return ActionReport.Error(HttpStatusCode.InternalServerError);

			await Task.Delay(2000);

			SmsIrResponse<StatusData>? statusReport = await smsService.GetStatus(response.Data.MessageId);
			if (statusReport.IsSuccessful)
			{
				message.Status = statusReport.Data.DeliveryState;
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

			SmsIrResponse<BulkData> sendResponse =
				await smsService.SendBuck(text, messages.Select(e => e.Mobile).ToList());
			if (!sendResponse.IsSuccessful)
				return ActionReport.Error(HttpStatusCode.InternalServerError);

			List<(Message Message, int Id)> messageAndIds = new();
			await Task.WhenAll(
				Task.Run(() =>
				{
					messageAndIds = messages.Zip(sendResponse.Data.MessageIds, (message, id) => (message, id))
						.ToList();
				}),
				Task.Delay(2000));
			SmsIrResponse<List<StatusesData>> statusReport = await smsService.GetStatuses(sendResponse.Data.PackId);
			if (statusReport.IsSuccessful)
			{
				foreach (StatusesData status in statusReport.Data.Where(e=>e.DeliveryState.HasValue))
				{
					(Message Message, int Id)? messageAndId =
						messageAndIds.FirstOrDefault(e => e.Id == status.MessageId);
					if (messageAndId is null) continue;
					messageAndId!.Value.Message.Status = status.DeliveryState;
				}
			}

			return await messageBusiness.SaveChanges();
		}
	}
}