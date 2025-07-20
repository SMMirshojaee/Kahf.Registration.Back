using System.Text;

namespace Registration.API.Common
{
    public static class SmsHelper
    {
        public static string ReplaceTemplates(this string smsText, string? firstName = null, string? lastName = null,
            string? nationalNumber = null, string? trackingCode = null)
        {
            if (!string.IsNullOrEmpty(firstName))
                smsText = smsText.Replace("{fn}", firstName);
            if (!string.IsNullOrEmpty(lastName))
                smsText = smsText.Replace("{ln}", lastName);
            if (!string.IsNullOrEmpty(nationalNumber))
                smsText = smsText.Replace("{nn}", nationalNumber);
            if (!string.IsNullOrEmpty(trackingCode))
                smsText = smsText.Replace("{tc}", trackingCode);
            return smsText;
        }
    }
}
