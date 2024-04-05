using AzureBlob.API.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileManagerAPI.Model;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel;
using FileManagerAPI.Services;

namespace FileManagerAPI
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
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);
            services.AddControllers();
            services.AddScoped<IAzureBlobService, AzureBlobService>();
            services.AddScoped<IAzureSecretQuestions, AzureSecretQuestions>();
            services.AddScoped<IAzureTableService, AzureTableService>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "https://red-meadow-0d468f610.4.azurestaticapps.net/")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
            services.Configure<AzureStorage>(Configuration.GetSection("AzureStorage"));

            services.Configure<AzureKeyVault>(Configuration.GetSection("AzureKeyVault"));

            services.Configure<SQLConnections>(Configuration.GetSection("SQLConnections"));
            services.Configure<AZTableStorage>(Configuration.GetSection("AZTableStorage"));


            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Azure Container File manager",
                        Description = "File manager APIs to upload and download files from Azure blob storage",
                        TermsOfService = new Uri("https://example.com/terms")
                        
                        //Contact = new OpenApiContact
                        //{
                        //    Name = "Example Contact",
                        //    Url = new Uri("https://example.com/contact")
                        //},
                        //License = new OpenApiLicense
                        //{
                        //    Name = "Example License",
                        //    Url = new Uri("https://example.com/license")
                        //}
                    });
                });

            
            //services.AddScoped<IAzureBlobService, AzureBlobService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                    //options.DocumentTitle = "Azure Container File manager";
                    //options.
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigins");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
