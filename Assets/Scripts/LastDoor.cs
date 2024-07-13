using UnityEngine;

public class LastDoor : Door
{
    [SerializeField] private AudioSource levelComplete;


    private new void Awake()
    {
        base.Awake();
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (levelComplete == null) {
            levelComplete = audioSources[1];   
        }
        
    }

    protected override void MoveToNextRoom(){
        Debug.Log("Level completed!!");
        levelComplete.Play();
        doorSound.Play();
        gameManager.levelCompleted();
    }

    protected override void MoveToNextRoom2()
    {
        Debug.Log("Level completed!!");
        levelComplete.Play();
        doorSound.Play();
        gameManager.levelCompleted();
    }
}