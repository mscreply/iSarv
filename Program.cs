using System.Globalization;
using System.Reflection;
using iSarv.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using iSarv.Data;
using iSarv.Data.CultureModels;
using iSarv.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? 
                       throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    }).AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SupportedUICultures = new[]
    {
        new CultureInfo("fa")
        {
            /* change calendar to Hijri */
            DateTimeFormat = {Calendar = new PersianCalendar()},
        },
        new CultureInfo("en")
    };
    options.SetDefaultCulture("fa");
    options.FallBackToParentUICultures = true;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

builder.Services.AddMvc()
    .AddDataAnnotationsLocalization(o =>
    {
        var type = typeof(ViewResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName ?? string.Empty);
        var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
        var localizer = factory?.Create("ViewResource", assemblyName.Name ?? string.Empty);
        o.DataAnnotationLocalizerProvider = (t, f) => localizer;
    })
    .AddViewLocalization();
builder.Services.AddScoped<RequestLocalizationCookiesMiddleware>();
builder.Services.AddSingleton<CultureLocalizer>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add builder.Services to the container.
builder.Services.AddRazorPages().AddViewLocalization();
builder.Services.AddControllers();

// Email Configuration
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseRequestLocalizationCookies();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapControllers();

// Initialization
await InitSeedData.CreateAdminAccountAsync(app.Services, app.Configuration);

app.Run();