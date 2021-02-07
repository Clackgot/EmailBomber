using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EmailBomber
{
    interface IEmailSendService
    {
        Task<IDocument> SendEmail();
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
            var emailSendTasks = emailSendsServices.ConvertAll(m => m.SendEmail());
            await Task.WhenAll(emailSendTasks);
        }

    }
}
