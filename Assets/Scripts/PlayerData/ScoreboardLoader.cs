using System.Data;
using UnityEngine;

public class ScoreboardLoader : MonoBehaviour
{    
    public ScoreboardListing[] ScoreboardListings;
    private ListingValues[] _listingValues;

    // The database's path and the query string to retrieve the player's values
    public IDatabase _database;
    private readonly string DATABASE_PATH = "";
    private readonly string DATABASE_QUERY = "";

    // The number of listings to show on the scoreboard
    // This should just be 10
    private readonly int NUM_LISTINGS = 10;

    // Use this for initialization
    void Start()
    {
        // Connect to the database
        _database = new DummyDatabase();
        _database.Connect( DATABASE_PATH );

        _listingValues = new ListingValues[ NUM_LISTINGS ];

        UpdateScoreboard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Call this when the scene ends
    public void CleanUp()
    {
        _database.Close();
    }

    // Load the player names and scores from the database
    private void LoadPlayerData()
    {
        IDataReader reader = _database.Query( DATABASE_QUERY );

        for( int i = 0; i < NUM_LISTINGS; ++i )
        {
            reader.Read();
            _listingValues[ i ].Name = reader.GetString( 1 );
            _listingValues[ i ].Score = reader.GetInt32( 2 );
        }

        reader.Close();
    }

    // Update the scoreboard with the loaded player values
    public void UpdateScoreboard()
    {
        LoadPlayerData();

        for( int i = 0; i < NUM_LISTINGS; ++i )
        {
            ScoreboardListings[ i ].Name.text = _listingValues[ i ].Name;
            ScoreboardListings[ i ].Score.text = _listingValues[ i ].Score.ToString();
        }
    }
}
