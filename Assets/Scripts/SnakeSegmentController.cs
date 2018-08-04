using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegmentController : MonoBehaviour
{
    [HideInInspector]
    public int Index;

    [HideInInspector]
    public SnakeController SnakeController;

    public float TailStartT;
    public AnimationCurve TailSizeFalloffCurve;

    private Vector3 _targetScale;
    public float ShrinkingSpeed;

    void Awake()
    {

    }

    void Update()
    {
        var dt = Time.deltaTime;

        // Update the scale of the tail segment based on location in tail
        var totalSegments = SnakeController.SegmentCount;
        var indexT = (float)Index / (totalSegments - 1);

        if (indexT > TailStartT)
        {
            var segmentSizeT = 1f / totalSegments;

            var sizeT = indexT.Remap(TailStartT, 1f, 0f, 1f);
            var curveScalingValue = Mathf.Clamp(TailSizeFalloffCurve.Evaluate(sizeT + segmentSizeT * SnakeController.NextSegmentT), 0f, 1f);

            _targetScale = Vector3.one * curveScalingValue;
        }
        else
        {
            return;
        }

        // Animate the scale
        if (_targetScale.magnitude < transform.localScale.magnitude)
        {
            var targetScale = transform.localScale.x - ShrinkingSpeed * dt;
            targetScale = Mathf.Clamp(targetScale, 0f, 1f);
            transform.localScale = Vector3.one * targetScale;
        }

    }
}
