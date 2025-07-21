using System.Net;
using System.Text;
using Registration.API.Business;
using SMS;

namespace Registration.API.Common
{
    public class SmsHelper(Magfa smsService, MessageBusiness messageBusiness)
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

            Response? response = await smsService.Send(text, mobile);
            if (response is null)
                return ActionReport.Error(HttpStatusCode.InternalServerError);
            message.Status = response.Messages.First().Status;
            return await messageBusiness.SaveChanges();
        }

    }
}
