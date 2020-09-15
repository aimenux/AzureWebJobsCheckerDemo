using System.Threading.Tasks;
using Lib.Configuration;

namespace Lib.Services
{
    public interface IWebJobInfoProvider
    {
        Task<string> GetWebJobStatusAsync(Settings settings);
    }
}
