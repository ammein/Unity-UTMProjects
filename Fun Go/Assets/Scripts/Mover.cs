using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
    private Rigidbody[] rb;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    private GameObject currentObject;

	// Use this for initialization
	void Start () {
        rb = GetComponentsInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(x, 0.0f, z);

        // Move for each components
        foreach (Rigidbody child in rb)
        {
            child.AddForce(movement * speed);
        }
        baseObject.position = this.gameObject.transform.position;
    }
}
