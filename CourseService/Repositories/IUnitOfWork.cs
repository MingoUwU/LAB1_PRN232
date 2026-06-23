using System;
using System.Threading.Tasks;
using CourseService.Entities;

namespace CourseService.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<Enrollment> Enrollments { get; }
        IGenericRepository<Semester> Semesters { get; }
        IGenericRepository<Subject> Subjects { get; }

        Task<int> SaveChangesAsync();
    }
}
