using CourseService.Data;
using CourseService.Entities;
using System.Threading.Tasks;

namespace CourseService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _context;

        public UnitOfWork(CourseDbContext context)
        {
            _context = context;
            Courses = new GenericRepository<Course>(_context);
            Enrollments = new GenericRepository<Enrollment>(_context);
            Semesters = new GenericRepository<Semester>(_context);
            Subjects = new GenericRepository<Subject>(_context);
        }

        public IGenericRepository<Course> Courses { get; private set; }
        public IGenericRepository<Enrollment> Enrollments { get; private set; }
        public IGenericRepository<Semester> Semesters { get; private set; }
        public IGenericRepository<Subject> Subjects { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
