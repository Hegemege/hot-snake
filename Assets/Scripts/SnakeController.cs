using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public int InitialSegmentCount;
    public float DistanceBetweenSegments;

    private int _segmentCount;
    private List<GameObject> _segments;
    private GameObject _segmentContainer;

    void Awake()
    {
        _segmentCount = InitialSegmentCount;
        _segments = new List<GameObject>();

        _segmentContainer = new GameObject("Segment Container");

        for (var i = 0; i < InitialSegmentCount; i++)
        {
            // Spawn and initialize new segment
            var newSegment = GameManager.Instance.SnakeSegmentPool.GetPooledObject();
            newSegment.transform.parent = _segmentContainer.transform;
            newSegment.transform.position = transform.position;
            newSegment.transform.localRotation = Quaternion.identity;
            newSegment.transform.localScale = Vector3.one;

            _segments.Add(newSegment);
        }
    }

    void FixedUpdate()
    {
        // Update the location of all segments
        // Go from last to first
        for (var i = _segmentCount - 1; i >= 0; i--)
        {
            Transform current = _segments[i].transform;
            Transform previous;

            if (i == 0)
            {
                previous = transform;
            }
            else
            {
                previous = _segments[i - 1].transform;
            }

            // Move towards previous segment, snap to max distance if too far
            var distance = Vector3.Distance(current.position, previous.position);

            // Previous segment hasn't moved far enough yet
            if (distance < DistanceBetweenSegments)
            {
                continue;
            }

            // Move the segment towards the previous segment, to specified distance
            var target = previous.position - (previous.position - current.position).normalized * DistanceBetweenSegments;

            // Make sure target is always at ground
            var ground = GameManager.Instance.GroundPosition(target * 2f);

            current.position = ground;
        }
    }
}
