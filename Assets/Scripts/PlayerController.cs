using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float DistanceFromGround;

    public LayerMask GroundLayerMask;

    private Vector3 _velocity;

    void Awake()
    {
        _velocity = new Vector3(1f, 0f, 0f) * Speed;

    }

    void Update()
    {
        // Sphere is always at origin
    }

    /// <summary>
    /// Returns the distance to ground at current location
    /// </summary>
    private Vector3 GroundPosition(Vector3 from)
    {
        Vector3 towardsPlanet = Vector3.zero - from;
        Ray ray = new Ray(from, towardsPlanet);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, GroundLayerMask))
        {
            return hit.point;
        }

        Debug.LogWarning("No ground under player. Shouldn't happen.");
        return Vector3.zero;
    }
}
