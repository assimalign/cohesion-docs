# Example: JSON Web Token Behavioral Tests

Exercise JSON Web Token signature verification, compact ES256 writing, and validation using the original repository tests.

[Examples](index.md) · [Assembly overview](../index.md)

## Test-project context

These are complete test-file listings from the package test project. Keep them in that project
when running them: the project supplies its package references, generated sources, and any shared
fixtures. The using blocks make the test-framework import explicit where the original project
supplies it globally.

## Code

### JsonWebTokenSignatureVerifierTests.cs

```csharp
using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.Tests;

/// <summary>
/// Verifies the reusable RSA and ECDSA JSON Web Token signature-verification primitives.
/// </summary>
public sealed class JsonWebTokenSignatureVerifierTests
{
    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Verify: RS256 signature round-trips with a public key")]
    public void Verify_WhenRs256SignatureIsValid_ShouldReturnTrue()
    {
        // Arrange
        using RSA privateKey = RSA.Create(2048);
        using RSA publicKey = RSA.Create();
        publicKey.ImportParameters(privateKey.ExportParameters(includePrivateParameters: false));
        string header = Encode("{\"alg\":\"RS256\",\"kid\":\"rsa-key-1\"}");
        string payload = Encode("{\"iss\":\"issuer\",\"aud\":\"resource\"}");
        string signingInputText = string.Concat(header, ".", payload);
        byte[] signingInput = Encoding.ASCII.GetBytes(signingInputText);
        byte[] signature = privateKey.SignData(signingInput, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        JsonWebToken token = JsonWebToken.Parse(
            string.Concat(signingInputText, ".", Base64Url.EncodeToString(signature)));
        RsaJsonWebTokenSignatureVerifier verifier = RsaJsonWebTokenSignatureVerifier.Create(publicKey, "rsa-key-1");

        // Act
        bool verified = verifier.CanVerify(token.Algorithm!, token.Header.KeyId) &&
            verifier.Verify(token.Algorithm!, signingInput, signature);

        // Assert
        verified.ShouldBeTrue();
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - CanVerify: ECDSA algorithm must match the named curve")]
    public void CanVerify_WhenEcdsaAlgorithmDoesNotMatchNamedCurve_ShouldReturnFalse()
    {
        // Arrange
        using ECDsa p256 = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        EcdsaJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(p256);

        // Act
        bool canVerify = verifier.CanVerify(JoseAlgorithms.ES384, keyId: null);

        // Assert
        canVerify.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - CreateEcdsa: A non-JOSE curve is rejected")]
    public void CreateEcdsa_WhenCurveIsSecp256K1_ShouldThrow()
    {
        // Arrange
        ECDsa publicKey;
        try
        {
            publicKey = ECDsa.Create(ECCurve.CreateFromFriendlyName("secp256k1"));
        }
        catch (PlatformNotSupportedException)
        {
            // macOS (Apple CryptoKit) cannot generate secp256k1 keys, so the rejection path cannot be
            // reached there; Windows and Linux legs exercise it. xUnit v2 has no dynamic skip.
            return;
        }
        using ECDsa _ = publicKey;

        // Act
        Action create = () => EcdsaJsonWebTokenSignatureVerifier.Create(publicKey);

        // Assert
        Should.Throw<ArgumentException>(create);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Verify: A malformed ES256 signature length is rejected")]
    public void Verify_WhenEs256SignatureLengthIsMalformed_ShouldReturnFalse()
    {
        // Arrange
        using ECDsa p256 = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        EcdsaJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(p256);
        byte[] signingInput = Encoding.ASCII.GetBytes("eyJhbGciOiJFUzI1NiJ9.e30");
        var malformedSignature = new byte[63];

        // Act
        bool verified = verifier.Verify(JoseAlgorithms.ES256, signingInput, malformedSignature);

        // Assert
        verified.ShouldBeFalse();
    }

    private static string Encode(string json) => Base64Url.EncodeToString(Encoding.UTF8.GetBytes(json));
}
```

### JsonWebTokenWriterTests.cs

```csharp
using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

using Assimalign.Cohesion.IdentityModel;
using Assimalign.Cohesion.IdentityModel.Token;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.Tests;

/// <summary>
/// Verifies ES256 compact-token writing and its integration with parsing, signature
/// verification, and the existing token validation path.
/// </summary>
public sealed class JsonWebTokenWriterTests
{
    private const string issuer = "cohesion-gateway";
    private const string audience = "secret-store";
    private const string keyId = "gateway-key-1";
    private static readonly DateTimeOffset _now = new(2026, 9, 6, 12, 0, 0, TimeSpan.Zero);

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Write: ES256 token round-trips through parse and verification")]
    public void Write_WhenEs256DescriptorProvided_ShouldRoundTripAndVerify()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using ECDsa publicKey = CreatePublicKey(privateKey);
        JsonWebTokenWriter writer = JsonWebTokenWriter.CreateEs256(privateKey, keyId);
        JsonWebTokenDescriptor descriptor = CreateDescriptor(_now.AddHours(1));
        descriptor.Claims.Add(new IdentityClaim("scope", "bootstrap"));

        // Act
        string compact = writer.Write(descriptor);
        JsonWebToken token = JsonWebToken.Parse(compact);
        IJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(publicKey, keyId);

        // Assert
        token.Algorithm.ShouldBe(JoseAlgorithms.ES256);
        token.Header.KeyId.ShouldBe(keyId);
        token.Header.Type.ShouldBe("JWT");
        token.Issuer.ShouldBe(issuer);
        token.Audiences.ShouldBe(new[] { audience });
        token.ExpiresAt.ShouldBe(_now.AddHours(1));
        token.Id.ShouldBe("bootstrap-1");
        token.Claims.TryGet("scope", out IIdentityClaim? scope).ShouldBeTrue();
        scope!.Value.AsString().ShouldBe("bootstrap");
        Base64Url.DecodeFromChars(token.Parts!.Signature).Length.ShouldBe(64);
        Verify(verifier, token).ShouldBeTrue();
        descriptor.Header.Parameters.ShouldBeEmpty();
        descriptor.Parts.ShouldBeNull();
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Verify: A tampered ES256 payload is rejected")]
    public void Verify_WhenPayloadTampered_ShouldReturnFalse()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using ECDsa publicKey = CreatePublicKey(privateKey);
        string compact = JsonWebTokenWriter.CreateEs256(privateKey, keyId)
            .Write(CreateDescriptor(_now.AddHours(1)));
        JsonWebTokenParts original = JsonWebToken.Parse(compact).Parts!;
        string tamperedPayload = Base64Url.EncodeToString(Encoding.UTF8.GetBytes(
            "{\"iss\":\"cohesion-gateway\",\"aud\":\"attacker\",\"exp\":1788699600}"));
        JsonWebToken tampered = JsonWebToken.Parse(
            string.Concat(original.Header, ".", tamperedPayload, ".", original.Signature));
        IJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(publicKey, keyId);

        // Act
        bool verified = Verify(verifier, tampered);

        // Assert
        verified.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Verify: An ES256 token is rejected by the wrong key")]
    public void Verify_WhenEcdsaKeyIsWrong_ShouldReturnFalse()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using ECDsa wrongKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        JsonWebToken token = JsonWebToken.Parse(
            JsonWebTokenWriter.CreateEs256(privateKey, keyId).Write(CreateDescriptor(_now.AddHours(1))));
        IJsonWebTokenSignatureVerifier verifier = EcdsaJsonWebTokenSignatureVerifier.Create(wrongKey, keyId);

        // Act
        bool verified = Verify(verifier, token);

        // Assert
        verified.ShouldBeFalse();
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Validate: Written exp and aud use the existing validation path")]
    public void Validate_WhenWrittenTokenIsExpiredAndAudienceDiffers_ShouldReportExistingCodes()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        JsonWebToken token = JsonWebToken.Parse(
            JsonWebTokenWriter.CreateEs256(privateKey, keyId).Write(CreateDescriptor(_now.AddHours(1))));
        JsonWebTokenValidationOptions options = new(_now.AddHours(2))
        {
            ClockSkew = TimeSpan.Zero,
            ExpectedAudience = "configuration-store",
            ExpectedIssuer = issuer,
        };
        options.AllowedAlgorithms.Add(JoseAlgorithms.ES256);

        // Act
        TokenValidationResult result = token.Validate(options);

        // Assert
        result.Succeeded.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.Code == TokenValidationCodes.Expired);
        result.Errors.ShouldContain(error => error.Code == TokenValidationCodes.AudienceMismatch);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - Write: A registered claim with two descriptor sources is rejected")]
    public void Write_WhenRegisteredClaimHasTwoSources_ShouldThrow()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        JsonWebTokenWriter writer = JsonWebTokenWriter.CreateEs256(privateKey, keyId);
        JsonWebTokenDescriptor descriptor = CreateDescriptor(_now.AddHours(1));
        descriptor.Claims.Add(new IdentityClaim(IdentityClaimTypes.Issuer, issuer));

        // Act
        Action write = () => writer.Write(descriptor);

        // Assert
        Should.Throw<ArgumentException>(write);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - CreateEs256: A non-P256 key is rejected")]
    public void CreateEs256_WhenKeyIsNotP256_ShouldThrow()
    {
        // Arrange
        using ECDsa privateKey = ECDsa.Create(ECCurve.NamedCurves.nistP384);

        // Act
        Action create = () => JsonWebTokenWriter.CreateEs256(privateKey, keyId);

        // Assert
        Should.Throw<ArgumentException>(create);
    }

    [Fact(DisplayName = "Cohesion Test [IdentityModel.Token.JsonWebToken] - CreateEs256: A same-size non-NIST curve is rejected")]
    public void CreateEs256_WhenCurveIsSecp256K1_ShouldThrow()
    {
        // Arrange
        ECDsa privateKey;
        try
        {
            privateKey = ECDsa.Create(ECCurve.CreateFromFriendlyName("secp256k1"));
        }
        catch (PlatformNotSupportedException)
        {
            // macOS (Apple CryptoKit) cannot generate secp256k1 keys, so the rejection path cannot be
            // reached there; Windows and Linux legs exercise it. xUnit v2 has no dynamic skip.
            return;
        }
        using ECDsa _ = privateKey;

        // Act
        Action create = () => JsonWebTokenWriter.CreateEs256(privateKey, keyId);

        // Assert
        Should.Throw<ArgumentException>(create);
    }

    private static JsonWebTokenDescriptor CreateDescriptor(DateTimeOffset expiresAt)
    {
        var descriptor = new JsonWebTokenDescriptor
        {
            Id = "bootstrap-1",
            Issuer = issuer,
            Subject = new SubjectIdentifier("gateway"),
            TokenType = "JWT",
            IssuedAt = _now,
            ExpiresAt = expiresAt,
        };
        descriptor.Audiences.Add(audience);
        return descriptor;
    }

    private static ECDsa CreatePublicKey(ECDsa privateKey)
    {
        byte[] subjectPublicKeyInfo = privateKey.ExportSubjectPublicKeyInfo();
        ECDsa publicKey = ECDsa.Create();
        publicKey.ImportSubjectPublicKeyInfo(subjectPublicKeyInfo, out int bytesRead);
        bytesRead.ShouldBe(subjectPublicKeyInfo.Length);
        return publicKey;
    }

    private static bool Verify(IJsonWebTokenSignatureVerifier verifier, JsonWebToken token)
    {
        string algorithm = token.Algorithm!;
        byte[] signingInput = Encoding.ASCII.GetBytes(token.SigningInput!);
        byte[] signature = Base64Url.DecodeFromChars(token.Parts!.Signature);
        return verifier.CanVerify(algorithm, token.Header.KeyId) &&
            verifier.Verify(algorithm, signingInput, signature);
    }
}
```

### JsonWebTokenValidatorTests.cs

```csharp
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Shouldly;

using Xunit;

namespace Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.Tests;

/// <summary>
/// Verifies the ES256 validator's accept/reject matrix: issuer trust, key selection, signature, algorithm,
/// required claims, temporal and lifetime rules, subject rules, and the caller-applied audience rule.
/// </summary>
public sealed class JsonWebTokenValidatorTests : IDisposable
{
    private const string displayPrefix = "Cohesion Test [IdentityModel.Token.JsonWebToken] - JsonWebTokenValidator: ";
    private const string issuer = "application";
    private const string subject = "gateway";
    private const string audience = "secret-store";
    private static readonly DateTimeOffset _now = new(2026, 9, 26, 12, 0, 0, TimeSpan.Zero);
    private static readonly TimeSpan _maximumLifetime = TimeSpan.FromHours(24);

    private readonly ECDsa _signingKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
    private readonly JsonWebKey _publicKey;

    public JsonWebTokenValidatorTests()
    {
        using JsonDocument document = JsonDocument.Parse(
            JsonWebKeyTests.CreateJwk(_signingKey.ExportParameters(includePrivateParameters: false)));
        JsonWebKey.TryParse(document.RootElement, out JsonWebKey? key).ShouldBeTrue();
        _publicKey = key!;
    }

    public void Dispose() => _signingKey.Dispose();

    [Fact(DisplayName = displayPrefix + "TryValidate: A trusted, well-formed ES256 token is accepted")]
    public void TryValidate_WhenTokenSatisfiesProfile_ReturnsTokenAndAudienceMatches()
    {
        // Arrange
        IJsonWebTokenValidator validator = CreateValidator();
        string compact = Write(Descriptor());

        // Act
        bool valid = validator.TryValidate(compact, _now, out JsonWebToken? token);

        // Assert
        valid.ShouldBeTrue();
        token.ShouldNotBeNull().Issuer.ShouldBe(issuer);
        token.HasAudience(audience).ShouldBeTrue();
        token.HasAudience("other").ShouldBeFalse();
    }

    [Fact(DisplayName = displayPrefix + "TryValidate: A lifetime exactly at the ceiling is accepted")]
    public void TryValidate_WhenLifetimeEqualsMaximum_ReturnsTrue()
    {
        // Arrange
        IJsonWebTokenValidator validator = CreateValidator();
        JsonWebTokenDescriptor descriptor = Descriptor();
        descriptor.ExpiresAt = _now + _maximumLifetime;

        // Act
        bool valid = validator.TryValidate(Write(descriptor), _now, out _);

        // Assert
        valid.ShouldBeTrue();
    }

    [Theory(DisplayName = displayPrefix + "TryValidate: Every violated rule rejects the token")]
    [InlineData("untrusted-issuer")]
    [InlineData("unknown-kid")]
    [InlineData("wrong-key")]
    [InlineData("tampered")]
    [InlineData("expired")]
    [InlineData("over-lifetime")]
    [InlineData("future-issued")]
    [InlineData("not-yet-valid")]
    [InlineData("expires-before-issued")]
    [InlineData("missing-jti")]
    [InlineData("missing-nbf")]
    [InlineData("missing-aud")]
    [InlineData("unsecured")]
    [InlineData("relabelled-algorithm")]
    [InlineData("malformed")]
    public void TryValidate_WhenRuleIsViolated_ReturnsFalse(string violation)
    {
        // Arrange
        IJsonWebTokenValidator validator = CreateValidator();
        JsonWebTokenDescriptor descriptor = Descriptor();
        string compact;
        switch (violation)
        {
            case "untrusted-issuer":
                descriptor.Issuer = "someone-else";
                compact = Write(descriptor);
                break;
            case "unknown-kid":
                compact = JsonWebTokenWriter.CreateEs256(_signingKey, "unknown").Write(descriptor);
                break;
            case "wrong-key":
                using (ECDsa other = ECDsa.Create(ECCurve.NamedCurves.nistP256))
                {
                    compact = JsonWebTokenWriter.CreateEs256(other, _publicKey.KeyId!).Write(descriptor);
                }
                break;
            case "tampered":
                string[] segments = Write(descriptor).Split('.');
                string payload = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(segments[1]))
                    .Replace(audience, "attacker", StringComparison.Ordinal);
                compact = string.Join('.', segments[0], Base64Url.EncodeToString(Encoding.UTF8.GetBytes(payload)), segments[2]);
                break;
            case "expired":
                descriptor.IssuedAt = _now.AddHours(-2);
                descriptor.NotBefore = _now.AddHours(-2);
                descriptor.ExpiresAt = _now.AddHours(-1);
                compact = Write(descriptor);
                break;
            case "over-lifetime":
                descriptor.ExpiresAt = _now + _maximumLifetime + TimeSpan.FromSeconds(1);
                compact = Write(descriptor);
                break;
            case "future-issued":
                descriptor.IssuedAt = _now.AddMinutes(6);
                compact = Write(descriptor);
                break;
            case "not-yet-valid":
                descriptor.NotBefore = _now.AddMinutes(6);
                compact = Write(descriptor);
                break;
            case "expires-before-issued":
                descriptor.IssuedAt = _now.AddMinutes(4);
                descriptor.ExpiresAt = _now.AddMinutes(3);
                compact = Write(descriptor);
                break;
            case "missing-jti":
                descriptor.Id = null;
                compact = Write(descriptor);
                break;
            case "missing-nbf":
                descriptor.NotBefore = null;
                compact = Write(descriptor);
                break;
            case "missing-aud":
                descriptor.Audiences.Clear();
                compact = Write(descriptor);
                break;
            case "unsecured":
                compact = Encode("{\"alg\":\"none\",\"kid\":\"" + _publicKey.KeyId + "\"}") + "." +
                    Write(descriptor).Split('.')[1] + ".";
                break;
            case "relabelled-algorithm":
                string[] parts = Write(descriptor).Split('.');
                compact = Encode("{\"alg\":\"ES384\",\"kid\":\"" + _publicKey.KeyId + "\",\"typ\":\"JWT\"}") + "." +
                    parts[1] + "." + parts[2];
                break;
            default:
                compact = "not-a-token";
                break;
        }

        // Act
        bool valid = validator.TryValidate(compact, _now, out JsonWebToken? token);

        // Assert
        valid.ShouldBeFalse();
        token.ShouldBeNull();
    }

    [Fact(DisplayName = displayPrefix + "TryValidate: An expected subject is enforced exactly")]
    public void TryValidate_WhenExpectedSubjectDiffers_ReturnsFalse()
    {
        // Arrange
        IJsonWebTokenValidator matching = CreateValidator(expectedSubject: subject);
        IJsonWebTokenValidator mismatching = CreateValidator(expectedSubject: "Gateway");
        string compact = Write(Descriptor());

        // Act
        bool accepted = matching.TryValidate(compact, _now, out _);
        bool rejected = mismatching.TryValidate(compact, _now, out _);

        // Assert
        accepted.ShouldBeTrue();
        rejected.ShouldBeFalse();
    }

    [Fact(DisplayName = displayPrefix + "TryValidate: A correctly signed token without a kid header selects no key")]
    public void TryValidate_WhenHeaderOmitsKeyId_ReturnsFalse()
    {
        // Arrange
        IJsonWebTokenValidator validator = CreateValidator();
        string payload = Write(Descriptor()).Split('.')[1];
        string withKeyId = Sign("{\"alg\":\"ES256\",\"kid\":\"" + _publicKey.KeyId + "\",\"typ\":\"JWT\"}", payload);
        string withoutKeyId = Sign("{\"alg\":\"ES256\",\"typ\":\"JWT\"}", payload);

        // Act
        bool accepted = validator.TryValidate(withKeyId, _now, out _);
        bool rejected = validator.TryValidate(withoutKeyId, _now, out JsonWebToken? token);

        // Assert
        accepted.ShouldBeTrue();
        rejected.ShouldBeFalse();
        token.ShouldBeNull();
    }

    [Fact(DisplayName = displayPrefix + "TryValidate: A non-string sub is refused unless the caller applies its own subject rule")]
    public void TryValidate_WhenSubjectIsNotAString_AppliesRequireSubject()
    {
        // Arrange
        IJsonWebTokenValidator requiring = CreateValidator();
        IJsonWebTokenValidator relaxed = JsonWebTokenValidator.CreateEs256(new JsonWebTokenValidationProfile(
            issuer,
            new JsonWebKeySet(_publicKey),
            _maximumLifetime)
        {
            RequireSubject = false,
        });
        long issuedAt = _now.ToUnixTimeSeconds();
        string payload = Encode(
            "{\"iss\":\"" + issuer + "\",\"sub\":5,\"aud\":\"" + audience + "\",\"iat\":" + issuedAt +
            ",\"nbf\":" + issuedAt + ",\"exp\":" + (issuedAt + 3600) + ",\"jti\":\"" + Guid.NewGuid().ToString("N") + "\"}");
        string compact = Sign("{\"alg\":\"ES256\",\"kid\":\"" + _publicKey.KeyId + "\",\"typ\":\"JWT\"}", payload);

        // Act
        bool required = requiring.TryValidate(compact, _now, out _);
        bool relaxedResult = relaxed.TryValidate(compact, _now, out JsonWebToken? token);

        // Assert
        required.ShouldBeFalse();
        relaxedResult.ShouldBeTrue();
        token.ShouldNotBeNull().Subject.ShouldBeNull();
    }

    [Fact(DisplayName = displayPrefix + "TryValidate: The issuer resolver observes keys added after construction")]
    public void TryValidate_WhenResolverGainsIssuer_AcceptsAfterTrustIsGranted()
    {
        // Arrange
        var trusted = new Dictionary<string, JsonWebKeySet>(StringComparer.Ordinal);
        IJsonWebTokenValidator validator = JsonWebTokenValidator.CreateEs256(new JsonWebTokenValidationProfile(
            name => trusted.TryGetValue(name, out JsonWebKeySet? keys) ? keys : null,
            _maximumLifetime));
        string compact = Write(Descriptor());
        bool before = validator.TryValidate(compact, _now, out _);

        // Act
        trusted[issuer] = new JsonWebKeySet(_publicKey);
        bool after = validator.TryValidate(compact, _now, out _);

        // Assert
        before.ShouldBeFalse();
        after.ShouldBeTrue();
    }

    [Fact(DisplayName = displayPrefix + "Profile: Invalid lifetime and skew are refused")]
    public void Constructor_WhenLifetimeOrSkewIsInvalid_Throws()
    {
        // Arrange
        var keys = new JsonWebKeySet(_publicKey);

        // Act
        Action zeroLifetime = () => _ = new JsonWebTokenValidationProfile(issuer, keys, TimeSpan.Zero);
        Action negativeSkew = () => _ = new JsonWebTokenValidationProfile(issuer, keys, _maximumLifetime)
        {
            ClockSkew = TimeSpan.FromSeconds(-1),
        };
        Action blankIssuer = () => _ = new JsonWebTokenValidationProfile(" ", keys, _maximumLifetime);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(zeroLifetime);
        Should.Throw<ArgumentOutOfRangeException>(negativeSkew);
        Should.Throw<ArgumentException>(blankIssuer);
    }

    private JsonWebTokenValidator CreateValidator(string? expectedSubject = null) =>
        JsonWebTokenValidator.CreateEs256(new JsonWebTokenValidationProfile(
            issuer,
            new JsonWebKeySet(_publicKey),
            _maximumLifetime)
        {
            ExpectedSubject = expectedSubject,
        });

    private string Write(JsonWebTokenDescriptor descriptor) =>
        JsonWebTokenWriter.CreateEs256(_signingKey, _publicKey.KeyId!).Write(descriptor);

    private static JsonWebTokenDescriptor Descriptor()
    {
        var descriptor = new JsonWebTokenDescriptor
        {
            Id = Guid.NewGuid().ToString("N"),
            Issuer = issuer,
            Subject = new SubjectIdentifier(subject),
            TokenType = "JWT",
            IssuedAt = _now,
            NotBefore = _now,
            ExpiresAt = _now.AddHours(1),
        };
        descriptor.Audiences.Add(audience);
        return descriptor;
    }

    private string Sign(string headerJson, string encodedPayload)
    {
        string signingInput = Encode(headerJson) + "." + encodedPayload;
        byte[] signature = _signingKey.SignData(
            Encoding.ASCII.GetBytes(signingInput),
            HashAlgorithmName.SHA256,
            DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
        return signingInput + "." + Base64Url.EncodeToString(signature);
    }

    private static string Encode(string json) => Base64Url.EncodeToString(Encoding.UTF8.GetBytes(json));
}
```

## Walkthrough

### JsonWebTokenSignatureVerifierTests

- **Covered behavior** — Verify: RS256 signature round-trips with a public key.
- **Covered behavior** — CanVerify: ECDSA algorithm must match the named curve.
- **Covered behavior** — CreateEcdsa: A non-JOSE curve is rejected.
- **Covered behavior** — Verify: A malformed ES256 signature length is rejected.

### JsonWebTokenWriterTests

- **Covered behavior** — Write: ES256 token round-trips through parse and verification.
- **Covered behavior** — Verify: A tampered ES256 payload is rejected.
- **Covered behavior** — Verify: An ES256 token is rejected by the wrong key.
- **Covered behavior** — Validate: Written exp and aud use the existing validation path.
- **Covered behavior** — Write: A registered claim with two descriptor sources is rejected.
- **Covered behavior** — CreateEs256: A non-P256 key is rejected.
- **Covered behavior** — CreateEs256: A same-size non-NIST curve is rejected.

### JsonWebTokenValidatorTests

- **Covered behavior** — JsonWebTokenValidator: TryValidate: A trusted, well-formed ES256 token is accepted.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: A lifetime exactly at the ceiling is accepted.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: Every violated rule rejects the token. Cases: `"untrusted-issuer"`, `"unknown-kid"`, `"wrong-key"`, `"tampered"`, `"expired"`, `"over-lifetime"`, `"future-issued"`, `"not-yet-valid"`, `"expires-before-issued"`, `"missing-jti"`, `"missing-nbf"`, `"missing-aud"`, `"unsecured"`, `"relabelled-algorithm"`, `"malformed"`.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: An expected subject is enforced exactly.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: A correctly signed token without a kid header selects no key.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: A non-string sub is refused unless the caller applies its own subject rule.
- **Covered behavior** — JsonWebTokenValidator: TryValidate: The issuer resolver observes keys added after construction.
- **Covered behavior** — JsonWebTokenValidator: Profile: Invalid lifetime and skew are refused.

The assertions define the expected result or failure boundary. Test-only doubles supply controlled
inputs; they are not additional shipped APIs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/tests/JsonWebTokenSignatureVerifierTests.cs`.
- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/tests/JsonWebTokenWriterTests.cs`.
- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/tests/JsonWebTokenValidatorTests.cs`.
- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/tests/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.Tests.csproj`.
