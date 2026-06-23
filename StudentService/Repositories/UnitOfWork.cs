using StudentService.Data;
using StudentService.Entities;
using System.Threading.Tasks;

namespace StudentService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentDbContext _context;

        public UnitOfWork(StudentDbContext context)
        {
            _context = context;
            Students = new GenericRepository<Student>(_context);
        }

        public IGenericRepository<Student> Students { get; private set; }

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
