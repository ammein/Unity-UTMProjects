using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Make it enable on unity to serialize
public class Boundary
{
    public float zMin, zMax, yMin, yMax;
}

[System.Serializable]
public class RotationBoundary
{
    public float xMin , xMax, yMin , yMax, zMin , zMax;
}

public class Mover : MonoBehaviour {
    private Rigidbody[] rb;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    private GameObject currentObject;
    public Boundary boundary; // Call the class
    public RotationBoundary rotationBoundary;

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
        float z = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(0.0f, 0.0f, z);
        moving(movement , rb , speed);
    }

    public void moving(Vector3 movement , Rigidbody[] rb , float speed)
    {
        // Move for each components
        foreach (Rigidbody child in rb)
        {
            child.position = new Vector3(
                0.0f,
                Mathf.Clamp(child.position.y, boundary.yMin, boundary.yMax),
                Mathf.Clamp(child.position.z, boundary.zMin, boundary.zMax)
                );

            rotationControl(child);

            child.AddForce(movement * speed);
            if (Input.GetKeyDown("space"))
            {
                transform.Translate(Vector3.up * 260 * Time.deltaTime, Space.World);
            }
        }
    }

    void rotationControl(Rigidbody child)
    {
        if(transform.position.y >= 0)
        {
            //Debug.Log("Running Rotation Control");
            Quaternion target = Quaternion.Euler(rotationBoundary.xMax , rotationBoundary.yMax , rotationBoundary.zMax);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 0.5f);
        }

        //Debug.Log("Current Rotation : " + transform.rotation);
    }
}
