using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class HiddenPole : MonoBehaviour
{
    GameObject[] correctPoles;
    GameObject[] wrongPoles;
    private int correctPoleCnt = 0;
    private int wrongPoleCnt = 0;

    private void Start()
    {
        //gameObject.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;


        correctPoles = GameObject.FindGameObjectsWithTag("Correct Pole");
        wrongPoles = GameObject.FindGameObjectsWithTag("Wrong Pole");
        correctPoleCnt = correctPoles.Length;
        wrongPoleCnt = wrongPoles.Length;
    }

    private void Update()
    {
        int correctCounter = 0;
        int wrongCounter = 0;
        for (int i = 0; i < correctPoleCnt; i++)
        {
            if (correctPoles[i].GetComponent<PoleCollision>().active)
            {
                correctCounter++;
            }
        }
        for (int i = 0; i < wrongPoleCnt; i++)
        {
            if (wrongPoles[i].GetComponent<PoleCollision>().active)
            {
                wrongCounter++;
            }
        }

        print("Correct " + correctCounter);
        print("Wrong " + wrongCounter);

        if (correctCounter == correctPoleCnt && wrongCounter == 0)
        {
            //gameObject.SetActive(true);
            //GetComponent<Renderer>().material.color = Color.green;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<Collider>().enabled = true;
        }
        else
        {
            //gameObject.SetActive(false);
            //GetComponent<Renderer>().material.color = Color.white;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Collider>().enabled = false;

        }
    }

}
