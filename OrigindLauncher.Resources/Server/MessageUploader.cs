using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            var req = RestRequestFactory.Create(Definitions.Rest.CrashReport)
                .AddBody($"{Config.Instance.PlayerAccount.Username}: {data}");
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

        public static void Upload(string data)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.UploadMessage).AddBody(data);
            rc.Post(req);
        }

        public static void UploadImage(Image data)
        {
            var strdata = ImageToBase64(data);

            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.UploadImage).AddBody(strdata);
            rc.Post(req);
        }

        private static string ImageToBase64(Image data)
        {
            using (var ms = new MemoryStream())
            {
                
                data.Save(ms, ImageFormat.Jpeg);
                var arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                return Convert.ToBase64String(arr);
            }
        }
    }
}