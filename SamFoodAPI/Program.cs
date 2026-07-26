using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SamFoodAPI.IRepo;
using SamFoodAPI.Middleware;
using SamFoodAPI.Model.Common;
using SamFoodAPI.Model.Context;
using SamFoodAPI.Model.DTO;
using SamFoodAPI.Repo;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

#region Injection Repositories and Services
builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
builder.Services.AddScoped<CategoryRepo>();
builder.Services.AddScoped<UnitCountRepo>();
builder.Services.AddScoped<ProductRepo>();
builder.Services.AddScoped<ProductIngredientRepo>();
builder.Services.AddScoped<ProductProcessingRecipeRepo>();
builder.Services.AddScoped<ProductImageRepo>();
builder.Services.AddScoped<InvoiceRepo>();
builder.Services.AddScoped<InvoiceDetailRepo>();
builder.Services.AddScoped<PromotionRepo>();
builder.Services.AddScoped<HistorySearchRepo>();
builder.Services.AddScoped<BannerRepo>();
builder.Services.AddScoped<BannerDetailRepo>();

builder.Services.AddScoped<CurrentUser>(provider =>
{
    var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var claims = context?.User?.Claims?.ToDictionary(x => x.Type, x => x.Value) ?? new Dictionary<string, string>();
    CurrentUser currentUser = ObjectMapper.GetCurrentUser(claims);
    return currentUser;
});
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

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,


                        IssuerSigningKeys = new[]
                        {
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        },
                        NameClaimType = "sub"
                    };
                });
builder.Services.AddAuthentication();


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
app.UseCors("MyCors");
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<DynamicAuthorizationMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.UseStaticFiles();

var folderImage = builder.Configuration.GetValue<string>("ImagePath") ?? "";
string pathName = "/api/shared/images";
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(folderImage),
    RequestPath = new PathString(pathName)
});


app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(folderImage),
    RequestPath = new PathString(pathName)
});

//List<PathStaticFile> staticFiles = builder.Configuration.GetSection("PathStaticFiles").Get<List<PathStaticFile>>() ?? new List<PathStaticFile>();

//foreach (var item in staticFiles)
//{
//    app.UseStaticFiles(new StaticFileOptions()
//    {
//        FileProvider = new PhysicalFileProvider(item.PathFull),
//        RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
//    });


//    app.UseDirectoryBrowser(new DirectoryBrowserOptions
//    {
//        FileProvider = new PhysicalFileProvider(item.PathFull),
//        RequestPath = new PathString($"/api/share/{item.PathName.Trim().ToLower()}")
//    });
//}


app.Run();
