using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SampleRepository : EfRepository<Sample>, ISampleRepository
    {
        public SampleRepository(SqlDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Sample> GetByEmailAsync(string email)
        {
            return await _dbContext.Samples
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
