using VisitorRegistrationShared.Dtos.Visitor;
using VisitorRegistrationShared.Responses;

namespace VisitorRegistrationUI.Services
{
    public class VisitorService
    {
        private readonly HttpClient http;

        public VisitorService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<GetVisitorDto?> GetVisitorByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            try
            {
                var response = await http.GetAsync($"api/visitor/by-email?email={email}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GetVisitorDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UI] Error calling GetVisitorByEmail: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResult> CreateVisitor(CreateVisitorDto dto)
        {
            var response = await http.PostAsJsonAsync("api/visitor", dto);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResult { Success = true };
            }

            var error = await response.Content.ReadAsStringAsync();

            return new ApiResult
            {
                Success = false,
                ErrorMessage = error
            };
        }
    }
}
