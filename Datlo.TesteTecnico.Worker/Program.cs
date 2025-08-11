using Datlo.TesteTecnico.Worker.Extensions;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

try
{
    // Configurar todas as dependências do Worker
    builder.Services.AddWorkerServices(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Hangfire Dashboard
    var dashboardEnabled = builder.Configuration.GetValue<bool>("Hangfire:DashboardEnabled", true);
    if (dashboardEnabled)
    {
        var dashboardPath = builder.Configuration.GetValue<string>("Hangfire:DashboardPath", "/hangfire");
        app.UseHangfireDashboard(dashboardPath);
    }

    app.UseRouting();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    throw;
}