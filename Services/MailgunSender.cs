using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace Blog.Services
{
    public class MailgunSender : IEmailSender
    {
        private MailgunSenderOptions _options { get; }
        
        public MailgunSender(IOptions<MailgunSenderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(_options.MailgunDomain, _options.MailgunApiKey, email, subject, message);
        }

        public Task SendEmailAsync(string domain, string apiKey, string email, string subject, string message)
        {
            RestClient client = new RestClient ();
            client.BaseUrl = new Uri ("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator ("api",
                                            apiKey);
            RestRequest request = new RestRequest ();
            request.AddParameter ("domain", domain, ParameterType.UrlSegment);
            request.Resource = $"{domain}/messages";
            request.AddParameter ("from", $"User <mailgun@{domain}>");
            request.AddParameter ("to", email);
            request.AddParameter ("subject", subject);
            request.AddParameter ("html", message);
            request.Method = Method.POST;
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();            
            var s = client.ExecuteAsync(request, r => taskCompletion.SetResult(r));                        

            return taskCompletion.Task;
        }
    }
}