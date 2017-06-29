using OrigindLauncher.Resources.Json;
using RestSharp;

namespace OrigindLauncher.Resources.Server
{
    public static class RestRequestFactory
    {
        public static RestRequest Create(string resource)
        {
            var req = new RestRequest(resource)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new JsonSerializer()
            };
            return req;
        }
    }
}