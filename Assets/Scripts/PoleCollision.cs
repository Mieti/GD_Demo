using UnityEngine;

public class PoleCollision : MonoBehaviour
{
    public bool active = false;
    private int colliderCount = 0;

    private AudioSource enterAudioSource; // Audio source for OnTriggerEnter2D
    private AudioSource exitAudioSource;  // Audio source for OnTriggerExit2D

    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;

    private SpriteRenderer sr;

    // Cooldown variables
    private float lastEnterSoundTime;
    private float lastExitSoundTime;
    public float enterSoundCooldown = 1f; // 1 second cooldown
    public float exitSoundCooldown = 1f;  // 1 second cooldown

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Ensure both audio sources are assigned
        if (enterAudioSource == null || exitAudioSource == null)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            if (audioSources.Length >= 2)
            {
                enterAudioSource = audioSources[0];
                exitAudioSource = audioSources[1];
            }
            else
            {
                Debug.LogError("Not enough AudioSource components found on " + gameObject.name);
            }
        }
    }

    private void Update()
    {
        if (active && colliderCount <= 0)
        {
            colliderCount = 0;
            active = false;
        }
        if (active && sr.sprite != on)
        {
            sr.sprite = on;
        }
        else if (!active && sr.sprite != off)
        {
            sr.sprite = off;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        colliderCount++;
        if (colliderCount ==1)
        {
            active = true;
            if (enterAudioSource != null)
            {
                if (Time.time - lastEnterSoundTime >= enterSoundCooldown)
                {
                    enterAudioSource.Play();
                    lastEnterSoundTime = Time.time;
                }
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        active = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        colliderCount--;
        if (colliderCount <= 0)
        {
            colliderCount = 0;
            active = false;
            if (exitAudioSource != null)
            {
                if (Time.time - lastExitSoundTime >= exitSoundCooldown)
                {
                    exitAudioSource.Play();
                    lastExitSoundTime = Time.time;
                }
            }
            
        }
    }
}
