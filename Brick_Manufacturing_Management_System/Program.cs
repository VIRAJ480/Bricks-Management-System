using Brick_Manufacturing_Management_System.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Myconnection in program.cs
var connectionString = builder.Configuration.GetConnectionString("MyConnection");
builder.Services.AddDbContext<BrickErpdbContext>(options =>
	options.UseSqlServer(connectionString));


// ── Session ──────────────────────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();       // in-memory session store
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // auto-logout after 30 min idle
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ── Must be between UseRouting() and UseAuthorization() ──────────────────────
app.UseSession();

app.UseAuthorization();

// ── Default route → Login page ───────────────────────────────────────────────
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
