using VisitorRegistrationShared.Dtos.Appointments;
using VisitorRegistrationShared.Dtos.Company;
using VisitorRegistrationShared.Responses;

namespace VisitorRegistrationUI.Services
{
    public class CompanyService
    {
        private readonly HttpClient http;

        public CompanyService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<GetCompanyDto>> GetCompanies()
        {
            var response = await http.GetAsync("api/company");

            if (!response.IsSuccessStatusCode)
                return new List<GetCompanyDto>();

            var companies = await response.Content.ReadFromJsonAsync<List<GetCompanyDto>>();

            return companies ?? new List<GetCompanyDto>();
        }

        public async Task<ApiResult> CreateCompany(CreateCompanyDto dto)
        {
            var response = await http.PostAsJsonAsync("api/company", dto);

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

        public async Task<ApiResult> UpdateCompany(UpdateCompanyDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/company/{dto.Id}", dto);

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

        public async Task<ApiResult> DeleteCompany(int id)
        {
            var response = await http.DeleteAsync($"api/company/{id}");

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
