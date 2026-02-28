using System.Windows;
using LibraryManagement.Data;

namespace LibraryManagement;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        LibraryDbContext db = new LibraryDbContext();
        db.Database.EnsureCreated();
    }
}
