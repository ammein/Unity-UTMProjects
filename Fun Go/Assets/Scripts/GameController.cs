using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject car;
    [HideInInspector]
    public GameObject spawn;
    [Range(1.0f, 5.0f)]
    public int numberSpawn;
    [HideInInspector]
    public GameObject obj;
    [Range(0.1f , 2.0f)]
    public float delaySpawnCar;

    public GameObject[] Maps;

    private float[] mapLength;
    public float totalValue;

    private float spawnPosition;
    private Quaternion spawnRotation;
    [Range(0, -10)]
    public float offsetXMap;

    void Start() {
        spawnPosition = transform.position.z + offsetXMap;
        spawnRotation = transform.rotation;
        StartCoroutine(OutputMap());
        StartCoroutine(CloneObject());
    }

    void Update()
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            mapLength[i] = Maps[i].transform.GetChild(0).transform.position.z;
            totalValue += mapLength[i];
            Debug.Log("All Map Length" + mapLength);
        }
    }

    IEnumerator OutputMap()
    {
        for (int i = 0; i < Maps.Length; i++)
        {
            yield return new WaitForSeconds(1);
            Instantiate(Maps[i], new Vector3(0.0f ,0.0f ,spawnPosition) , spawnRotation);
            spawnPosition += Maps[i].transform.GetChild(0).transform.position.z;
            spawnRotation *= Maps[i].transform.GetChild(0).transform.rotation;
            if (i == (Maps.Length - 1))
            {
                yield break;
            }
        }
        yield return null;
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
