using System.Threading.Tasks;

namespace Albergue.Administrator.HostedServices.Hub
{
    public interface ILocalesStatusHub
    {
        Task OnStartAsync(string id);
        Task OnFinishAsync(string id);
    }
}