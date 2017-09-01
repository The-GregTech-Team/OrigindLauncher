namespace OrigindLauncher.Resources.Server
{
    public static class AccountExtension
    {
        public static LoginStatus Login(this Account account)
        {
            return AccountManager.Login(account.Username, account.Password);
        }

        public static LoginStatus UpdateLoginStatus(this Account account)
        {
            return AccountManager.UpdateLoginStatus(account.Username, account.Password);
        }

        public static RegisterStatus Register(this Account account)// 对没错 说的就是你 看代码的那位 请不要调用我们的私有接口 蟹蟹 请加群609600081
        {
            return AccountManager.Register(account);
        }
    }
}