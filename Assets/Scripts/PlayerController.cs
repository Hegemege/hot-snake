using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float SphereRadius;
    public float DistanceFromGround;
    public float TurningSensitivity;

    public LayerMask GroundLayerMask;

    void Awake()
    {
        // Snap to ground + offset
        SnapToGround();
        SnapRotation();
    }

    void FixedUpdate()
    {
        // Sphere is always at origin
        Move();
        SnapToGround();
    }

    private void Move()
    {
        var dt = Time.fixedDeltaTime;
        // Move along velocity vector, snap to ground at new location
        var newLocation = transform.position + transform.forward * Speed * dt;
        transform.position = newLocation;

        // Rotate so upvector is out from the planet
        SnapRotation();

        // Handle turning input 
        TurningInput();

        SnapToGround();
    }

    /// <summary>
    /// Snaps the rotation such that up vector is always pointing from the sphere
    /// </summary>
    private void SnapRotation()
    {
        var groundNormal = GameManager.Instance.GroundPosition(transform.position.normalized * SphereRadius * 2f).normalized;
        var newForward = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        transform.localRotation = Quaternion.LookRotation(newForward, groundNormal);
    }

    private void TurningInput()
    {
        var dt = Time.fixedDeltaTime;
        var horizontalInput = Input.GetAxis("Horizontal") * TurningSensitivity * dt;

        // Rotate around local Y axis based on the input
        transform.localRotation = Quaternion.AngleAxis(horizontalInput, transform.up) * transform.localRotation;
    }

    /// <summary>
    /// Snaps the object onto the sphere
    /// </summary>
    private void SnapToGround()
    {
        var groundPoint = GameManager.Instance.GroundPosition(transform.position.normalized * SphereRadius * 2f); // Elevated current ground position
        transform.position = groundPoint.normalized * (SphereRadius + DistanceFromGround);
    }
}
