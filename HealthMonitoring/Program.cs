using HealthMonitoring.Configuration;
using HealthMonitoring.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.AddAppLogger();

services.Configure<TelegramSettings>(builder.Configuration.GetSection(TelegramSettings.SectionName));

services.AddBackgroundJob();
services.AddAppServices();

var app = builder.Build();
app.Run();