namespace ConsentFormEngine.Core.Mailing
{
    public class Mail
    {
        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
        public string? Attachments { get; set; }
        public string ToList { get; set; }
        public string CcList { get; set; }
        public string BccList { get; set; }
        public string? UnsubscribeLink { get; set; }

        public Mail()
        {
            Subject = string.Empty;
            TextBody = string.Empty;
            HtmlBody = string.Empty;
            ToList = string.Empty;
        }

        public Mail(string subject, string textBody, string htmlBody, string toList)
        {
            Subject = subject;
            TextBody = textBody;
            HtmlBody = htmlBody;
            ToList = toList;
        }
    }


}
