// 📁 Program.cs
// লোকেশন: Tutorbub/Program.cs

using Tutorbub;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// ===== ফাইল আপলোডের জন্য Kestrel limit বাড়ানো =====
// ============================================================
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // ৬০০ MB পর্যন্ত request body গ্রহণ করবে
    serverOptions.Limits.MaxRequestBodySize = 600 * 1024 * 1024;
});

// ============================================================
// ===== Form Options limit =====
// ============================================================
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 600 * 1024 * 1024; // ৬০০ MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// ===== MVC সাপোর্ট =====
builder.Services.AddControllersWithViews();

// ============================================================
// ===== সেশন সাপোর্ট =====
// ============================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// IConfiguration ইনজেক্ট
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// ============================================================
// ===== Configure the HTTP request pipeline =====
// ============================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ===== স্ট্যাটিক ফাইলস =====
app.UseStaticFiles();

app.UseRouting();

// ===== সেশন মিডলওয়্যার =====
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===== Teacher Controller Route =====
app.MapControllerRoute(
    name: "teacher",
    pattern: "Teacher/{action=TeacherDashboard}/{id?}",
    defaults: new { controller = "Teacher" });

app.Run();