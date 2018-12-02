using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Debug.LogWarning("Collision Enter " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.transform.position.z >= 20.0f)
            {
                Debug.LogWarning("GameObject Position : " + collision.gameObject.transform.position.z);
                Physics.IgnoreLayerCollision(9, 9);
                return;
            }
            return;
        }
    }
}
