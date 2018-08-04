﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public int InitialSegmentCount;
    public float DistanceBetweenSegments;

    private LinkedList<SnakeSegmentController> _segments;
    private GameObject _segmentContainer;

    private Vector3 _lastSegmentLocation;

    [HideInInspector]
    public float NextSegmentT;

    private Animator _animator;
    public float BlinkInterval;
    public float BlinkIntervalRandomness;
    private float _blinkTimer;
    private float _blinkTarget;

    public int SegmentCount
    {
        get
        {
            return _segments.Count;
        }
    }

    void Awake()
    {
        _segments = new LinkedList<SnakeSegmentController>();
        _segmentContainer = new GameObject("Segment Container");

        for (var i = 0; i < InitialSegmentCount; i++)
        {
            AddNewTailSegment();
        }

        _animator = GetComponentInChildren<Animator>();
        ResetBlinkTimer();
    }

    void Start()
    {
        _lastSegmentLocation = transform.position;
    }

    void Update()
    {
        var dt = Time.deltaTime;

        _blinkTimer += dt;
        if (_blinkTimer > _blinkTarget)
        {
            ResetBlinkTimer();
            _animator.SetTrigger("Blink");
        }
    }

    private void ResetBlinkTimer()
    {
        _blinkTimer = 0f;
        _blinkTarget = BlinkInterval + Random.Range(-BlinkIntervalRandomness, BlinkIntervalRandomness);
    }

    void FixedUpdate()
    {
        // If head is too far away from last segment, put last tail segment at head
        var tail = _segments.Last;
        var distanceToNext = Vector3.Distance(transform.position, _lastSegmentLocation);

        NextSegmentT = Mathf.Clamp(distanceToNext / DistanceBetweenSegments, 0f, 1f);

        if (distanceToNext > DistanceBetweenSegments)
        {
            _segments.RemoveLast();
            _segments.AddFirst(tail);

            tail.Value.transform.localPosition = transform.localPosition;
            tail.Value.transform.localRotation = transform.localRotation;
            tail.Value.transform.localScale = Vector3.one;
            _lastSegmentLocation = transform.position;
        }

        LinkedListNode<SnakeSegmentController> current = null;
        for (var i = 0; i < _segments.Count; i++)
        {
            if (current == null)
            {
                current = _segments.First;
            }

            current.Value.Index = i;
            current = current.Next;
        }
    }

    public void AddNewTailSegment()
    {
        var targetTransform = transform;
        if (_segments.Count > 0)
        {
            targetTransform = _segments.Last.Value.transform;
        }

        // Spawn and initialize new segment
        var newSegment = GameManager.Instance.SnakeSegmentPool.GetPooledObject();
        newSegment.transform.parent = _segmentContainer.transform;
        newSegment.transform.position = targetTransform.position;
        newSegment.transform.localRotation = targetTransform.localRotation;
        newSegment.transform.localScale = Vector3.one;

        var segmentController = newSegment.GetComponent<SnakeSegmentController>();
        segmentController.SnakeController = this;

        _segments.AddLast(segmentController);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            AddNewTailSegment();

            var otherEatable = other.GetComponent<Eatable>();
            otherEatable.Eat();

            _animator.SetTrigger("Eat");
        }
    }
}
