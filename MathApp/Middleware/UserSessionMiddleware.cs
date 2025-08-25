using MathApp.Models.DbModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MathApp.Middleware
{
    public class UserSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userSession = context.Session.GetString("UserSessionKey");
            if (!string.IsNullOrEmpty(userSession))
            {
                var user = JsonConvert.DeserializeObject<User>(userSession);
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
