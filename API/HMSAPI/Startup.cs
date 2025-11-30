
using HMSBAL.Interfaces.Auth;
using HMSBAL.Interfaces.Masters;
using HMSBAL.Interfaces.User;
using HMSBAL.Services.Auth;
using HMSBAL.Services.Masters;
using HMSBAL.Services.User;
using HMSDAL.Interfaces.Auth;
using HMSDAL.Interfaces.User;
using HMSDAL.Repositories.Auth;
using HMSDAL.Repositories.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Text;

namespace HMSAPI
{
    /// <summary>
    /// Configures application services and the HTTP request pipeline for the ASP.NET Core application
    /// </summary>
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ------------------- Caching (Redis) -------------------
            //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
            //    .Connect(Configuration["Redis:ConnectionString"]));

            services.AddDistributedMemoryCache();

            //var configuration = ConfigurationOptions.Parse(Configuration["Redis:ConnectionString"], true);
            //configuration.AbortOnConnectFail = false;

            //var multiplexer = ConnectionMultiplexer.Connect(configuration);
            //services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            // ------------------- JWT Authentication -------------------
            var jwt = Configuration.GetSection("Jwt");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]))
                    };
                });

            // ------------------- Controllers -------------------
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            }); 
            
            services.AddEndpointsApiExplorer();

            // ------------------- Swagger -------------------
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "HMS API",
                    Version = "v1",
                    Description = "API documentation for HMS"
                });

                // JWT Bearer definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter ‘Bearer {token}’"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                //options.AddPolicy("AllowSpecificOrigin", policy =>
                //              policy.WithOrigins(allowedOrigins)
                //                    .AllowAnyHeader()
                //                    .AllowAnyMethod()
                //                    .AllowCredentials());
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory
            (
                Configuration.GetConnectionString("Connstr"),
                MySqlDialect.Provider
            ));

            // ------------------- Register Dependencies -------------------
            RegisterCustomServices(services);

        }

        private void RegisterCustomServices(IServiceCollection services)
        {
            services.AddTransient<ILoginService, BLLoginHandler>();
            services.AddTransient<ILoginRepository, DBLoginContext>();

            services.AddTransient<IUserRightsService, BLUserRightsHandler>();
            services.AddTransient<IUserRightsRepository, DBUserRightsContext>();

            services.AddTransient<IAppointmentsService, BLAppointmentsHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ------------------- Global Error Logging -------------------
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Info("Application Starting...");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ------------------- Swagger UI -------------------
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS API v1");
                c.RoutePrefix = string.Empty; // open Swagger at root URL
            });

            // ------------------- Middleware -------------------
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
