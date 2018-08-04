using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatPopcornPSPool : MonoBehaviour
{
    // Hacky copypaste
    private ParticleSystem _ps;

    void Awake()
    {
        _ps = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!_ps.IsAlive())
        {
            _ps.Clear();
            gameObject.SetActive(false);
            transform.parent = GameManager.Instance.EatPopcornPSPool.Container.transform;
            return;
        }

        if (!_ps.isPlaying)
        {
            _ps.Play();
        }
    }
}
