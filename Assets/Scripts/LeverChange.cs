using UnityEngine;
using System.Collections;

public class LeverController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public GameObject toHidePole;
    public GameObject toShowPole;
    private int colliderCount = 0;
    private bool active = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (active && colliderCount <= 0)
        {
            colliderCount = 0;
            active = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        colliderCount++;
        if (colliderCount == 1)
        {
            ActivateLever();
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
            DeactivateLever();
        }
    }

    private void ActivateLever()
    {
        active = true;
        spriteRenderer.flipX = true;
        if (toShowPole != null)
        {
            toShowPole.SetActive(true);
        }

        if (toHidePole != null)
        {
            toHidePole.SetActive(false);
        }


    }

    private void DeactivateLever()
    {
        active = false;
        spriteRenderer.flipX = false;
        if (toShowPole != null)
        {
            toShowPole.SetActive(false);
        }

        if (toHidePole != null)
        {
            toHidePole.SetActive(true);
        }



    }

}
