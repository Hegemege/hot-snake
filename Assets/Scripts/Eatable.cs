using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EatableType
{
    Hot,
    Cold,
    Neutral
}

public class Eatable : MonoBehaviour
{
    public EatableType EatableType;
    public int HotnessChangePercentage;
    public int ScoreAmount;
    public float EatAnimationLength;
    public AnimationCurve EatScaleCurve;
    public AnimationCurve EatYCurve;

    private Rotator _rotatorController;

    private float _eatTimer;
    private bool _eaten;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private float _disappearTimer;
    public float DisappearTime;
    private float _disappearTime;

    private bool _spawning = true;

    void Awake()
    {
        _rotatorController = GetComponentInChildren<Rotator>();
        RerollDisappearTime();
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (_spawning)
        {
            if (transform.localScale.x < 1f)
            {
                transform.localScale += Vector3.one * dt;
            }
            else
            {
                transform.localScale = Vector3.one;
                _spawning = false;
            }
        }

        if (_eaten)
        {
            _eatTimer += dt;

            var t = _eatTimer / EatAnimationLength;

            if (t > 1f)
            {
                RemoveFromPlay();
                return;
            }

            transform.position = _initialPosition + transform.up * EatYCurve.Evaluate(t);
            transform.localScale = EatScaleCurve.Evaluate(t) * Vector3.one;
        }
        else
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;

            _disappearTimer += dt;

            if (_disappearTimer > _disappearTime - 1f)
            {
                if (transform.localScale.x > 0f)
                {
                    transform.localScale -= dt * Vector3.one * 1f;
                }
            }

            if (_disappearTimer > _disappearTime)
            {
                RemoveFromPlay();
                return;
            }
        }
    }

    private void RemoveFromPlay()
    {
        gameObject.SetActive(false);
        // Reset state variables
        _eaten = false;
        _eatTimer = 0f;
        _disappearTimer = 0f;
        gameObject.tag = "Collectible";
        transform.localScale = Vector3.zero;
        _spawning = true;
        GameManager.Instance.LevelGenerator.SpawnCollectible();

        RerollDisappearTime();
    }

    private void RerollDisappearTime()
    {
        _disappearTime = DisappearTime * Random.Range(1f, 2f);
    }

    public void Eat()
    {
        if (_eaten) return;
        _eaten = true;
        gameObject.tag = "Untagged";
    }
}
