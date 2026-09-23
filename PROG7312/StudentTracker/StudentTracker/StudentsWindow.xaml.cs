using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentTracker
{
    public partial class StudentsWindow : Window
    {
        private readonly List<Student> _allStudents = new();
        private bool _isEditing;
        private string? _editingStudentNumber;

        public StudentsWindow()
        {
            InitializeComponent();
            DateTimeText.Text = $"{System.DateTime.Now:dd MMMM yyyy} | {System.DateTime.Now:h:mm tt}";
            LoadSampleData();
            RefreshGrid();
        }

        private void LoadSampleData()
        {
            _allStudents.AddRange(new[]
            {
                new Student { StudentNumber = "ST0001", FirstName = "Thabo", LastName = "Nkosi", Gender = "Male", Qualification = "BSc IT", Campus = "Bellville", Status = "Active" },
                new Student { StudentNumber = "ST0002", FirstName = "Amahle", LastName = "Dube", Gender = "Female", Qualification = "Dip HR", Campus = "Wits", Status = "Active" },
                new Student { StudentNumber = "ST0003", FirstName = "Liam", LastName = "Petersen", Gender = "Male", Qualification = "BCom", Campus = "Bellville", Status = "Inactive" }
            });
        }

        private void RefreshGrid(IEnumerable<Student>? students = null)
        {
            StudentsGrid.ItemsSource = null;
            StudentsGrid.ItemsSource = students?.ToList() ?? _allStudents.ToList();
        }

        private void ClearForm()
        {
            StudentNumberBox.Clear();
            FirstNameBox.Clear();
            LastNameBox.Clear();
            GenderBox.Clear();
            QualificationBox.Clear();
            CampusBox.Clear();
            _isEditing = false;
            _editingStudentNumber = null;
            StudentNumberBox.IsReadOnly = false;
        }

        private void LoadForm(Student student)
        {
            StudentNumberBox.Text = student.StudentNumber;
            FirstNameBox.Text = student.FirstName;
            LastNameBox.Text = student.LastName;
            GenderBox.Text = student.Gender;
            QualificationBox.Text = student.Qualification;
            CampusBox.Text = student.Campus;
        }

        private Student? GetFormStudent(string status = "Active")
        {
            if (string.IsNullOrWhiteSpace(StudentNumberBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Student number, first name, and last name are required.",
                    "Student management", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            return new Student
            {
                StudentNumber = StudentNumberBox.Text.Trim(),
                FirstName = FirstNameBox.Text.Trim(),
                LastName = LastNameBox.Text.Trim(),
                Gender = GenderBox.Text.Trim(),
                Qualification = QualificationBox.Text.Trim(),
                Campus = CampusBox.Text.Trim(),
                Status = status
            };
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student student)
                LoadForm(student);
        }

        private void NewStudentButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            StudentsGrid.UnselectAll();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is not Student student)
            {
                MessageBox.Show("Select a student to edit.",
                    "Student management", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _isEditing = true;
            _editingStudentNumber = student.StudentNumber;
            LoadForm(student);
            StudentNumberBox.IsReadOnly = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is not Student student)
            {
                MessageBox.Show("Select a student to delete.",
                    "Student management", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Delete {student.FirstName} {student.LastName}?",
                "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _allStudents.RemoveAll(s => s.StudentNumber == student.StudentNumber);
            ClearForm();
            RefreshGrid();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var term = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(term))
            {
                RefreshGrid();
                return;
            }

            var filtered = _allStudents.Where(s =>
                s.StudentNumber.ToLower().Contains(term) ||
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                s.Gender.ToLower().Contains(term) ||
                s.Qualification.ToLower().Contains(term) ||
                s.Campus.ToLower().Contains(term) ||
                s.Status.ToLower().Contains(term));

            RefreshGrid(filtered);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            RefreshGrid();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new DashboardWindow());

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditing)
            {
                var existing = _allStudents.FirstOrDefault(s => s.StudentNumber == _editingStudentNumber);
                if (existing == null)
                    return;

                var updated = GetFormStudent(existing.Status);
                if (updated == null)
                    return;

                existing.FirstName = updated.FirstName;
                existing.LastName = updated.LastName;
                existing.Gender = updated.Gender;
                existing.Qualification = updated.Qualification;
                existing.Campus = updated.Campus;
            }
            else
            {
                var student = GetFormStudent();
                if (student == null)
                    return;

                if (_allStudents.Any(s => s.StudentNumber.Equals(student.StudentNumber, System.StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("A student with this number already exists.",
                        "Student management", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _allStudents.Add(student);
            }

            ClearForm();
            RefreshGrid();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            StudentsGrid.UnselectAll();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new DashboardWindow());

        private void StudentsButton_Click(object sender, RoutedEventArgs e) { }

        private void CoursesButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new CourseWindow());

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new RegistrationWindow());

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new ReportsWindow());

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Navigate(this, new SettingWindow());

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
            => NavigationHelper.Logout(this);
    }
}
