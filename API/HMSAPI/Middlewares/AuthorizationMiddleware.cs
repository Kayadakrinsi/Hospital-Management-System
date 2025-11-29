using HMSMAL.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HMSAPI.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration config, IDistributedCache cache)
        {
            _next = next;
            _config = config;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute>() != null;

            if (allowAnonymous)
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Token missing");
                return;
            }

            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);

                // Try fetching user details from Redis cache
                var cacheKey = $"user_session_{token}";
                var cachedData = _cache.GetString(cacheKey);

                if (cachedData == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Session expired or invalid");
                    return;
                }

                // Store user info in HttpContext for global access
                foreach (var part in cachedData.Split(';'))
                {
                    var kv = part.Split('=');
                    if (kv.Length == 2)
                    {
                        switch (kv[0])
                        {
                            case "UserId": LoggedInUserDetails.UserId = Convert.ToInt32(kv[1]); break;
                            case "UserName": LoggedInUserDetails.UserName = kv[1]; break;
                            case "RoleId": LoggedInUserDetails.RoleId = Convert.ToInt32(kv[1]); break;
                            case "DeptId": LoggedInUserDetails.DepartmentId = Convert.ToInt32(kv[1]); break;
                            case "Role": LoggedInUserDetails.Role = kv[1]; break;
                            case "Dept": LoggedInUserDetails.Department = kv[1]; break;
                        }
                    }
                }

                //context.Items["UserInfo"] = userInfo;
            }
            catch
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid token");
                return;
            }

            await _next(context);
        }
    }
    public static class JwtAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}




