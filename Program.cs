using System.Globalization;
using System.Reflection;
using iSarv.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using iSarv.Data;
using iSarv.Data.CultureModels;
using iSarv.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.Run();