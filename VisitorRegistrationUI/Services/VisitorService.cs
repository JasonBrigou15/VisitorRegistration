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

        public async Task<List<GetVisitorDto>> GetVisitors()
        {
            var response = await http.GetAsync("api/visitor");

            if (!response.IsSuccessStatusCode)
                return new List<GetVisitorDto>();

            var visitors = await response.Content.ReadFromJsonAsync<List<GetVisitorDto>>();

            return visitors ?? new List<GetVisitorDto>();
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

        public async Task<ApiResult> UpdateVisitor(UpdateVisitorDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/visitor/{dto.Id}", dto);

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

        public async Task<ApiResult> DeleteVisitor(int id)
        {
            var response = await http.DeleteAsync($"api/visitor/{id}");

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
