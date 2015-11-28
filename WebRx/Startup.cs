using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WebRx.Boundary;
using WebRx.Data.Person;
using WebRx.Interactors;

namespace WebRx
{
  public class Startup
  {
    public Startup()
    {
      // Set up configuration sources.
      var builder = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .AddEnvironmentVariables();
      this.Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddMvc();

      services.AddSingleton<IReactiveBoundary, ReactiveBoundary>();
      services.AddSingleton<IPersonRepository, PersonRepository>();

      // interactors
      services.AddSingleton<IInteractor, Interactors.Person.GetAll>();
      services.AddSingleton<IInteractor, Interactors.Person.GetById>();
      services.AddSingleton<IInteractor, Interactors.Person.GetByIdReactive>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider provider)
    {
      loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseIISPlatformHandler();

      app.UseStaticFiles();

      app.UseMvc();

      // activate interactors
      foreach(var interactor in provider.GetServices<IInteractor>())
      {
        interactor.Activate();
      }
    }

    // Entry point for the application.
    public static void Main(string[] args) => WebApplication.Run<Startup>(args);
  }
}
