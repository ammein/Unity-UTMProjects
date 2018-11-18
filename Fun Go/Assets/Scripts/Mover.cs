﻿using System.Collections;
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
[RequireComponent(typeof(DetectGround))]
public class Mover : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Car Controls : ", order = 0)]
    [Tooltip("This is for stop vehicle")]
    public bool pauseCar;
    [Tooltip("Only for a car base object")]
    private GameObject baseObject; // To move along with the objects
    [Tooltip("For End Position. End Game Position")]
    private Transform targetObject;
    [Tooltip("Speed that can be bigger than float number. Ex : 1 - 100")]
    public float speed;
    [Tooltip("For accelerate num (Float Applicable) : 0.5 ...")]
    public float speedAccelerate;
    [Tooltip("This is for number of JumpForce. Try hit space")]
    public float jumpForce;
    [Tooltip("Max Speed Limit (Integer)")]
    public int maxSpeed;
    [Tooltip("For on Jump , make realistic fall (Integer)")]
    public int jumpWeight;
    [Header("Boundaries : ", order = 1)]
    [Space(20, order = 0)]
    public Boundary boundary; // Call the class
    public RotationUpBoundary rotationUpBoundary;
    public RotationDownBoundary rotationDownBoundary;
    [Space(20, order = 0)]
    [Header("Rotation Controls : ", order = 3)]
    [Tooltip("This is for trigger LookAt rotation if the position is off. (Seconds)")]
    public float timeHoldForRotation;
    [Tooltip("This is for Turn Rate on LookAt rotation. Float applicable")]
    public float turnRate;
    [Tooltip("To get the Detect Ground Script")]
    public DetectGround detectGround;
    private Vector3 the_return;
    private Vector3 desiredDirection;
    private Quaternion reset;
    private GameObject tyreObject;
    private Rigidbody rigidBase;
    [HideInInspector]
    public double Speed;

    // For Another Script Access
    private bool isGrounded; // To assign a local bool from DetectGround

    // Use this for initialization
    void Start()
    {
        if (baseObject == null)
        {
            baseObject = gameObject.transform.Find("Base").gameObject;
        }else if(gameObject.GetComponent<Mover>() == null && baseObject == null)
        {
            gameObject.AddComponent<Mover>();
            baseObject = gameObject.transform.Find("Base").gameObject;
        }
        tyreObject = gameObject.transform.Find("wheels").gameObject;
        rb = tyreObject.GetComponent<Rigidbody>();
        rigidBase = gameObject.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
        targetObject = GameObject.Find("/EndPosition").transform;
        Speed = 0;
    }

    void Update()
    {
        baseObject.transform.position = tyreObject.transform.position;
        baseObject.transform.rotation = tyreObject.transform.rotation;
        desiredDirection = transform.position - targetObject.position;
        the_return = Vector3.RotateTowards(transform.forward, desiredDirection, turnRate * Time.deltaTime, 1);
        // Initialize and get current gameObject DetectGround script 
        // (Must onUpdate because it triggers on collision)
        isGrounded = detectGround.isGrounded;

        reset = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

        if (!tyreObject.activeSelf)
        {
            baseObject.SetActive(false);
        }
        else if (tyreObject.activeSelf)
        {
            baseObject.SetActive(true);
            tyreObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        if (!pauseCar)
        {
            Moving(movement, rb, speed);
        }
        else
        {
            rb.AddForce(Vector3.zero, ForceMode.Impulse);
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
        Speed = rb.velocity.magnitude * 3.6;
    }


    public void Moving(Vector3 movement, Rigidbody rb, float speed)
    {
        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );

        // For Max Speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        RotationControlCheck();
        if (isGrounded)
        {
            rb.AddForce(movement * speed, ForceMode.Acceleration);
            rigidBase.mass = 1;
            return;
        }
        else if (!isGrounded)
        {
            rigidBase.mass = jumpWeight;
            rb.AddForce(movement * 0.0f, ForceMode.Acceleration);
            return;
        }

        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
        }
    }

    //IEnumerator RotationControl()
    //{
    //    if (!ranOnce)
    //    {
    //        ranOnce = true;
    //        yield return new WaitForSeconds(timeHoldForRotation);
    //        //Debug.Log("Ground ? = " + isGrounded);
    //        Quaternion lookAt = Quaternion.LookRotation(the_return);
    //        if (rb.transform.rotation == lookAt)
    //            yield return null;
    //        rb.transform.rotation = lookAt;
    //        rb.WakeUp();
    //    }
    //    ranOnce = false;
    //}

    void RotationControlCheck()
    {
        //Debug.Log("RB Rotation" + rb.rotation);
        //Debug.Log("Transform Rotation" + rb.transform.rotation);
        if (rb.rotation != reset)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
        }
        if (rb.rotation.y < 0.5f)
        {
            rb.AddForce(Vector3.zero, ForceMode.Impulse);
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            Vector3 getPosition = rb.transform.position;
        }
    }
}