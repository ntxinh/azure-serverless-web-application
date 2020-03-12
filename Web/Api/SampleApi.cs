using AutoMapper;
using Core.Collections;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Extensions.Responses;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Api
{
    public class SampleApi : BaseApi
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly ISampleViewModelService _sampleViewModelService;
        private readonly IMapper _mapper;
        private readonly ITemplateCosmosRepository _templateCosmosRepository;
        private readonly IBlobStorageService _blobStorageService;

        public SampleApi(ISampleRepository sampleRepository,
            ISampleViewModelService sampleViewModelService,
            IMapper mapper,
            ITemplateCosmosRepository templateCosmosRepository,
            IBlobStorageService blobStorageService
        )
        {
            _sampleRepository = sampleRepository;
            _sampleViewModelService = sampleViewModelService;
            _mapper = mapper;
            _templateCosmosRepository = templateCosmosRepository;
            _blobStorageService = blobStorageService;
        }

        [FunctionName("GetSample")]
        public async Task<IActionResult> GetSample(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetSample/{id}")] HttpRequest req,
            int id)
        {
            try
            {
                // Track an Event
                TelemetryClientWrapper.Instance.TrackEvent("Get a sample");

                var sample = await _sampleRepository.GetByIdAsync(id);

                // Map one
                var sampleVm = _mapper.Map<SampleViewModel>(sample);
                
                var data = new ApiResponseBuilder<SampleViewModel>()
                    .StatusCode(200)
                    .Message("Success")
                    .Result(sampleVm)
                    .Build();

                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                // Track an Exception
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
                throw ex;
            }
        }

        [FunctionName("GetSamples")]
        public async Task<IActionResult> GetSamples(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                // Track an Event
                TelemetryClientWrapper.Instance.TrackEvent("Get list samples");

                var samples = await _sampleRepository.ListAllAsync();

                // Map list
                var samplesVm = _mapper.Map<IReadOnlyList<Sample>, IReadOnlyList<SampleViewModel>>(samples);

                return new OkObjectResult(samplesVm);
            }
            catch (Exception ex)
            {
                // Track an Exception
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
                throw ex;
            }
        }

        [FunctionName("AddSample")]
        public async Task<IActionResult> AddSample(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            try
            {
                // Track an Event
                TelemetryClientWrapper.Instance.TrackEvent("Received new sample request");

                var model = await GetFormBody<AddSampleViewModel>(req);
                if (model != null)
                {
                    TelemetryClientWrapper.Instance.TrackEvent("Saving to database");
                    var sample = await _sampleRepository.AddAsync(new Sample(model.Email, model.Password));

                    var sampleVm = _mapper.Map<SampleViewModel>(sample);

                    return new OkObjectResult(sampleVm);
                }
            }
            catch (Exception ex)
            {
                // Track an Exception
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
                throw ex;
            }
            return new BadRequestObjectResult("");
        }

        [FunctionName("GetTemplateFromCosmos")]
        public async Task<IActionResult> GetTemplateFromCosmos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            try
            {
                // Track an Event
                TelemetryClientWrapper.Instance.TrackEvent("SimpleCustomEvent");

                var template = new Template();
                template.Name = "Template 1";
                var result = await _templateCosmosRepository.AddAsync(template);
                var result2 = await _templateCosmosRepository.GetByIdAsync(result.Id);
                return new OkObjectResult(result2);
            }
            catch (Exception ex)
            {
                // Track an Exception
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
                throw ex;
            }
        }

        [FunctionName("GetUserInformation")]
        public IActionResult GetUserInformation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ClaimsPrincipal principal)
        {
            try
            {
                // Track an Event
                TelemetryClientWrapper.Instance.TrackEvent("SimpleCustomEvent");

                var result = new Dictionary<string, string>();

                // Get current user claims
                //ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
                //ClaimsPrincipal principal = req.HttpContext.User;
                if (null != principal)
                {
                    foreach (Claim claim in principal.Claims)
                    {
                        result.Add(claim.Type, claim.Value);
                    }
                }

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                // Track an Exception
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
                throw ex;
            }
        }

        [FunctionName("UploadFile")]
        public async Task<IActionResult> UploadFile(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            try
            {
                var files = req.Form.Files;

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName;
                        fileName = fileName.Contains("\\")
                            ? fileName.Trim('"').Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                            : fileName.Trim('"');

                        await _blobStorageService.UploadFromStream("dbbackup", fileName, formFile.OpenReadStream());
                    }
                }
                return new OkObjectResult(new
                {
                    Data = "OK"
                });
            }
            catch (Exception ex)
            {
                // Track an Exception
                throw ex;
            }
            return new BadRequestObjectResult("");
        }
    }
}
