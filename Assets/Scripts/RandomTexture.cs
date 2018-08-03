using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTexture : MonoBehaviour
{
    public List<Texture> TexturePool;

    private MeshRenderer mr;

    void Awake()
    {
        mr = GetComponentInChildren<MeshRenderer>();

        var randomTexture = TexturePool[Random.Range(0, TexturePool.Count - 1)];
        mr.material.SetTexture("_MainTex", randomTexture);
    }
}
