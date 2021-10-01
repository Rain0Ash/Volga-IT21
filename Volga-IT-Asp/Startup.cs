using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Volga_IT.Extractor;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Helpers;
using Volga_IT.Models;

namespace Volga_IT
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<ApplicationHtmlContext>();
            services.AddSingleton<IWordCounterRecordSorter, WordCounterRecordSorter>();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Volga_IT", Version = "v1" }); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder application, IWebHostEnvironment environment, ILoggerFactory logger)
        {
            if (environment.IsDevelopment())
            {
                application.UseExceptionHandler("/error-local-development");
                application.UseDeveloperExceptionPage();
                application.UseSwagger();
                application.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Volga_IT v1"));
            }
            else
            {
                application.UseExceptionHandler("/error");
                application.UseHsts();
            }
            
            DirectoryInfo info = Directory.CreateDirectory(Path.Join(ApplicationHelper.Directory, "Logs"));
            logger.AddFile(Path.Join(info.FullName, "logger.log"));

            application.UseHttpsRedirection();

            application.UseRouting();

            application.UseAuthorization();

            application.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}