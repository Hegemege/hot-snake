using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public ParticleSystemPool EatPopcornPSPool;
    public ParticleSystemPool SnowFlakePSPool;
    public ParticleSystemPool SmokePSPool;
    public LayerMask GroundLayerMask;
    public List<GenericObjectPool> ColdCollectiblePools;
    public List<GenericObjectPool> HotCollectiblePools;
    [HideInInspector]
    public LevelGenerator LevelGenerator;

    public float SphereRadius;

    [HideInInspector]
    public GameObject PlayerRef;
    [HideInInspector]
    public SnakeController SnakeController;

    public float HotnessRateMin;
    public float HotnessRateMax;
    public int SnakeLengthOptimum;
    public int SnakeLengthNonOptimalDiff;

    public float HotnessLevel; //-1f to 1f
    public int Score;

    public Gradient SnakeSegmentColorGradient;

    public float SnakeLengthEffect;
    public bool Alive;

    public float DeathAnimationInterval;

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

    void Start()
    {
        StartLevel();
        GetPlayerRef();
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (SnakeController && Alive)
        {
            var optimumLengthDiff = SnakeController.SegmentCount - SnakeLengthOptimum;
            var lengthChangeSign = Mathf.Sign(optimumLengthDiff);

            var nonOptimalRatio = Mathf.Abs(optimumLengthDiff) / (float)SnakeLengthNonOptimalDiff;
            nonOptimalRatio = Mathf.Clamp(nonOptimalRatio, 0f, 1f);
            SnakeLengthEffect = nonOptimalRatio * lengthChangeSign; //-1f .. 1f

            var hotnessLevelSign = Mathf.Abs(HotnessLevel) < 0.001f ? 0 : Mathf.Sign(HotnessLevel);

            var heatlevelRate = Mathf.Lerp(HotnessRateMin, HotnessRateMax, (HotnessLevel + 1f) * 0.5f) * hotnessLevelSign;
            HotnessLevel += heatlevelRate * dt;

            if (Mathf.Abs(HotnessLevel) >= 1f)
            {
                Death();
            }
        }
    }

    public void StartLevel()
    {
        Alive = true;
        HotnessLevel = 0f;
    }

    private void GetPlayerRef()
    {
        SnakeController = PlayerRef != null ? PlayerRef.GetComponent<SnakeController>() : null;
    }

    public void EndLevel()
    {
        StartLevel();
        SceneManager.LoadScene("main");
        GetPlayerRef();
    }

    private void Death()
    {
        Alive = false;

        StartCoroutine(PopSnakeSegments());
    }

    private IEnumerator PopSnakeSegments()
    {
        var cameraController = SnakeController.GetComponent<PlayerController>().CameraFollowController;

        SnakeController.GetComponent<SnakeDeathController>().Death();
        yield return new WaitForSeconds(DeathAnimationInterval);
        var tail = SnakeController.FirstSegment;
        while (tail != null)
        {
            var deathController = tail.Value.GetComponent<SnakeDeathController>();
            cameraController.Target = deathController.CameraAnchor;
            deathController.Death();
            yield return new WaitForSeconds(DeathAnimationInterval);
            tail = tail.Next;
        }

        cameraController.Target = null;

        yield return new WaitForSeconds(3f);
        EndLevel();
    }

    public void ObjectEaten(Eatable eatable)
    {
        var direction = 0;
        switch (eatable.EatableType)
        {
            case EatableType.Hot:
                direction = 1;
                break;
            case EatableType.Cold:
                direction = -1;
                break;
        }

        var changePercentage = (float)eatable.HotnessChangePercentage;
        if (direction != Mathf.RoundToInt(Mathf.Sign(HotnessLevel)))
        {
            changePercentage *= 1.5f;
        }

        HotnessLevel += direction * changePercentage / 100f;
        Score += eatable.ScoreAmount;
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