using UnityEngine;

public class LastDoor : Door
{

    private AudioSource doorSound;
    private AudioSource levelComplete;
    

    private void Awake()
    {
        if (levelComplete == null || doorSound == null) {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            doorSound = audioSources[0];
            levelComplete = audioSources[1];
            
        }
    }

    protected override void MoveToNextRoom(){
        Debug.Log("Level completed!!");
        levelComplete.Play();
        doorSound.Play();
    }
}