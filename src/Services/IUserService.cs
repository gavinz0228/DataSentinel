using System;
using System.Threading.Tasks;
namespace DataSentinel.Services{
    public interface IUserService
    {
        Task<string> Authenticate(string userName, string password);
    }
}