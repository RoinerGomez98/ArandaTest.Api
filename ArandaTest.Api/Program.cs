using ArandaTest.Api.Decorators;
using ArandaTest.Domain.Utils;
using ArandaTest.Infrastructure.Data;
using ArandaTest.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
InfrastructureServiceExtensions.AddInfrastructure(builder.Services, builder.Configuration);
InfrastructureServiceExtensions.AddSwaggerConfig(builder.Services, builder.Configuration);
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMvcCore(m =>
{
    m.Filters.Add<ErrorsFilterAttribute>();
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthorizationToken>();
});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    var securityC = new Security(builder.Configuration);
    InfrastructureServiceExtensions.AddFirstData(context, securityC);

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();

app.UseCors("ArandaPolicy");
app.UseSession();

var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Images");
if (!Directory.Exists(folderPath))
{
    Directory.CreateDirectory(folderPath);
}
app.UseStaticFiles(new StaticFileOptions
{

    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});
app.UseAuthorization();

app.MapControllers();

app.Run();
