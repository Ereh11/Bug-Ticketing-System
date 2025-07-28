using BugTrackingSystem.DAL;
using BugTrackingSystem.BL;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Cors;
var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  // Your Angular app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
// Add services to the container.
#region Default
builder.Services.AddControllers();
builder.Services.AddOpenApi();
#endregion

#region Services
builder.Services.AddAccessLayer(builder.Configuration);
builder.Services.AddBusinessLayer();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
#endregion


var app = builder.Build();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
#region Middlewares
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion

