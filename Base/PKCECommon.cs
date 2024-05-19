using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace KGQT.Base
{
    public static class PKCECommon
    {
        public static string CodeVerifier;

        public static string CodeChallenge;

        //public static void Init()
        //{
        //    CodeVerifier = GenerateCodeVerifier();
        //    CodeChallenge = GenerateCodeChallenge(CodeVerifier);
        //}

        public static string GenerateCodeVerifier()
        {
            SecureRandom random = new SecureRandom();
            byte[] codeVerifier = new byte[32];
            random.NextBytes(codeVerifier);
            return Convert.ToBase64String(codeVerifier).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                string codeChallenge = Base64UrlEncode(challengeBytes);



                return codeChallenge;
            }
        }

        public static string GenCode()
        {
            var rng = RandomNumberGenerator.Create();



            var bytes = new byte[32];
            rng.GetBytes(bytes);

            CodeVerifier = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

            using (var sha256 = SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(CodeVerifier));
                return CodeChallenge = Convert.ToBase64String(challengeBytes)
                        .TrimEnd('=')
                        .Replace('+', '-')
                        .Replace('/', '_');
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            string base64Url = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64Url;
        }
    }
}
