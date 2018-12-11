using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExplodeEffect : MonoBehaviour {

    public GameObject remains;

    // Source : https://gamedevelopment.tutsplus.com/tutorials/how-to-make-an-object-shatter-into-smaller-fragments-in-unity--gamedev-11795
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(remains, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
