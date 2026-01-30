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
    }
}