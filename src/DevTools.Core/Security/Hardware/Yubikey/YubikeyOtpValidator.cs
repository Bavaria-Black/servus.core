using DevTools.Core.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DevTools.Core.Tests")]
namespace DevTools.Core.Security.Hardware.Yubikey
{
    public class YubikeyOtpValidator
    {
        private readonly int _apiId;
        private readonly string _apiSecret;

        string[] _validationUrls = new[]
        {
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

        public async Task<bool> ValidateAsync(string otp)
        {
            var tcs = new TaskCompletionSource<bool>();
            var rnd = new Random();
            byte[] nonce = new byte[16];
            rnd.NextBytes(nonce);
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

                        tcs.SetResult(true);
                    }
                }
            });

            return await tcs.Task;
        }

        internal string CalculateHash(string input)
        {
            var key = System.Text.Encoding.ASCII.GetBytes(_apiSecret);
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(input);
            return Convert.ToBase64String(myhmacsha1.ComputeHash(byteArray));
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
            if (otp.Length < 34 && otp.Length > 48)
            {
                throw new ArgumentException("otp has to be between 34 and 48 characters long");
            }
        }
    }
}
