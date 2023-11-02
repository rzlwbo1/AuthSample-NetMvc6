using AuthSampleRoleBased.Models.DTOs;

namespace AuthSampleRoleBased.Repositories.Abstract
{
    public interface IUserAuthService
    {
        Task<Status> LoginAsync(LoginVm loginVm);
        Task<Status> RegistrationAsync(RegistrationVm registrationVm);
        Task LogoutAsync();
    }
}
