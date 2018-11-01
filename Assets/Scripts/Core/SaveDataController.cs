using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This SINGLETON class is reposonsible for handling save data in the game
public class SaveDataController
{
    // This struct represents the dynamic fields that can be saved to the database
	// Use this struct to pass around data for consistency
    public struct DataStruct
    {
        public int TotalMoney, CurrentLevel, LevelMoney;
    }

    // The database fields that will be used as a "Key" for the data value
    private static readonly string TOTAL_MONEY = "total_money";
    private static readonly string CURRENT_LEVEL = "current_level";
    private static readonly string HIGH_SCORES = "high_scores";

    private static SaveDataController _instance = null;
    private int _totalMoney;
    private int _levelMoney;
    private int _currentLevel;
    private string _highScores;
    private bool _isSubmitting;
    private int _scoreToSubmit;

    public const int NUM_LEVELS = 2;

    private SaveDataController()
    {
        _totalMoney = 0;
        _levelMoney = 0;
        _currentLevel = 1;

        ValidateDB();
    }

    // Return the instance of the controller
    // Use this to get access to the controller
    public static SaveDataController GetInstance()
    {
        if( _instance == null )
        {
            _instance = new SaveDataController();
        }

        return _instance;
    }

    // This function saves the passed state to the database
    public void SaveDataSnapshot( DataStruct data )
    {
        LevelMoney = data.LevelMoney;
        TotalMoney += LevelMoney;
        CurrentLevel = data.CurrentLevel;

        SaveDataToDisk();
    }

    // This function will return the current state of the database
    // Use this anywhere where you will need to get the data
    public DataStruct GetDataSnapshot()
    {
        DataStruct data;
        data.TotalMoney = PlayerPrefs.GetInt( TOTAL_MONEY );
        data.CurrentLevel = PlayerPrefs.GetInt( CURRENT_LEVEL );
        data.LevelMoney = LevelMoney;

        return data;
    }

    // This function validates the database at each init of SaveDataController
    // If the requested data doesn't exist, create a new data set 
    // and save it to the disk
    private void ValidateDB()
    {
        bool isValid = true;
        if( PlayerPrefs.HasKey( TOTAL_MONEY ) )
        {
            TotalMoney = PlayerPrefs.GetInt( TOTAL_MONEY );
        }
        else
        {
            isValid = false;
            TotalMoney = 0;
        }
        if( PlayerPrefs.HasKey( CURRENT_LEVEL ) )
        {
            CurrentLevel = PlayerPrefs.GetInt( CURRENT_LEVEL );
        }
        else
        {
            isValid = false;
            CurrentLevel = 1;
        }

        if( PlayerPrefs.HasKey( HIGH_SCORES ) )
        {
            HighScores = PlayerPrefs.GetString( HIGH_SCORES );
        }
        else
        {
            isValid = false;
            HighScores = "";
        }

        if( !isValid )
        {
            Debug.Log( "Data not found, Creating new dataset and saving data" );
            PlayerPrefs.SetInt( TOTAL_MONEY, 0 );
            PlayerPrefs.SetInt( CURRENT_LEVEL, 1 );
            PlayerPrefs.SetString( HIGH_SCORES, "" );

            SaveDataToDisk();
        }
        else
        {
            Debug.Log( "DB is good." );
        }
    }

    // This function writes the current cached data to the disk
    // It is a relatively expensive operation
    public void SaveDataToDisk()
    {
        Debug.Log( "Writing DB to disk" );
        PlayerPrefs.Save();
    }

    // Clear all data in the database
    private void ResetDataBase( bool isDeletingAll )
    {
        if( isDeletingAll ) PlayerPrefs.DeleteAll();
    }

    // Delete a row from the database given its key
    private void DeleteAKeyFromDB( string key )
    {
        PlayerPrefs.DeleteKey( key );
    }

    // Some useful member accessors
    public int TotalMoney
    {
        get
        {
            return _totalMoney;
        }
        set
        {
            if( value >= 0 )
            {
                _totalMoney = value;
                if( PlayerPrefs.HasKey( TOTAL_MONEY ) )
                {
                    PlayerPrefs.SetInt( TOTAL_MONEY, value );
                }
            }
        }
    }

    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            if( value > 0 && value <= NUM_LEVELS )
            {
                _currentLevel = value;
                if( PlayerPrefs.HasKey( CURRENT_LEVEL ) )
                {
                    PlayerPrefs.SetInt( CURRENT_LEVEL, value );
                }
            }
        }
    }

    public int LevelMoney
    {
        get
        {
            return _levelMoney;
        }
        set
        {
            if( value >= 0 )
            {
                _levelMoney = value;
            }
        }
    }

    public string HighScores
    {
        get
        {
            return _highScores;
        }
        set
        {
            _highScores = value;
            if( PlayerPrefs.HasKey( HIGH_SCORES ) )
            {
                PlayerPrefs.SetString( HIGH_SCORES, value );
            }
        }
    }

    public bool IsSubmitting
    {
        get
        {
            return _isSubmitting;
        }
        set
        {
            _isSubmitting = value;
        }
    }

    public int ScoreToSubmit
    {
        get
        {
            return _scoreToSubmit;
        }
        set
        {
            if( value >= 0 )
            {
                _scoreToSubmit = value;
            }
        }
    }
}
