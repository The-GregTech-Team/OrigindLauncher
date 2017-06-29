using System.Security.Cryptography;
using System.Text;

namespace OrigindLauncher.Resources.String
{
    public static class SHA512Helper
    {
        /// <summary>
        ///     Compute a string into Sha512
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Compute(string source)
        {
            var s512 = SHA512.Create();
            var hash = s512.ComputeHash(Encoding.UTF8.GetBytes(source));
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}