using SQLite;

namespace MCHMIS.Mobile.Interface
{

    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
