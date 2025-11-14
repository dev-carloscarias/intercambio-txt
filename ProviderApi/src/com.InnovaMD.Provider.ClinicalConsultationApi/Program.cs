using com.InnovaMD.Provider.PortalApi;
using Microsoft.AspNetCore.Builder;
using System.IO;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
});
var startup = new Startup(builder.Environment);
startup.ConfigureServices(builder.Services, builder);
var app = builder.Build();
startup.Configure(app, app.Environment);
app.Run();