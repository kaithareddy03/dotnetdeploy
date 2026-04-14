using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RSign.Authentication;
using RSign.Common;
using RSign.ManageDocument.Helpers;
using RSign.ManageDocument.Interfaces;
using RSign.Models;
using RSign.Models.APIModels;
using RSign.Models.EmailQueueProcessor;
using RSign.Models.Helpers;
using RSign.Models.Interfaces;
using RSign.Models.Repository;
using RSign.SendAPI.API;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Authentication & authorization with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder!.Configuration.GetSection("JWT")["Key"]!))
    }
);
builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("BearerAuthentication").AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, CustomAuthorizeAccess>("BearerAuthentication", null);

builder.Services.Configure<AppSettingsConfig>(builder.Configuration);
builder.Services.AddDbContext<RSignDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RSignContext"));
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisUris"];
    options.InstanceName = "rsignsend_";
});
var multiplexer = RedisService.Connect(builder.Configuration["RedisUris"]);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

builder.Services.AddScoped<IAuthenticateRepository, AuthenticateRepository>();
builder.Services.AddScoped<IEnvelopeRepository, EnvelopeRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentContentsRepository, DocumentContentsRepository>();
builder.Services.AddScoped<IDraftRepository, DraftRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IMasterDataRepository, MasterDataRepository>();
builder.Services.AddScoped<IRecipientRepository, RecipientRepository>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenericRepository, GenericRepository>();
builder.Services.AddScoped<IConditionalControlRepository, ConditionalControlRepository>();
builder.Services.AddScoped<IEnvelopeHelperMain, EnvelopeHelperMain>();
builder.Services.AddScoped<IESignHelper, ESignHelper>();
builder.Services.AddScoped<IModelHelper, ModelHelper>();
builder.Services.AddScoped<IAsposeHelper, AsposeHelper>();
builder.Services.AddScoped<IDownloadDocSotrageHelper, DownloadDocSotrageHelper>();
builder.Services.AddScoped<IApiHelper, ApiHelpers>();
builder.Services.AddScoped<IEmailQueueProcessor, EmailQueueProcessor>();
builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
builder.Services.AddScoped<IGhostScriptHelper, GhostScriptHelper>();
builder.Services.AddScoped<IConversionRepository, ConversionRepository>();
builder.Services.AddScoped<IWordPdfTemplateHelpers, WordPdfTemplateHelpers>();
builder.Services.AddScoped<IIntegrationRepository, IntegrationRepository>();
builder.Services.AddScoped<IManageAdminRepository, ManageAdminRepository>();
builder.Services.AddScoped<ApiEndpoints>();
builder.Services.AddScoped<AuthenticateEndPoints>();
builder.Services.AddScoped<AuthenticateService>();
builder.Services.AddScoped<EnvelopeEndpoint>();
builder.Services.AddScoped<TemplateEndpoint>();
builder.Services.AddScoped<UserEndpoint>();
builder.Services.AddScoped<SettingsEndPoint>();
builder.Services.AddScoped<DocumentEndPoint>();
builder.Services.AddScoped<RecipientEndPoint>();
builder.Services.AddScoped<SendEndPoint>();
builder.Services.AddScoped<AddOnEndpoint>();
builder.Services.AddScoped<DraftEndPoint>();
builder.Services.AddScoped<LogEndPoint>();
builder.Services.AddScoped<TemplateDocumentEndPoint>();
builder.Services.AddScoped<RoleEndPoint>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<PdfEndPoints>();
builder.Services.AddScoped<IPdfRepository, PdfRepository>();
builder.Services.AddScoped<ISendRepository, SendRepository>();
builder.Services.AddScoped<IContactDetailRepository, ContactDetailRepository>();
builder.Services.AddScoped<IValidationRepository, ValidationRepository>();
builder.Services.AddScoped<IAddOnRepository, AddOnRepository>();
builder.Services.AddScoped<DocumentPackageEndpoint>();
builder.Services.AddScoped<IDocumentPackageRepository, DocumentPackageRepository>();
builder.Services.AddScoped<UploadDocumentEndPoint>();
builder.Services.AddScoped<IUploadDocumentRepository, UploadDocumentRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<DashboardEndpoint>();
builder.Services.AddScoped<IntegrationEnvelopeEndPoint>();
builder.Services.AddScoped<IIntegrationEnvelope, IntegrationEnvelopeRepository>();
builder.Services.AddScoped<ISignRepository, SignRepository>();
builder.Services.AddScoped<IBulkUploadRepository, BulkUploadRepository>();
builder.Services.AddScoped<ITemplateInfoRepository, TemplateInfoRepository>();
builder.Services.AddScoped<ICommonHelper, CommonHelper>();
builder.Services.AddScoped<ITemplateDocumentRepository, TemplateDocumentRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISharedAccessRepository, SharedAccessRepository>();
builder.Services.AddScoped<SendToRSignEndPoint>();
builder.Services.AddScoped<EnvelopeIndexDto>();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SendAPI",
        Description = "This project for managing RSign Sender API",
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        //Type = SecuritySchemeType.,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();
// Add custom security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "ALLOW-FROM https://app.bullhornstaffing.com/ https://identity.vincere.io/  https://rsign-sandbox.vincere.io/");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("set-cookie", "SameSite=none");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    //context.Response.Headers.Add("Content-Security-Policy","default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none';");
    context.Response.Headers.Add("Content-Security-Policy", "'self'");

    context.Response.Cookies.Append("sessionId", "rsign123", new CookieOptions
    {
        Secure = true, 
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        Path = "/",
        Expires = DateTimeOffset.UtcNow.AddHours(10)
    });


    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(x => x
 .AllowAnyMethod()
.AllowAnyHeader()
.WithExposedHeaders("Content-Disposition")
.SetIsOriginAllowed(origin => true) // allow any origin
.AllowCredentials()); // allow credentials
app.UseCors("AllowAngular");

using (var scope = app.Services.CreateScope())
{
    //This snippet defines a minimal endpoint registration for an ASP.NET Core Web API. 
    //Minimal APIs are a lightweight way to build HTTP APIs with minimal ceremony.Instead of using controllers, attributes, and dependency injection via constructors(as in traditional MVC or Web API), you define endpoints directly on the WebApplication object.
    //No controllers — endpoints are registered directly in Program.cs or via extension methods like your RegisterSendAPI
    //Simplified routing -uses MapGet, MapPost, etc., instead of[HttpGet], [Route].
    //Fast startup and low overhead — ideal for microservices or small APIs.
    //you can modularize endpoint registration using extension methods (like you're doing).

    var authService = scope.ServiceProvider.GetService<AuthenticateEndPoints>()!;
    authService.RegisterAuthApis(app);
    var envelope = scope.ServiceProvider.GetService<EnvelopeEndpoint>()!;
    envelope.RegisterEnvelopeAPI(app);
    var template = scope.ServiceProvider.GetService<TemplateEndpoint>()!;
    template.RegisterTemplateAPI(app);
    var user = scope.ServiceProvider.GetService<UserEndpoint>()!;
    user.RegisterEnvelopeAPI(app);
    var settings = scope.ServiceProvider.GetService<SettingsEndPoint>()!;
    settings.RegisterSettingsAPI(app);
    var pdfEndpoints = scope.ServiceProvider.GetService<PdfEndPoints>()!;
    pdfEndpoints.PdfApis(app);
    var documentEndPoint = scope.ServiceProvider.GetService<DocumentEndPoint>()!;
    documentEndPoint.RegisterDocumentAPI(app);
    var recipientEndPoint = scope.ServiceProvider.GetService<RecipientEndPoint>()!;
    recipientEndPoint.RegisterRecipientAPI(app);
    var sendEndPoint = scope.ServiceProvider.GetService<SendEndPoint>()!;
    sendEndPoint.RegisterSendAPI(app);
    var addOnEndpoint = scope.ServiceProvider.GetService<AddOnEndpoint>()!;
    addOnEndpoint.RegisterAddOnAPI(app);
    var draftEndPoint = scope.ServiceProvider.GetService<DraftEndPoint>()!;
    draftEndPoint.RegisterDraftAPI(app);
    var UploadDocumentEndPoint = scope.ServiceProvider.GetService<UploadDocumentEndPoint>()!;
    UploadDocumentEndPoint.UploadDocumentAPI(app);
    var DocumentPackageEndpoint = scope.ServiceProvider.GetService<DocumentPackageEndpoint>()!;
    DocumentPackageEndpoint.DocumentPackageAPI(app);
    var SendToRSignEndPoint = scope.ServiceProvider.GetService<SendToRSignEndPoint>()!;
    SendToRSignEndPoint.RegisterSendToRSignAPI(app);
    var dashboard = scope.ServiceProvider.GetService<DashboardEndpoint>()!;
    dashboard.RegisterDashboardAPI(app);
    var integrationEnvelope = scope.ServiceProvider.GetService<IntegrationEnvelopeEndPoint>()!;
    integrationEnvelope.RegisterIntegrationEnvelopeAPI(app);
    var logEndPoint = scope.ServiceProvider.GetService<LogEndPoint>()!;
    logEndPoint.RegisterLogAPI(app);
    var templateDocumentEndPoint = scope.ServiceProvider.GetService<TemplateDocumentEndPoint>()!;
    templateDocumentEndPoint.RegisterTemplateDocumentAPI(app);
    var roleEndPoint = scope.ServiceProvider.GetService<RoleEndPoint>()!;
    roleEndPoint.RegisterRoleAPI(app);
}
app.Run();
