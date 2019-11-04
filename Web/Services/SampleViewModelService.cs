using Core.Entities;
using Core.Interfaces;
using Web.Interfaces;

namespace Web.Services
{
    public class SampleViewModelService: ISampleViewModelService
    {
        private readonly IAsyncRepository<Sample> _sampleRepository;

        public SampleViewModelService(IAsyncRepository<Sample> sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }
    }
}
