﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Make it enable on unity to serialize
public class Boundary
{
    public float xMin , xMax , yMin , yMax , zMin , zMax;
}

public class Mover : MonoBehaviour {
    private Rigidbody[] rb;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    private GameObject currentObject;
    public Boundary boundary; // Call the class

	// Use this for initialization
	void Start () {
        rb = GetComponentsInChildren<Rigidbody>();
    }

    void Update()
    {
        baseObject.position = this.gameObject.transform.position;
        baseObject.rotation = this.gameObject.transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, 0.0f, z);
        moving(movement , rb , speed);
    }

    public void moving(Vector3 movement , Rigidbody[] rb , float speed)
    {
        // Move for each components
        foreach (Rigidbody child in rb)
        {
            child.position = new Vector3(
                Mathf.Clamp(child.position.x, boundary.xMin, boundary.xMax),
                Mathf.Clamp(child.position.y, boundary.yMin, boundary.yMax),
                0.0f
                );
            child.AddForce(movement * speed);
        }
    }
}
