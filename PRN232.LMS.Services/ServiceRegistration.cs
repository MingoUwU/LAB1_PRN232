using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Repositories;
using PRN232.LMS.Repositories.Implementations;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Implementations;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Mapping;

namespace PRN232.LMS.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddLmsServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<ISubjectService, SubjectService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);

            return services;
        }
    }
}

