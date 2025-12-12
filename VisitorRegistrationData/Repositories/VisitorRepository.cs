using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationData.Repositories
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly VisitorRegistrationDbContext context;

        public VisitorRepository(VisitorRegistrationDbContext context)
        {
            this.context = context;
        }

        public async Task<Visitor> CreateVisitor(Visitor visitor)
        {
            await context.Visitors.AddAsync(visitor);
            await context.SaveChangesAsync();
            return visitor;
        }

        public async Task DeleteVisitor(int id)
        {
            var visitor = await context.Visitors.FindAsync(id);
            visitor!.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<List<Visitor>> GetAllVisitors() => await context.Visitors
            .Include(v => v.Company)
            .Where(v => v.Company == null || !v.Company.IsDeleted)
            .ToListAsync();

        public async Task<Visitor?> GetVisitorByEmail(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await context.Visitors
                .Include(v => v.Company)
                .Where(v => v.Company == null || !v.Company.IsDeleted)
                .SingleOrDefaultAsync(v => !v.IsDeleted && v.Email == normalizedEmail);
        }

        public async Task<Visitor?> GetVisitorById(int id) => await context.Visitors
            .Include(v => v.Company)
            .Where(v => v.Company == null || !v.Company.IsDeleted)
            .SingleOrDefaultAsync(v => v.Id == id);

        public async Task UpdateVisitor(Visitor visitor)
        {
            var existingVisitor = await context.Visitors.FindAsync(visitor.Id);

            context.Entry(existingVisitor!).CurrentValues.SetValues(visitor);

            await context.SaveChangesAsync();
        }
    }
}
