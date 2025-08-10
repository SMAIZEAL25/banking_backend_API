using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace BankingApp.Application.Response
{
    public class CustomResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        // Success Responses
        public static CustomResponse<T> Success(T data, string message = null, int statusCode = StatusCodes.Status200OK)
            => new() { IsSuccess = true, Data = data, Message = message, StatusCode = statusCode };

        // Client Error Responses (4xx)
        public static CustomResponse<T> BadRequest(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status400BadRequest };

        public static CustomResponse<T> Unauthorized(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status401Unauthorized };

        public static CustomResponse<T> Forbidden(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status403Forbidden };

        public static CustomResponse<T> NotFound(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status404NotFound };

        public static CustomResponse<T> PaymentRequired(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status402PaymentRequired };

        public static CustomResponse<T> Conflict(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status409Conflict };

        public static CustomResponse<T> FailedDependency(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status424FailedDependency };

        // Server Error Responses (5xx)
        public static CustomResponse<T> ServerError(string message)
            => new() { IsSuccess = false, Message = message, StatusCode = StatusCodes.Status500InternalServerError };
    }
}
