using Servus.Core.Security.Hardware.Yubikey;
using System.Threading.Tasks;
using Xunit;

namespace Servus.Core.Tests.Security.Hardware.Yubikey;

public class YubikeyOtpValidatorTests
{
    [Fact]
    public void ExtractClientId()
    {
        const string otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
        var clientId = YubikeyOtpValidator.ExtractClientId(otp);
        Assert.Equal("vvvvvvcu", clientId);
    }

    [Fact]
    public async Task ValidateAsync()
    {
        const string otp = "vvvvvvcurikvhjcvnlnbecbkubjvuittbifhndhn";
        var validator = new YubikeyOtpValidator(82, "asadfdsdfs");
        var result = await validator.ValidateAsync(otp);
        Assert.True(result);
    }
}