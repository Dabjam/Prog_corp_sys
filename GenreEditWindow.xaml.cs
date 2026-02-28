using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class GenreEditWindow : Window
{
    LibraryDbContext context;
    Genre? editingGenre;

    public GenreEditWindow(LibraryDbContext db, Genre? genre)
    {
        InitializeComponent();
        context = db;
        editingGenre = genre;
        if (genre != null)
            Title = "Редактировать жанр";

        if (editingGenre != null)
        {
            NameBox.Text = editingGenre.Name;
            DescriptionBox.Text = editingGenre.Description ?? "";
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Введите название жанра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string desc = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim();

        if (editingGenre != null)
        {
            editingGenre.Name = NameBox.Text.Trim();
            editingGenre.Description = desc;
        }
        else
        {
            context.Genres.Add(new Genre
            {
                Name = NameBox.Text.Trim(),
                Description = desc
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
