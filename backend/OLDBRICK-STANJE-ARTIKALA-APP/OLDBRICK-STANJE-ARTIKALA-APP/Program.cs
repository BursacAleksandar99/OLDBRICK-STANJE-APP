using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Data;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.Auth;
using Npgsql;
using OLDBRICK_STANJE_ARTIKALA_APP.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.User;
using Microsoft.OpenApi.Models;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.BeerServices;
using OLDBRICK_STANJE_ARTIKALA_APP.Services.DailyReports;
using OLDBRICK_STANJE_ARTIKALA_APP.Custom_items;

namespace OLDBRICK_STANJE_ARTIKALA_APP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
         ?? builder.Configuration["DATABASE_URL"];

            Console.WriteLine("=== EFFECTIVE DB SETTINGS (local) ===");
            Console.WriteLine(cs);


            try
            {
                var b = new Npgsql.NpgsqlConnectionStringBuilder(cs);
                Console.WriteLine($" Host={b.Host} Port={b.Port} DB={b.Database} User={b.Username} SSL={b.SslMode}");
            }
            catch { /* ignore parse errors */ }
            // ===== CORS (JEDNOM) =====
            const string corsPolicyName = "FrontendCors";

            var allowedOrigins = new[]
            {
                "https://oldbrick-stanje-app.vercel.app",
                "http://localhost:5174",
                "http://localhost:3000"
            };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                // .AllowCredentials() // koristi samo ako saljes cookies; za JWT u headeru ne treba
                );
            });

            // Controllers + FloatConverter
            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new FloatConverter());
                });

            // DB
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine(">>> DefaultConnection from config: " + connectionstring);

            // JWT settings
            var jwtSettings = new JwtSettings();
            builder.Configuration.GetSection("Jwt").Bind(jwtSettings);

            // Swagger (tvoj sa Bearer)
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Oldbrick API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Unesi samo token"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // DI
            builder.Services.AddSingleton(jwtSettings);
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBeerService, BeerService>();
            builder.Services.AddScoped<IDailyReportService, DailyReportService>();
            builder.Services.AddScoped<IDailyBeerStateService, DailyBeerStateService>();
            builder.Services.AddScoped<IProsutoService, ProsutoService>();

            // Auth
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            // Endpoints explorer (za swagger)
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            // CORS mora posle routing-a, pre auth
            app.UseCors(corsPolicyName);

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/health", () => Results.Ok("OK"));

            app.Run();
        }
    }
}
