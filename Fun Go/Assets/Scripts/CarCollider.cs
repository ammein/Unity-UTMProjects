﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour {

    [HideInInspector]
    public bool boomFirst = false;
    [HideInInspector]
    public bool boomSecond = false;

    private GameObject destroyObject;
    private GameController gameController;

    private void Start()
    {
        destroyObject = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().destroy.destroyObject;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void BoomEffect()
    {
        Debug.Log("Run Boom !");
        GameObject destroy = Instantiate(destroyObject, transform.position, transform.rotation);
        foreach (Transform destroyChild in destroy.transform)
        {
            Debug.Log("Got Destroy Instantiate ? " + destroy);
            Debug.Log("Destroy Object " + destroyObject);
            if (destroy != null && destroyChild != null)
            {
                destroyChild.GetComponent<DestructionController>().objectToDestroy = destroyObject;
            }
            else
            {
                break;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach(string detect in gameController.destroy.detectTagToDestroy)
        {
            if (collision.gameObject.CompareTag(detect) && gameObject.transform.parent.CompareTag("ParentPlayer"))
            {
                gameObject.transform.parent.GetComponent<Mover>().myCar.getFirstBoom = true;
                BoomEffect();
            }

            if (collision.gameObject.CompareTag(detect) && gameObject.transform.parent.CompareTag("SecondParentPlayer"))
            {
                gameObject.transform.parent.GetComponent<Mover>().myCar.getSecondBoom = true;
                BoomEffect();
            }
        }
    }

    private void Update()
    {
        destroyObject = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().destroy.destroyObject;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
