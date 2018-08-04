using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public SnakeSegmentPool SnakeSegmentPool;
    public GenericObjectPool TreePool;
    public GenericObjectPool RockPool;
    public GenericObjectPool StonePool;
    public GenericObjectPool MushroomPool;
    public LayerMask GroundLayerMask;
    public List<GenericObjectPool> CollectiblePools;
    [HideInInspector]
    public LevelGenerator LevelGenerator;

    public float SphereRadius;

    public GameObject PlayerRef;

    public float HotnessRateMin;
    public float HotnessRateMax;

    [HideInInspector]
    public float HotnessLevel;

    void Awake()
    {
        // Setup singleton
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        _instance = this;

        // Initialize stuff

        // Get self-references
        LevelGenerator = GetComponent<LevelGenerator>();
    }

    void Update()
    {

    }

    public void StartLevel()
    {
        HotnessLevel = 0.5f;
    }

    /// <summary>
    /// Returns the point on the ground sphere under the given point
    /// </summary>
    public Vector3 GroundPosition(Vector3 from)
    {
        Vector3 towardsPlanet = Vector3.zero - from;
        Ray ray = new Ray(from, towardsPlanet);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, GroundLayerMask))
        {
            return hit.point;
        }

        Debug.LogWarning("No ground under player. Shouldn't happen.");
        return Vector3.zero;
    }
}