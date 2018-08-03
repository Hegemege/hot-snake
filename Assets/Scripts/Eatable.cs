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
}
