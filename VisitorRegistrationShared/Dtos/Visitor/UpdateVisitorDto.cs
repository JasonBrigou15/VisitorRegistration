namespace VisitorRegistrationShared.Dtos.Visitor
{
    public class UpdateVisitorDto
    {
        public int Id { get; set; }

        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;
    }
}
