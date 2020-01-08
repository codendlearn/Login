using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Login.IdentityServer
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate next;

        private readonly ILogger logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                logger.LogError(ex, "HttpStatusCodeException.", context.Request.Headers, context.Request.Body);
                context.Response.ContentType = "application/json";
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception.", context.Request.Headers, context.Request.Body);
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            ClearCacheHeaders(context.Response);
            string result = new ErrorDetails() { Message = message, ErrorCode = (int)statusCode }.ToString();
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(result);
        }

        private static void ClearCacheHeaders(HttpResponse response)
        {
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
        }
    }

    public class ErrorDetails
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";

        public HttpStatusCodeException(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, Exception inner) : this(statusCode, inner.ToString()) { }

        public HttpStatusCodeException(HttpStatusCode statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
        {
            this.ContentType = @"application/json";
        }
    }
}
