using System.Globalization;
using System.Reflection;
using System.Text;
using Application.Areas.Identity.Data;
using Application.Data;
using Application.Filters;
using Application.Middleware;
using Application.Services;
using Application.Services.Clients;
using Application.Services.Employees;
using Application.Services.Products;
using Application.Services.Repository;
using Application.Services.Stocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Services.Deliveries;
using Server.Services.Orders;
using Server.Services.Repository;
using Server.Services.Sales;

namespace Application
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContextConnection"));
            });

            // Identity
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
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
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            // Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Admin", "Manager"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("Admin", "Manager", "User"));
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // Repositories
            builder.Services.AddScoped<IStockRepository, StockRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();

            // Services
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ISaleService, SaleService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();

            // API
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RSAS API",
                    Version = "v1",
                    Description = "API для системы управления заказами и складом",
                    Contact = new OpenApiContact
                    {
                        Name = "RSAS Team",
                        Email = "support@rsas.com"
                    }
                });

                // Добавляем XML-комментарии
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
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
                        Array.Empty<string>()
                    }
                });

                // Добавляем аннотации для авторизации
                c.OperationFilter<SwaggerAuthorizeOperationFilter>();
            });

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5001") // Frontend URL
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Seed Data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetService<UserManager<AppUser>>();
                var roleManager = services.GetService<RoleManager<IdentityRole>>();
                var employeeRepository = services.GetService<IEmployeeRepository>();
                await AppDbConfigure.SeedRoles(roleManager);
                await AppDbConfigure.SeedUsers(userManager, employeeRepository);
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            // Добавляем middleware для обработки ошибок
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Culture settings
            app.Use(async (context, next) =>
            {
                var currentThreadCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
                currentThreadCulture.NumberFormat = NumberFormatInfo.InvariantInfo;

                Thread.CurrentThread.CurrentCulture = currentThreadCulture;
                Thread.CurrentThread.CurrentUICulture = currentThreadCulture;

                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}