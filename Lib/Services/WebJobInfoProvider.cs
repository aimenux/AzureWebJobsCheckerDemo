using System.Net.Http;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lib.Services
{
    public class WebJobInfoProvider : IWebJobInfoProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public WebJobInfoProvider(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetWebJobStatusAsync(Settings settings)
        {
            var webJobName = settings.Name;
            var webJobType = settings.Type;

            var webjobRelativeUrl = BuildRelativeWebJobUrl(webJobName, webJobType);

            using (var httpResponseMessage = await _httpClient.GetAsync(webjobRelativeUrl))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync();
                    var infos = JsonConvert.DeserializeObject<WebJobInfo>(content);
                    return infos.Status;
                }

                _logger.LogError("Failed to get status for webjob '{webJobName}'", webJobName);
            }

            return null;
        }

        private static string BuildRelativeWebJobUrl(string webJobName, string webJobType)
        {
            return $"/api/{webJobType}jobs/{webJobName}".ToLower();
        }
    }
}
