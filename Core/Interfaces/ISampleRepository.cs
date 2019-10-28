using Core.Entities;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISampleRepository : IAsyncRepository<Sample>
    {
        Task<Sample> GetByEmailAsync(string email);
    }
}
