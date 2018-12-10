using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour {

    public GameObject objectToDestroy;

    // Source : https://gamedevelopment.tutsplus.com/tutorials/how-to-make-an-object-shatter-into-smaller-fragments-in-unity--gamedev-11795

    private void Awake()
    {
        StartCoroutine(StartAnimate());
    }

    IEnumerator StartAnimate()
    {
        yield return null;
        while (true)
        {
            GameObject clone = Instantiate(objectToDestroy, transform.position, transform.rotation);
            Destroy(gameObject);
            yield return new WaitForSeconds(1);
            Destroy(clone);
            yield break;
        }
    }
}
