using FluentValidation;
using PRN232.LMS.Services.Models;
using System;
using System.Text.RegularExpressions;

namespace PRN232.LMS.Services.Validators
{
    public class StudentRequestModelValidator : AbstractValidator<StudentRequestModel>
    {
        public StudentRequestModelValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .Length(3, 100).WithMessage("Full name must be between 3 and 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.StudentCode)
                .NotEmpty().WithMessage("Student code is required.")
                .Must(BeAValidFptuStudentCode).WithMessage("Student code must be in FPTU style (e.g., SE19886, CE18793).");

            RuleFor(x => x.DateOfBirth)
                .InclusiveBetween(new DateTime(1900, 1, 1), new DateTime(2100, 1, 1))
                .WithMessage("Date of birth must be valid.");
        }

        private bool BeAValidFptuStudentCode(string studentCode)
        {
            if (string.IsNullOrWhiteSpace(studentCode)) return false;
            var regex = new Regex(@"^(SE|CE|ME|HE|SA)\d{5}$");
            return regex.IsMatch(studentCode);
        }
    }
}
