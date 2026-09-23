using System.Windows;

namespace StudentTracker
{
    /// <summary>
    /// Sign-up window — same layout language as the login page.
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.",
                    "Sign Up", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // No database yet — UI-only confirm for now
            MessageBox.Show($"Account created for {username}",
                "Sign Up", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            Close();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Student Tracker\nEmeris",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
