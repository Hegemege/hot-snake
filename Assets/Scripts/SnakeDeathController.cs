using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeDeathController : MonoBehaviour
{
    public GameObject ModelRoot;
    public GameObject CameraAnchor;

    public void Death()
    {
        ModelRoot.SetActive(false);
    }
}
