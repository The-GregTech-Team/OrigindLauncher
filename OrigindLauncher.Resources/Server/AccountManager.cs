using System;
using System.Net;
using RestSharp;

namespace OrigindLauncher.Resources.Server
{
    public enum LoginStatus
    {
        Successful,
        NotFound,
        UnknownError
    }

    public enum RegisterStatus
    {
        Successful,
        UserExists,
        UnknownError
    }

    public static class AccountManager
    {
        public static RegisterStatus Register(Account account) // 对没错 说的就是你 看代码的那位 请不要调用我们的私有接口 蟹蟹 请加群609600081
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.Register).AddBody(account);
            var result = rc.Post(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    return RegisterStatus.Successful;

                case HttpStatusCode.BadRequest:
                    return RegisterStatus.UserExists;

                default:
                    return RegisterStatus.UnknownError;
            }
        }

        public static LoginStatus Login(string userName, string hashedPassword)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.Login);
            req.AddQueryParameter("username", userName);
            req.AddQueryParameter("password", hashedPassword);
            var result = rc.Get(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return LoginStatus.Successful;

                case HttpStatusCode.NotFound:
                    return LoginStatus.NotFound;

                default:
                    return LoginStatus.UnknownError;
            }
        }

        [Obsolete]
        public static LoginStatus UpdateLoginStatus(string userName, string hashedPassword)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.LoginStatus);
            req.AddQueryParameter("username", userName);
            req.AddQueryParameter("password", hashedPassword);
            var result = rc.Put(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return LoginStatus.Successful;

                case HttpStatusCode.NotFound:
                    return LoginStatus.NotFound;

                default:
                    return LoginStatus.UnknownError;
            }
        }

        public static void UpdateLoginStatusAsync(string userName, string hashedPassword, Action<LoginStatus> callback)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.Login);
            req.AddQueryParameter("username", userName);
            req.AddQueryParameter("password", hashedPassword);
            rc.PutAsync(req, (response, handle) =>
            {
                var result = response;

                switch (result.StatusCode)
                {
                    case HttpStatusCode.OK:
                        callback(LoginStatus.Successful);
                        break;

                    case HttpStatusCode.NotFound:
                        callback(LoginStatus.NotFound);
                        break;

                    default:
                        callback(LoginStatus.UnknownError);
                        break;
                }
            });
        }

        public static LoginStatus UserExists(string userName)
        {
            var rc = new RestClient(Definitions.OrigindServerUrl);
            var req = RestRequestFactory.Create(Definitions.Rest.UserExists);
            req.AddQueryParameter("username", userName);
            var result = rc.Get(req);

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    return LoginStatus.Successful;

                case HttpStatusCode.NotFound:
                    return LoginStatus.NotFound;

                default:
                    return LoginStatus.UnknownError;
            }
        }
    }
}