using System.Net.Http;
using VisitorRegistrationShared.Dtos.Company;

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
    }
}
