using System.Net;
using OrigindLauncher.Resources.Configs;
using RestSharp;

namespace OrigindLauncher.Resources.Server
{
    public static class MessageUploadManager
    {
        public static bool CrashReport(UploadData data)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.CrashReport).AddBody($"{Config.Instance.PlayerAccount.Username}: {data}");
            var result = rc.Post(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Suggests(UploadData data)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.Suggests).AddBody(data);
            var result = rc.Post(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return true;
                default:
                    return false;
            }
        }
    }
}