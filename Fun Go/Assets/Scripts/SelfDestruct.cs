using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float timeToDestroy = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().destroy.timeToDestroyAll;
        StartCoroutine(BeginDestroyCountdown(timeToDestroy));
	}

    IEnumerator BeginDestroyCountdown(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
        yield return null;
    }
}
