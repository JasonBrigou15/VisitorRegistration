namespace VisitorRegistrationApi.Dtos.Company
{
    public static class CompanyMappingExtension
    {
        public static VisitorRegistrationData.Entities.Company CreateDtoToEntity(this CreateCompanyDto createCompanyDto)
        {
            var company = new VisitorRegistrationData.Entities.Company
            {
                Name = createCompanyDto.Name,
            };

            return company;
        }

        public static void UpdateDtoToEntity(this UpdateCompanyDto updateCompanyDto, VisitorRegistrationData.Entities.Company company)
        {
            company.Name = updateCompanyDto.Name;
        }
    }
}
