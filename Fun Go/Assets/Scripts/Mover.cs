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
    public float yPositiveAxis;
}

[System.Serializable]
public class RotationDownBoundary
{
    public float yNegativeAxis;
}

public class Mover : MonoBehaviour {
    private Rigidbody rb;
    public bool pauseGame;
    public Transform baseObject; // To move along with the objects
    public float speed;
    public Boundary boundary; // Call the class
    public RotationUpBoundary rotationUpBoundary;
    public RotationDownBoundary rotationDownBoundary;
    public float speedAccelerate;
    private bool isGrounded = true;
    public Transform targetObject;
    private Vector3 the_return;
    private Quaternion clampRotate = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    public float timeHoldForRotation;
    public float turnRate;
    private Vector3 desiredDirection;
    private bool ranOnce = false;
    public float jumpForce;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        baseObject.position = this.gameObject.transform.position;
        baseObject.rotation = this.gameObject.transform.rotation;
        desiredDirection = transform.position - targetObject.position;
        the_return = Vector3.RotateTowards(transform.position, desiredDirection,turnRate * Time.deltaTime, 1);
    }

    // Update is called once per frame
    void FixedUpdate () {
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        if (!pauseGame)
        {
            if (!isGrounded)
            {
                Moving(Vector3.zero, rb, speed , true);
            }
            else
            {
                Moving(movement, rb, speed, false);
            }
        }
        else
        {
            rb.AddForce(Vector3.zero);
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;

        }
    }

    public void Moving(Vector3 movement, Rigidbody rb, float speed, bool stopForce)
    {
        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );

        if (!stopForce && isGrounded)
        {
            rb.AddForce(movement * speed, ForceMode.Acceleration);
        }
        if (!isGrounded)
        {
            rb.AddForce(movement * 0.0f, ForceMode.Impulse);
            StartCoroutine(RotationControl());
        }
        else if(stopForce)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump") , ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Hill"))
        {
            isGrounded = true;
            //print("On the ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Hill"))
        {
            isGrounded = false;
            //print("In the air");
        }
    }

    IEnumerator RotationControl()
    {
        if (!ranOnce)
        {
            ranOnce = true;
            yield return new WaitForSeconds(timeHoldForRotation);
            //Debug.Log("Ground ? = " + isGrounded);
            Quaternion lookAt = Quaternion.LookRotation(the_return);
            if (!(rb.transform.rotation == lookAt))
                rb.transform.rotation = lookAt;
            else
                rb.transform.rotation = Quaternion.identity;
            rb.WakeUp();
        }
        ranOnce = false;
    }
}
