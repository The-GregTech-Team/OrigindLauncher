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

        public static RegisterStatus Register(this Account account)
        {
            return AccountManager.Register(account);
        }
    }
}