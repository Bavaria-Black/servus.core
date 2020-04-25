using Servus.Core.Security.Hardware.Yubikey;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Servus.Core.Tests.Security.Hardware.Yubikey
{
    [TestClass]
    public class YubikeyOtpValidatorTests
    {
        [TestMethod]
        public void ExtractClientId()
        {
            const string otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var clientId = YubikeyOtpValidator.ExtractClientId(otp);
            Assert.AreEqual("vvvvvvcu", clientId);
        }

        [TestMethod]
        public async Task ValidateAsync()
        {
            const string otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var validator = new YubikeyOtpValidator(82, "asadfdsdfs");
            var result = await validator.ValidateAsync(otp);
            Assert.IsTrue(result);
        }
    }
}