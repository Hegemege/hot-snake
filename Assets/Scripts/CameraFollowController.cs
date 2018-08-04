using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [HideInInspector]
    public PlayerController PlayerController;

    public float FollowPositionSmoothing;
    public float FollowRotationSmoothing;

    public GameObject Target;

    void Awake()
    {
        Target = PlayerController.CameraAnchor;
    }

    void FixedUpdate()
    {
        if (!Target || !Target.activeInHierarchy) return;

        var dt = Time.fixedDeltaTime;
        var targetTransform = Target.transform;

        // Follow camera anchor smoothly
        transform.position = Vector3.Slerp(transform.position, targetTransform.position, FollowPositionSmoothing * dt);

        // Turn smoothly towards camera anchor
        transform.rotation = Quaternion.Slerp(transform.rotation, targetTransform.rotation, FollowRotationSmoothing * dt);
    }
}
