using Grpc.Core;
using GRpc.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once IdentifierTypo
namespace GRpc.Client
{
    public class Startup
    {
        private const string HostString = "localhost:50061";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton(
                new CandidateService.CandidateServiceClient(
                    new Channel(HostString, ChannelCredentials.Insecure)));
            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.Run(async (context) => { await context.Response.WriteAsync("Hello World!"); });
        }
    }
}