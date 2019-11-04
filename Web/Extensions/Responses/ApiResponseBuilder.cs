using System;

namespace Web.Extensions.Responses
{
    public class ApiResponseBuilder<T>
    {
        private readonly ApiResponse<T> apiResponse;

        public ApiResponseBuilder()
        {
            apiResponse = new ApiResponse<T>();
            apiResponse.RequestId = Guid.NewGuid().ToString();
            apiResponse.Version = "1.0.0";
        }

        public ApiResponse<T> Build()
        {
            return apiResponse;
        }

        public ApiResponseBuilder<T> StatusCode(in int statusCode)
        {
            apiResponse.StatusCode = statusCode;
            return this;
        }

        public ApiResponseBuilder<T> Message(in string message)
        {
            if (message != null)
            {
                apiResponse.Message = message;
            }
            return this;
        }

        public ApiResponseBuilder<T> Result(in T result)
        {
            if (result != null)
            {
                apiResponse.Result = result;
            }
            return this;
        }
    }
}
