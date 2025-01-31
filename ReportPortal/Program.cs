using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReportPortal.BL.Maps;
using ReportPortal.BL.Services;
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

var builder = WebApplication.CreateBuilder(args);
string FrontEndUrl = builder.Configuration["FrontEndUrl"];

// DaaBase
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseLazyLoadingProxies().UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers();

// hub settings
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IRunRepository, RunRepository>();
builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
builder.Services.AddScoped<ITestReviewRepository, TestReviewRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IRunService, RunService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<ITestResultService, TestResultService>();
builder.Services.AddScoped<ITestReviewService, TestReviewService>();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Use CORS first
app.UseRouting();
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Authentication and Authorization should come after CORS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<RunUpdatesHub>("/hubs/runUpdates");

app.Run();




