using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeDeathController : MonoBehaviour
{
    public GameObject ModelRoot;
    public GameObject CameraAnchor;

    public bool ScaleOnDeath;
    private bool _dying;

    public void Death()
    {
        _dying = true;
        StartCoroutine(DeactivateModel());

        var particlePool = GameManager.Instance.HotnessLevel < 0 ? GameManager.Instance.SnowFlakePSPool : GameManager.Instance.SmokePSPool;
        var particles = particlePool.GetPooledObject();

        particles.transform.position = transform.position;
        particles.transform.rotation = transform.rotation;
    }

    void Update()
    {
        if (_dying && ScaleOnDeath)
        {
            ModelRoot.transform.localScale -= Time.deltaTime * Vector3.one * 2f;
        }
    }

    private IEnumerator DeactivateModel()
    {
        yield return new WaitForSeconds(0.5f);
        ModelRoot.SetActive(false);
        ModelRoot.transform.localScale = Vector3.one;
    }
}
