using LOGINPRUEBA.web.Data;
using LOGINPRUEBA.web.Data.Entities;
using LOGINPRUEBA.web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()

//Para CRUD Multinivel para que no de vueltas
.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//INYECTAMOS EL DATACONTEXT
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=SqlConnection"));

//INYECCION DE IDENTIDAD DE CONTRASE?A, CAMBIAR PARAMETROS TODO
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = true;
    cfg.Password.RequiredUniqueChars = 1;
    cfg.Password.RequireLowercase = true;
    cfg.Password.RequireNonAlphanumeric = true;
    cfg.Password.RequireUppercase = true;
    cfg.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

//INYECTAMOS LOS SEEDERS
builder.Services.AddTransient<SeedDb>();

//INYECTAMOS EL USERHELPER
builder.Services.AddScoped<IUserHelper, UserHelper>();

var app = builder.Build();

//VA CON LOS SEEDERS
SeedData(app);

//VA CON LOS SEDDERS
static void SeedData(WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    IServiceScope serviceScope = scopedFactory!.CreateScope();
    using IServiceScope? scope = serviceScope;
    SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
    service!.SeedAsync().Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();