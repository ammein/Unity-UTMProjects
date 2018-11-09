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
    public float xMin, xMax , yMin , yMax , zMin , zMax;
}

public class Mover : MonoBehaviour {
    private Rigidbody rb;
    public bool pauseGame;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public float rotation;
    public Boundary boundary; // Call the class
    public RotationBoundary rotationBoundary;
    private float timeCount = 0.0f;
    public float speedAccelerate;

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
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        Quaternion clampRotate = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        if (!pauseGame)
        {
            if (transform.rotation.y > 0 && transform.rotation.z > 0)
            {
                // Reset Transform Rotation to origin while in air
                rb.transform.rotation = Quaternion.Slerp(Quaternion.identity, clampRotate, Time.deltaTime);
                timeCount = timeCount + Time.deltaTime;
                // To Stop moving when on air
                moving(Vector3.zero, rb, speed , true);
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
            else
            {
                moving(movement, rb, speed , false);
            }
        }
        else
        {
            rb.AddForce(Vector3.zero);
        }
        Debug.Log("Rotation Read : " + rb.transform.rotation);
        Debug.Log("Velocity Read : " + rb.velocity);
    }

    public void moving(Vector3 movement , Rigidbody rb , float speed , bool stopForce)
    {
        float tiltUp = Input.GetAxis("Jump");
        Debug.Log("Running Moving function");
        // Move for each components
        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );

        rb.transform.rotation = new Quaternion(
            Mathf.Clamp(transform.rotation.x, rotationBoundary.xMin, rotationBoundary.xMax),
            Mathf.Clamp(transform.rotation.y, rotationBoundary.yMin, rotationBoundary.yMax),
            Mathf.Clamp(transform.rotation.z, rotationBoundary.zMin, rotationBoundary.zMax),
            1.0f
            );

        if (!stopForce)
        {
            rb.AddForce(movement * speed, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(movement * speed, ForceMode.Impulse);
        }

        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * speed * Input.GetAxis("Jump"));
        }
    }
}
