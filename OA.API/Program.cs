using OA.API.Middleware;
using Microsoft.EntityFrameworkCore;
using OA.Domain.Inform;
using OA.Infrastructure;
using OA.API.Filter;
using OA.API.Interface;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OA.API.Filter.Authorization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<IInformRepository, InformRepository>();
builder.Services.AddSingleton<IReqeustJsonBodyReader, DefaultRequestJsonBodyReader>();
builder.Services.AddControllers(x =>
{
    x.ModelBinderProviders.Insert(0, new ApiModelBindProvider());
    //去掉mvc自定义的模型校验model
    x.ModelValidatorProviders.Clear();
    x.Filters.Add<ApiActionFilter>();
    x.Filters.Add<ApiResultFilter>();
    x.Filters.Add<ApiExceptionFilter>();
});
//.AddJsonOptions(x => x.JsonSerializerOptions.Converters.Insert(0, new TextJsonDateTimeConverter() { }));
builder.Services.TryAddEnumerable(new ServiceDescriptor(typeof(IAuthorizeAction), typeof(TimestampAuthorizeAction), ServiceLifetime.Singleton));
builder.Services.TryAddSingleton<IAuthorizeProvider, AuthorizeProvider>();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BusinessDbContext>(builder =>
{
    var sqlconn = configuration.GetConnectionString("SqlServerConnection");
    builder.UseSqlServer(sqlconn);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiProtocolMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
