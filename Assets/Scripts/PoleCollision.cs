using UnityEngine;

public class PoleCollision : MonoBehaviour
{
    // variable used by external objects to trigger events
    public bool active = false;
    private int colliderCount = 0;

    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if(active && colliderCount <= 0){
            colliderCount = 0;
            active = false;
        }
        if(active && sr.sprite != on){
            sr.sprite = on;
        }
        else if(!active && sr.sprite != off)
        {
            sr.sprite = off;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        colliderCount++;
        if (colliderCount > 0)
        {
            active = true;        }
        //ChangeColorIfNeeded();
    }
    // Maybe not needed with the counter
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
        }
    }

}
