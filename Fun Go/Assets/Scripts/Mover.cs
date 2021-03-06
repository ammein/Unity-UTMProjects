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

[RequireComponent(typeof(ResetAnimation))]
[ExecuteInEditMode]
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
    [Header("Speed in Km/Hr :")]
    [Tooltip("Speed that can be bigger than float number. Ex : 1 - 100")]
    public float speedForce;
    [Tooltip("For accelerate num (Float Applicable) : 0.5 ...")]
    public float speedAccelerate;
    [Tooltip("Max Speed Limit (Integer)")]
    public int maxSpeed;
    [Header("Jump Configuration")]
    [Tooltip("For on Jump , make realistic fall (Integer)")]
    public int jumpWeight;
    [Tooltip("This is for number of JumpForce. Try hit space")]
    public float jumpForce;
    public float delayInputPressed;
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
    [Header("Euler Angles Values(Read Only) :")]
    [SerializeField]
    private float eulerAnglesX;
    [SerializeField]
    private float eulerAnglesY;
    [SerializeField]
    private float eulerAnglesZ;
    private DetectGround detectGround;
    private Vector3 the_return;
    private Vector3 desiredDirection;
    private Quaternion reset;
    private GameObject tyreObject;
    private Rigidbody rigidBase;
    [HideInInspector]
    public double Speed;
    private bool _isJumping;
    private ResetAnimation resetScript;
    private IEnumerator resetAnimationCoroutine;
    private bool ranOnce;
    private StateCondition state;

    // For Another Script Access
    private bool isGrounded; // To assign a local bool from DetectGround


    // Use this for initialization
    void Start()
    {
        if (baseObject == null)
        {
            baseObject = gameObject.transform.Find("Base").gameObject;
        }
        tyreObject = gameObject.transform.Find("wheels").gameObject;
        rb = tyreObject.GetComponent<Rigidbody>();
        rigidBase = gameObject.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
        targetObject = GameObject.Find("/EndPosition").transform;
        detectGround = gameObject.transform.Find("wheels").GetComponent<DetectGround>();
        resetScript = gameObject.GetComponent<ResetAnimation>();
        if (detectGround)
        {
            Debug.Log("Assigning Detect Ground Script");
        }
        else
        {
            Debug.LogWarning("You did not attach Detect Ground Script in your wheels object. Please Assign now !");
            Debug.Break();
        }
        Speed = 0;
        StartCoroutine(Jump());
        resetScript.gameRunning = true;
        StartCoroutineAnimation();
    }

    private void StartCoroutineAnimation()
    {
        resetAnimationCoroutine = resetScript.UpdatePosAnimation();
        StartCoroutine(resetAnimationCoroutine);
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

        // To See Rotation Values
        eulerAnglesX = WrapAngle(rb.rotation.eulerAngles.x);
        eulerAnglesY = WrapAngle(rb.rotation.eulerAngles.y);
        eulerAnglesZ = WrapAngle(rb.rotation.eulerAngles.z);

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
        if (baseObject.transform.position.z > boundary.zMax)
        {
            resetScript.gameRunning = false;
        }
        else
        {
            resetScript.gameRunning = true;
        }
    }

    // Source : https://forum.unity.com/threads/solved-how-to-get-rotation-value-that-is-in-the-inspector.460310/
    public static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    // Source : https://forum.unity.com/threads/solved-how-to-get-rotation-value-that-is-in-the-inspector.460310/
    public static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        if (!pauseCar)
        {
            Moving(movement, rb, speedForce);
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

        // For Max Speed in Km/Hr
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed / 3.6f);
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
    }

    IEnumerator Jump()
    {
        while (true)
        {
            _isJumping = false;
            if (Input.GetKeyDown("space") && !_isJumping)
            {
                _isJumping = true;
                JumpCode();
                yield return new WaitForSeconds(delayInputPressed);
                _isJumping = false;
            }
            yield return null;
        }
    }

    public void JumpCode()
    {
        rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
    }

    void RotationControlCheck()
    {
        //Debug.Log("RB Rotation" + rb.rotation);
        //Debug.Log("Transform Rotation" + rb.transform.rotation);
        if (rb.rotation != reset)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
        }
    }
}