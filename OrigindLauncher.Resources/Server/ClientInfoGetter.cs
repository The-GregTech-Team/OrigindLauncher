using OrigindLauncher.Resources.Json;
using OrigindLauncher.Resources.Server.Data;
using RestSharp;

namespace OrigindLauncher.Resources.Server
{
    internal class ClientInfoGetter
    {
        public static ClientInfo Get()
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var result = rc.Get(RestRequestFactory.Create(Definitions.Rest.ClientJson));

            return result.Content.JsonCast<ClientInfo>();
        }
    }
}