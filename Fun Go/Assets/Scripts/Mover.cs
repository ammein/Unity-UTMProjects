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
    public float yMin, yMax;
}

public class Mover : MonoBehaviour {
    private Rigidbody[] rb;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    private GameObject currentObject;
    public Boundary boundary; // Call the class
    public RotationBoundary rotationBoundary;
    private float currentRotation;

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
        currentRotation = Mathf.Clamp(transform.rotation.y, rotationBoundary.yMin, rotationBoundary.yMax);
        if (transform.position.y >= 0)
        {
            transform.rotation = Quaternion.identity * Quaternion.AngleAxis(currentRotation, transform.right);
        }

        //Debug.Log("Current Rotation : " + transform.rotation);
    }
}
