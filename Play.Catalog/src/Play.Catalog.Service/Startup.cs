using Microsoft.OpenApi.Models;
using Play.Catalog.Service.Entities;
using Play.Common.Settings;
using Play.Common.MongoDB;
using MassTransit;
using MassTransit.Definition;
using Play.Common.MassTransit;

namespace Play.Catalog.Service
{
    public class Startup
    {
        private ServiceSettings serviceSettings = null!;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private const string AllowedOriginSetting = "AllowedOrigin";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)

        {
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo()
                .AddMongoRepository<Item>("items")
                .AddMassTransitWithRabbitMq();

            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
