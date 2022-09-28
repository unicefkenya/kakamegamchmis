using System;
using Xamarin.Forms;
using MCHMIS.Mobile.Interface;
using global::SQLite;
using System.IO;

[assembly: Dependency(typeof(MCHMIS.Mobile.Droid.Helpers.SQLite))]
namespace MCHMIS.Mobile.Droid.Helpers
{
    public class SQLite : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            string sqliteFilename = "MCHMISv2018.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string path = Path.Combine(documentsPath, sqliteFilename);
            return new SQLiteConnection(path);
        }
    }
}