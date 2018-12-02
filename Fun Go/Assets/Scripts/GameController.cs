using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject car;
    public GameObject spawn;
    [Range(1.0f,5.0f)]
    public int numberSpawn;
    public Boundary boundary;

    // Use this for initialization
    void Start() {
        spawn = GameObject.Find("SpawnPlayer").gameObject;
        for (int i = 0; i < numberSpawn; i++)
        {
            Instantiate(car, spawn.transform.position, spawn.transform.rotation);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
