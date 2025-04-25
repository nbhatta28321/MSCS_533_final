using SQLite;
using LocationTrackingApp.Models;
using System.IO;

namespace LocationTrackingApp
{
    public partial class App : Application
    {
        private static SQLiteConnection _database;

        public static SQLiteConnection Database
        {
            get
            {
                if (_database == null)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "locationdata.db3");
                    _database = new SQLiteConnection(dbPath);
                    _database.CreateTable<LocationModel>();
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
