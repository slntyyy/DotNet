using System;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace ApiClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string DefAllowSpecificOrigins = "_LQAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 注册数据库上下文
            services.AddControllers()
                .AddXmlSerializerFormatters()
                .AddNewtonsoftJson()
                .AddJsonOptions(options =>
                {
                    // Use the default property (Pascal) casing.
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

                    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("v1", new OpenApiInfo
                   {
                       Title = "LQ Client API",
                       Version = "v1",

                       //    Description = "A simple example ASP.NET Core Web API",
                       //    TermsOfService = new Uri("https://example.com/terms"),
                       //    Contact = new OpenApiContact
                       //    {
                       //        Name = "Shayne Boyer",
                       //        Email = string.Empty,
                       //        Url = new Uri("https://twitter.com/spboyer"),
                       //    },
                       //    License = new OpenApiLicense
                       //    {
                       //        Name = "Use under LICX",
                       //        Url = new Uri("https://example.com/license"),
                       //    }
                   });
                   c.OrderActionsBy(o => o.RelativePath);
                   // Set the comments path for the Swagger JSON and UI.
                   var xmlFile = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                   var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                   c.IncludeXmlComments(xmlPath);
               });

            services.AddCors(options =>
            {
                options.AddPolicy(DefAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://*.liqu.com")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LQ Client API V1");
                c.RoutePrefix = string.Empty;

                c.InjectStylesheet("/swagger/ui/custom.css");
                c.InjectJavascript("/swagger/ui/custom.js");

            });


            app.UseDefaultFiles();
            app.UseStaticFiles();

            // 强制使用Https
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(DefAllowSpecificOrigins);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
