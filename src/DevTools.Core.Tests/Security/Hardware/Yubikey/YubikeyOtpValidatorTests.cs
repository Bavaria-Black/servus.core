using DevTools.Core.Security.Hardware.Yubikey;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        public async Task ValidateAsync()
        {
            var otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
            var validator = new YubikeyOtpValidator(82, "asadfdsdfs");
            var a = await validator.ValidateAsync(otp);
            Assert.IsTrue(a);
        }
    }
}
