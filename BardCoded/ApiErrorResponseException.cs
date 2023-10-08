using Bardcoded.Shaded.Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.Serialization;

namespace Bardcoded
{
    [Serializable]
    internal class ApiErrorResponseException : Exception
    {
        public string Bard { get; }
        public HttpStatusCode StatusCode { get; }
        public ProblemDetails? Result { get; }

        public ApiErrorResponseException()
        {
        }

        public ApiErrorResponseException(string? message, string bard, HttpStatusCode statusCode, ProblemDetails? res, Exception inner = null) : base(message,inner)
        {
            this.Bard = bard;
            this.StatusCode = statusCode;
            this.Result = res;
        }
    }
}