using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Trace;

using StackExchange.Redis;

namespace otel_test_1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ConnectionMultiplexer>((_) => ConnectionMultiplexer.Connect(Configuration.GetValue<string>("REDIS_URL")));

            services.AddOpenTelemetryTracing((svc, builder) =>
            {
                var connection = svc.GetRequiredService<ConnectionMultiplexer>();
                builder
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddRedisInstrumentation(connection, options => options.FlushInterval = TimeSpan.FromSeconds(1))
                    //.AddOtlpExporter(config =>
                    //{
                    //    config.Endpoint = $"{this.Configuration.GetValue<string>("OPENTELEMETRYAGENT")}:55680";
                    //    //config.
                    //})
                    .AddZipkinExporter(config =>
                    {
                        var zipkinHostName = Configuration.GetValue<Uri>("OpenTelemetryAgent");
                        config.ServiceName = nameof(otel_test_1);
                        config.Endpoint = new Uri($"http://{zipkinHostName}:9411/api/v2/spans");
                    })
                    .AddConsoleExporter()
                    ;
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
