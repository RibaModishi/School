using System.Windows;

namespace StudentTracker
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
            DateTimeText.Text = $"{System.DateTime.Now:dd MMMM yyyy} | {System.DateTime.Now:h:mm tt}";
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new DashboardWindow());

        private void StudentsButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new StudentsWindow());

        private void CoursesButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new CourseWindow());

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new RegistrationWindow());

        private void ReportsButton_Click(object sender, RoutedEventArgs e) { }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new SettingWindow());

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Logout(this);
    }
}
