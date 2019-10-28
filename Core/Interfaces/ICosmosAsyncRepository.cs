using Core.Collections;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICosmosAsyncRepository<T> where T : BaseCollection
    {
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
