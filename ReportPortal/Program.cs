using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReportPortal.BL.Maps;
using ReportPortal.BL.Services;
using ReportPortal.BL.Services.Caching;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL;
using ReportPortal.DAL.Repositories;
using ReportPortal.DAL.Repositories.Interfaces;
using ReportPortal.Hubs;
using ReportPortal.Interfaces;
using ReportPortal.Maps;
using ReportPortal.Services;
using ReportPortal.Services.Interfaces;
using System.Text;
using ReportPortal.MiddleWare;
using ReportPortal.BL.Configuration;
using ReportPortal.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Single source of configuration: urls, connection string and JWT all come from the
// "AppSettings" section (appsettings.json). Standard env-var override still works via
// AppSettings__BackendUrl / AppSettings__ConnectionString etc.
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()
                  ?? throw new InvalidOperationException("Missing 'AppSettings' configuration section.");
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.WebHost.UseUrls(appSettings.BackendUrl);
builder.Services.AddDbContext<ApplicationContext>(options => options.UseLazyLoadingProxies().UseSqlServer(appSettings.ConnectionString));

// Add services to the container.
// Global tenant-scope guard: non-admins can only touch resources in their own projects.
builder.Services.AddControllers(options => options.Filters.Add<ProjectScopeFilter>());

// hub settings
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectScopeRepository, ProjectScopeRepository>();
builder.Services.AddScoped<ISubprojectRepository, SubprojectRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IRunRepository, RunRepository>();
builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
builder.Services.AddScoped<ITestReviewRepository, TestReviewRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISubprojectService, SubprojectService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IRunService, RunService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<ITestReviewService, TestReviewService>();
builder.Services.AddScoped<ITrxParserService, TrxParserService>();
builder.Services.AddSingleton<IFolderTreeCache, FolderTreeCache>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(ServiceMappingProfile));
builder.Services.AddAutoMapper(typeof(ControllerMappingProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = appSettings.Jwt.Issuer,
            ValidAudience = appSettings.Jwt.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Key))
        };
    });

// Permission-based authorization: a policy per discrete permission, mapped from roles.
builder.Services.AddAuthorization(options => options.AddPermissionPolicies());

// CORS
var corsPolicyName = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, builder =>
    {
        builder.WithOrigins(appSettings.FrontendUrl)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// No user seeding: the first admin is created explicitly via POST api/UserManagement/SetupAdmin
// (allowed only while the Users table is empty).

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // NOTE: no UseDeveloperExceptionPage — ExceptionHandlingMiddleware is the single handler so
    // domain exceptions map to proper status codes (404/409/403) without leaking stack traces.
}

// Use CORS first
app.UseCors(corsPolicyName);
app.UseRouting();

// NOTE: running over plain HTTP for now (no UseHttpsRedirection). HTTPS/dev-cert is the next step.

// Authentication and Authorization should come after CORS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<RunUpdatesHub>("/hubs/runUpdates");

app.Run();




