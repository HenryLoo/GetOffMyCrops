using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***
 * This SINGLETON class is reposonsible for handling save data in the game.
 * 
 * */
public class SaveDataController
{
	/***
	 * This struct represents the dynamic fields can be saved to the DataBase.
	 * Use this struct to pass around data for consistency.
	 * */
	public struct DataStruct
	{
		public int totalMoney, currenLevel, levelMoney, maxLevels;

		// initializes members of struct to a given value
		public static implicit operator DataStruct( int value )
		{
			return new DataStruct() { totalMoney = value, currenLevel = value, levelMoney = value, maxLevels = value };
		}
	}

	// the DB fields that will be used as a "Key" for the data value
	public static readonly string TOTAL_MONEY = "total_money";
	public static readonly string CURRENT_LEVEL = "current_level";
	public static readonly string MAX_LEVEL = "max_level";
	public static readonly int NUM_LEVELS = 7;

	private static SaveDataController _instance = null;
	private int _totalMoney;
	private int _levelMoney;
	private int _currentLevel;
	
	private SaveDataController()
	{
		_totalMoney = 0;
		_levelMoney = 0;
		_currentLevel = 0;

		ValidateDB();
	}
	
	// return the instance of the controller. Use this to get access to the controller.
	public static SaveDataController GetInstance()
	{
		if ( _instance == null )
		{
			_instance = new SaveDataController();
		}

		return _instance;
	}

	/***
	 * This function saves the passed state to the database
	 * */
	public void SaveDataSanpshot( DataStruct data )
	{
		LevelMoney = data.levelMoney;
		TotalMoney += LevelMoney;
		CurrentLevel = data.currenLevel;

		SaveDataToDisk();
	}

	/***
	 * This function will return the current state of the DB. Use this anywhere where you will need to get the data
	 * */
	public DataStruct GetDataSnapshot()
	{
		DataStruct data = 0;
		data.totalMoney = PlayerPrefs.GetInt( TOTAL_MONEY );
		data.currenLevel = PlayerPrefs.GetInt( CURRENT_LEVEL );
		data.levelMoney = LevelMoney;
		data.maxLevels = PlayerPrefs.GetInt( MAX_LEVEL );

		return data;
	}

	/***
	 * This function validates the DB at each init of SaveDataController.
	 * If the requested date is present, do a thumbs up!
	 * If the requested data is missing, create a new data set, and save it to the disk.
	 * */
	private void ValidateDB()
	{
		bool isValid = true;
		if ( PlayerPrefs.HasKey( TOTAL_MONEY ) )
		{
			TotalMoney = PlayerPrefs.GetInt( TOTAL_MONEY );
		}
		else
		{
			isValid = false;
			TotalMoney = 0;
		}
		if ( PlayerPrefs.HasKey( CURRENT_LEVEL ) )
		{
			CurrentLevel = PlayerPrefs.GetInt( CURRENT_LEVEL );
		}
		else
		{
			isValid = false;
			CurrentLevel = 0;
		}

		if ( !isValid )
		{
			Debug.Log( "data not found, Creating new dataset and saving data" );
			PlayerPrefs.SetInt( TOTAL_MONEY, 0 );
			PlayerPrefs.SetInt( CURRENT_LEVEL, 0 );
			PlayerPrefs.SetInt( MAX_LEVEL, NUM_LEVELS );

			SaveDataToDisk();
		}
		else
		{
			Debug.Log( "DB is good." );
		}
	}

	/***
	 * this function writes the current cached data to the disk. It is a relatively expensive operation. Use it sparringly. 
	 * */
	public void SaveDataToDisk()
	{
		Debug.Log( "Writing DB to disk" );
		PlayerPrefs.Save();
	}

	/***
	 * This function clears the database. Use is to
	 * Be very cautious to call this.
	 * */
	private void ResetDataBase( bool yesDeletEverything )
	{
		if ( yesDeletEverything ) PlayerPrefs.DeleteAll();
	}

	/***
	 * Use this to delete a key-value pair from the database
	 * */
	private void DeleteAKeyFromDB( string keyname )
	{
		PlayerPrefs.DeleteKey( keyname );
	}

	// some useful memeber accessors
	public int MaxLevel
	{
		get
		{
			return PlayerPrefs.GetInt( CURRENT_LEVEL );
		}
	}

	public int TotalMoney
	{
		get
		{
			return _totalMoney;
		}
		set
		{
			if ( value >= 0 )
			{
				_totalMoney = value;
				if ( PlayerPrefs.HasKey( TOTAL_MONEY ) )
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
			if ( value >= 0 && value <= MaxLevel )
			{
				_currentLevel = value;
				if ( PlayerPrefs.HasKey( CURRENT_LEVEL ) )
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
			if ( value >= 0 )
			{
				_levelMoney = value;
			}
		}
	}
}
