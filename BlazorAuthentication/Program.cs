using BlazorAuthentication.Core.Services;
using BlazorAuthentication.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    //Google Authentication
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["GoogleKeys:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
    })
    //Facebook Authentication
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    })
    .AddTwitter(twitterOptions =>
    {
        twitterOptions.ConsumerKey = builder.Configuration["TwitterAuthSetting:ApiKey"];
        twitterOptions.ConsumerSecret = builder.Configuration["TwitterAuthSetting:ApiSecret"];
        twitterOptions.RetrieveUserDetails = false;
        //twitterOptions.CallbackPath = new PathString("/signin-twitter");
    })
    //GitHub Authentication
    .AddGitHub(options =>
    {
        options.ClientSecret = builder.Configuration["GithubKeys:ClientSecret"];
        options.ClientId = builder.Configuration["GithubKeys:ClientId"];
        options.CallbackPath = "/signin-oidc";
        options.Scope.Add("user:email");
    });



var connectionString = builder.Configuration.GetConnectionString("BlazorAuthenticationContextConnection") ?? throw new InvalidOperationException("Connection string 'BlazorAuthenticationContextConnection' not found.");

builder.Services.AddDbContext<BlazorAuthenticationContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BlazorAuthenticationContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<IPersonService, PersonService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
