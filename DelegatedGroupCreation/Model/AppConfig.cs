using System;
using System.Collections.Generic;
using System.Text;

namespace DelegatedGroupCreation.Model
{
  class AppConfig
  {
    public string ClientID { get; set; }
    public string TenantID { get; set; }
    public string KeyVaultUrl { get; set; }
    public string KeyVaultSecretName { get; set; }
  }
}
