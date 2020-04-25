using Servus.Core.Encoding;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Servus.Core.Tests")]
namespace Servus.Core.Security.Hardware.Yubikey
{
    public class YubikeyOtpValidator
    {
        private readonly int _apiId;
        private readonly string _apiSecret;

        readonly string[] _validationUrls = {
            "api.yubico.com",
            "api2.yubico.com",
            "api3.yubico.com",
            "api4.yubico.com",
            "api5.yubico.com",
        };

        public YubikeyOtpValidator(int apiId, string apiSecret)
        {
            _apiId = apiId;
            _apiSecret = apiSecret;
        }

        public Task<bool> ValidateAsync(string otp)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var rnd = RandomNumberGenerator.Create();
            var nonce = new byte[16];
            rnd.GetBytes(nonce);
            var nonceString = ModHexEncoding.ModHex.GetString(nonce);

            Parallel.ForEach(_validationUrls, async (url) =>
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri($"https://{url}/wsapi/2.0/verify");
                    var result = await client.GetAsync($"?id={_apiId}&nonce={nonceString}&otp={otp}");
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var content = await result.Content.ReadAsStringAsync();
                        Console.WriteLine(content);

                        tcs.TrySetResult(true);
                    }
                }
            });

            return tcs.Task;
        }

        internal string CalculateHash(string input)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(_apiSecret);
            using (var myhmacsha1 = new HMACSHA1(key))
            {
                byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(input);
                return Convert.ToBase64String(myhmacsha1.ComputeHash(byteArray));
            }
        }

        internal static string ExtractClientId(string otp)
        {
            ValidateLength(otp);
            return otp.Substring(0, otp.Length - 32);
        }

        internal static string RemoveClientId(string otp)
        {
            ValidateLength(otp);
            return otp.Substring(otp.Length - 32);
        }

        private static void ValidateLength(string otp)
        {
            if (otp == null || (otp.Length < 34 && otp.Length > 48))
            {
                throw new ArgumentException("otp has to be between 34 and 48 characters long");
            }
        }
    }
}
