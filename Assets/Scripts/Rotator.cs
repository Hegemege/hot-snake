using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 RotationAxis;
    public float RotationSpeed;

    private float t;

    private float _initialRotationSpeed;

    void Awake()
    {
        _initialRotationSpeed = RotationSpeed * Random.Range(0.8f, 1.2f);
    }

    void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        t += dt;

        transform.localRotation = Quaternion.AngleAxis(t * RotationSpeed, RotationAxis);
    }
}
