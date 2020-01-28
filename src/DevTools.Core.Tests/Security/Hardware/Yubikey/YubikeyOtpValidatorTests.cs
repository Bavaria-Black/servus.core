using DevTools.Core.Security.Hardware.Yubikey;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevTools.Core.Tests.Security.Hardware.Yubikey
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
        public void ValidateAsync()
        {
            var otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var validator = new YubikeyOtpValidator(82, "asadfdsdfs");
            var a = validator.Validate(otp);
            Assert.IsTrue(a);
        }
    }
}
