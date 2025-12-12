using VisitorRegistrationShared.Dtos.Visitor;

namespace VisitorRegistrationService.Dtos.Visitor
{
    public static class VisitorMappingExtension
    {
        public static GetVisitorDto ToGetDto(this VisitorRegistrationData.Entities.Visitor visitor)
        {
            return new GetVisitorDto
            {
                Id = visitor.Id,
                Firstname = visitor.Firstname,
                Lastname = visitor.Lastname,
                Email = visitor.Email,
                CompanyName = visitor.Company?.Name ?? string.Empty,
            };
        }

        public static VisitorRegistrationData.Entities.Visitor CreateDtoToEntity(this CreateVisitorDto createVisitorDto, 
            VisitorRegistrationData.Entities.Company? company)
        {
            var visitor = new VisitorRegistrationData.Entities.Visitor
            {
                Firstname = createVisitorDto.Firstname,
                Lastname = createVisitorDto.Lastname,
                Email = createVisitorDto.Email,
                CompanyId = company?.Id,
                Company = company
            };

            return visitor;
        }

        public static void UpdateDtoToEntity(this UpdateVisitorDto updateVisitorDto, VisitorRegistrationData.Entities.Visitor visitor, 
            VisitorRegistrationData.Entities.Company? company)
        {
            visitor.Firstname = updateVisitorDto.Firstname;
            visitor.Lastname = updateVisitorDto.Lastname;
            visitor.Email = updateVisitorDto.Email;

            if (company != null)
            {
                visitor.CompanyId = company.Id;
                visitor.Company = company;
            }
            else
            {
                visitor.CompanyId = null;
                visitor.Company = null;
            }
        }
    }
}
