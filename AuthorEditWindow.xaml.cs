using System.Globalization;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class AuthorEditWindow : Window
{
    LibraryDbContext context;
    Author? editingAuthor;

    public AuthorEditWindow(LibraryDbContext db, Author? author)
    {
        InitializeComponent();
        context = db;
        editingAuthor = author;
        if (author != null)
            Title = "Редактировать автора";

        if (editingAuthor != null)
        {
            FirstNameBox.Text = editingAuthor.FirstName;
            LastNameBox.Text = editingAuthor.LastName;
            BirthDateBox.Text = editingAuthor.BirthDate.HasValue
                ? editingAuthor.BirthDate.Value.ToString("dd.MM.yyyy")
                : "";
            CountryBox.Text = editingAuthor.Country ?? "";
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
        {
            MessageBox.Show("Нужны имя и фамилия.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DateTime? birthDate = null;
        if (!string.IsNullOrWhiteSpace(BirthDateBox.Text))
        {
            if (DateTime.TryParse(BirthDateBox.Text, CultureInfo.GetCultureInfo("ru-RU"),
                DateTimeStyles.None, out DateTime parsed))
                birthDate = parsed;
            else
            {
                MessageBox.Show("Дата в формате дд.мм.гггг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        string country = string.IsNullOrWhiteSpace(CountryBox.Text) ? null : CountryBox.Text.Trim();

        if (editingAuthor != null)
        {
            editingAuthor.FirstName = FirstNameBox.Text.Trim();
            editingAuthor.LastName = LastNameBox.Text.Trim();
            editingAuthor.BirthDate = birthDate;
            editingAuthor.Country = country;
        }
        else
        {
            context.Authors.Add(new Author
            {
                FirstName = FirstNameBox.Text.Trim(),
                LastName = LastNameBox.Text.Trim(),
                BirthDate = birthDate,
                Country = country
            });
        }
        context.SaveChanges();
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
