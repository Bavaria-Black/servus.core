using Servus.Core.Security.Hardware.Yubikey;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Servus.Core.Tests.Security.Hardware.Yubikey
{
    [TestClass]
    public class YubikeyOtpValidatorTests
    {
        [TestMethod]
        public void Validate()
        {
            var otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var clientId = YubikeyOtpValidator.ExtractClientId(otp);
            Assert.AreEqual("vvvvvvcu", clientId);
        }

        [TestMethod]
        public async Task ValidateAsync()
        {
            var otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var validator = new YubikeyOtpValidator(82, "asadfdsdfs");
            var a = await validator.ValidateAsync(otp);
            Assert.IsTrue(a);
        }
    }
}
