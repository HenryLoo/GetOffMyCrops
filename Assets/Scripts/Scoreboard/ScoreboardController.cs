using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour, IButtonAction
{
    // Reference to panels
    public GameObject Submission;
    public GameObject HighScores;

    // Reference to the name submission slots
    public GameObject[] NameSlots;
    public Text[] NameLetters;

    // The list of score row elements
    public ScoreboardListing[] ScoreboardListings;

    // The number of listings to show on the scoreboard
    // This should just be 10
    private const int NUM_LISTINGS = 10;
    
    // Hold the index of the currently selected name slot
    private int _selectedNameSlot;

    // Hold the letter indexes of each name slot
    private int[] _slotLetters;

    // Name slot constants
    private const float SELECTED_SLOT_ALPHA = 1;
    private const float DESELECTED_SLOT_ALPHA = 0.5f;
    private const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

    // Submitted score colour
    private readonly Color SUBMITTED_SCORE_COLOUR = new Color( 0, 0.75f, 1 );

    GameData _data;

    // Use this for initialization
    void Start()
    {
        // Get all existing scores
        _data = SaveDataController.GetInstance().LoadData();

        // Handle score submit
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            // Hide the high scores
            HighScores.SetActive( false );

            // Enable name submission
            _selectedNameSlot = 0;
            _slotLetters = new int[ NameSlots.Length ];
            UpdateNameSlots();
        }
        else
        {
            // Hide the name submission
            Submission.SetActive( false );

            // Display all the scores
            UpdateScoreboard();
        }

        // Press any key to return to main menu
        GameInput.AttachInput(
            actionClick: OnButtonClickAction,
            backClick: OnButtonClickBack,
            leftClick: OnButtonClickLeft,
            rightClick: OnButtonClickRight,
            downClick: OnButtonClickDown,
            upClick: OnButtonClickUp );
    }

    // Update the selected states of each slot
    private void UpdateNameSlots()
    {
        for( int i = 0; i < NameSlots.Length; ++i )
        {
            GameObject slot = NameSlots[ i ];
            Image slotImage = slot.GetComponent<Image>();
            float alpha = ( i == _selectedNameSlot ) ? SELECTED_SLOT_ALPHA : 
                DESELECTED_SLOT_ALPHA;
            slotImage.color = new Color( slotImage.color.r, slotImage.color.g, 
                slotImage.color.b, alpha );
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameInput.UpdateInput();
    }

    // Update the scoreboard with the loaded player values
    private void UpdateScoreboard()
    {
        for( int i = 0; i < _data.HighScores.Count; ++i )
        {
            string rank = ( i + 1 ).ToString();
            ScoreboardListings[ i ].Name.text = rank + ". " + _data.HighScores[ i ].Name;
            ScoreboardListings[ i ].Score.text = _data.HighScores[ i ].Score.ToString();
        }
    }

    // Switch scenes to the main menu
    private void ReturnToMainMenu()
    {
        GameStateLoader.SwitchState( GameStateLoader.GAME_STATES.MAIN_MENU );
    }

    // Update the letter of a name slot
    private void UpdateSlotLetter( int letterIndex )
    {
        _slotLetters[ _selectedNameSlot ] = letterIndex;
        NameLetters[ _selectedNameSlot ].text = LETTERS[ letterIndex ].ToString();
    }

    public void OnButtonClickUp()
    {
        // If submitting name, move to previous letter
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            int letterIndex = _slotLetters[ _selectedNameSlot ] - 1;
            if( letterIndex < 0 ) letterIndex = LETTERS.Length - 1;
            UpdateSlotLetter( letterIndex );
            return;
        }

        ReturnToMainMenu();
    }

    public void OnButtonClickDown()
    {
        // If submitting name, move to next letter
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            int letterIndex = _slotLetters[ _selectedNameSlot ] + 1;
            if( letterIndex >= LETTERS.Length ) letterIndex = 0;
            UpdateSlotLetter( letterIndex );
            return;
        }

        ReturnToMainMenu();
    }

    public void OnButtonClickLeft()
    {
        // If submitting name, move one slot to the left
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            _selectedNameSlot = Mathf.Max( 0, _selectedNameSlot - 1 );
            UpdateNameSlots();
            return;
        }

        ReturnToMainMenu();
    }

    public void OnButtonClickRight()
    {
        // If submitting name, move one slot to the right
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            _selectedNameSlot = Mathf.Min( NameSlots.Length - 1, _selectedNameSlot + 1 );
            UpdateNameSlots();
            return;
        }

        ReturnToMainMenu();
    }

    public void OnButtonClickAction()
    {
        // If submitting name, submit the score
        if( SaveDataController.GetInstance().IsSubmitting )
        {
            int rank = SubmitScore();

            // Enable the high scores
            Submission.SetActive( false );
            HighScores.SetActive( true );

            // If submitted score was high enough to be on high scores,
            // then highlight it
            if( rank < NUM_LISTINGS )
            {
                ScoreboardListings[ rank ].Name.color = SUBMITTED_SCORE_COLOUR;
                ScoreboardListings[ rank ].Score.color = SUBMITTED_SCORE_COLOUR;
            }

            // Update the scoreboard to show the newly submitted score
            UpdateScoreboard();

            return;
        }

        ReturnToMainMenu();
    }

    public void OnButtonClickSkill()
    {
        if( !SaveDataController.GetInstance().IsSubmitting ) ReturnToMainMenu();
    }

    public void OnButtonClickBack()
    {
        if( !SaveDataController.GetInstance().IsSubmitting ) ReturnToMainMenu();
    }

    private void SaveScores()
    {
        //_data.HighScores = serialized;
        SaveDataController save = SaveDataController.GetInstance();
        save.SaveData( _data );
        save.SaveDataToDisk();
    }

    private string GetSubmittedName()
    {
        string name = "";
        foreach( Text letter in NameLetters )
        {
            name += letter.text;
        }

        return name;
    }

    // Submit the score and return the index of the submitted score
    // This index corresponds to the "rank" of the score in the high scores list
    private int SubmitScore()
    {
        ListingValues listing;
        listing.Name = GetSubmittedName();
        listing.Score = SaveDataController.GetInstance().ScoreToSubmit;

        // Insert this score in appropriate order
        bool isInserted = false;
        int rank = 0;
        for( int i = 0; i < _data.HighScores.Count; ++i )
        {
            if( listing.Score > _data.HighScores[ i ].Score )
            {
                _data.HighScores.Insert( i, listing );
                isInserted = true;
                rank = i;
                break;
            }
        }

        // If the listing hasn't been added yet, just push to the back of 
        // the list
        if( !isInserted )
        {
            _data.HighScores.Add( listing );
            rank = _data.HighScores.Count - 1;
        }

        // Remove the lowest score if there too many listings
        if( _data.HighScores.Count > NUM_LISTINGS )
        {
            _data.HighScores.RemoveAt( _data.HighScores.Count - 1 );
        }

        // Save the score using SaveDataController
        SaveScores();

        // Done with submit request
        SaveDataController.GetInstance().IsSubmitting = false;

        return rank;
    }
}
