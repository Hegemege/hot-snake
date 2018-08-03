using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController PlayerController;

    public float FollowPositionSmoothing;
    public float FollowRotationSmoothing;

    void Awake()
    {

    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        var targetTransform = PlayerController.CameraAnchor.transform;

        // Follow camera anchor smoothly
        transform.position = Vector3.Slerp(transform.position, targetTransform.position, FollowPositionSmoothing * dt);

        // Turn smoothly towards camera anchor
        transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, FollowRotationSmoothing * dt);
    }
}
