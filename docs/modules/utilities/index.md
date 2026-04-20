# Utilities

A small catch-all of types that don't fit the larger sections but solve one specific, recurring problem.

## Pages in this section

- [**Port Finder**](./port-finder) — find a free TCP port for test servers, child processes, dev tooling.
- [**Yubikey OTP**](./yubikey) — validate Yubikey OTPs against the Yubico API.
- [**Notify Property Changed**](./notify-property-changed) — ergonomic base class for `INotifyPropertyChanged`.
- [**IWithId**](./with-id) — marker interface for entities with a GUID identifier.

## Namespace map

| Namespace | Types |
|---|---|
| `Servus.Core.Network` | `PortFinder` |
| `Servus.Core.Security.Hardware.Yubikey` | `YubikeyOtpValidator` |
| `Servus.Core` | `NotifyPropertyChangedBase`, `IWithId` |
