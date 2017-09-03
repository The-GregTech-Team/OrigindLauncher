using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows;
using OrigindLauncher.Resources.Configs;
using OrigindLauncher.UI;
using RestSharp;


namespace OrigindLauncher.Resources.Server
{
    public static class LoginManager
    {
        private static readonly Timer Timer = new Timer(3500);
        private static readonly RestClient CachedRestClient;
        private static readonly RestRequest Rbq;

        static LoginManager()
        {
            Timer.Stop();
            Timer.Elapsed += TimerOnElapsed;
            CachedRestClient = new RestClient(Definitions.OrigindServerUrl);
            Rbq = RestRequestFactory.Create(Definitions.Rest.PullLoginVerify);
            Rbq.AddQueryParameter("username", Config.Instance.PlayerAccount.Username);
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                PullMessage();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private static void PullMessage()
        {
            var response = CachedRestClient.Get(Rbq);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return;
                case HttpStatusCode.OK:
                    Application.Current.Dispatcher.Invoke(() => new LoginVerifyWindow().Show());
                    return;
                case HttpStatusCode.Accepted:
                    try
                    {
                        UploadScreenData();

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e);
                        //throw;
                    }
                    return;
            }
        }

        private static void UploadScreenData()
        {
            if (Config.Instance.AllowScreenshotShare)
            {
                var screenCapture = LaunchProgressWindow.ScreenCapture();
                MessageUploadManager.UploadImage(screenCapture);
            }
            else
            {
                MessageUploadManager.Upload($"=来自客户端: {Config.Instance.PlayerAccount.Username}: 未开启屏幕截图分享.");
            }
        }

        public static bool LoginVerify(bool isSuccessful)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.LoginVerify);
            req.AddQueryParameter("successStr", isSuccessful ? "true" : "false");
            req.AddQueryParameter("username", Config.Instance.PlayerAccount.Username);
            req.AddQueryParameter("password", Config.Instance.PlayerAccount.Password);
            var response = rc.Get(req);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.NotFound:
                    return false;
            }
            return false;
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        public static void Start()
        {
            Timer.Start();
        }
    }
}
