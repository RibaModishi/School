using System.Windows;

namespace StudentTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml (Login page).
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.",
                    "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (username == "admin" && password == "Q123@")
            {
                MessageBox.Show("Login Successfully",
                    "Login", MessageBoxButton.OK, MessageBoxImage.Information);

                var dashboard = new DashboardWindow();
                dashboard.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.",
                    "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            var signUp = new SignUpWindow();
            signUp.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
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
