using VisitorRegistrationShared.Dtos.Employee;
using VisitorRegistrationShared.Responses;

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

        public async Task<List<GetEmployeeDto>> GetEmployees()
        {
            var response = await http.GetAsync("api/employee");

            if (!response.IsSuccessStatusCode)
                return new List<GetEmployeeDto>();

            var employees = await response.Content.ReadFromJsonAsync<List<GetEmployeeDto>>();
            return employees ?? new List<GetEmployeeDto>();
        }

        public async Task<ApiResult> CreateEmployee(CreateEmployeeDto dto)
        {
            var response = await http.PostAsJsonAsync("api/employee", dto);

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

        public async Task<ApiResult> UpdateEmployee(UpdateEmployeeDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/employee/{dto.Id}", dto);

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

        public async Task<ApiResult> DeleteEmployee(int id)
        {
            var response = await http.DeleteAsync($"api/employee/{id}");

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
