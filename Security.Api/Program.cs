using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Security.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();

var secretKey = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("secretKey") ?? throw new Exception("secretKey is not defined"));

// manual without Identity
//builder.Services.AddAuthentication(option =>
//{
//	option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//	option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//	option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(option =>
//{
//	option.TokenValidationParameters = new TokenValidationParameters
//	{
//		ValidateIssuerSigningKey = true,
//		IssuerSigningKey = new SymmetricSecurityKey(secretKey),
//		ValidateLifetime = true,
//		ValidateAudience = false,
//		ValidateIssuer = false,
//		ClockSkew = TimeSpan.Zero,
//	};
//});

// with Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    }
    ).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

