using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lib.Services
{
    public class WebJobInfoProvider : IWebJobInfoProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public WebJobInfoProvider(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<string> GetWebJobStatusAsync(WebJob webjob)
        {
            var (infos, statusCode) = await GetWebJobResponseAsync(webjob);

            if (infos == null)
            {
                _logger.LogError("Failed to get status for webjob '{webJobName}' [StatusCode = {statusCode}]",
                    webjob.Name,
                    statusCode);
            }

            return infos?.Status;
        }

        public async Task<WebJobInfo> GetWebJobInfoAsync(WebJob webjob)
        {
            var (infos, statusCode) = await GetWebJobResponseAsync(webjob);

            if (infos == null)
            {
                _logger.LogError("Failed to get infos for webjob '{webJobName}' [StatusCode = {statusCode}]",
                    webjob.Name,
                    statusCode);
            }

            return infos;
        }

        private static string BuildRelativeWebJobUrl(string webJobName, string webJobType)
        {
            return $"/api/{webJobType}jobs/{webJobName}".ToLower();
        }

        private static AuthenticationHeaderValue GetAuthenticationHeader(WebJob webjob)
        {
            var bytes = Encoding.ASCII.GetBytes($"{webjob.UserName}:{webjob.Password}");
            var base64 = Convert.ToBase64String(bytes);
            return new AuthenticationHeaderValue("Basic", base64);
        }

        private async Task<Tuple<WebJobInfo, HttpStatusCode>> GetWebJobResponseAsync(WebJob webjob)
        {
            var webJobName = webjob.Name;
            var webJobType = webjob.Type;

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(webjob.Url);
            httpClient.DefaultRequestHeaders.Authorization = GetAuthenticationHeader(webjob);

            var webjobRelativeUrl = BuildRelativeWebJobUrl(webJobName, webJobType);

            using (var httpResponseMessage = await httpClient.GetAsync(webjobRelativeUrl))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    return new Tuple<WebJobInfo, HttpStatusCode>(null, httpResponseMessage.StatusCode);
                }

                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var infos = JsonConvert.DeserializeObject<WebJobInfo>(content);
                return new Tuple<WebJobInfo, HttpStatusCode>(infos, httpResponseMessage.StatusCode);
            }
        }
    }
}
