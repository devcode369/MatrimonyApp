using System.Text;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.Services;
using API.Services.Inerfaces;
using API.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");

using var scope= app.Services.CreateScope();
var service=scope.ServiceProvider;

try{
   var context=service.GetRequiredService<DataContext>();
   var userManger=service.GetRequiredService<UserManager<AppUser>>();
   var roleManager=service.GetRequiredService<RoleManager<AppRole>>();

   await context.Database.MigrateAsync();
  await Seed.SeedUsers(userManger,roleManager);

}
catch(Exception ex)
{
    var logger=service.GetService<ILogger<Program>>();
    logger.LogError(ex,"Error Occured");
}

app.Run();
