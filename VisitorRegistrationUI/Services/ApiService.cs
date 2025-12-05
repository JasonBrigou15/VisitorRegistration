using VisitorRegistrationShared.Dtos.Visitor;
using VisitorRegistrationShared.Responses;

namespace VisitorRegistrationUI.Services
{
    public class ApiService
    {
        private readonly HttpClient http;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<GetVisitorDto?> GetVisitorById(int id)
        {
            var response = await http.GetAsync($"api/visitor/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GetVisitorDto>();
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
