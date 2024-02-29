using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger,IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try{
               await _next(httpContext);
            }
            catch(Exception ex)
            {
                  _logger.LogError(ex,ex.Message);    
                  httpContext.Response.ContentType="application/json";
                  httpContext.Response.StatusCode=(int)HttpStatusCode.InternalServerError;
            
            var response =_env.IsDevelopment()
            ?new APIExceptions(httpContext.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
            :new APIExceptions(httpContext.Response.StatusCode,ex.Message,"Internal ServerError");

            var options=new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase};

            var json=JsonSerializer.Serialize(response,options);

            await httpContext.Response.WriteAsync(json);
            }
        }
    }
}