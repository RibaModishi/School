using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Campus_Connect_Startup.Services;

/// <summary>
/// Registers a user with Firebase Auth and sends a real confirmation email.
/// Docs: https://firebase.google.com/docs/reference/rest/auth
/// </summary>
public sealed class FirebaseAuthService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly string _apiKey;

    public FirebaseAuthService(string apiKey)
    {
        _apiKey = apiKey?.Trim() ?? string.Empty;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_apiKey)
        && !_apiKey.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
        && !_apiKey.StartsWith("PASTE_", StringComparison.OrdinalIgnoreCase);

    public async Task<AuthResult> RegisterAndSendConfirmationAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return AuthResult.Fail(
                "Firebase API key is missing. Open appsettings.json and paste your Web API key " +
                "from Firebase Console → Project settings → General.");
        }

        var signUp = await SignUpAsync(email, password, cancellationToken);
        if (!signUp.Success || string.IsNullOrWhiteSpace(signUp.IdToken))
        {
            return signUp;
        }

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            await UpdateDisplayNameAsync(signUp.IdToken, fullName, cancellationToken);
        }

        var verification = await SendEmailVerificationAsync(signUp.IdToken, cancellationToken);
        if (!verification.Success)
        {
            return AuthResult.Fail(
                $"Account was created, but the confirmation email could not be sent: {verification.ErrorMessage}");
        }

        return AuthResult.Ok(signUp.Email ?? email);
    }

    /// <summary>
    /// Signs in an existing user and resends the Firebase confirmation email.
    /// </summary>
    public async Task<AuthResult> ResendConfirmationEmailAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return AuthResult.Fail("Firebase API key is missing. Check appsettings.json.");
        }

        var signIn = await SignInAsync(email, password, cancellationToken);
        if (!signIn.Success || string.IsNullOrWhiteSpace(signIn.IdToken))
        {
            return signIn;
        }

        var verification = await SendEmailVerificationAsync(signIn.IdToken, cancellationToken);
        if (!verification.Success)
        {
            return AuthResult.Fail(verification.ErrorMessage ?? "Could not resend the confirmation email.");
        }

        return AuthResult.Ok(email);
    }

    private async Task<AuthResult> SignUpAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_apiKey}";
        var payload = new { email, password, returnSecureToken = true };

        using var response = await Http.PostAsJsonAsync(url, payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return AuthResult.Fail(MapFirebaseError(body));
        }

        var data = JsonSerializer.Deserialize<SignUpResponse>(body, JsonOptions);
        if (data is null || string.IsNullOrWhiteSpace(data.IdToken))
        {
            return AuthResult.Fail("Unexpected response from Firebase.");
        }

        return AuthResult.Ok(data.Email ?? email, data.IdToken);
    }

    private async Task<AuthResult> SignInAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}";
        var payload = new { email, password, returnSecureToken = true };

        using var response = await Http.PostAsJsonAsync(url, payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return AuthResult.Fail(MapFirebaseError(body));
        }

        var data = JsonSerializer.Deserialize<SignUpResponse>(body, JsonOptions);
        if (data is null || string.IsNullOrWhiteSpace(data.IdToken))
        {
            return AuthResult.Fail("Could not sign in to resend the confirmation email.");
        }

        return AuthResult.Ok(data.Email ?? email, data.IdToken);
    }

    private async Task UpdateDisplayNameAsync(
        string idToken,
        string displayName,
        CancellationToken cancellationToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={_apiKey}";
        var payload = new { idToken, displayName, returnSecureToken = true };
        using var response = await Http.PostAsJsonAsync(url, payload, cancellationToken);
        _ = response;
    }

    private async Task<AuthResult> SendEmailVerificationAsync(
        string idToken,
        CancellationToken cancellationToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_apiKey}";
        var payload = new { requestType = "VERIFY_EMAIL", idToken };

        using var response = await Http.PostAsJsonAsync(url, payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return AuthResult.Fail(MapFirebaseError(body));
        }

        return AuthResult.Ok();
    }

    private static string MapFirebaseError(string responseBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.TryGetProperty("error", out var error)
                && error.TryGetProperty("message", out var message))
            {
                return message.GetString() switch
                {
                    "EMAIL_EXISTS" =>
                        "An account with this email already exists. Enter the same email and password, then click Resend confirmation email.",
                    "INVALID_PASSWORD" => "Incorrect password. Check your password and try again.",
                    "EMAIL_NOT_FOUND" => "No account found for that email.",
                    "INVALID_LOGIN_CREDENTIALS" => "Incorrect email or password.",
                    "INVALID_EMAIL" => "The email address is not valid.",
                    "WEAK_PASSWORD : Password should be at least 6 characters" =>
                        "Password must be at least 6 characters.",
                    "OPERATION_NOT_ALLOWED" =>
                        "Enable Email/Password in Firebase Console → Authentication → Sign-in method.",
                    "CONFIGURATION_NOT_FOUND" =>
                        "Firebase Auth is not set up for this API key. Open Firebase Console, " +
                        "enable Authentication → Email/Password, then copy the Web API key from " +
                        "Project settings → General into appsettings.json.",
                    "TOO_MANY_ATTEMPTS_TRY_LATER" => "Too many attempts. Please try again later.",
                    "API_KEY_INVALID" => "The Firebase API key is invalid. Check appsettings.json.",
                    var other => string.IsNullOrWhiteSpace(other)
                        ? "Registration failed. Please try again."
                        : other.Replace('_', ' ')
                };
            }
        }
        catch (JsonException)
        {
            // Fall through.
        }

        return "Registration failed. Please try again.";
    }

    private sealed class SignUpResponse
    {
        [JsonPropertyName("idToken")]
        public string? IdToken { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }
}

public sealed class AuthResult
{
    public bool Success { get; private init; }
    public string? Email { get; private init; }
    public string? IdToken { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static AuthResult Ok(string? email = null, string? idToken = null) => new()
    {
        Success = true,
        Email = email,
        IdToken = idToken
    };

    public static AuthResult Fail(string message) => new()
    {
        Success = false,
        ErrorMessage = message
    };
}
