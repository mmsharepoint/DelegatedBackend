using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DelegatedGroupCreation.Controller
{
  class KeyVault
  {
    private Uri keyVaultUri;
    private ILogger log;
    public KeyVault (Uri uri, ILogger log)
    {
      keyVaultUri = uri;
      this.log = log;
    }

    public async Task<string> retrieveSecret(string name)
    {
      var keyVaultClient = new SecretClient(vaultUri: keyVaultUri, credential: new DefaultAzureCredential());

      try
      {
        KeyVaultSecret secret = keyVaultClient.GetSecret(name);
        return secret.Value;
      }
      catch (Exception ex)
      {
        log.LogError(ex.Message);
        return null;
      }
    }
  }
}
