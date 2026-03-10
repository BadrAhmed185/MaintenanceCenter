using MaintenanceCenter.Application.Common;
using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Application.Services;
using MaintenanceCenter.Domain;
using MaintenanceCenter.Domain.Entities;
using MaintenanceCenter.Infrastructure.Repositories;
using MaintenanceCenter.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace MaintenanceCenter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // 🔑 Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token.\nExample: Bearer 12345abcdef"
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
                        new string[] {}
                    }
                });
            });



            // 1. Configure Database Context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            Console.WriteLine("Hello\n");
            Console.WriteLine(connectionString);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            // 2. Configure Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // You can configure password policies here later
                // options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // After .AddDefaultTokenProviders(), add this:
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // This prevents Identity from hijacking the default scheme
                options.LoginPath = "/Auth/Login";
            });

            // 2. Configure JWT Authentication to read from the cookie
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Prevent Identity from overriding JWT as the default scheme
            })
            .AddJwtBearer(options =>
            {
                // ... your existing TokenValidationParameters here ...
                var jwtSettings = builder.Configuration.GetSection("JWT").Get<JwtSettings>();
                if (jwtSettings == null)
                {
                    throw new InvalidOperationException("JWT settings are not configured properly.");
                }

                options.SaveToken = true;

                /// important to change in prod
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    // UNCOMMENTED AND CONFIGURED:
                    ValidateIssuer = false,
                   // ValidIssuer = jwtSettings.ValidIssuer,

                    ValidateAudience = false,
                   // ValidAudience = jwtSettings.ValidAudience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                Console.WriteLine(options.TokenValidationParameters.IssuerSigningKey);

                // ADD THIS EVENT TO EXTRACT TOKEN FROM COOKIE:
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // 1. Try to get the token from the Header (Swagger)
                        var authHeader = context.Request.Headers["Authorization"].ToString();
                        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                        {
                            context.Token = authHeader.Substring("Bearer ".Length).Trim();
                            Console.WriteLine("Token extracted from Header.");
                        }
                        // 2. FALLBACK to Cookie ONLY if the header is missing (Browser UI)
                        else if (context.Request.Cookies.ContainsKey("jwt"))
                        {
                            context.Token = context.Request.Cookies["jwt"];
                            Console.WriteLine("Token extracted from Cookie.");
                        }

                        return Task.CompletedTask;
                    },
                    // ADD THIS: It will tell you exactly WHY a 401 is happening in the console
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication Failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });


            // 4. Register Application Services
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


            // Add this right after you register your DbContext
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IWorkshopService, WorkshopService>();
            builder.Services.AddScoped<ISparePartService, SparePartService>();
            builder.Services.AddScoped<IMaintenanceService, MaintananceService_Service>();
            builder.Services.AddScoped<ITechnicianService, TechnicianService>();
            builder.Services.AddScoped<IMaintenanceRequestService, MaintenanceRequestService>();

            // add this earlier in Program.cs
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            builder.Services.AddScoped<TokenService>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();

                  //      app.UseHttpsRedirection();
        
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //   app.UseHsts();


            }

            app.UseHttpsRedirection();
            // ... down in the pipeline ...
            app.UseStaticFiles(); // Required to serve CSS, JS, Images from wwwroot
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionMiddleware>();


            // Map both API and MVC routes
            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
