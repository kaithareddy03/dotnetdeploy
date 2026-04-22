using dotenv.net;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using RSign.Common;
using RSign.ManageDocument.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models;
using RSign.Models.EmailQueueProcessor;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;

DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: true, envFilePaths: new[] { "../.env" }));

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "RSign API",
        Description = "REST API for managing RSign Signing Documents",
    });
});

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

builder.Services.AddCors();
builder.Services.Configure<AppSettingsConfig>(builder.Configuration);
builder.Services.AddDbContext<RSignDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RSignContext"));
});

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
builder.Host.UseNLog();

builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentContentsRepository, DocumentContentsRepository>();
builder.Services.AddScoped<IEnvelopeRepository, EnvelopeRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IMasterDataRepository, MasterDataRepository>();
builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenericRepository, GenericRepository>();
builder.Services.AddScoped<IConditionalControlRepository, ConditionalControlRepository>();
builder.Services.AddScoped<IEnvelopeHelperMain, EnvelopeHelperMain>();
builder.Services.AddScoped<IESignHelper,ESignHelper>();
builder.Services.AddScoped<IModelHelper, ModelHelper>();
builder.Services.AddScoped<IAsposeHelper, AsposeHelper>();
builder.Services.AddScoped<IApiHelper, ApiHelpers>();
builder.Services.AddScoped<IEmailQueueProcessor, EmailQueueProcessor>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();

var app = builder.Build();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials
//}

app.MapControllers();

app.Run();
