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
    [Header("Speed in Km/Hr :")]
    [Tooltip("Speed that can be bigger than float number. Ex : 1 - 100")]
    public float speedForce;
    [Tooltip("For accelerate num (Float Applicable) : 0.5 ...")]
    [Range(0.0f , 5.0f)]
    public float speedAccelerate;
    [Tooltip("Max Speed Limit (Integer)")]
    public int maxSpeed;
    [Space(20 , order = 0)]
    [Header("Jump Configuration :")]
    [Tooltip("For on Jump , make realistic fall (Integer)")]
    [SerializeField]
    public int jumpWeight;
    [Tooltip("This is for number of JumpForce. Try hit space")]
    public float jumpForce;
    [Header("Single Player Delay Input Press :")]
    [Range(1.0f , 5.0f)]
    public float delayInputJump;
    [Header("Multiplayer Delay Input Press :")]
    [Range(0.0f , 1.0f)]
    public float delayInputJumpSecond;
    [Space(20 , order = 1)]
    [Header("Rotation Controls : ", order = 3)]
    [Tooltip("This is for trigger LookAt rotation if the position is off. (Seconds)")]
    public float timeHoldForRotation;
    [Tooltip("This is for Turn Rate on LookAt rotation. Float applicable")]
    public float turnRate;
    [Header("Respawn Settings")]
    public int NumOfBlink;
    public float blinkWait;
}


[RequireComponent(typeof(ResetAnimation))]
public class Mover : MonoBehaviour
{
    [Tooltip("This is for stop vehicle")]
    [SerializeField]
    public bool pauseCar = false;
    [Header("Car Controls : ", order = 0)]
    public CarConfigurations carConfig;
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
    private bool _isJumping , _isJumpingSecond;
    private ResetAnimation resetScript;
    private bool ranOnce;
    public Car myCar = null;
    private SingleOrMultiple play;
    private GameObject secondGameObject;
    private GameObject firstGameObject;

    private GameObject cloneSecondPlayer;

    private GameController gameController;

    [Header("Player Coin")]
    private int firstPlayerCoin = 0;
    private int secondPlayerCoin = 0;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Initialize();
        InitCoroutine();
        Physics.IgnoreLayerCollision(10, 10);
    }

    public bool UpdateRespawnFirst()
    {
        return myCar.respawnStatusFirst;
    }

    public bool UpdateRespawnSecond()
    {
        return myCar.respawnStatusSecond;
    }

    /// <summary>
    /// Made the initialize because bug always happen when switch condition SINGLE or MULTIPLE
    /// </summary>
    void Initialize()
    {
        secondGameObject = GameObject.FindGameObjectWithTag("SecondParentPlayer");
        firstGameObject = GameObject.FindGameObjectWithTag("ParentPlayer");
        play = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().play;
        myCar = new Car(gameObject, secondGameObject, play);
        myCar.InitStart();
        myCar.AssignTyreAccesories(gameController.accessory.tyre[2] , firstGameObject.tag , new Color(1,1,1,1));
        myCar.AssignFullBody(gameController.accessory.body[0], firstGameObject.tag, new Color(0.5f, 0.25f, 0.60f, 1.0f));
        myCar.AssignRandomAccessoriesClone(gameController.accessory);
        targetObject = GameObject.Find("/EndPosition").transform;
    }

    void InitCoroutine()
    {
        StartCoroutine(Jump());
        StartCoroutine(CheckPlayerStatus());
        StartCoroutine(BlinkRespawn());
    }

    void Update()
    {
        myCar.StickBase();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        firstPlayerCoin = gameController.firstCoin;
        secondPlayerCoin = gameController.secondCoin;
        myCar.SetCoin(firstPlayerCoin, secondPlayerCoin);
        //desiredDirection = transform.position - targetObject.position;
        //the_return = Vector3.RotateTowards(transform.forward, desiredDirection, carConfig.turnRate * Time.deltaTime, 1);
        // Initialize and get current gameObject DetectGround script 
        // (Must onUpdate because it triggers on collision)
        myCar.RespawnNow();
        UpdateRespawnFirst();
        UpdateRespawnSecond();
        myCar.DetectBaseGround();
        UpdatePauseCar();
        eulerAnglesX = WrapAngle(myCar.rotX);
        eulerAnglesY = WrapAngle(myCar.rotY);
        eulerAnglesZ = WrapAngle(myCar.rotZ);
        MoveOrNotMove();
        myCar.CloneJumpNow();
        myCar.GetZMax();
    }

    public void MoveOrNotMove()
    {
        if (!UpdatePauseCar())
        {
            myCar.EnableGravity();
            myCar.Moving();
        }
        else
        {
            myCar.Stop();
            myCar.DisableGravity();
        }
    }

    public bool UpdatePauseCar()
    {
        return pauseCar;
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
        MoveOrNotMove();
    }

    IEnumerator Jump()
    {
        while (true)
        {
            _isJumping = false;
            _isJumpingSecond = false;
            switch (play)
            {
                case SingleOrMultiple.SINGLE:
                    if (Input.GetKeyDown("space") && !_isJumping)
                    {
                        _isJumping = true;
                        myCar.JumpNow();
                        yield return new WaitForSeconds(carConfig.delayInputJump);
                        _isJumping = false;
                    }
                    if (myCar.CloneJumpNow() && !_isJumping)
                    {
                        _isJumping = true;
                        myCar.JumpNow();
                        yield return new WaitForSeconds(carConfig.delayInputJump);
                        myCar.cloneFlag = false;
                        _isJumping = false;
                    }
                    break;

                case SingleOrMultiple.MULTIPLE:
                    // First Player
                    if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
                    {
                        _isJumping = true;
                        myCar.JumpNow();
                        yield return new WaitForSeconds(carConfig.delayInputJumpSecond);
                        _isJumping = false;
                    }

                    // Second Player
                    if (Input.GetKeyDown(KeyCode.UpArrow) && !_isJumpingSecond)
                    {
                        _isJumpingSecond = true;
                        myCar.JumpNow();
                        yield return new WaitForSeconds(carConfig.delayInputJumpSecond);
                        _isJumpingSecond = false;
                    }
                    if (myCar.CloneJumpNow() && !_isJumping)
                    {
                        _isJumping = true;
                        myCar.JumpNow();
                        yield return new WaitForSeconds(carConfig.delayInputJump);
                        myCar.cloneFlag = false;
                        _isJumping = false;
                    }
                    break;
            }
            yield return null;
        }
    }

    IEnumerator CheckPlayerStatus()
    {
        int countdown = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().countStart;
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                if (GameObject.FindGameObjectWithTag("SecondParentPlayer").activeInHierarchy)
                {
                    GameObject.FindGameObjectWithTag("SecondParentPlayer").SetActive(false);
                    yield break;
                }
                else
                {
                    yield return null;
                }
                yield return new WaitForSeconds(countdown);
                break;

            case SingleOrMultiple.MULTIPLE:
                if (!GameObject.FindGameObjectWithTag("SecondParentPlayer").activeInHierarchy)
                {
                    GameObject.FindGameObjectWithTag("SecondParentPlayer").SetActive(true);
                    yield break;
                }
                else
                {
                    yield return null;
                }
                yield return new WaitForSeconds(countdown);
                break;
        }
        yield break;
    }

    IEnumerator BlinkRespawn()
    {
        while (true)
        {
            if (UpdateRespawnFirst())
            {
                for(int i = 0; i < carConfig.NumOfBlink; i++)
                {
                    myCar.Blink();
                    yield return new WaitForSeconds(carConfig.blinkWait);
                    myCar.UnBlink();
                    yield return new WaitForSeconds(carConfig.blinkWait);
                    if (i == (carConfig.NumOfBlink - 1))
                    {
                        myCar.respawnStatusFirst = false;
                    }
                }
            }

            if (UpdateRespawnSecond())
            {
                for (int i = 0; i < carConfig.NumOfBlink; i++)
                {
                    myCar.Blink();
                    yield return new WaitForSeconds(carConfig.blinkWait);
                    myCar.UnBlink();
                    yield return new WaitForSeconds(carConfig.blinkWait);
                    if (i == (carConfig.NumOfBlink - 1))
                    {
                        myCar.respawnStatusSecond = false;
                    }
                }
            }
            yield return null;
        }
    }
}