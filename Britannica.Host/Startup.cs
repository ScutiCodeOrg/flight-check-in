using Britannica.Application;
using Britannica.Infrastructure;
using CrossCuttingCunconers.Common.Options;
using CrossCuttingCunconers.Logging;
using CrossCuttingCunconers.OpenApi;
using CrossCuttingCunconers.OpenIdConnect;

namespace Britannica.Host;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        ApplicationName = Configuration["ApplicationName"] ?? string.Empty;
    }

    public IConfiguration Configuration { get; }
    public string ApplicationName { get; }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        services.AddHttpContextAccessor();
        services.AddControllers()
            .AddNewtonsoftJson(o => o.UseCamelCasing(true))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        services.AddOpenApiServices(Configuration);
        services.AddOICDApi(Configuration.GetSection(nameof(OIDCApiOptions)).Get<OIDCApiOptions>());

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        IContainer container = BuildContainer(services);
        return new AutofacServiceProvider(container);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }


        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseWebHostOpenApi(ApplicationName)
            .UseWebHostRequestLogging()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
    }

    private IContainer BuildContainer(IServiceCollection services)
    {
        var builder = new ContainerBuilder();
        var assemblyList = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Application.DI.Assembly
        };

        builder.RegisterApplication(assemblyList);
        builder.RegisterInfrastructure(Configuration);

        builder.Populate(services);

        var container = builder.Build();
        return container;
    }
}
