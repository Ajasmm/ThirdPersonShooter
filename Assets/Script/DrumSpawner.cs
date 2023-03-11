using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSpawner : MonoBehaviour
{
    [SerializeField] private Terrain terrian;
    [SerializeField] private float radius = 20F;
    [SerializeField] private float countToInstantiate = 20;

    [SerializeField] private GameObject drumPrefab;

    public List<GameObject> drumList = new List<GameObject>();

    int j = 0;

    Vector3 pointOnCircle, pointOnTerrain, instancePoint;
    private void OnEnable()
    {
        StartCoroutine(InstanciateOnRandomAPoints());
    }

    IEnumerator InstanciateOnRandomAPoints()
    {
        /*
        for(int i =0; i< countToInstantiate; i++)
        {
            if (j > 10)
            {
                j = 0;
                continue;
            }

            instancePoint = FindRandomPosOnTerrain();
            if (!InstanciateDrumAt(instancePoint))
            {
                i--;
                j++;
            }
            yield return null;
        }

        GamePlayMode gamePlayMode = LevelManager.instance.GetGamePlayMode();
        gamePlayMode.objectsToDestroy = drumList;
        */
        yield return null;
    }

    private Vector3 FindRandomPosOnTerrain()
    {
        pointOnCircle = UnityEngine.Random.insideUnitCircle * radius;
        pointOnCircle.z = pointOnCircle.y;
        pointOnCircle.y = 0;

        pointOnTerrain = transform.position + pointOnCircle;
        pointOnTerrain.y = terrian.SampleHeight(transform.position + pointOnCircle);

        return pointOnTerrain;
    }
    public bool InstanciateDrumAt(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos + (Vector3.up * 1.1F), 0.5F);
        if (colliders.Length > 0) return false;

        GameObject obj = Instantiate(drumPrefab, pos + Vector3.up, Quaternion.identity, transform);
        drumList.Add(obj);

        return true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

