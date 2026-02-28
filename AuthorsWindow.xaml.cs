using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class AuthorsWindow : Window
{
    LibraryDbContext context;

    public AuthorsWindow(LibraryDbContext db)
    {
        InitializeComponent();
        context = db;
        LoadAuthors();
        AuthorsGrid.SelectionChanged += AuthorsGrid_SelectionChanged;
    }

    void AuthorsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        EditButton.IsEnabled = AuthorsGrid.SelectedItem != null;
        DeleteButton.IsEnabled = AuthorsGrid.SelectedItem != null;
    }

    void LoadAuthors()
    {
        context.Authors.Load();
        AuthorsGrid.ItemsSource = context.Authors.Local.ToObservableCollection();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        AuthorEditWindow w = new AuthorEditWindow(context, null);
        if (w.ShowDialog() == true)
            LoadAuthors();
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        Author a = AuthorsGrid.SelectedItem as Author;
        if (a == null) return;
        AuthorEditWindow w = new AuthorEditWindow(context, a);
        if (w.ShowDialog() == true)
            LoadAuthors();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        Author a = AuthorsGrid.SelectedItem as Author;
        if (a == null) return;
        if (MessageBox.Show("Удалить автора " + a.FullName + "? (книги автора тоже удалятся)",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        context.Authors.Remove(a);
        context.SaveChanges();
        LoadAuthors();
    }
}
