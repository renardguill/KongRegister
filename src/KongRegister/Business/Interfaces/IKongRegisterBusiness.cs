using System.Threading.Tasks;

namespace KongRegister.Business.Interfaces
{
    public interface IKongRegisterBusiness
    {
        Task<string> RegisterAsync();
        Task<bool> UnregisterAsync();

        bool OnStartup();
        bool Disabled();
    }
}
