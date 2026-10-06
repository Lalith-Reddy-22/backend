using Medtrix_PmTool_V2_Api.DbContext;
using Medtrix_PmTool_V2_Api.Interfaces.Employee;
using Medtrix_PmTool_V2_Api.Interfaces.Leave;
using Medtrix_PmTool_V2_Api.Interfaces.Reports;
using Medtrix_PmTool_V2_Api.Interfaces.ResourceAllocation;
using Medtrix_PmTool_V2_Api.Interfaces.Timesheet;
using Medtrix_PmTool_V2_Api.Interfaces.Asset;
using Medtrix_PmTool_V2_Api.Interfaces.Task;
using Medtrix_PmTool_V2_Api.Interfaces.Project;
using Medtrix_PmTool_V2_Api.Interfaces.Ticket;
using Medtrix_PmTool_V2_Api.Interfaces.Finance;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Medtrix_PmTool_V2_Api.Middleware;
using Medtrix_PmTool_V2_Api.Repositories.Employee;
using Medtrix_PmTool_V2_Api.Repositories.Leave;
using Medtrix_PmTool_V2_Api.Repositories.Reports;
using Medtrix_PmTool_V2_Api.Repositories.ResourceAllocation;
using Medtrix_PmTool_V2_Api.Repositories.Timesheet;
using Medtrix_PmTool_V2_Api.Repositories.Asset;
using Medtrix_PmTool_V2_Api.Repositories.Task;
using Medtrix_PmTool_V2_Api.Repositories.Project;
using Medtrix_PmTool_V2_Api.Repositories.Ticket;
using Medtrix_PmTool_V2_Api.Repositories.Finance;
using Medtrix_PmTool_V2_Api.Repositories.Auth;
using Medtrix_PmTool_V2_Api.Services.Auth;
using Medtrix_PmTool_V2_Api.Services.Email;
using Medtrix_PmTool_V2_Api.Interfaces.Client;
using Medtrix_PmTool_V2_Api.Repositories.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Medtrix_PmTool_V2_Api.Services.Export;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── JWT Authentication setup 
var jwtKey = builder.Configuration["Jwt:SecretKey"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.Zero  
        };
    });

// Role-based authorization 

builder.Services.AddAuthorization(options =>
{
    //  Admin, PM, PM_Employee only
    options.AddPolicy("FinanceAccess", policy =>
        policy.RequireClaim("userType", "1", "2", "4"));

    //all authenticated users
    options.AddPolicy("AllUsers", policy =>
        policy.RequireClaim("userType", "1", "2", "3", "4", "5", "6"));
});

// Add services to the container.
builder.Services.AddTransient<DbContext>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();
builder.Services.AddScoped<IDailyReports, DailyReports>();
builder.Services.AddScoped<IWeekUtilization, WeekUtilizationRepository>();
builder.Services.AddScoped<ITeamUtilization, TeamUtilizationRepository>();
builder.Services.AddScoped<IClientRevenue, ClientRevenueRepository>();
builder.Services.AddScoped<IAllReport, AllReportRepository>();
builder.Services.AddScoped<ILeave, LeaveRepository>();
builder.Services.AddScoped<IResourceAllocation, ResourceAllocationRepository>();
builder.Services.AddScoped<ITimesheet, TimesheetRepository>();
builder.Services.AddScoped<ITask, TaskRepository>();
builder.Services.AddScoped<IAsset, AssetRepository>();
builder.Services.AddScoped<IProject, ProjectRepository>();
builder.Services.AddScoped<ITicket, TicketRepository>();
builder.Services.AddScoped<IFinance, FinanceRepository>();
builder.Services.AddScoped<IFinanceProject, FinanceProjectRepository>();
builder.Services.AddScoped<IFinanceProjectList, FinanceProjectListRepository>();
builder.Services.AddScoped<IFinanceProjectList2, FinanceProjectList2Repository>();
builder.Services.AddScoped<IFinanceRateCard, FinanceRateCardRepository>();
builder.Services.AddScoped<IAuth, AuthRepository>();
builder.Services.AddScoped<IForgotPassword, ForgotPasswordRepository>();
builder.Services.AddScoped<IFinanceProjectListRateCard, FinanceProjectListRateCardRepository>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IClient, ClientRepository>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddHttpClient<EmailService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Enter your JWT token. Example: eyJhbGci..."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

// Global exception handler 
app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseAuthentication(); 

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
