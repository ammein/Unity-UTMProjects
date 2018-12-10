using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour {

    [HideInInspector]
    public bool boom = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hill"))
        {
            GameObject destroy = Instantiate(gameObject.transform.parent.GetComponent<Mover>().destroyObject, transform.position, transform.rotation);
            gameObject.transform.parent.transform.parent = destroy.transform;
            foreach(Transform destroyChildren in destroy.transform)
            {
                destroyChildren.GetComponent<DestructionController>().objectToDestroy = destroy;
            }
            boom = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hill"))
        {
            boom = false;
        }
    }
}
