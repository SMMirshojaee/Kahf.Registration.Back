using System.Net;
using Registration.API.Business;
using SMS;

namespace Registration.API.Common
{
    public class SmsHelper_Old(Magfa smsService, MessageBusiness messageBusiness)
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

        public async Task<ActionReport> Send(int applicantId, string nationalNumber, string mobile, string text, int? userId)
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

            Response? response = await smsService.Send(text, (message.Id, mobile));
            if (response is null)
                return ActionReport.Error(HttpStatusCode.InternalServerError);

            await Task.Delay(2000);

            StatusResponse? statusReport = await smsService.GetStatuses([response.Messages.First().Id]);
            if (statusReport is not null)
            {
                MessageStatus? magfaMessage = statusReport.Dlrs.FirstOrDefault();
                if (magfaMessage == null)
                {
                    return ActionReport.Success();
                }
                message.Status = magfaMessage.Status;
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

            Response? response = await smsService.Send(text, messages.Select(e => (e.Id, e.Mobile)).ToArray());
            if (response is null)
                return ActionReport.Error(HttpStatusCode.InternalServerError);

            await Task.Delay(2000);
            StatusResponse? statusReport = await smsService.GetStatuses(response.Messages.Select(e => e.Id));
            if (statusReport is not null)
            {
                foreach (MagfaMessage magfaMessage in response.Messages)
                {
                    MessageStatus? status = statusReport.Dlrs.FirstOrDefault(e => e.Mid == magfaMessage.Id);
                    if (status is null || status.Status == -1) continue;
                    Message? message = messages.FirstOrDefault(e => magfaMessage.Recipient.EndsWith(e.Mobile[^9..]));
                    if (message is null) continue;
                    message.Status = status.Status;
                }
            }
            return await messageBusiness.SaveChanges();
        }

    }
}
