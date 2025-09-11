# VisitorRegistration – re-build van mijn stageproject

## Over dit project
Dit project is een **heropbouw** van mijn stageopdracht (VDAB-opleiding Softwareontwikkeling).  
Het doel is om een **bezoekersregistratiesysteem** te maken waarmee bezoekers zich kunnen aanmelden, bedrijven en werknemers worden beheerd, en validaties centraal staan.

De focus van deze re-build ligt op:
- properdere architectuur
- centrale validaties met FluentValidation
- testbare businesslogica via unit tests

## Solution structuur
- **VisitorRegistrationApi**  
  - ASP.NET Core Web API met controllers voor Companies & Employees  
- **VisitorRegistrationData**  
  - Entities (Company, Employee)  
  - Repositories & interfaces  
  - EF Core DbContext & migrations  
- **VisitorRegistrationService**  
  - DTO’s (Create/Get/Update voor Company & Employee)  
  - Validators (FluentValidation)  
  - Services voor Company & Employee logica  
  - Mapping tussen entities en DTO’s  
- **VisitorRegistrationShared**  
  - Helpers en extensies (bv. `StringExtensions` voor normalisatie)  
- **VisitorRegistrationTest**  
  - Unit tests voor CompanyService & EmployeeService  

## ⚙Technologieën
- ASP.NET Core  
- Entity Framework Core  
- FluentValidation  
- C# & LINQ  
- xUnit (unit testing)

## Huidige functionaliteiten
- Companies:
  - CRUD-operaties  
  - Validaties via FluentValidation (bv. duplicate check via genormaliseerde naam)  
- Employees:
  - CRUD-operaties  
  - Validaties via FluentValidation  
- Centrale mapping tussen DTO’s en Entities  
- Unit tests voor Company- en EmployeeService  

## Gepland
- Toevoegen van Visitors (registratie op basis van voornaam, achternaam, e-mail)  
- Sign-up flow waarbij bestaande bezoekersgegevens getoond worden en velden vooraf ingevuld zijn  
- Mogelijkheid om afspraken te plannen door een Employee te selecteren  
- Frontend in **Fluent UI Blazor** met losse routing file  
- Meer unit tests (Visitors, integratietests)  

## Wat ik geleerd heb
- Toepassen van een gelaagde architectuur in .NET  
- Werken met DTO’s en mapping extensies  
- Validaties verplaatsen van controllers naar centrale validators  
- Schrijven van unit tests om businesslogica te valideren  
- Code opsplitsen in losse lagen voor beter onderhoud en testbaarheid  

---

**Status:** Work in progress (niet de afgewerkte stage-versie)  
