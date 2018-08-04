using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SnakeController : MonoBehaviour
{
    public int InitialSegmentCount;
    public int MaxSegmentCount;
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

    private float _hitTimer;
    private float _hitTimerMax = 1f;

    public SkinnedMeshRenderer MR;
    private bool _setHotTextures = true;
    public Texture ColdTexture;
    public Texture MediumTexture;
    public Texture HotTexture;

    public int GrowthAmount;

    public int SegmentCount
    {
        get
        {
            return _segments.Count;
        }
    }

    public LinkedListNode<SnakeSegmentController> FirstSegment
    {
        get
        {
            return _segments.First;
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

        GameManager.Instance.SnakeController = this;
    }

    void Start()
    {
        _lastSegmentLocation = transform.position;
    }

    void Update()
    {
        if (!GameManager.Instance.Alive)
        {
            _animator.speed = 0f;
            return;
        }

        if (MR)
        {
            // Head coloring
            if (GameManager.Instance.HotnessLevel < 0f && _setHotTextures)
            {
                _setHotTextures = !_setHotTextures;
                MR.material.SetTexture("_MainTex2", ColdTexture);

            }
            else if (GameManager.Instance.HotnessLevel > 0f && !_setHotTextures)
            {
                _setHotTextures = !_setHotTextures;
                MR.material.SetTexture("_MainTex2", HotTexture);
            }

            var blendT = Mathf.Abs(GameManager.Instance.HotnessLevel);
            MR.material.SetFloat("_Blend", blendT);

        }

        var dt = Time.deltaTime;

        _blinkTimer += dt;
        if (_blinkTimer > _blinkTarget)
        {
            ResetBlinkTimer();
            _animator.SetTrigger("Blink");
        }

        if (_hitTimer > 0)
        {
            _hitTimer -= dt;
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
            tail.Value.ResetCollider();
            _segments.RemoveLast();
            _segments.AddFirst(tail);

            tail.Value.transform.localPosition = transform.localPosition;
            tail.Value.transform.localRotation = transform.localRotation;
            tail.Value.ModelRoot.transform.localScale = Vector3.one;
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
        if (SegmentCount > MaxSegmentCount) return;

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

        newSegment.SetActive(true);

        var segmentController = newSegment.GetComponent<SnakeSegmentController>();
        segmentController.SnakeController = this;

        _segments.AddLast(segmentController);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            Grow();

            var otherEatable = other.GetComponent<Eatable>();
            otherEatable.Eat();

            if (otherEatable.EatableType == EatableType.Hot && GameManager.Instance.HotnessLevel > 0f)
            {
                Grow();
            }

            if (otherEatable.EatableType == EatableType.Cold && GameManager.Instance.HotnessLevel < 0f)
            {
                Grow();
            }

            GameManager.Instance.ObjectEaten(otherEatable);

            _animator.SetTrigger("Eat");

            var eatPS = GameManager.Instance.EatPopcornPSPool.GetPooledObject();
            eatPS.transform.parent = transform;
            eatPS.transform.localRotation = Quaternion.identity;
            eatPS.transform.localPosition = Vector3.forward * 0.35f;
        }
        else if (other.gameObject.CompareTag("SnakeSegment"))
        {
            if (!other.enabled) return;
            if (_hitTimer > 0) return;

            // Cut the snake in half, kill the tail
            // Take the segment and all after it and kill them all
            var segmentController = other.GetComponent<SnakeSegmentController>();
            if (!segmentController.Alive) return;

            var node = _segments.First;
            for (var i = 0; i < segmentController.Index; i++)
            {
                if (node.Next == null) break;
                node = node.Next;
            }

            while (node != null)
            {
                node.Value.KillSegment();
                if (node.Next == null)
                {
                    _segments.Remove(node);
                    break;
                }

                node = node.Next;
                _segments.Remove(node.Previous);
            }

            _hitTimer = _hitTimerMax;
        }
    }

    private void Grow()
    {
        for (var i = 0; i < GrowthAmount; i++)
        {
            AddNewTailSegment();
        }
    }
}
