using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    [Header("Get Clone Car Object")]
    public GameObject car;
    [HideInInspector]
    public GameObject spawn;
    [HideInInspector]
    public GameObject obj;
    [Header("Spawn Settings")]
    [Range(0.1f , 2.0f)]
    public float delaySpawnCar;
    [Range(1.0f, 5.0f)]
    public int numberSpawn;

    [Header("Get All Map Prefabs")]
    public GameObject[] Maps;

    private float[] mapLength;

    private float spawnPosition;
    private Quaternion spawnRotation;
    [Header("Offset Map Position on First Spawn")]
    [Range(0, -10)]
    public float offsetXMap;
    [Header("Countdown Settings")]
    [Range(0, 5)]
    public int countStart;

    void Start() {
        spawnPosition = transform.position.z + offsetXMap;
        spawnRotation = transform.rotation;
        StartCoroutine(OutputMap());
        StartCoroutine(CloneObject());
    }

    void Update()
    {
        // Update each frame for get All Map Length Value
        GetAllMapLength();
    }

    IEnumerator OutputMap()
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            Instantiate(Maps[i], new Vector3(0.0f ,0.0f ,spawnPosition) , spawnRotation);
            spawnPosition += Maps[i].transform.GetChild(0).transform.position.z;
            spawnRotation *= Maps[i].transform.GetChild(0).transform.rotation;
            yield return new WaitForSeconds(1);
            if (i == (Maps.Length - 1))
            {
                yield break;
            }
        }
        yield return null;
    }

    public float GetAllMapLength()
    {
        return spawnPosition;
    }

    IEnumerator CloneObject()
    {
        spawn = GameObject.Find("SpawnPlayer").gameObject;
        for (int i = 0; i < numberSpawn; i++)
        {
            yield return new WaitForSeconds(delaySpawnCar);
            obj = Instantiate(car, spawn.transform.position, spawn.transform.rotation);
            if (i == (numberSpawn - 1))
            {
                yield break;
            }
        }
        yield return null;
    }
}
