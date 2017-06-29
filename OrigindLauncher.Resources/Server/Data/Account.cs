namespace OrigindLauncher.Resources.Server
{
    public class Account
    {
        public string Password;
        public string Type;

        public string Username;

        public Account(string username, string password, string type)
        {
            Username = username;
            Password = password;
            Type = type;
        }
    }
}