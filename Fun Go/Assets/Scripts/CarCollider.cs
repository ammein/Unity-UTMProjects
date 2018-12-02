using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Collision Enter " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreLayerCollision(9, 9);
        }
    }
}
