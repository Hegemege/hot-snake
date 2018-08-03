using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Trees;

    public int TreeCount;
    public float TreeSpawnProximityThreshold;

    void Awake()
    {
        for (var i = 0; i < TreeCount; i++)
        {
            SpawnTree();
        }
    }

    private void SpawnTree()
    {
        var tree = GameManager.Instance.TreePool.GetPooledObject();

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

            for (var i = 0; i < Trees.Count; i++)
            {
                var distance = Vector3.Distance(Trees[i].transform.position, point);
                if (distance < TreeSpawnProximityThreshold)
                {
                    doContinue = true;
                    break;
                }
            }

            if (Vector3.Distance(GameManager.Instance.PlayerRef.transform.position, point) < 3f)
            {
                doContinue = true;
            }

            if (doContinue) continue;

            break;
        }

        tree.transform.localPosition = point;

        var normal = point.normalized;
        var cross = Vector3.Cross(normal, Random.onUnitSphere);
        tree.transform.localRotation = Quaternion.LookRotation(cross, normal);

        Trees.Add(tree);
    }
}
