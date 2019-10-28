using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Web.Api
{
    public class BaseApi
    {
        public async Task<T> GetFormBody<T>(HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(requestBody);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
