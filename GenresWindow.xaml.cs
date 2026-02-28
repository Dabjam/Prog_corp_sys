using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class GenresWindow : Window
{
    LibraryDbContext context;

    public GenresWindow(LibraryDbContext db)
    {
        InitializeComponent();
        context = db;
        LoadGenres();
        GenresGrid.SelectionChanged += GenresGrid_SelectionChanged;
    }

    void GenresGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        EditButton.IsEnabled = GenresGrid.SelectedItem != null;
        DeleteButton.IsEnabled = GenresGrid.SelectedItem != null;
    }

    void LoadGenres()
    {
        context.Genres.Load();
        GenresGrid.ItemsSource = context.Genres.Local.ToObservableCollection();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        GenreEditWindow w = new GenreEditWindow(context, null);
        if (w.ShowDialog() == true)
            LoadGenres();
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        Genre g = GenresGrid.SelectedItem as Genre;
        if (g == null) return;
        GenreEditWindow w = new GenreEditWindow(context, g);
        if (w.ShowDialog() == true)
            LoadGenres();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        Genre g = GenresGrid.SelectedItem as Genre;
        if (g == null) return;
        if (MessageBox.Show("Удалить жанр \"" + g.Name + "\"? (книги этого жанра тоже удалятся)",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        context.Genres.Remove(g);
        context.SaveChanges();
        LoadGenres();
    }
}
