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
    public void ExtractClientId_ThrowsForTooLongOtp()
    {
        const string otp = "cccccccccclibubjhkuttefctkgejjgerdjfihbkhtddivju1234567890";

        Assert.Throws<ArgumentException>(() => YubikeyOtpValidator.ExtractClientId(otp));
    }

    [Fact]
    public void RemoveClientId_ThrowsForTooLongOtp()
    {
        const string otp = "cccccccccclibubjhkuttefctkgejjgerdjfihbkhtddivju1234567890";

        Assert.Throws<ArgumentException>(() => YubikeyOtpValidator.RemoveClientId(otp));
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
