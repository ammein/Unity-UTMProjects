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

    void Start() {
        StartCoroutine(CloneObject());
    }

    IEnumerator CloneObject()
    {
        spawn = GameObject.Find("SpawnPlayer").gameObject;
        for (int i = 0; i <= numberSpawn; i++)
        {
            yield return new WaitForSeconds(0.2f);
            obj = Instantiate(car, spawn.transform.position, spawn.transform.rotation);
            if (i == numberSpawn)
            {
                yield break;
            }
        }
        yield return null;
    }
}
