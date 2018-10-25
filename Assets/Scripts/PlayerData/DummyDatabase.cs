using System.Data;

public class DummyDatabase : IDatabase
{
    private readonly int[] DUMMY_SCORES = { 300, 240, 220, 210, 200, 140, 110,
        130, 100, 100 };
    private readonly string[] DUMMY_NAMES = { "Name 1", "Name 2", "Name 3", "Name 4",
        "Name 5", "Name 6", "Name 7", "Name 8", "Name 9", "Name 10" };

    private readonly int NUM_LISTINGS = 10;

    public bool Close()
    {
        // Not a real database, so just pretend it closed properly
        return true;
    }

    public bool Connect( string dbPath )
    {
        // Not a real database, so just pretend it connected properly
        return true;
    }

    public IDataReader Query( string queryString )
    {
        // Generate a table of dummy values and return its reader
        DataTable table = new DataTable();
        DataColumn idCol = table.Columns.Add( "ID", typeof( int ) );
        table.PrimaryKey = new DataColumn[] { idCol };

        table.Columns.Add( "Name", typeof( string ) );
        table.Columns.Add( "Score", typeof( int ) );

        for( int i = 0; i < NUM_LISTINGS; ++i )
        {
            table.Rows.Add( new object[] { i + 1, DUMMY_NAMES[ i ],
                DUMMY_SCORES[ i ] } );
        }

        DataTableReader reader = table.CreateDataReader();
        return reader;
    }
}
