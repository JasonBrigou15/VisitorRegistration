using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;

namespace VisitorRegistrationData.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly VisitorRegistrationDbContext context;
        public AdminRepository(VisitorRegistrationDbContext context)
        {
            this.context = context;
        }

        public async Task<Admin?> GetAdminByEmail(string email)
        {
            return await context.Admins.SingleOrDefaultAsync(a => a.Email == email);
        }
    }
}
