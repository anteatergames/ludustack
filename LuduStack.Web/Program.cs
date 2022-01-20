using LuduStack.Application.Requests.Notification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Infra.CrossCutting.Identity;
using LuduStack.Infra.CrossCutting.Identity.Model;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Infra.CrossCutting.IoC;
using LuduStack.Infra.Data.MongoDb;
using LuduStack.Web;
using LuduStack.Web.Extensions;
using LuduStack.Web.Middlewares;
using LuduStack.Web.ModelBinders;
using LuduStack.Web.RewriterRules;
using LuduStack.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebEssentials.AspNetCore.Pwa;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string[] supportedCultures = new[]
    {
        "en-US",
        "en",
        "pt-BR",
        "pt",
        "ru-RU",
        "ru",
        "bs",
        "sr",
        "hr",
        "de"
    };

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.ConsentCookie.Name = "LuduStack.Consent";
});

MongoDbPersistence.Configure();

builder.Services.AddIdentityMongoDbProvider<ApplicationUser, Role>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}, options =>
{
    options.ConnectionString = builder.Configuration["MongoSettings:Connection"];
    options.DatabaseName = builder.Configuration["MongoSettings:DatabaseName"];
})
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = ".LuduStack.Identity.Application";
        o.LoginPath = new PathString("/login");
        o.AccessDeniedPath = new PathString("/home/access-denied");
    })
    .AddFacebook(o =>
    {
        o.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        o.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        o.Fields.Add("picture");
        o.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                string profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").GetString();
                identity.AddClaim(new Claim("urn:facebook:picture", profileImg));
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                string profileImg = context.User.GetProperty("picture").GetString();
                identity.AddClaim(new Claim("urn:google:picture", profileImg));
                return Task.FromResult(0);
            }
        };
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
        microsoftOptions.SaveTokens = true;
        microsoftOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                ClaimsIdentity identity = (ClaimsIdentity)context.Principal.Identity;
                identity.AddClaim(new Claim("urn:microsoft:accesstoken", context.TokenResponse.AccessToken));

                return Task.FromResult(0);
            }
        };
    })
    .AddGithub(githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:Github:ClientId"];
        githubOptions.ClientSecret = builder.Configuration["Authentication:Github:ClientSecret"];
    });

builder.Services.AddAutoMapperSetup();

builder.Services.AddSession(opt =>
{
    opt.Cookie.Name = ".LuduStack.Session";
    opt.Cookie.IsEssential = true;
});

builder.Services.AddResponseCompression();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

builder.Services.AddLocalization();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
    options.CacheProfiles.Add("Default",
        new CacheProfile
        {
            Duration = 2592000,
            VaryByHeader = HeaderNames.ETag
        });
    options.CacheProfiles.Add("Short",
        new CacheProfile
        {
            Duration = 86400,
            VaryByHeader = HeaderNames.ETag
        });
    options.CacheProfiles.Add("Never",
        new CacheProfile
        {
            Location = ResponseCacheLocation.None,
            NoStore = true
        });
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
})
.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(SharedResources));
});

builder.Services.AddMemoryCache();

builder.Services.AddProgressiveWebApp(new PwaOptions { EnableCspNonce = false });

builder.Services.AddTransient<ICookieMgrService, CookieMgrService>();

builder.Services.Configure<ConfigOptions>(myOptions =>
{
    myOptions.FacebookAppId = builder.Configuration["Authentication:Facebook:AppId"];
    myOptions.ReCaptchaSiteKey = builder.Configuration["ReCaptcha:SiteKey"];
});

builder.Services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(SendNotificationRequestHandler).GetTypeInfo().Assembly, typeof(DeleteCourseCommandHandler).GetTypeInfo().Assembly);

RegisterServices(builder.Services);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else if (app.Environment.IsProduction()) // this code redirects from old domain.
{
    app.Use(async (context, next) =>
    {
        if (!context.Request.Host.Host.Contains("ludustack.com"))
        {
            string withDomain = "https://www.ludustack.com" + context.Request.Path;
            context.Response.Redirect(withDomain);
        }
        else if (!context.Request.IsHttps)
        {
            string withHttps = "https://" + context.Request.Host + context.Request.Path;
            context.Response.Redirect(withHttps);
        }
        else
        {
            await next();
        }
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    await next();
});

app.UseHttpsRedirection();
app.UseHsts();

FileExtensionContentTypeProvider provider = new();
provider.Mappings[".webmanifest"] = "application/manifest+json";
provider.Mappings[".vtt"] = "text/vtt";

RewriteOptions rewriteOptions = new RewriteOptions()
    .AddRedirectToHttps()
    .Add(new NonWwwRule())
    .AddRedirectToWwwPermanent();

app.UseRewriter(rewriteOptions);

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=2592000";
    }
});

app.UseRouting();

app.UseRequestLocalization(options =>
    options
        .SetDefaultCulture("en-US")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        );

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseSitemapMiddleware();

app.UseEndpoints(routes =>
{
    routes.MapControllerRoute(
        name: "azurestorage",
        pattern: "{controller=storage}/{action=image}/{id}"
    );

    routes.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    routes.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();

CreateUserRoles(app.Services).Wait();

static void RegisterServices(IServiceCollection services)
{
    // Adding dependencies from another layers (isolated from Presentation)
    NativeInjectorBootStrapper.RegisterServices(services);
}

static async Task CreateUserRoles(IServiceProvider serviceProvider)
{
    RoleManager<Role> roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

    List<Roles> roles = Enum.GetValues(typeof(Roles)).Cast<Roles>().ToList();

    foreach (Roles role in roles)
    {
        await CreateIfNotExists(roleManager, role.ToString());
    }
}

static async Task CreateIfNotExists(RoleManager<Role> RoleManager, string roleName)
{
    bool roleCheck = await RoleManager.RoleExistsAsync(roleName);
    if (!roleCheck)
    {
        await RoleManager.CreateAsync(new Role(roleName));
    }
}