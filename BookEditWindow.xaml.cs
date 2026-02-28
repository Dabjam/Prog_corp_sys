using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement;

public partial class BookEditWindow : Window
{
    LibraryDbContext context;
    Book? editingBook;

    public BookEditWindow(LibraryDbContext db, Book? book)
    {
        InitializeComponent();
        context = db;
        editingBook = book;
        if (book != null)
            Title = "Редактировать книгу";

        context.Authors.Load();
        context.Genres.Load();
        AuthorCombo.ItemsSource = context.Authors.Local.ToList();
        GenreCombo.ItemsSource = context.Genres.Local.ToList();

        if (editingBook != null)
        {
            TitleBox.Text = editingBook.Title;
            AuthorCombo.SelectedItem = context.Authors.Local.FirstOrDefault(a => a.Id == editingBook.AuthorId);
            GenreCombo.SelectedItem = context.Genres.Local.FirstOrDefault(g => g.Id == editingBook.GenreId);
            YearBox.Text = editingBook.PublishYear.ToString();
            QuantityBox.Text = editingBook.QuantityInStock.ToString();
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show("Введите название книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        Author author = AuthorCombo.SelectedItem as Author;
        if (author == null)
        {
            MessageBox.Show("Выберите автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        Genre genre = GenreCombo.SelectedItem as Genre;
        if (genre == null)
        {
            MessageBox.Show("Выберите жанр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(YearBox.Text, out int year) || year < 1000 || year > 2100)
        {
            MessageBox.Show("Укажите год издания (1000-2100).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(QuantityBox.Text, out int qty) || qty < 0)
        {
            MessageBox.Show("Количество должно быть не меньше 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (editingBook != null)
        {
            editingBook.Title = TitleBox.Text.Trim();
            editingBook.AuthorId = author.Id;
            editingBook.GenreId = genre.Id;
            editingBook.PublishYear = year;
            editingBook.QuantityInStock = qty;
        }
        else
        {
            context.Books.Add(new Book
            {
                Title = TitleBox.Text.Trim(),
                AuthorId = author.Id,
                GenreId = genre.Id,
                PublishYear = year,
                QuantityInStock = qty
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
