using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.Common
{
    public static class ApiResponseFactory
    {
        public static APIResponse Success(object? data = null, string? message = "")
        {
            return new APIResponse
            {
                status = 1,
                message = message ?? "",
                error = "",
                data = data
            };
        }

        public static APIResponse Fail(Exception? ex, string message, object? data = null)
        {
            return new APIResponse
            {
                status = 0,
                message = message,
                error = ex?.ToString(),
                data = data
            };
        }

        public static APIResponse Unauthorized(string message)
        {
            return new APIResponse
            {
                status = 403,
                message = message,
                //error = ex?.ToString(),
                //data = data
            };
        }

        public class APIResponse
        {
            public int status { get; set; }
            public string message { get; set; } = string.Empty;
            public object data { get; set; } = new object();
            public string error { get; set; } = string.Empty;
        }
    }
}
