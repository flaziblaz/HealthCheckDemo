using Microsoft.Extensions.Diagnostics.HealthChecks;

using Health;
using Microsoft.AspNetCore.Builder;
using HealthChecks.UI.Client;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck(
        
        "OrderingDB-check",
        new Health.SqlConnectionHealthCheck(builder.Configuration["Health"]),
        HealthStatus.Unhealthy,
        new string[] { "orderingdb" })
    .AddCheck(
         "Portal Check",
        new Health.PortalHealthCheck(),
        HealthStatus.Unhealthy,
        new string[] { "Portal" }
    );
builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(15);
    opt.MaximumHistoryEntriesPerEndpoint(60);
    opt.SetApiMaxActiveRequests(1);

    opt.AddHealthCheckEndpoint("health api", "/hc");
})
            .AddInMemoryStorage();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.MapHealthChecks("/hc");
app.MapHealthChecks("/hc", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
// build the app, register other middleware

app.UseHealthChecksUI(config => config.UIPath = "/hc-ui");
app.UseAuthorization();

app.MapControllers();

app.Run();
