using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Obstacles;

    public int TreeCount;
    public int RockCount;
    public float ObstacleProximityThreshold;

    public int InitialPickupCount;

    void Awake()
    {
        for (var i = 0; i < TreeCount; i++)
        {
            SpawnTree();
        }

        for (var i = 0; i < RockCount; i++)
        {
            SpawnRock();
        }

        for (var i = 0; i < InitialPickupCount; i++)
        {
            SpawnCollectible();
        }
    }

    private void SpawnTree()
    {
        var tree = GameManager.Instance.TreePool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition();
        tree.transform.localPosition = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        tree.transform.localRotation = Quaternion.LookRotation(cross, normal);

        Obstacles.Add(tree);
    }

    private void SpawnRock()
    {
        var rock = GameManager.Instance.RockPool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition();
        rock.transform.localPosition = point;

        rock.transform.localRotation = Random.rotation;
        rock.transform.localScale = Random.Range(0.5f, 1f) * Vector3.one;

        Obstacles.Add(rock);
    }

    private Vector3 GetRandomObstacleSpawnPosition()
    {
        // Pick random point, not close to current trees
        Vector3 point = Random.onUnitSphere * GameManager.Instance.SphereRadius;
        int tryCount = 0;
        while (true)
        {
            tryCount++;
            if (tryCount > 100)
            {
                break;
            }

            point = Random.onUnitSphere * GameManager.Instance.SphereRadius;
            var doContinue = false;

            for (var i = 0; i < Obstacles.Count; i++)
            {
                var distance = Vector3.Distance(Obstacles[i].transform.position, point);
                if (distance < ObstacleProximityThreshold)
                {
                    doContinue = true;
                    break;
                }
            }

            if (Vector3.Distance(GameManager.Instance.PlayerRef.transform.position, point) < 2f)
            {
                doContinue = true;
            }

            if (doContinue) continue;

            break;
        }

        return point;
    }

    public void SpawnCollectible()
    {
        var randomCollectiblePool = GameManager.Instance.CollectiblePools[Random.Range(0, GameManager.Instance.CollectiblePools.Count)];
        var randomCollectible = randomCollectiblePool.GetPooledObject();

        // Place it somewhere, not too close to trees or the player
        Vector3 point = Random.onUnitSphere * GameManager.Instance.SphereRadius;
        int tryCount = 0;
        while (true)
        {
            tryCount++;
            if (tryCount > 100)
            {
                break;
            }

            point = Random.onUnitSphere * GameManager.Instance.SphereRadius;
            var doContinue = false;

            for (var i = 0; i < Obstacles.Count; i++)
            {
                var distance = Vector3.Distance(Obstacles[i].transform.position, point);
                if (distance < ObstacleProximityThreshold / 2f)
                {
                    doContinue = true;
                    break;
                }
            }

            if (Vector3.Distance(GameManager.Instance.PlayerRef.transform.position, point) < 2f)
            {
                doContinue = true;
            }

            if (doContinue) continue;

            break;
        }

        randomCollectible.transform.position = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        randomCollectible.transform.localRotation = Quaternion.LookRotation(cross, normal);

        randomCollectible.SetActive(true);
    }
}
