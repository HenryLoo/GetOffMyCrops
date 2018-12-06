using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    PlayerJump,
    PlayerPlant,
    PlayerScare,
    PlayerHarvest,
    RatSpawn,
    RatScared,
    CrowSpawn,
    CrowScared,
    PigSpawn,
    PigScared,
    CountdownBlip,
    CountdownStart,
    Interlude,
    Coin,
    UIClick,
    UIAction
}

public enum MusicType
{
    Title,
    Win,
    Lose,
    Level1,
    Level2,
    Level3,
    Level4
}

[System.Serializable]
public class SoundMapping
{
    public SoundType Name;
    public AudioClip[] AudioClips;
    public AudioSource AudioSource;
}

[System.Serializable]
public class MusicMapping
{
    public MusicType Name;
    public AudioClip AudioClip;
}

[RequireComponent( typeof( AudioSource ) )]
public class SoundController : MonoBehaviour
{
    // List of sound mappings
    public List<SoundMapping> SoundEffectList;

    // List of music mappings
    public List<MusicMapping> MusicList;

    // The audio source to play music from
    private AudioSource _musicSource;

    // Singleton instance
    // Allows other scripts to call functions from SoundController
    public static SoundController Instance = null;

    // The lowest a sound effect will be randomly pitched
    private const float LOWEST_PITCH = .95f;

    // The highest a sound effect will be randomly pitched
    private const float HIGHEST_PITCH = 1.05f;

    private static bool _isFadingOutMusic = false;
    private const float MUSIC_VOLUME = 0.6f;
    private const float MUSIC_FADE_AMOUNT = 0.5f;

    void Awake()
    {
        // Check if there is already an instance of SoundManager
        if( Instance == null )
        {
            // If not, set it to this.
            Instance = this;
        }
        // If instance already exists
        else if( Instance != this )
        {
            // Destroy this, this enforces our Singleton pattern such that 
            // there can only be one instance of SoundController
            Destroy( gameObject );
        }

        // Set SoundController to DontDestroyOnLoad so that it won't be 
        // destroyed when reloading our scene.
        DontDestroyOnLoad( gameObject );
    }

    void Start()
    {
        // Load audio sources
        _musicSource = GetComponent<AudioSource>();
        _musicSource.volume = MUSIC_VOLUME;

        foreach( SoundMapping mapping in SoundEffectList )
        {
            mapping.AudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Fade out music volume if appropriate
        if( _isFadingOutMusic )
        {
            Instance._musicSource.volume -= MUSIC_FADE_AMOUNT * Time.deltaTime;
            if( Instance._musicSource.volume <= 0 )
            {
                _isFadingOutMusic = false;
                StopMusic();
            }
        }
    }

    // Play a sound, given its type
    public static void PlaySound( SoundType type, bool isRandomPitch = true )
    {
        // Find the sound with the given name
        if ( IsInvalid() )
        {
            return;
        }
        foreach( SoundMapping mapping in Instance.SoundEffectList )
        {
            if( mapping.Name == type && mapping.AudioClips != null &&
                mapping.AudioClips.Length > 0 )
            {
                // Audio was found, so play it
                Debug.Log( "SoundController.PlaySound(): name: " + mapping.Name +
                    ", audio: " + mapping.AudioClips );
                Instance.RandomizeSfx( isRandomPitch, mapping );
                return;
            }
        }

        // No audio found
        Debug.LogError( "SoundController.PlaySound(): No audio was bound to: " + type );
    }

    // Randomly choose a sound from a set of audio clips and slightly 
    // adjust their pitch
    private void RandomizeSfx( bool isRandomPitch, SoundMapping mapping )
    {
        // Generate a random number between 0 and the length of the 
        // array of clips passed in
        int randomIndex = Random.Range( 0, mapping.AudioClips.Length );

        // Choose a random pitch to play the clip at
        float pitch = isRandomPitch ?
            Random.Range( LOWEST_PITCH, HIGHEST_PITCH ) : 1;

        // Set the pitch of the audio source to the randomly chosen pitch
        mapping.AudioSource.pitch = pitch;

        // Play the clip
        mapping.AudioSource.clip = mapping.AudioClips[ randomIndex ];
        mapping.AudioSource.Play();
    }

    // Play a music track if it is not already playing
    public static void PlayMusic( MusicType type, bool isResetting = false )
    {
        if ( IsInvalid() )
        {
            return;
        }
        // Find the sound with the given name
        foreach( MusicMapping mapping in Instance.MusicList )
        {
            if( mapping.Name == type && mapping.AudioClip != null &&
                ( Instance._musicSource.clip != mapping.AudioClip || isResetting ) )
            {
                // Audio was found, so play it
                Debug.Log( "SoundController.PlayMusic(): name: " + mapping.Name +
                    ", audio: " + mapping.AudioClip );
                Instance._musicSource.clip = mapping.AudioClip;
                Instance._musicSource.Play();

                // Reset music volume
                _isFadingOutMusic = false;
                Instance._musicSource.volume = MUSIC_VOLUME;
                return;
            }
        }
    }

    // Stop the current music track
    public static void StopMusic()
    {
        if ( IsInvalid() )
        {
            return;
        }
        Instance._musicSource.Stop();
    }

    public static void FadeOutMusic()
    {
        _isFadingOutMusic = true;
    }

    private static bool IsInvalid()
    {
        return Instance == null;
    }
}
