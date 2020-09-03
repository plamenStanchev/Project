namespace Scheduler.Web.Infrastructure.Oidc
{
    using System.Collections.Generic;

    public class OidcProviders
    {
         public List<OidcProvider> OidcProvidersValue { get; set; }
            = new List<OidcProvider>();
    }
}
