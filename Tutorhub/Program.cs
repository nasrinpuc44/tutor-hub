// 📁 Program.cs
using Tutorbub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== সেশন সাপোর্ট =====
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

// Configure the HTTP request pipeline.
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