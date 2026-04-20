# Yubikey OTP Validation

`YubikeyOtpValidator` validates a one-time password against the Yubico cloud API. Provide your [Yubico API ID and secret](https://upgrade.yubico.com/getapikey/), then call `ValidateAsync`.

## Usage

```csharp
using Servus.Core.Security.Hardware.Yubikey;

var validator = new YubikeyOtpValidator(
    apiId:     12345,
    apiSecret: "base64-encoded-secret");

string otp = "cccccccccccb..."; // the 44-char string emitted by a Yubikey keystroke

if (await validator.ValidateAsync(otp))
{
    // authenticated — proceed
}
else
{
    // OTP invalid, replayed, or upstream rejected it
}
```

## Notes

- The OTP must be the full string emitted by the Yubikey (12-char ID prefix + encrypted payload).
- Validation hits Yubico servers over HTTPS; add appropriate retry/timeout handling at the call site if your flow needs it.
- The internal details (HMAC-SHA1 signing, client-ID extraction) are not part of the public API.

## API

```csharp
public class YubikeyOtpValidator
{
    public YubikeyOtpValidator(int apiId, string apiSecret);

    public Task<bool> ValidateAsync(string otp);
}
```
