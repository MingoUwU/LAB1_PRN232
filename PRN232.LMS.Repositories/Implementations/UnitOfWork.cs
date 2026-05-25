using PRN232.LMS.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmsDbContext _context;
        private IGenericRepository<Entities.Student>? _students;
        private IGenericRepository<Entities.Enrollment>? _enrollments;
        private IGenericRepository<Entities.Course>? _courses;
        private IGenericRepository<Entities.Subject>? _subjects;
        private IGenericRepository<Entities.Semester>? _semesters;

        public UnitOfWork(LmsDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Entities.Student> Students => _students ??= new GenericRepository<Entities.Student>(_context);
        public IGenericRepository<Entities.Enrollment> Enrollments => _enrollments ??= new GenericRepository<Entities.Enrollment>(_context);
        public IGenericRepository<Entities.Course> Courses => _courses ??= new GenericRepository<Entities.Course>(_context);
        public IGenericRepository<Entities.Subject> Subjects => _subjects ??= new GenericRepository<Entities.Subject>(_context);
        public IGenericRepository<Entities.Semester> Semesters => _semesters ??= new GenericRepository<Entities.Semester>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
