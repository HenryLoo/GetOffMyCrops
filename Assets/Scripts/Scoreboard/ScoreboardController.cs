using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

public class ScoreboardController : MonoBehaviour, IButtonAction
{    
    // The list of score row elements
    public ScoreboardListing[] ScoreboardListings;

    // The list of highscores, already sorted by decreasing order
    private List<ListingValues> _listingValues;

    // The number of listings to show on the scoreboard
    // This should just be 10
    private const int NUM_LISTINGS = 10;

    // The name for empty score rows
    private const string PLACEHOLDER_LISTING = "Nobody";

    GameData _data;

    // Use this for initialization
    void Start()
    {
        _listingValues = new List<ListingValues>();

        // Get all existing scores
        LoadPlayerData();

        // Handle score submit
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            int score = SaveDataController.GetInstance().ScoreToSubmit;
            // TODO: replace the placeholder "Player" name
            ListingValues listing = new ListingValues( "Player", score );
            SubmitScore( listing );

            // Done with submit request
            SaveDataController.GetInstance().IsSubmitting = false;
        }

        // Display all the scores
        UpdateScoreboard();

        // Press any key to return to main menu
        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft,
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );
    }

    // Update is called once per frame
    void Update()
    {
        GameInput.UpdateInput();
    }

    // Load the player names and scores from the database
    private void LoadPlayerData()
    {
        // Get high scores from SaveDataController and deserialize the string
        _data = SaveDataController.GetInstance().LoadData();
        string scores = _data.HighScores;
        string[] pairs = scores.Split( ';' );
        for( int i = 0; i < NUM_LISTINGS; ++i )
        {
            // Fill in empty rows with "nobody"
            if( i >= pairs.Length || pairs[ i ] == "" )
            {
                _listingValues.Add( new ListingValues( PLACEHOLDER_LISTING, 0 ) );
                continue;
            }

            // Otherwise, actually create the row for the score
            string[] values = pairs[ i ].Split( ':' );
            int score = 0;
            int.TryParse( values[ 1 ], out score );
            _listingValues.Add( new ListingValues( values[ 0 ], score ) );
        }
    }

    // Update the scoreboard with the loaded player values
    public void UpdateScoreboard()
    {
        for( int i = 0; i < NUM_LISTINGS; ++i )
        {
            ScoreboardListings[ i ].Name.text = _listingValues[ i ].Name;
            ScoreboardListings[ i ].Score.text = _listingValues[ i ].Score.ToString();
        }
    }

    private void ReturnToMainMenu()
    {
        GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

    public void OnButtonClickUp()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickDown()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickLeft()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickRight()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickAction()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickSkill()
    {
        ReturnToMainMenu();
    }

    public void OnButtonClickBack()
    {
        ReturnToMainMenu();
    }

    private void SaveScores()
    {
        // Serialize the scores and save them
        string serialized = "";
        for( int i = 0; i < _listingValues.Count; ++i )
        {
            serialized += ( _listingValues[ i ].Name + ":" +
                _listingValues[ i ].Score.ToString() );

            if( i < _listingValues.Count - 1 )
            {
                serialized += ";";
            }
        }

        _data.HighScores = serialized;
        SaveDataController save = SaveDataController.GetInstance();
        save.SaveData( _data );
        save.SaveDataToDisk();
    }

    private void SubmitScore( ListingValues listing )
    {
        // Insert this score in appropriate order
        for( int i = 0; i < _listingValues.Count; ++i )
        {
            if( listing.Score > _listingValues[ i ].Score )
            {
                _listingValues.Insert( i, listing );
                break;
            }
        }

        // Remove the lowest score
        _listingValues.RemoveAt( _listingValues.Count - 1 );

        // Save the score using SaveDataController
        SaveScores();
    }
}
