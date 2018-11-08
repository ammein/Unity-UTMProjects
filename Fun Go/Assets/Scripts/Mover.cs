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
    private Rigidbody rb;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    private GameObject currentObject;
    public Boundary boundary; // Call the class
    public RotationBoundary rotationBoundary;
    private float currentRotation;
    public float smoothAngle;
    public float smooth;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
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

    public void moving(Vector3 movement , Rigidbody rb , float speed)
    {
        float tiltUp = Input.GetAxis("Jump") * smoothAngle;
        Debug.Log("Running Moving function");
        // Move for each components
        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );

        //currentRotation = Mathf.Clamp(transform.rotation.y, rotationBoundary.yMin, rotationBoundary.yMax);
        //if (transform.position.y >= 0)
        //{
        //    transform.rotation = Quaternion.identity;
        //}

        Quaternion target = Quaternion.Euler(0.0f, tiltUp,0.0f);

        rb.AddForce(movement * speed);
        if (Input.GetKeyDown("space"))
        {
            rb.velocity = new Vector3(0, 50, 0);
            //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        }
    }
}
