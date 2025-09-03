using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //Standard Sound Effect
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Create the gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign the audioClip
        audioSource.clip = audioClip;

        //Assign volume
        audioSource.volume = volume;

        //Play the Sound
        audioSource.Play();

        //Get length of SFX Clip
        float clipLength = audioSource.clip.length;

        //Destroy the object when the clip is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    //Sound effect that selects from a range of different sounds at random
    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //Assign a random index
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    //Sound effect with a random pitch
    public void PlayRandomPitchSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        //Randomize the pitch
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }


    //Player sounds effects, same as the above sounds except they lack a transform value

    public void PlayPlayerSoundFXClip(AudioClip audioClip, float volume)
    {
        //Create the gameObject without a transform
        AudioSource audioSource = Instantiate(soundFXObject);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomPlayerSoundFXClip(AudioClip[] audioClip, float volume)
    {
        //Assign a random index
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomPitchPlayerSoundFXClip(AudioClip audioClip, float volume)
    {
        //Create the gameObject without a transform
        AudioSource audioSource = Instantiate(soundFXObject);

        audioSource.clip = audioClip;

        audioSource.volume = volume;

        //Randomize the pitch
        audioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
}
