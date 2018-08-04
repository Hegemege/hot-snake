using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAlive : MonoBehaviour
{
    private static bool _spawned;

    void Awake()
    {
        if (_spawned) Destroy(gameObject);

        _spawned = true;
        DontDestroyOnLoad(gameObject);
    }
}
