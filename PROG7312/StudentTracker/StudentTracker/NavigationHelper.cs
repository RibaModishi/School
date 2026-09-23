using System.Windows;

namespace StudentTracker
{
    public static class NavigationHelper
    {
        public static void Navigate(Window current, Window next)
        {
            next.Show();
            current.Close();
        }

        public static void Logout(Window current)
        {
            Navigate(current, new MainWindow());
        }
    }
}
