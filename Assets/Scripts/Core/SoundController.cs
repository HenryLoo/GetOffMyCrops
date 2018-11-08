using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Player_Jump,
    Player_Plant,
    Player_Scare,
    Player_Harvest,
    Rat_Spawn,
    Big_Spawn,
    Rat_Scared,
    UI_Congratulation,
    UI_Feedback,
}

[System.Serializable]
public class SoundMapping
{
    public SoundType name;
    public AudioClip[] audioClips;
}

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{

    public SoundMapping[] soundEffectList;
    
    public AudioSource efxSource;                   //Drag a reference to the audio source which will play the sound effects.
    public static SoundController instance = null;     //Allows other scripts to call functions from SoundManager.             
    private readonly float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    private readonly float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.


    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySound(SoundType audioName)
    {
        foreach (SoundMapping map in instance.soundEffectList)
        {
            Debug.Log("Search " + map.name + ":" + map.audioClips +" audios.");
            if (map.name == audioName && map.audioClips != null && map.audioClips.Length > 0)
            {
                Debug.Log("Find " + map.name + " audios. Go play");
                instance.RandomizeSfx(map.audioClips);
                return;
            }
        }
        // Exception line
        Debug.LogError("No audio was bound to " + audioName);
    }

    //Used to play single sound clips.
    protected void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }


    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    protected void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }

}
