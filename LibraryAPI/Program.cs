using LibraryAPI.Models;
using LibraryAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// SIMPLIFIED JWT Configuration - Less Strict
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("LibraryAPISecretKey12345678901234567890")) // 32 chars!
        };

        // For debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication Failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token Validated for: {context.Principal?.Identity?.Name}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Add DbContext
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseInMemoryDatabase("LibraryDb"));

// Add Repository
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

    // Add Bearer token support to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT ORDER: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureCreated();

    if (!context.Authors.Any())
    {
        var authors = new[]
        {
            new Author { Id = 1, Name = "J.K. Rowling", Email = "jk@example.com" },
            new Author { Id = 2, Name = "George Orwell", Email = "george@example.com" },
            new Author { Id = 3, Name = "Jane Austen", Email = "jane@example.com" }
        };
        context.Authors.AddRange(authors);
        context.SaveChanges();

        var books = new[]
        {
            new Book { Id = 1, Title = "Harry Potter", ISBN = "123456", PublicationYear = 1997, AuthorId = 1 },
            new Book { Id = 2, Title = "1984", ISBN = "789012", PublicationYear = 1949, AuthorId = 2 },
            new Book { Id = 3, Title = "Pride and Prejudice", ISBN = "345678", PublicationYear = 1813, AuthorId = 3 }
        };
        context.Books.AddRange(books);
        context.SaveChanges();

        Console.WriteLine("✅ Database seeded successfully");
    }
}

Console.WriteLine("🚀 Application started. Testing JWT...");

app.Run();