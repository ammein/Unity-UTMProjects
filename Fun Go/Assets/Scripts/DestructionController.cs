using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionController : MonoBehaviour {

    public GameObject objectToDestroy;
    private GameObject clone;

    // Source : https://gamedevelopment.tutsplus.com/tutorials/how-to-make-an-object-shatter-into-smaller-fragments-in-unity--gamedev-11795
    private void Start()
    {
        Physics.IgnoreLayerCollision(10, 9);
    }

    void Update()
    {
        if(objectToDestroy != null)
        {
            Debug.Log("Objects " + objectToDestroy);
            Instantiate(objectToDestroy, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
