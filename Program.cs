using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.GraphQL;
using ProjectManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<TaskRepository>();
builder.Services.AddScoped<OtpRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddScoped<ProjectMemberRepository>();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

// CORS - allow the React frontend to call this API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

app.Run();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterPolicy", policy =>
    {
        policy.AllowAnyOrigin()   // Or specify: .WithOrigins("http://192.168.2.4")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// In app pipeline (before MapControllers):
app.UseCors("FlutterPolicy");