using BlazorApp1.Shared;

namespace BlazorApp1.Client.Service
{
    public interface IAuthService
    {
        Task<RegisterResult> Register(RegisterModel registerModel);
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();

        Task<List<RoleModel>> GetRoles();

        Task<List<UserModel>> GetUsers();
        Task<RegisterResult> AssignRole(UserRoleModel model);
    }
}
