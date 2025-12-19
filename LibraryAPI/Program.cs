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
// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// JWT Configuration
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
                Encoding.UTF8.GetBytes("LibraryAPISecretKey12345678901234567890"))
        };

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


if (builder.Environment.IsDevelopment())
{
    // Use SQL Server locally
    builder.Services.AddDbContext<LibraryContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDb")));
}
else
{
    // Use SQLite in Azure
    builder.Services.AddDbContext<LibraryContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("LibraryDb")));
}

// Add Repositories
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Azure uses port 8080
});

var app = builder.Build();

// Configure the HTTP request pipeline


    app.UseSwagger();
    app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureCreated();

    // Only seed if no authors exist (fresh database)
    if (!context.Authors.Any())
    {
        Console.WriteLine("🌱 Seeding database...");

        // 1. Seed Authors FIRST
        var authors = new[]
        {
            new Author { Id = 1, Name = "J.K. Rowling", Email = "jk@example.com" },
            new Author { Id = 2, Name = "George Orwell", Email = "george@example.com" },
            new Author { Id = 3, Name = "Jane Austen", Email = "jane@example.com" }
        };
        context.Authors.AddRange(authors);
        context.SaveChanges();
        Console.WriteLine("✅ Authors seeded");

        // 2. Seed Books WITHOUT GenreId first (GenreId is nullable)
        var books = new[]
        {
            new Book { Id = 1, Title = "Harry Potter", ISBN = "123456", PublicationYear = 1997, AuthorId = 1 },
            new Book { Id = 2, Title = "1984", ISBN = "789012", PublicationYear = 1949, AuthorId = 2 },
            new Book { Id = 3, Title = "Pride and Prejudice", ISBN = "345678", PublicationYear = 1813, AuthorId = 3 }
        };
        context.Books.AddRange(books);
        context.SaveChanges();
        Console.WriteLine("✅ Books seeded (no genres yet)");

        // 3. Seed Genres
        var genres = new[]
        {
            new Genre { Id = 1, Name = "Fantasy", Description = "Fantasy literature" },
            new Genre { Id = 2, Name = "Science Fiction", Description = "Sci-fi books" },
            new Genre { Id = 3, Name = "Classic", Description = "Classic literature" },
            new Genre { Id = 4, Name = "Mystery", Description = "Mystery and thriller" }
        };
        context.Genres.AddRange(genres);
        context.SaveChanges();
        Console.WriteLine("✅ Genres seeded");

        // 4. Now assign genres to existing books
        // Get fresh references to avoid tracking issues
        var book1 = context.Books.Find(1);
        var book2 = context.Books.Find(2);
        var book3 = context.Books.Find(3);

        if (book1 != null) book1.GenreId = 1; // Harry Potter → Fantasy
        if (book2 != null) book2.GenreId = 2; // 1984 → Science Fiction  
        if (book3 != null) book3.GenreId = 3; // Pride and Prejudice → Classic

        context.SaveChanges();
        Console.WriteLine("✅ Genres assigned to books");

        Console.WriteLine("🎉 Database seeding completed successfully!");
    }
    else
    {
        Console.WriteLine("📊 Database already has data, skipping seed");
    }
}

// Let Azure control the port via environment variables
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
Console.WriteLine($"🚀 Application started on port {port}");
Console.WriteLine("📚 Swagger UI: /swagger");
Console.WriteLine("📖 Books API: /api/books");

app.Run();