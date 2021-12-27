using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disney.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        public Task Execute(string apiKey, string subject, string message, string email);
    }
}
