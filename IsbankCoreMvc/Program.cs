using IsbankCoreMvc.Models;
using IsbankCoreMvc.Services;
using IsBankMvc.Abstraction;
using IsBankMvc.Business;
using IsBankMvc.DataAccess;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<UserAuthData, UserAuthData>();

builder.Services.RegisterAbstraction();
builder.Services.RegisterBusiness();
builder.Services.RegisterDataAccess();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.Configure<CookiePolicyOptions>(opt =>
{
    opt.CheckConsentNeeded = context => true;
    opt.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAntiforgery(opt =>
{
    opt.Cookie.Name = "xsrf";
    opt.Cookie.HttpOnly = false;
});
builder.Services.Configure<IdentityOptions>(IdentityConfiguration.ConfigureOptions);
builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");   
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest,
    CheckConsentNeeded = context => true
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();