using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Dtos.Admin;

namespace VisitorRegistrationService
{
    public class AdminService
    {
        private readonly IAdminRepository adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            this.adminRepository = adminRepository;
        }

        public async Task<bool> ValidateAdminLogin(LoginAdminDto loginAdminDto)
        {
            var admin = await adminRepository.GetAdminByEmail(loginAdminDto.Email);

            if (admin == null)
            {
                return false;
            }

            return admin.Password == loginAdminDto.Password;
        }
    }
}
