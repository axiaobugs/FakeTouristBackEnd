using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XiechengAPI.Database;
using XiechengAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace XiechengAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setupAction => { setupAction.ReturnHttpNotAcceptable = true; })
                .AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction =>
                    setupAction.InvalidModelStateResponseFactory =
                        context =>
                        {
                            var problemDetail = new ValidationProblemDetails(context.ModelState)
                            {
                                Type = "Error in Validate",
                                Title = "Fail to validation",
                                Status = StatusCodes.Status422UnprocessableEntity,
                                Detail = "More Info",
                                Instance = context.HttpContext.Request.Path
                            };
                            problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                            return new UnprocessableEntityObjectResult(problemDetail)
                            {
                                ContentTypes = {"application/problem+json"}
                            };
                        });
            services.AddTransient<ITouristRepository, TouristRouteRepository>();
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
