using System;
using System.Collections.Generic;
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
        private static readonly Timer Timer = new Timer(2000);

        static LoginManager()
        {
            Timer.Stop();
            Timer.Elapsed += TimerOnElapsed;
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                PullMessage();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void PullMessage()
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.PullLoginVerify);
            req.AddQueryParameter("username", Config.Instance.PlayerAccount.Username);
            var response = rc.Get(req);
            switch (response.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return;
                case HttpStatusCode.OK:
                    Application.Current.Dispatcher.Invoke(() => new LoginVerifyWindow().Show());
                    return;
            }
        }

        public static void LoginVerify(bool isSuccessful)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.LoginVerify);
            req.AddQueryParameter("successStr", isSuccessful ? "true" : "false");
            req.AddQueryParameter("username", Config.Instance.PlayerAccount.Username);
            req.AddQueryParameter("password", Config.Instance.PlayerAccount.Password);
            rc.Get(req);
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
