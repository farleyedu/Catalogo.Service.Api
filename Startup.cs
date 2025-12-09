using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Catalogo.Service.Api;

/// <summary>
/// Start class.
/// </summary>
public class Startup
{
    /// <summary>
    /// Start.
    /// </summary>
    /// <param name="configuration">Config file info.</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        // Configurando o as constantes.
        ConfigureConstants();
    }

    /// <summary>
    /// This method configures the constants(<see cref="Constants"/>) of the application.
    /// </summary>
    private void ConfigureConstants()
    {
        Constants.ConnectionFilePath = Configuration.GetSection("Connections")["MRT001"];
        Constants.ClickExpirationTime = Convert.ToInt32(Configuration.GetSection("CacheOptions")["ClickExpirationTime"]);
        Constants.AbsoluteExpirationTime = Convert.ToInt32(Configuration.GetSection("CacheOptions")["AbsoluteExpirationTime"]);
        Constants.IsApplicationCacheEnabled = Convert.ToBoolean(Configuration.GetSection("CacheOptions")["IsEnabled"]);
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
            .AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
        services.AddControllers(options =>
        {
            options.Filters.Add<HttpResponseExceptionFilter>();
        }).AddNewtonsoftJson();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Catalogo Service API",
                Description = "Serviço de unificação e integração de dados do Catalogo de Produtos Martins.",
                Version = "v1"
            });

            // Comentários.
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        // Configuração do Cors.
        app.UseCors(builder => builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials()); // Permitindo tudo.
        // app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseResponseCompression();
        app.UseSwagger();
        // Configuração do Swagger.
        app.UseSwaggerUI(options =>
        {
#if DEBUG
            // Para desenvolvimento.
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalogo Service API(dev) - v1");
#else
                // Para produção.
                options.SwaggerEndpoint("../swagger/v1/swagger.json", "Catalogo Service API(prd/hml) - v1");
#endif
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
