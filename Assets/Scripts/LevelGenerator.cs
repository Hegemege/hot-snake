using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Obstacles;

    public int TreeCount;
    public int RockCount;
    public int StoneCount;
    public int MushroomCount;
    public float ObstacleProximityThreshold;
    public float PlayerProximityThreshold;

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

        for (var i = 0; i < StoneCount; i++)
        {
            SpawnStone();
        }

        for (var i = 0; i < MushroomCount; i++)
        {
            SpawnMushroom();
        }

        for (var i = 0; i < InitialPickupCount; i++)
        {
            SpawnCollectible();
        }
    }

    public void SpawnCollectible()
    {
        var randomCollectiblePool = GameManager.Instance.CollectiblePools[Random.Range(0, GameManager.Instance.CollectiblePools.Count)];
        var randomCollectible = randomCollectiblePool.GetPooledObject();

        // Place it somewhere, not too close to trees or the player
        var point = GetRandomObstacleSpawnPosition(0.5f, 0.5f);
        randomCollectible.transform.position = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        randomCollectible.transform.localRotation = Quaternion.LookRotation(cross, normal);

        randomCollectible.SetActive(true);
    }

    private void SpawnTree()
    {
        var tree = GameManager.Instance.TreePool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition(1f, 1f);
        tree.transform.localPosition = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        tree.transform.localRotation = Quaternion.LookRotation(cross, normal);

        Obstacles.Add(tree);
    }

    private void SpawnRock()
    {
        var rock = GameManager.Instance.RockPool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition(0.5f, 1f);
        rock.transform.localPosition = point;

        rock.transform.localRotation = Random.rotation;
        rock.transform.localScale = Random.Range(0.8f, 1f) * Vector3.one;

        Obstacles.Add(rock);
    }

    private void SpawnStone()
    {
        var stone = GameManager.Instance.StonePool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition(0.3f, 0f);
        stone.transform.localPosition = point;

        var normal = point.normalized * (Random.Range(0f, 1f) > 0.5f ? 1 : -1);
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        stone.transform.localRotation = Quaternion.LookRotation(cross, normal);
        stone.transform.localScale = Random.Range(0.7f, 1.2f) * Vector3.one;
    }

    private void SpawnMushroom()
    {
        var mushroom = GameManager.Instance.MushroomPool.GetPooledObject();

        var point = GetRandomObstacleSpawnPosition(0.3f, 0f);
        mushroom.transform.localPosition = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        mushroom.transform.localRotation = Quaternion.LookRotation(cross, normal);
        mushroom.transform.localScale = Random.Range(0.7f, 1.2f) * Vector3.one;
    }

    private Vector3 GetRandomObstacleSpawnPosition(float proximityScale = 1f, float playerProximityScale = 1f)
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
                if (distance < ObstacleProximityThreshold * proximityScale)
                {
                    doContinue = true;
                    break;
                }
            }

            if (Vector3.Distance(GameManager.Instance.PlayerRef.transform.position, point) < PlayerProximityThreshold * playerProximityScale)
            {
                doContinue = true;
            }

            if (doContinue) continue;

            break;
        }

        return point;
    }
}
