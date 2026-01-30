using VisitorRegistrationShared.Dtos.Employee;

namespace VisitorRegistrationUI.Services
{
    public class EmployeeService
    {
        private readonly HttpClient http;

        public EmployeeService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<GetEmployeeDto>> GetEmployeesByCompany(int id)
        {
            var response = await http.GetAsync($"api/employee/by-company?companyId={id}");

            if (!response.IsSuccessStatusCode)
                return new List<GetEmployeeDto>();

            var employees = await response.Content.ReadFromJsonAsync<List<GetEmployeeDto>>();

            return employees ?? new List<GetEmployeeDto>();
        }
    }
}
