using Application.Services.Interfaces;
using Application.Utility;
using AspNetCoreRateLimit;
using Domain;
using Domain.Dtos.Account;
using Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

builder.Services.AddInMemoryRateLimiting();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOption>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SSOModel>(builder.Configuration.GetSection("SSO"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddAntiforgery(o => { o.Cookie.Name = "X-XSRF"; o.HeaderName = "X-XCSRF"; o.SuppressXFrameOptionsHeader = false; });

builder.Services.AddMvc(opt =>
{
    //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    //opt.Filters.Add(new AuthorizeFilter(policy));

    //opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

#region JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie(x =>
    {
        x.Cookie.Name = SD.AuthInfo;
    })
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.SaveToken = true;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JWTokenService.Key,
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero
        };
        jwtOptions.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidate>();
                return tokenValidatorService.Execute(context);
            },
            OnMessageReceived = context =>
            {
                var authInfo = context.Request.Cookies[SD.AuthInfo];
                if (authInfo != null)
                {
                    var token = JsonSerializer.Deserialize<AuthInfo>(authInfo).Token;
                    context.Token = ProtectorData.Decrypt(token);
                }
                return Task.CompletedTask;
            }
        };
    });
#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:5173", "https://itc-client-u2gu.vercel.app")
            .AllowCredentials();
    });
});



var app = builder.Build();


app.Lifetime.ApplicationStarted.Register(async () =>
{
    var scope = app.Services.CreateScope();
    var initailzier = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

    await initailzier.Execute();
});


app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpRateLimiting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

var antiforgery = app.Services.GetRequiredService<IAntiforgery>();



//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.Value.StartsWith("/"))
//    {
//        var tokens = antiforgery.GetAndStoreTokens(context);
//        context.Response.Cookies.Delete("X-CSRF");
//        context.Response.Cookies.Append("X-CSRF", tokens.RequestToken,
//        new CookieOptions()
//        {
//            HttpOnly = false,
//            Secure = context.Request.Scheme == "https",
//            IsEssential = true,
//            SameSite = SameSiteMode.Strict,
//        });
//    }

//    #region Security headers

//    context.Response.Headers.Remove("X-Powered-By");
//    context.Response.Headers.Remove("Server");
//    context.Response.Headers.Remove("X-AspNet-Version");
//    context.Response.Headers.Remove("X-AspNetMvc-Version");
//    context.Response.Headers.Remove("X-Frame-Options");
//    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';base-uri 'self';font-src 'self' https: data:;form-action 'self';frame-ancestors 'self';img-src 'self' data:;object-src 'none';script-src 'self';script-src-attr 'none';style-src 'self' https: 'unsafe-inline';upgrade-insecure-requests");
//    context.Response.Headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
//    context.Response.Headers.Add("Cross-Origin-Opener-Policy", "same-origin");
//    context.Response.Headers.Add("Cross-Origin-Resource-Policy", "same-origin");
//    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
//    context.Response.Headers.Add("Strict-Transport-Security", "max-age=15552000; includeSubDomains");
//    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
//    context.Response.Headers.Add("Origin-Agent-Cluster", "?1");
//    context.Response.Headers.Add("X-DNS-Prefetch-Control", "off");
//    context.Response.Headers.Add("X-Download-Options", "noopen");
//    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
//    context.Response.Headers.Add("X-Frame-Options", "DENY");
//    context.Response.Headers.Add("X-XSS-Protection", "0");

//    #endregion

//    await next();
//});



app.MapControllers();

app.Run();
