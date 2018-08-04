using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float SpeedBoostModifier;
    public float LengthSpeedEffect;
    public float DistanceFromGround;
    public float TurningSensitivity;
    public float LengthTurningEffect;

    public GameObject CameraAnchor;
    public CameraFollowController CameraFollowController;

    private float _hitTimer;
    private float _hitTimerMax = 1f;

    private float _initialSpeed;
    private float _initialTurningRate;

    void Awake()
    {
        // Detach camera and give reference
        CameraFollowController.PlayerController = this;
        CameraFollowController.transform.parent = null;

        // Snap to ground + offset
        SnapToGround();
        SnapRotation();

        _initialSpeed = Speed;
        _initialTurningRate = TurningSensitivity;
    }

    void Update()
    {
        if (_hitTimer > 0)
        {
            _hitTimer -= Time.deltaTime;
        }

        Speed = _initialSpeed + SpeedBoostModifier * Mathf.Clamp(GameManager.Instance.HotnessLevel, -1f, 1f) + LengthSpeedEffect * GameManager.Instance.SnakeLengthEffect;
        TurningSensitivity = _initialTurningRate + LengthTurningEffect * GameManager.Instance.SnakeLengthEffect;
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
        var groundNormal = GameManager.Instance.GroundPosition(transform.position.normalized * GameManager.Instance.SphereRadius * 2f).normalized;
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
        var groundPoint = GameManager.Instance.GroundPosition(transform.position.normalized * GameManager.Instance.SphereRadius * 2f); // Elevated current ground position
        transform.position = groundPoint.normalized * (GameManager.Instance.SphereRadius + DistanceFromGround);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // Can't hit an obstacle immediately after
            if (_hitTimer > 0) return;

            // Take damage, flash snake, turn 90 degrees away
            // Figure out which side the tree is on
            var towardsTree = (other.transform.position - transform.position).normalized;
            var sideVector = Vector3.ProjectOnPlane(towardsTree, transform.forward);

            var side = Vector3.Dot(sideVector, transform.right) > 0 ? -1 : 1;

            var newForward = Quaternion.AngleAxis(side * 90f, transform.up) * transform.forward;
            transform.localRotation = Quaternion.LookRotation(newForward, transform.up);
            _hitTimer = _hitTimerMax;
        }
    }
}
