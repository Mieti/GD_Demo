using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float cameraMovementSpeed = 10.0f;
    [SerializeField] private float zOffset = -30f;

    private void FollowTarget()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, zOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraMovementSpeed);
    }

    private void Update()
    {
        FollowTarget();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
