using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{

    [SerializeField] private Transform target;
    private LineRenderer line;
    public float length;
    [SerializeField] private Vector3 initialPosition;
    private int index;
    private float movementThreshold = 0.4f;
    [SerializeField] private LayerMask wrapableLayer;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        length = 0f;
        line.positionCount = 2;
        line.SetPosition(0, initialPosition);
        index = 1;
    }
    private void FollowTarget()
    {
        if(line.GetPosition(index) == target.position){
            return;
        }
        line.SetPosition(index, target.position);
    }  

    private bool CheckForObstaclesV2(Vector2 currentPosition, int loopNodeIndex)
    {
        for (int i = index-1; i >= loopNodeIndex ; i--)
        {
            Vector2 end = line.GetPosition(i);
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, end - currentPosition, Vector2.Distance(currentPosition, end), wrapableLayer);
            // Debug.DrawRay(currentPosition, end - currentPosition, Color.red, 5f);
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
            
            // Change the color of the hit object (for example)
            Renderer renderer = hitObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red; // Change color to red (example)
            }
                return true;
            }
        }
        return false;
    }
    bool AreVector2Equal(Vector2 v1, Vector2 v2, float tolerance){
        if (v2.x < v1.x-tolerance || v2.x > v1.x+tolerance){
            return false;
        }
        if (v2.y < v1.y-tolerance || v2.y > v1.y+tolerance){
            return false;
        }
        return true;
    }  
    private void CheckLoop()
    {
        float tolerance = 0.3f;
        Vector2 position = line.GetPosition(index);
        for (int i = index-2; i >= 0 ; i--)
        {
            if(AreVector2Equal(position, line.GetPosition(i), tolerance)){
                // BUG: oggetto esterno a curva 
                if (CheckForObstaclesV2(position, i))
                {
                    // attorno ad un oggetto, non rimovere
                    Debug.Log("Loop attorno ad oggetto.");
                    return;
                }
                Debug.Log("Loop rimuovibile");
                ShortenRope(i);
                return;
            }
            
        }
    }
    private void ShortenRope(int toIndex)
    {
        Vector3[] positions = new Vector3[line.positionCount];
        line.GetPositions(positions);
        var posList = positions.ToList<Vector3>();
        posList = posList.GetRange(0, toIndex+1);
        posList.Add(target.position);
        index = toIndex+1;
        line.positionCount = posList.Count;
        line.SetPositions(posList.ToArray());
        
    }

    private bool HasMovedSignificantly(Vector3 last, Vector3 current)
    {
        return Vector3.Distance(last, current) > movementThreshold;
    }
    private void AddPosition()
    {
        Vector3 lastPosition = line.GetPosition(index-1);
        Vector3 currentPosition = line.GetPosition(index);
        if (HasMovedSignificantly(lastPosition, currentPosition))
        {
            Vector3 middlePoint = (lastPosition+currentPosition)/2;
            line.positionCount++;
            line.SetPosition(index++, middlePoint);
            line.SetPosition(index, target.position);
        }
    }
    private void CalculateLength()
    {
        var len =0f;
        for (int i = 2; i < index; i++)
        {
            len += Vector2.Distance(line.GetPosition(i), line.GetPosition(i-1));
        }
        length = len;
    }
    
    void Update()
    {
        FollowTarget();
        CheckLoop();
        AddPosition();
        CalculateLength();
    }
    
}
