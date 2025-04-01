namespace VisitorRegistrationData.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public bool IsDeleted { get; set; } = false;
    }
}