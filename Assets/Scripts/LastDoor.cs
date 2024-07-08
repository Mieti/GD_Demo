using UnityEngine;

public class LastDoor : Door
{
    protected override void MoveToNextRoom(){
        Debug.Log("Level completed!!");
    }
}