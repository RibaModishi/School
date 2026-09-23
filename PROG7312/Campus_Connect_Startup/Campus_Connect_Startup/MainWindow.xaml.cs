using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Campus_Connect_Startup.Services;

namespace Campus_Connect_Startup;

public partial class MainWindow : Window
{
    private readonly FirebaseAuthService _authService;

    public MainWindow()
    {
        InitializeComponent();
        var config = AppConfig.Load();
        _authService = new FirebaseAuthService(config.Firebase.ApiKey);
        Loaded += (_, _) => FullNameBox.Focus();
    }

    private void FullNameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        FullNamePlaceholder.Visibility = string.IsNullOrEmpty(FullNameBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailBox.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        ConfirmPlaceholder.Visibility = string.IsNullOrEmpty(ConfirmPasswordBox.Password)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private async void CreateAccountButton_Click(object sender, RoutedEventArgs e)
    {
        ClearFeedback();

        var fullName = FullNameBox.Text.Trim();
        var email = EmailBox.Text.Trim();
        var password = PasswordBox.Password;
        var confirmPassword = ConfirmPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(fullName))
        {
            ShowError("Please enter your full name.");
            FullNameBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowError("Please enter your email address.");
            EmailBox.Focus();
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Please enter a valid email address (must contain @ and .).");
            EmailBox.Focus();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Please enter a password.");
            PasswordBox.Focus();
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Password and Confirm Password do not match.");
            ConfirmPasswordBox.Focus();
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            PasswordBox.Focus();
            return;
        }

        SetBusy(true);

        try
        {
            var result = await _authService.RegisterAndSendConfirmationAsync(email, password, fullName);

            if (!result.Success)
            {
                ShowError(result.ErrorMessage ?? "Registration failed.");
                return;
            }

            MessageBox.Show(
                $"Account created for {result.Email}.\n\n" +
                "Please check your inbox (and Spam/Junk folder) for a confirmation email from Firebase, " +
                "then click the link to activate your account before signing in.\n\n" +
                "If nothing arrives, fill in the same email and password and click Resend confirmation email.",
                "Check your inbox",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ClearForm();
            ShowStatus("Confirmation requested. Check inbox and Spam/Junk. Use Resend if needed.");
        }
        catch (HttpRequestException)
        {
            ShowError("Could not reach Firebase. Check your internet connection and try again.");
        }
        catch (TaskCanceledException)
        {
            ShowError("The request timed out. Please try again.");
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void ResendButton_Click(object sender, RoutedEventArgs e)
    {
        ClearFeedback();

        var email = EmailBox.Text.Trim();
        var password = PasswordBox.Password;

        if (!IsValidEmail(email))
        {
            ShowError("Enter the email address for your existing account.");
            EmailBox.Focus();
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Enter your password so we can resend the confirmation email.");
            PasswordBox.Focus();
            return;
        }

        SetBusy(true);

        try
        {
            var result = await _authService.ResendConfirmationEmailAsync(email, password);
            if (!result.Success)
            {
                ShowError(result.ErrorMessage ?? "Could not resend confirmation email.");
                return;
            }

            MessageBox.Show(
                $"Confirmation email resent to {result.Email}.\n\n" +
                "Check your inbox and Spam/Junk folder. The sender is usually something like noreply@…firebaseapp.com.",
                "Confirmation resent",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ShowStatus("Confirmation email resent. Check inbox and Spam/Junk.");
        }
        catch (HttpRequestException)
        {
            ShowError("Could not reach Firebase. Check your internet connection and try again.");
        }
        catch (Exception ex)
        {
            ShowError($"Unexpected error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SignInLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        MessageBox.Show(
            "Sign in will be available once your email is confirmed.",
            "Campus Connect",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        e.Handled = true;
    }

    private static bool IsValidEmail(string email) =>
        email.Contains('@') && email.Contains('.');

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorText.Visibility = Visibility.Visible;
        StatusText.Visibility = Visibility.Collapsed;
    }

    private void ShowStatus(string message)
    {
        StatusText.Text = message;
        StatusText.Visibility = Visibility.Visible;
        ErrorText.Visibility = Visibility.Collapsed;
    }

    private void ClearFeedback()
    {
        ErrorText.Text = string.Empty;
        ErrorText.Visibility = Visibility.Collapsed;
        StatusText.Text = string.Empty;
        StatusText.Visibility = Visibility.Collapsed;
    }

    private void ClearForm()
    {
        FullNameBox.Clear();
        EmailBox.Clear();
        PasswordBox.Clear();
        ConfirmPasswordBox.Clear();
        FullNamePlaceholder.Visibility = Visibility.Visible;
        EmailPlaceholder.Visibility = Visibility.Visible;
        PasswordPlaceholder.Visibility = Visibility.Visible;
        ConfirmPlaceholder.Visibility = Visibility.Visible;
    }

    private void SetBusy(bool isBusy)
    {
        CreateAccountButton.IsEnabled = !isBusy;
        ResendButton.IsEnabled = !isBusy;
        CreateAccountButton.Content = isBusy ? "WORKING…" : "CREATE MY ACCOUNT";
        FullNameBox.IsEnabled = !isBusy;
        EmailBox.IsEnabled = !isBusy;
        PasswordBox.IsEnabled = !isBusy;
        ConfirmPasswordBox.IsEnabled = !isBusy;
    }
}
