using DeliveryApp.Api;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();

app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; });
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Delivery Service");
});

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


app.Run();