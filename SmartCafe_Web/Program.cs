using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SmartCafeCookie";
        options.LoginPath = "/Account/Signin";
        options.LogoutPath = "/Account/Signout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// ADD THIS: Add Session services
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ADD THIS: Use Session before Authorization
app.UseSession();

// You already have authentication — add it here properly:
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
