using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OrigindLauncher.Resources.String
{
    public static class SHA128Computer
    {
        public static string Compute(string source)
        {
            var sha1 = SHA1.Create();

            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static string Compute(Stream source)
        {
            var sha1 = SHA1.Create();

            var hash = sha1.ComputeHash(source);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}