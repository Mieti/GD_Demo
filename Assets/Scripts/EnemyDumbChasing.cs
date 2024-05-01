using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDumbChasing : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distancePlayerStop = 5f;
    [SerializeField] private float distanceChangeDirPattugl = 2.5f;
    
    private bool movingRight;
    private bool inizioPattugliamento;
    
    private Rigidbody2D myRigidBody;
    private Vector2 myStartingPosition;
    
    //varaibili di supporto
    private Vector2 myPosition;
    private Vector2 playerPosition;
    private float distancePlayer;
    private Vector2 directionToPlayer_norm;
    private Vector2 directionToMyStartPos_norm;

    

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = this.GetComponent<Rigidbody2D>();
        myStartingPosition = this.transform.position;
        movingRight = true;
        inizioPattugliamento = true;
    }

    void Update()
    {
    }

    
    private void FixedUpdate()
    {
        myPosition = this.transform.position;
        playerPosition = player.transform.position;
        distancePlayer = Vector2.Distance(myPosition, playerPosition);

        logicEnemy(myPosition, playerPosition, distancePlayer);
    }

    
    private void logicEnemy(Vector2 enemyPos, Vector2 playerPos, float distPlayer)
    {
        if (distPlayer < distancePlayerStop)
        {
            inizioPattugliamento = false;
            directionToPlayer_norm = (playerPos - enemyPos).normalized;
            moveEnemyTo(directionToPlayer_norm);
        }
        else
        {
            if (inizioPattugliamento)
            {
                pattugliamento();
            }
            else
            {
                directionToMyStartPos_norm = (myStartingPosition - enemyPos).normalized;
                moveEnemyTo(directionToMyStartPos_norm);
                if (Vector2.Distance(enemyPos, myStartingPosition) < 0.1f)
                {
                    inizioPattugliamento = true;
                    stopEnemy();
                }
            }
        }
    }

    private void moveEnemyTo(Vector2 direction_normalized)
    {
        myRigidBody.velocity = direction_normalized * speed;
    }

    private void stopEnemy()
    {
        myRigidBody.velocity = Vector2.zero;
    }

    private void pattugliamento()
    {
        if (movingRight)
        {
            moveEnemyTo(Vector2.right);
        }
        else
        {
            moveEnemyTo(Vector2.left);
        }

        //Controllo se il nemico ha raggiunto l'estremo e in tal caso cambia direzione
        if (myPosition.x >= myStartingPosition.x + distanceChangeDirPattugl)
        {
            movingRight = false;
        }
        else if (myPosition.x <= myStartingPosition.x - distanceChangeDirPattugl)
        {
            movingRight = true;
        }
    }
    
}
