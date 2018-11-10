using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Make it enable on unity to serialize
public class Boundary
{
    public float zMin, zMax, yMin, yMax;
}

[System.Serializable]
public class RotationUpBoundary
{
    public float xMin, xMax , yMin , yMax , zMin , zMax;
}

[System.Serializable]
public class RotationDownBoundary
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
    public RotationUpBoundary rotationUpBoundary;
    public RotationDownBoundary rotationDownBoundary;
    public float speedAccelerate;
    private bool isGrounded;
    public Transform targetObject;
    private Vector3 relativePos;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        baseObject.position = this.gameObject.transform.position;
        baseObject.rotation = this.gameObject.transform.rotation;
        relativePos = targetObject.position - rb.transform.position;
        isGrounded = true;
    }

    // Update is called once per frame
    void FixedUpdate () {
        //float z = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        Quaternion clampRotate = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        if (!pauseGame)
        {
            if (!isGrounded)
            {
                // Reset Transform Rotation to origin while in air
                rb.transform.rotation = Quaternion.Slerp(Quaternion.identity, clampRotate, Time.deltaTime * 3.0f);
                // To Stop moving when on air
                Moving(Vector3.zero, rb, speed , true);
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
            else if (isGrounded && transform.rotation.y >= 0.2)
            {
                Debug.Log("Running at Rotation : " + transform.rotation + " ,And it should apply the clampRotate");
                rb.transform.rotation = Quaternion.Slerp(Quaternion.identity, clampRotate, Time.deltaTime * 3.0f);
            }
            else
            {
                Moving(movement, rb, speed , false);
            }
        }
        else
        {
            rb.AddForce(Vector3.zero);
        }
        //Debug.Log("Rotation Read : " + rb.transform.eulerAngles);
        //Debug.Log("Rotation Quaternion Read : " + Quaternion.identity);
        //Debug.Log("Velocity Read : " + rb.velocity);
    }

    public void Moving(Vector3 movement , Rigidbody rb , float speed , bool stopForce)
    {
        //float tiltUp = Input.GetAxis("Jump");
        //Debug.Log("Running Moving function");
        // Move for each components
        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );

        if (!stopForce && isGrounded)
        {
            rb.AddForce(movement * speed, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(Vector3.zero ,ForceMode.Impulse);
        }

        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * speed * Input.GetAxis("Jump"));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Plane")
        {
            isGrounded = true;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        Debug.Log(angle);
        return Mathf.Clamp(angle, min, max);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Plane")
        {
            isGrounded = false;
        }
    }
}
