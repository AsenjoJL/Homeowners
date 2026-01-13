using HOMEOWNER.Data;
using HOMEOWNER.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Allow environment variables like env:Twilio__AccountSid
builder.Configuration.AddEnvironmentVariables();

// ✅ Add MVC
builder.Services.AddControllersWithViews();

// ✅ Firebase Configuration
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"] ?? "homeowner-c355d";

// Auto-detect Firebase credentials if not set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")))
{
    // Try to find Firebase key file in Downloads folder
    var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    var firebaseKeyFile = Directory.GetFiles(downloadsPath, "*homeowner-c355d-firebase*.json")
        .FirstOrDefault();
    
    if (firebaseKeyFile != null && File.Exists(firebaseKeyFile))
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseKeyFile);
        Console.WriteLine($"✓ Auto-detected Firebase credentials: {firebaseKeyFile}");
    }
    else
    {
        Console.WriteLine("⚠️  WARNING: GOOGLE_APPLICATION_CREDENTIALS not set and Firebase key file not found.");
        Console.WriteLine("   Set it manually: $env:GOOGLE_APPLICATION_CREDENTIALS=\"C:\\path\\to\\key.json\"");
    }
}

// ✅ Register Firebase Service as IDataService
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSingleton<IDataService>(sp => sp.GetRequiredService<FirebaseService>());

// ✅ Keep SQL Server DbContext for backward compatibility (can be removed later)
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    // Register a dummy DbContext if connection string is not available
    // Note: ApplicationDbContext is kept for backward compatibility but not actively used
    // All controllers should use IDataService (FirebaseService) instead
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer("Server=.;Database=TempDb;Trusted_Connection=true;TrustServerCertificate=true"));
}

// ✅ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Route Config
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
