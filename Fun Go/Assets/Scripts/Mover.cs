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

[System.Serializable]
public class CarConfigurations
{
    [Tooltip("This is for stop vehicle")]
    public bool pauseCar;
    [Tooltip("Only for a car base object")]
    private GameObject baseObject; // To move along with the objects
    [Header("Speed in Km/Hr :")]
    [Tooltip("Speed that can be bigger than float number. Ex : 1 - 100")]
    public float speedForce;
    [Tooltip("For accelerate num (Float Applicable) : 0.5 ...")]
    public float speedAccelerate;
    [Tooltip("Max Speed Limit (Integer)")]
    public int maxSpeed;
    [Space(20 , order = 0)]
    [Header("Jump Configuration :")]
    [Tooltip("For on Jump , make realistic fall (Integer)")]
    public int jumpWeight;
    [Tooltip("This is for number of JumpForce. Try hit space")]
    public float jumpForce;
    public float delayInputPressed;
    [Space(20 , order = 1)]
    [Header("Rotation Controls : ", order = 3)]
    [Tooltip("This is for trigger LookAt rotation if the position is off. (Seconds)")]
    public float timeHoldForRotation;
    [Tooltip("This is for Turn Rate on LookAt rotation. Float applicable")]
    public float turnRate;
}


[RequireComponent(typeof(ResetAnimation))]
[ExecuteInEditMode]
public class Mover : MonoBehaviour
{
    [Header("Car Controls : ", order = 0)]
    public CarConfigurations carConfig;
    private Rigidbody rb;
    [Header("Boundaries : ", order = 1)]
    [Space(20, order = 0)]
    public Boundary boundary; // Call the class
    public RotationUpBoundary rotationUpBoundary;
    public RotationDownBoundary rotationDownBoundary;
    [Tooltip("For End Position. End Game Position")]
    private Transform targetObject;
    [Space(20, order = 0)]
    [Header("Euler Angles Values(Read Only) :")]
    [SerializeField]
    private float eulerAnglesX;
    [SerializeField]
    private float eulerAnglesY;
    [SerializeField]
    private float eulerAnglesZ;
    private Vector3 the_return;
    private Vector3 desiredDirection;
    private Quaternion reset;
    private GameObject tyreObject;
    private Rigidbody rigidBase;
    [HideInInspector]
    public double Speed;
    private bool _isJumping;
    private ResetAnimation resetScript;
    private bool ranOnce;
    public Car myCar;


    // Use this for initialization
    void Start()
    {
        myCar = new Car(gameObject , carConfig);
        myCar.InitStart();
        targetObject = GameObject.Find("/EndPosition").transform;
        if (myCar.detectGround)
        {
            Debug.Log("Assigning Detect Ground Script");
        }
        else
        {
            Debug.LogWarning("You did not attach Detect Ground Script in your wheels object. Please Assign now !");
            Debug.Break();
        }
        StartCoroutine(Jump());
        //StartCoroutineAnimation();
    }

    void Update()
    {
        myCar.StickBase();
        desiredDirection = transform.position - targetObject.position;
        the_return = Vector3.RotateTowards(transform.forward, desiredDirection, carConfig.turnRate * Time.deltaTime, 1);
        // Initialize and get current gameObject DetectGround script 
        // (Must onUpdate because it triggers on collision)
        myCar.DetectGround();
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
        if (!carConfig.pauseCar)
        {
            myCar.Moving(boundary);
        }
        else
        {
            rb.AddForce(Vector3.zero, ForceMode.Impulse);
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
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
                myCar.JumpNow();
                yield return new WaitForSeconds(carConfig.delayInputPressed);
                _isJumping = false;
            }
            yield return null;
        }
    }
}