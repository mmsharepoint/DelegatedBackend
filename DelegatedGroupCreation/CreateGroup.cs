using System;
using System.IO;
using System.Threading.Tasks;
using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Specialized;
using PnP.Core.Auth;
using System.Net.Http;
using DelegatedGroupCreation.Model;
using PnP.Core.Services;
using DelegatedGroupCreation.Controller;

namespace DelegatedGroupCreation
{
  class CreateGroup
  {
    private AppConfig appConfig;
    private readonly IPnPContextFactory pnpContextFactory;

    public CreateGroup(AppConfig appConfig, IPnPContextFactory pnpContextFactory)
    {
      this.appConfig = appConfig;
      this.pnpContextFactory = pnpContextFactory;
    }

    [FunctionName("CreateGroup")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestMessage req,
        ILogger log)
    {
      NameValueCollection query = req.RequestUri.ParseQueryString();
      string groupName = query["groupName"];
      log.LogInformation(groupName);
      //string reqUser = query["user"];
      //log.LogInformation(qryUrl);
      KeyVault keyVault = new KeyVault(new Uri(appConfig.KeyVaultUrl), log);
      string secret = await keyVault.retrieveSecret(appConfig.KeyVaultSecretName);
      SecureString clientSecret = new SecureString();
      foreach (char c in secret) clientSecret.AppendChar(c);
      string userToken = req.Headers.Authorization.Parameter;

      OnBehalfOfAuthenticationProvider onBehalfAuthProvider = new OnBehalfOfAuthenticationProvider(this.appConfig.ClientID,
                                                                                                    this.appConfig.TenantID,
                                                                                                    clientSecret, () => userToken);

      string accessToken = await onBehalfAuthProvider.GetAccessTokenAsync(new Uri("https://graph.microsoft.com"));
      GraphController graphController = new GraphController(accessToken, log);

      UnifiedGroup response = await graphController.CreateGroup(groupName);
      string siteUrl = await graphController.GetSiteUrl(response.id);
      using (var pnpContext = await pnpContextFactory.CreateAsync(new System.Uri(siteUrl), onBehalfAuthProvider))
      {
        var webId = pnpContext.Web.Id;
        var siteId = pnpContext.Site.Id;
        response.siteID = siteId.ToString();
        response.webID = webId.ToString();
      }

      response.siteUrl = siteUrl;
      return new OkObjectResult(response);
    }
    }
}
