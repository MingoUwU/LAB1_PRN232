using System;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Entities.Student> Students { get; }
        IGenericRepository<Entities.Enrollment> Enrollments { get; }
        IGenericRepository<Entities.Course> Courses { get; }
        IGenericRepository<Entities.Subject> Subjects { get; }
        IGenericRepository<Entities.Semester> Semesters { get; }

        Task<int> SaveChangesAsync();
    }
}
