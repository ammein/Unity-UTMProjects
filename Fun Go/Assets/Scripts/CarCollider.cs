using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour {

    [HideInInspector]
    public bool boom = false;

    private GameObject destroyObject;
    private GameController gameController;


    private void Start()
    {
        destroyObject = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().destroy.destroyObject;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StartCoroutine(RunBoom());
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach(string detect in gameController.destroy.detectTagToDestroy)
        {
            if (collision.gameObject.CompareTag(detect))
            {
                boom = true;
            }
        }
    }

    IEnumerator RunBoom()
    {
        while (true)
        {
            Debug.Log("Boom = " + boom);
            if (boom)
            {
                Debug.Log("Run Boom !");
                GameObject destroy = Instantiate(destroyObject, transform.position, transform.rotation);
                foreach (Transform destroyChild in destroy.transform)
                {
                    if (destroy != null)
                    {
                        destroyChild.GetComponent<DestructionController>().objectToDestroy = destroyObject;
                    }
                }
                yield return new WaitForSeconds(2);
            }
            else
            {
                yield return null;
            }
            yield return null;
        }
    }

    // Debug Only
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && gameObject.transform.parent.CompareTag("ParentPlayer"))
    //    {
    //        GameObject destroy = Instantiate(destroyObject, transform.position, transform.rotation);
    //        foreach(Transform destroyChild in destroy.transform)
    //        {
    //            if(destroy != null)
    //            {
    //                destroyChild.GetComponent<DestructionController>().objectToDestroy = destroyObject;
    //            }
    //        }
    //    }
    //}

}
