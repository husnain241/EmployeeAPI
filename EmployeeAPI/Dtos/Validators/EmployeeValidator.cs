using EmployeeAPI.Dtos;
using FluentValidation;

public class EmployeeValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeValidator()
    {
        // 1. Name validation (NotEmpty, Length aur "ali" check)
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naam likhna zaroori hai.")
            .MinimumLength(3).WithMessage("Naam kam az kam 3 characters ka hona chahiye.")
            .Must(name => !name.ToLower().Contains("ali"))
            .WithMessage("Naam mein 'ali' istemal karne ki ijazat nahi hai."); // User requirement

        RuleFor(x => x.Email)
     .Matches(@"^[a-zA-Z0-9._%+-]+@company\.com$")
     .WithMessage("Sirf @company.com wali emails allow hain.");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 60).WithMessage("Age 18 se 60 ke darmiyan honi chahiye");

        
    }
}