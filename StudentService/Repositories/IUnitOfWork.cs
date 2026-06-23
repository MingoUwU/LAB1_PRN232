using System;
using System.Threading.Tasks;
using StudentService.Entities;

namespace StudentService.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Student> Students { get; }
        Task<int> SaveChangesAsync();
    }
}
