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

    void Awake()
    {
        _rotatorController = GetComponentInChildren<Rotator>();
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (_eaten)
        {
            _eatTimer += dt;

            var t = _eatTimer / EatAnimationLength;

            if (t > 1f)
            {
                gameObject.SetActive(false);
                // Reset state variables
                _eaten = false;
                _eatTimer = 0f;
                gameObject.tag = "Collectible";
                transform.localScale = Vector3.one;
                GameManager.Instance.LevelGenerator.SpawnCollectible();
                return;
            }

            transform.position = _initialPosition + transform.up * EatYCurve.Evaluate(t);
            transform.localScale = EatScaleCurve.Evaluate(t) * Vector3.one;
        }
        else
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
        }
    }

    public void Eat()
    {
        if (_eaten) return;
        _eaten = true;
        gameObject.tag = "Untagged";
    }
}
