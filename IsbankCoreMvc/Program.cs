using IsBankMvc.Abstraction;
using IsBankMvc.Business;
using IsBankMvc.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterAbstraction();
builder.Services.RegisterBusiness();
builder.Services.RegisterDataAccess();

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();