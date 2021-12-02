using DelegatedGroupCreation.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DelegatedGroupCreation.Controller
{
  class GraphController
  {
    private HttpClient httpClient;

    public GraphController(string accessToken)
    {
      this.httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);
    }
    public async Task<UnifiedGroup> CreateGroup(string groupName)
    {
      Uri uri = new Uri("https://graph.microsoft.com/v1.0/groups");
      string[] groupTypes = { "Unified" };
      var group = new { displayName = groupName,
                  description = groupName,
                  groupTypes = groupTypes,
                  mailEnabled = true,
                  mailNickname = groupName.Replace(" ", ""), // Ensure valid alias!!
                  visibility = "Private",
                  securityEnabled = false };
      string contentString = JsonConvert.SerializeObject(group);
      var data = new StringContent(contentString, Encoding.UTF8, "application/json");
      var httpResult = await this.httpClient.PostAsync(uri, data);
      string result = await httpResult.Content.ReadAsStringAsync();
      var newGroup = JsonConvert.DeserializeObject<UnifiedGroup>(result);
      return newGroup;
    }

    public async Task<string> GetSiteUrl(string groupID)
    {
      string url = String.Format("https://graph.microsoft.com/v1.0/groups/{0}/sites/root", groupID);
      Uri uri = new Uri(url);
      var httpResult = await this.httpClient.GetAsync(uri);
      if (httpResult.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        // Wait and once again please ...
      }
      string result = await httpResult.Content.ReadAsStringAsync();
      var apps = JsonConvert.DeserializeObject<Model.Site>(result);
      return apps.webUrl;
    }
  }
}
