using VisitorRegistrationShared.Dtos.Appointments;
using VisitorRegistrationShared.Responses;
using VisitorRegistrationUI.Models;

namespace VisitorRegistrationUI.Services
{
    public class AppointmentService
    {
        private readonly HttpClient http;

        public AppointmentService(IHttpClientFactory httpClientFactory)
        {
            http = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<GetAppointmentDto>> GetAppointments()
        {
            var response = await http.GetAsync("api/appointment");

            if (!response.IsSuccessStatusCode)
                return new List<GetAppointmentDto>();

            var appointments = await response.Content.ReadFromJsonAsync<List<GetAppointmentDto>>();

            return appointments ?? new List<GetAppointmentDto>();
        }

        public async Task<ApiResult> CreateAppointment(CreateAppointmentDto dto)
        {
            var response = await http.PostAsJsonAsync("api/appointment", dto);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResult { Success = true };
            }

            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return new ApiResult
                {
                    Success = false,
                    ErrorMessage = errorResponse?.Error ?? "Er ging iets mis"
                };
            }
            catch
            {
                return new ApiResult
                {
                    Success = false,
                    ErrorMessage = "Er ging iets mis bij het maken van de afspraak"
                };
            }
        }

        public async Task<ApiResult> UpdateAppointment(UpdateAppointmentDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/appointment/{dto.Id}", dto);

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

        public async Task<ApiResult> DeleteAppointment(int id)
        {
            var response = await http.DeleteAsync($"api/appointment/{id}");

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