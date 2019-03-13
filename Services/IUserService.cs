using System;
namespace DataSentinel.Services{
    public interface IUserService
    {
        bool Authenticate(string userName, string password, out string token);
    }
}