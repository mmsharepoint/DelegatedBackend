using DelegatedGroupCreation.Model;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(DelegatedGroupCreation.Startup))]
namespace DelegatedGroupCreation
{
  class Startup: FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      var config = builder.GetContext().Configuration;
      var appConfig = new AppConfig();
      config.Bind(appConfig);

      builder.Services.AddSingleton(appConfig);

      builder.Services.AddPnPCore(options =>
      {
        // Disable telemetry because of mixed versions on AppInsights dependencies
        options.DisableTelemetry = true;

        // Configure an authentication provider with certificate (Required for app only)

        //// And set it as default
        //options.DefaultAuthenticationProvider = authProvider;

        // Add a default configuration with the site configured in app settings
        //options.Sites.Add("Default",
        //       new PnP.Core.Services.Builder.Configuration.PnPCoreSiteOptions
        //       {
        //         SiteUrl = azureFunctionSettings.SiteUrl,
        //         AuthenticationProvider = authProvider
        //       });
      });
    }
  }
}
