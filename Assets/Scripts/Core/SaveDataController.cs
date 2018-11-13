using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// This singleton class is responsible for handling save data in the game
public class SaveDataController
{
    private static SaveDataController _instance = null;

    // Temporary data values
    private int _levelMoney;
    private bool _isSubmitting;
    private int _scoreToSubmit;

    // Hold permanent data values
    private GameData _data;

    public const int NUM_LEVELS = 2;

    // The name of the save file
    private static readonly string SAVE_FILE = "data.sav";

    private SaveDataController()
    {
        // Set temporary data values
        _levelMoney = 0;

        // Set persistent data values
        _data = new GameData();
        LoadDataFromDisk();
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

    // Return the already loaded data file
    public GameData LoadData()
    {
        return _data;
    }

    // Load the data file from disk and return it
    public GameData LoadDataFromDisk()
    {
        if( File.Exists( Application.persistentDataPath + "/" + SAVE_FILE ) )
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open( Application.persistentDataPath + 
                "/" + SAVE_FILE, FileMode.Open, FileAccess.Read );
            _data = ( GameData ) formatter.Deserialize( stream );
            stream.Close();

            Debug.Log( "SaveDataController.LoadDataFromDisk(): Data found at: " + 
                Application.persistentDataPath + "/" + SAVE_FILE );
            return _data;
        }
        else
        {
            // Set persistent data values
            _data.TotalMoney = 0;
            _data.CurrentLevel = 1;
            _data.HighScores = "";
            SaveDataToDisk();

            Debug.Log( "SaveDataController.LoadDataFromDisk(): " +
                "No data file found, creating new one" );
        }

        return null;
    }
    
    // Saves the data, in preparation to be written to disk
    public void SaveData( GameData data )
    {
        _data = data;
    }

    // Writes the saved data to disk
    public void SaveDataToDisk()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = File.Open( Application.persistentDataPath + "/" + 
            SAVE_FILE, FileMode.OpenOrCreate );
        formatter.Serialize( stream, _data );
        stream.Close();
    }

    // Reset the current play session's data
    // Do not reset high scores
    public void ResetPlayData()
    {
        LevelMoney = 0;
        _data.TotalMoney = 0;
        _data.CurrentLevel = 1;
        SaveData( _data );
        SaveDataToDisk();
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
