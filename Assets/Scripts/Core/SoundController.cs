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
    PigSpawn,
    RatScared,
    UIWin,
    UIFeedback,
}

[System.Serializable]
public class SoundMapping
{
    public SoundType Name;
    public AudioClip[] AudioClips;
}

[RequireComponent( typeof( AudioSource ) )]
public class SoundController : MonoBehaviour
{
    // List of sound mappings
    public List<SoundMapping> SoundEffectList;

    // Drag a reference to the audio source which will play the sound effects
    public AudioSource SoundSource;

    // Singleton instance
    // Allows other scripts to call functions from SoundController
    public static SoundController Instance = null;

    // The lowest a sound effect will be randomly pitched
    private readonly float LOWEST_PITCH = .95f;

    // The highest a sound effect will be randomly pitched
    private readonly float HIGHEST_PITCH = 1.05f;

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

    // Play a sound, given its type
    public static void PlaySound( SoundType type )
    {
        // Find the sound with the given name
        foreach( SoundMapping mapping in Instance.SoundEffectList )
        {
            Debug.Log( "SoundController.PlaySound(): name: " + mapping.Name +
                ", audio: " + mapping.AudioClips );
            if( mapping.Name == type && mapping.AudioClips != null &&
                mapping.AudioClips.Length > 0 )
            {
                // Audio was found, so play it
                Debug.Log( "Found audio to play: " + mapping.Name );
                Instance.RandomizeSfx( mapping.AudioClips );
                return;
            }
        }

        // No audio found
        Debug.LogError( "SoundController.PlaySound(): No audio was bound to: " + type );
    }

    // Randomly choose a sound from a set of audio clips and slightly 
    // adjust their pitch
    private void RandomizeSfx( params AudioClip[] clips )
    {
        // Generate a random number between 0 and the length of the 
        // array of clips passed in
        int randomIndex = Random.Range( 0, clips.Length );

        // Choose a random pitch to play the clip at
        float randomPitch = Random.Range( LOWEST_PITCH, HIGHEST_PITCH );

        // Set the pitch of the audio source to the randomly chosen pitch
        SoundSource.pitch = randomPitch;

        // Play the clip
        SoundSource.PlayOneShot( clips[ randomIndex ] );
    }

}
