using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class MainWindow : Window
{
    private LibraryDbContext db;

    public MainWindow()
    {
        InitializeComponent();
        db = new LibraryDbContext();
        Loaded += MainWindow_Loaded;
    }

    void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadFilters();
        LoadBooks();
    }

    void LoadFilters()
    {
        db.Authors.Load();
        db.Genres.Load();

        var genres = new ObservableCollection<Genre>();
        genres.Add(new Genre { Id = 0, Name = "(Все жанры)" });
        foreach (var item in db.Genres.Local.OrderBy(x => x.Name))
            genres.Add(item);

        var authors = new ObservableCollection<Author>();
        authors.Add(new Author { Id = 0, FirstName = "", LastName = "(Все авторы)" });
        foreach (var item in db.Authors.Local.OrderBy(x => x.LastName))
            authors.Add(item);

        GenreFilter.ItemsSource = genres;
        AuthorFilter.ItemsSource = authors;
        GenreFilter.SelectedIndex = 0;
        AuthorFilter.SelectedIndex = 0;
    }

    void LoadBooks()
    {
        IQueryable<Book> query = db.Books.Include(b => b.Author).Include(b => b.Genre);

        string searchText = SearchBox != null ? SearchBox.Text.Trim() : "";
        if (searchText != "")
            query = query.Where(b => b.Title.Contains(searchText));

        if (GenreFilter.SelectedItem is Genre selectedGenre && selectedGenre.Id != 0)
            query = query.Where(b => b.GenreId == selectedGenre.Id);
        if (AuthorFilter.SelectedItem is Author selectedAuthor && selectedAuthor.Id != 0)
            query = query.Where(b => b.AuthorId == selectedAuthor.Id);

        BooksGrid.ItemsSource = query.ToList();
    }

    private void Filter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        LoadBooks();
    }

    private void SearchBox_KeyUp(object sender, KeyEventArgs e)
    {
        LoadBooks();
    }

    private void ResetFilters_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
        GenreFilter.SelectedIndex = 0;
        AuthorFilter.SelectedIndex = 0;
        LoadBooks();
    }

    private void BooksGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        bool selected = BooksGrid.SelectedItem != null;
        EditBookButton.IsEnabled = selected;
        DeleteBookButton.IsEnabled = selected;
    }

    private void AddBook_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using (var context = new LibraryDbContext())
            {
                BookEditWindow w = new BookEditWindow(context, null);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    db = new LibraryDbContext();
                    LoadFilters();
                    LoadBooks();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditBook_Click(object sender, RoutedEventArgs e)
    {
        Book book = BooksGrid.SelectedItem as Book;
        if (book == null) return;

        try
        {
            using (var context = new LibraryDbContext())
            {
                Book bookToEdit = context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .FirstOrDefault(b => b.Id == book.Id);
                if (bookToEdit == null) return;

                BookEditWindow w = new BookEditWindow(context, bookToEdit);
                w.Owner = this;
                if (w.ShowDialog() == true)
                {
                    db = new LibraryDbContext();
                    LoadFilters();
                    LoadBooks();
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteBook_Click(object sender, RoutedEventArgs e)
    {
        Book book = BooksGrid.SelectedItem as Book;
        if (book == null) return;
        if (MessageBox.Show("Удалить книгу \"" + book.Title + "\"?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;
        db.Books.Remove(book);
        db.SaveChanges();
        LoadBooks();
    }

    private void ManageAuthors_Click(object sender, RoutedEventArgs e)
    {
        AuthorsWindow w = new AuthorsWindow(db);
        w.Owner = this;
        w.ShowDialog();
        db = new LibraryDbContext();
        LoadFilters();
        LoadBooks();
    }

    private void ManageGenres_Click(object sender, RoutedEventArgs e)
    {
        GenresWindow w = new GenresWindow(db);
        w.Owner = this;
        w.ShowDialog();
        db = new LibraryDbContext();
        LoadFilters();
        LoadBooks();
    }
}
