using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetAdminByEmail(string email);
    }
}
