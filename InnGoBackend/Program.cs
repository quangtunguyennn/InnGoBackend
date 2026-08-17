using InnGoBackend.Models.Context;
using InnGoBackend.Repositories.AuthRepository;
using InnGoBackend.Repositories.BlogRepository;
using InnGoBackend.Repositories.BookingRepository;
using InnGoBackend.Repositories.BranchRepository;
using InnGoBackend.Repositories.PaymentRepository;
using InnGoBackend.Repositories.ReviewRepository;
using InnGoBackend.Repositories.RoomRepository;
using InnGoBackend.Repositories.ServiceRepository;
using InnGoBackend.Repositories.WishlistRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Cho phép t?t c? các domain / port
              .AllowAnyMethod()  // Cho phép t?t c? các ph??ng th?c (GET, POST, PUT, DELETE...)
              .AllowAnyHeader(); // Cho phép t?t c? các headers
    });
});

// config DB  
builder.Services.AddDbContext<InnGoDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
// Add WJT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options
=>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});

// config repos 

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IWishlistRepository,  WishlistRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>(); 
builder.Services.AddScoped<IBlogRepository,  BlogRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "InnGo" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token directly below (do not type 'Bearer ')."
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

//app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
