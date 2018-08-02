using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public GameObject SegmentContainer;
    public int InitialSegmentCount;
    public float DistanceBetweenSegments;

    private int _segmentCount;
    private List<GameObject> _segments;

    void Awake()
    {
        _segmentCount = InitialSegmentCount;

        for (var i = 0; i < InitialSegmentCount; i++)
        {
            // Spawn and initialize new segment
            var newSegment = GameManager.Instance.SnakeSegmentPool.GetPooledObject();
            newSegment.transform.parent = SegmentContainer.transform.parent;
            newSegment.transform.localPosition = Vector3.zero;
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
            // Move towards previous segment, snap to max distance if too far

        }
    }
}
