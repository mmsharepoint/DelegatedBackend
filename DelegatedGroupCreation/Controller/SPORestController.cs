using DelegatedGroupCreation.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DelegatedGroupCreation.Controller
{
  class SPORestController
  {
    private HttpClient httpClient;
    private ILogger log;

    public SPORestController(string accessToken, ILogger log)
    {
      this.httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);
      this.log = log;
    }
    public async Task<bool> DisableTeamify(string siteUrl)
    {
      Uri uri = new Uri(String.Format("{0}/_api/groupsitemanager/HideTeamifyPrompt", siteUrl));
      
      var body = new { siteUrl = siteUrl };
      string contentString = JsonConvert.SerializeObject(body);
      var data = new StringContent(contentString, Encoding.UTF8, "application/json");
      var httpResult = await this.httpClient.PostAsync(uri, data);
      string result = await httpResult.Content.ReadAsStringAsync();
      
      return true;
    }

    public async Task<string> GetSiteUrl(string groupID)
    {
      string url = String.Format("https://graph.microsoft.com/v1.0/groups/{0}/sites/root", groupID);
      Uri uri = new Uri(url);
      var httpResult = await this.httpClient.GetAsync(uri);
      int count = 0;
      while (httpResult.StatusCode == System.Net.HttpStatusCode.NotFound || count < 5)
      {
        Thread.Sleep(30000);
        httpResult = await this.httpClient.GetAsync(uri);
        count++;
        log.LogInformation(count.ToString() + ". retry to get site");
      }
      string result = await httpResult.Content.ReadAsStringAsync();
      var apps = JsonConvert.DeserializeObject<Model.Site>(result);
      return apps.webUrl;
    }
  }
}
