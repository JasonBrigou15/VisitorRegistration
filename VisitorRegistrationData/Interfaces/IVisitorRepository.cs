using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Interfaces
{
    public interface IVisitorRepository
    {
        Task<List<Visitor>> GetAllVisitors();

        Task<Visitor?> GetVisitorById(int id);

        Task<Visitor?> GetVisitorByEmail(string email);

        Task<Visitor> CreateVisitor(Visitor visitor);

        Task UpdateVisitor(Visitor visitor);

        Task DeleteVisitor(int id);
    }
}
