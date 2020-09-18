using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Models;

namespace Lib.Services
{
    public interface IWebJobInfoProvider
    {
        Task<string> GetWebJobStatusAsync(WebJob webjob);
        Task<WebJobInfo> GetWebJobInfoAsync(WebJob webjob);
    }
}
