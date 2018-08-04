using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegmentController : MonoBehaviour
{
    [HideInInspector]
    public int Index;

    [HideInInspector]
    public SnakeController SnakeController;

    public GameObject ModelRoot;

    public float TailStartT;
    public AnimationCurve TailSizeFalloffCurve;

    private Vector3 _targetScale;
    public float ShrinkingSpeed;

    private SphereCollider _collider;

    private float _spawnTimer = 3f;

    [HideInInspector]
    public bool Alive;

    void Awake()
    {
        ModelRoot.transform.localScale = Vector3.zero;
        Alive = true;
        _collider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (_spawnTimer > 0)
        {
            _spawnTimer -= dt;
        }
        else
        {
            _collider.enabled = true;
        }

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
            _targetScale = Vector3.one;
            ModelRoot.transform.localScale = Vector3.one;
            return;
        }

        // Animate the scale
        if (_targetScale.magnitude < ModelRoot.transform.localScale.magnitude)
        {
            var targetScale = ModelRoot.transform.localScale.x - ShrinkingSpeed * dt;
            targetScale = Mathf.Clamp(targetScale, 0f, 1f);
            ModelRoot.transform.localScale = Vector3.one * targetScale;
        }

        if (!Alive && ModelRoot.transform.localScale.magnitude < 0.01f)
        {
            gameObject.SetActive(false);
            Alive = true;
            ModelRoot.transform.localScale = Vector3.zero;
        }
    }

    public void ResetCollider()
    {
        _collider.enabled = false;
        _spawnTimer = 2f;
    }

    public void KillSegment()
    {
        Alive = false;
        _targetScale = Vector3.zero;
    }
}
