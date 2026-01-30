namespace VisitorRegistrationShared.Responses
{
    public class ApiResult
    {
        public bool Success { get; set; } = false;

        public string? ErrorMessage { get; set; } = null;
    }
}
