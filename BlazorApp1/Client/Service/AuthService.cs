using BlazorApp1.Client.Helper;
using BlazorApp1.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace BlazorApp1.Client.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var result = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);
           var response = await result.Content.ReadFromJsonAsync<RegisterResult>();
            return response!;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult!;
            }

            await _localStorage.SetItemAsync("authToken", loginResult!.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token!);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<List<RoleModel>> GetRoles()
        {
            var result = await _httpClient.GetAsync("api/register");
            var response = await result.Content.ReadFromJsonAsync<List<RoleModel>>();
            return response!;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            var result = await _httpClient.GetAsync("api/register/users");
            var response = await result.Content.ReadFromJsonAsync<List<UserModel>>();
            return response!;
        }

        public async Task<RegisterResult> AssignRole(UserRoleModel model)
        {
            var result = await _httpClient.PostAsJsonAsync("api/register/assign-role", model);
            var response = await result.Content.ReadFromJsonAsync<RegisterResult>();
            return response!;
        }
    }
}
