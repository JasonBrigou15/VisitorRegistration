using VisitorRegistrationShared.Dtos.Admin;
using VisitorRegistrationShared.Responses;
using VisitorRegistrationUI.Models;

namespace VisitorRegistrationUI.Services
{
    public class AdminService
    {
        private readonly HttpClient http;

        public bool IsAdminLoggedIn { get; private set; }

        public AdminService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<ApiResult> Login(LoginAdminDto loginAdminDto)
        {
            var response = await http.PostAsJsonAsync("api/Admin/login", loginAdminDto);

            if (response.IsSuccessStatusCode)
            {
                IsAdminLoggedIn = true;
                return new ApiResult { Success = true };
            }

            var error = await response.Content.ReadAsStringAsync();

            return new ApiResult
            {
                Success = false,
                ErrorMessage = error
            };
        }

        public async Task CheckSession()
        {
            try
            {
                var response = await http.GetAsync("api/admin/check");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CheckResult>();
                    IsAdminLoggedIn = result?.IsAdmin ?? false;
                }
            }
            catch
            {
                IsAdminLoggedIn = false;
            }
        }

        public async Task Logout()
        {
            await http.PostAsync("api/admin/logout", null);
            IsAdminLoggedIn = false;
        }
    }
}
