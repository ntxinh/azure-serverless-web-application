using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class SampleService : ISampleService
    {
        private readonly IAsyncRepository<Sample> _sampleRepository;

        public SampleService(IAsyncRepository<Sample> sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }
    }
}
