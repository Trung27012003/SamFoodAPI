using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamFoodAPI.Middleware;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Repo;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


#region Injection Repositories and Services
builder.Services.AddScoped<CategoryRepo>();
builder.Services.AddScoped<UnitCountRepo>();

//builder.Services.AddScoped<CurrentUser>(provider =>
//{
//    var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
//    var claims = context?.User?.Claims?.ToDictionary(x => x.Type, x => x.Value) ?? new Dictionary<string, string>();
//    CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
//    //CurrentUser currentUser = new CurrentUser();
//    return currentUser;
//});
#endregion

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Config connect database
Config.ConnectionString = builder.Configuration.GetValue<string>("ConnectionString") ?? "";
builder.Services.AddDbContext<SamFoodContext>(o => o.UseSqlServer(Config.ConnectionString));


builder.Services.AddMvc().AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);


//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();

    });
});

// Load JWT settings
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

//builder.Services.AddAuthentication("Bearer")
//                .AddJwtBearer("Bearer", options =>
//                {
//                    options.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true,
//                        ValidateIssuerSigningKey = true,

                        
//                        IssuerSigningKeys = new[]
//                        {
//                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
//                        },
//                        NameClaimType = "sub"
//                    };
//                });
//builder.Services.AddAuthentication();


builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Chuyển tất cả URL thành chữ thường
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("MyCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
