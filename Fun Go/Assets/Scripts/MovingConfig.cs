using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public float speedForce, maxSpeed, speedAccelerate, jumpWeight, turnRate, jumpForce, rotX, rotY, rotZ;
    public float speedForceSecond, maxSpeedSecond, speedAccelerateSecond, jumpWeightSecond, turnRateSecond, jumpForceSecond, rotXSecond, rotYSecond, rotZSecond;
    public GameObject carObject, baseObject, tyreObject , carObjectSecond , baseObjectSecond , tyreObjectSecond;
    private Rigidbody rb , rigidBase , rbSecond , rigidBaseSecond;
    public double Speed , SpeedSecond;
    public ResetAnimation resetScript;
    public ResetAnimation resetScriptSecond;
    public bool isGrounded , isBaseGrounded , isGroundedSecond , isBaseGroundedSecond;
    public DetectGround baseGrounded;
    public DetectGround baseGroundedSecond;
    public DetectGround detectGround;
    public DetectGround detectGroundSecond;
    private float timer = 0;
    private float countdown = 0;
    public Boundary boundary;
    public Boundary boundarySecond;
    [HideInInspector]
    public bool cloneFlag = false;
    private SingleOrMultiple playerDouble;

    public Car(GameObject gameObject)
    {
        carObject = gameObject;
        speedForce = carObject.GetComponent<Mover>().carConfig.speedForce;
        speedAccelerate = carObject.GetComponent<Mover>().carConfig.speedAccelerate;
        maxSpeed = carObject.GetComponent<Mover>().carConfig.maxSpeed;
        jumpWeight = carObject.GetComponent<Mover>().carConfig.jumpWeight;
        turnRate = carObject.GetComponent<Mover>().carConfig.turnRate;
        jumpForce = carObject.GetComponent<Mover>().carConfig.jumpForce;
        boundary = carObject.GetComponent<Mover>().boundary;
        Speed = 0;
    }

    public Car(GameObject gameObjectFirst , GameObject gameObjectSecond , SingleOrMultiple play)
    {
        playerDouble = play;
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                // First Player
                carObject = gameObjectFirst;
                speedForce = carObject.GetComponent<Mover>().carConfig.speedForce;
                speedAccelerate = carObject.GetComponent<Mover>().carConfig.speedAccelerate;
                maxSpeed = carObject.GetComponent<Mover>().carConfig.maxSpeed;
                jumpWeight = carObject.GetComponent<Mover>().carConfig.jumpWeight;
                turnRate = carObject.GetComponent<Mover>().carConfig.turnRate;
                jumpForce = carObject.GetComponent<Mover>().carConfig.jumpForce;
                boundary = carObject.GetComponent<Mover>().boundary;
                Speed = 0;
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                carObject = gameObjectFirst;
                speedForce = carObject.GetComponent<Mover>().carConfig.speedForce;
                speedAccelerate = carObject.GetComponent<Mover>().carConfig.speedAccelerate;
                maxSpeed = carObject.GetComponent<Mover>().carConfig.maxSpeed;
                jumpWeight = carObject.GetComponent<Mover>().carConfig.jumpWeight;
                turnRate = carObject.GetComponent<Mover>().carConfig.turnRate;
                jumpForce = carObject.GetComponent<Mover>().carConfig.jumpForce;
                boundary = carObject.GetComponent<Mover>().boundary;
                Speed = 0;

                // SecondPlayer
                carObjectSecond = gameObjectSecond;
                carObjectSecond.name = "SecondPlayerCar";
                carObjectSecond.tag = "SecondParentPlayer";
                speedForceSecond = carObjectSecond.GetComponent<Mover>().carConfig.speedForce;
                speedAccelerateSecond = carObjectSecond.GetComponent<Mover>().carConfig.speedAccelerate;
                maxSpeedSecond = carObjectSecond.GetComponent<Mover>().carConfig.maxSpeed;
                jumpWeightSecond = carObjectSecond.GetComponent<Mover>().carConfig.jumpWeight;
                turnRateSecond = carObjectSecond.GetComponent<Mover>().carConfig.turnRate;
                jumpForceSecond = carObjectSecond.GetComponent<Mover>().carConfig.jumpForce;
                boundarySecond = carObjectSecond.GetComponent<Mover>().boundary;
                SpeedSecond = 0;
                break;

        }
    }


    /// <summary>
    /// Init this Car on Start()
    /// </summary>
    public void InitStart()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.MULTIPLE:
                // First Player
                baseObject = carObject.transform.Find("Base").gameObject;
                tyreObject = carObject.transform.Find("wheels").gameObject;
                rb = tyreObject.GetComponent<Rigidbody>();
                rigidBase = carObject.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
                resetScript = carObject.GetComponent<ResetAnimation>();
                detectGround = carObject.transform.Find("wheels").GetComponent<DetectGround>();
                if (!detectGround)
                {
                    detectGround = carObject.transform.Find("wheels").GetComponent<DetectGround>();
                }
                //Debug.Log("Detect Ground ? " + detectGround);
                if (!baseObject.GetComponent<DetectGround>())
                {
                    baseGrounded = baseObject.AddComponent<DetectGround>();
                    for (int i = 0; i <= detectGround.groundTagName.Length; i++)
                    {
                        baseGrounded.groundTagName = detectGround.groundTagName;
                    }
                }
                else
                {
                    baseGrounded = baseObject.GetComponent<DetectGround>();
                    for (int i = 0; i < detectGround.groundTagName.Length; i++)
                    {
                        baseGrounded.groundTagName = detectGround.groundTagName;
                    }
                }
                if (!baseObject.GetComponent<CarCollider>())
                {
                    baseObject.AddComponent<CarCollider>();
                }

                // Second Player
                baseObjectSecond = carObjectSecond.transform.Find("Base").gameObject;
                tyreObjectSecond = carObjectSecond.transform.Find("wheels").gameObject;
                rbSecond = tyreObjectSecond.GetComponent<Rigidbody>();
                Debug.Log("Second Player lock with Rigidbody ? " + rbSecond.transform.parent.name);
                rigidBaseSecond = carObjectSecond.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
                resetScriptSecond = carObjectSecond.GetComponent<ResetAnimation>();
                detectGroundSecond = carObjectSecond.transform.Find("wheels").GetComponent<DetectGround>();
                if (!detectGroundSecond)
                {
                    detectGroundSecond = carObjectSecond.transform.Find("wheels").GetComponent<DetectGround>();
                    for(int i =0; i<= detectGround.groundTagName.Length; i++)
                    {
                        detectGroundSecond.groundTagName = detectGround.groundTagName;
                    }
                }
                //Debug.Log("Detect Ground ? " + detectGround);
                if (!baseObjectSecond.GetComponent<DetectGround>())
                {
                    baseGroundedSecond = baseObjectSecond.AddComponent<DetectGround>();
                    for (int i = 0; i <= detectGroundSecond.groundTagName.Length; i++)
                    {
                        baseGroundedSecond.groundTagName = detectGroundSecond.groundTagName;
                    }
                }
                else
                {
                    baseGroundedSecond = baseObjectSecond.GetComponent<DetectGround>();
                    for (int i = 0; i < detectGroundSecond.groundTagName.Length; i++)
                    {
                        baseGroundedSecond.groundTagName = detectGroundSecond.groundTagName;
                    }
                }
                if (!baseObjectSecond.GetComponent<CarCollider>())
                {
                    baseObjectSecond.AddComponent<CarCollider>();
                }
                ReadAngles();
                break;

            case SingleOrMultiple.SINGLE:
                baseObject = carObject.transform.Find("Base").gameObject;
                tyreObject = carObject.transform.Find("wheels").gameObject;
                rb = tyreObject.GetComponent<Rigidbody>();
                rigidBase = carObject.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
                resetScript = carObject.GetComponent<ResetAnimation>();
                detectGround = carObject.transform.Find("wheels").GetComponent<DetectGround>();
                if (!detectGround)
                {
                    detectGround = carObject.transform.Find("wheels").GetComponent<DetectGround>();
                }
                //Debug.Log("Detect Ground ? " + detectGround);
                if (!baseObject.GetComponent<DetectGround>())
                {
                    baseGrounded = baseObject.AddComponent<DetectGround>();
                    for (int i = 0; i <= detectGround.groundTagName.Length; i++)
                    {
                        baseGrounded.groundTagName = detectGround.groundTagName;
                    }
                }
                else
                {
                    baseGrounded = baseObject.GetComponent<DetectGround>();
                    for (int i = 0; i < detectGround.groundTagName.Length; i++)
                    {
                        baseGrounded.groundTagName = detectGround.groundTagName;
                    }
                }
                if (!baseObject.GetComponent<CarCollider>())
                {
                    baseObject.AddComponent<CarCollider>();
                }
                ReadAngles();
                break;
        }
    }

    /// <summary>
    /// To Stick the Base Position with Wheels
    /// </summary>
    public void StickBase()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                baseObject.transform.position = tyreObject.transform.position;
                baseObject.transform.rotation = tyreObject.transform.rotation;
                if (!tyreObject.activeSelf || !baseObject.activeSelf)
                {
                    tyreObject.SetActive(false);
                    baseObject.SetActive(false);
                }
                else if (tyreObject.activeSelf || baseObject.activeSelf)
                {
                    baseObject.SetActive(true);
                    tyreObject.SetActive(true);
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                baseObject.transform.position = tyreObject.transform.position;
                baseObject.transform.rotation = tyreObject.transform.rotation;
                if (!tyreObject.activeSelf || !baseObject.activeSelf)
                {
                    tyreObject.SetActive(false);
                    baseObject.SetActive(false);
                }
                else if (tyreObject.activeSelf || baseObject.activeSelf)
                {
                    baseObject.SetActive(true);
                    tyreObject.SetActive(true);
                }

                // Second Player
                baseObjectSecond.transform.position = tyreObjectSecond.transform.position;
                baseObjectSecond.transform.rotation = tyreObjectSecond.transform.rotation;
                if (!tyreObjectSecond.activeSelf || !baseObjectSecond.activeSelf)
                {
                    tyreObjectSecond.SetActive(false);
                    baseObjectSecond.SetActive(false);
                }
                else if (tyreObjectSecond.activeSelf || baseObjectSecond.activeSelf)
                {
                    baseObjectSecond.SetActive(true);
                    tyreObjectSecond.SetActive(true);
                }
                break;
        }
    }

    /// <summary>
    /// Physic Jump
    /// </summary>
    public void JumpNow()
    {
        // Only Let First Player
        if(carObject.CompareTag("ParentPlayer"))
        {
            rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
        }
        // Only Let Clone Player
        else if (carObject.CompareTag("ClonePlayer") && CloneJumpNow() && carObject.GetInstanceID() == carObject.GetInstanceID())
        {
            Debug.LogWarning("Clone Jump !" + carObject.GetInstanceID());
            rb.AddForce(Vector3.up * jumpForce * 1, ForceMode.Impulse);
        }
        // Only Let Second Player
        if(carObject.CompareTag("SecondParentPlayer") && carObjectSecond.GetInstanceID() == carObjectSecond.GetInstanceID() && !CloneJumpNow())
        {
            Debug.Log("Second Player Jump Now. Jump Value : " + Input.GetAxis("SecondJump"));
            rb.AddForce(Vector3.up * jumpForceSecond * Input.GetAxis("SecondJump"), ForceMode.Impulse);
        }
    }

    public bool CloneJumpNow()
    {
        return cloneFlag;
    }


    /// <summary>
    /// For Read Rigidbody Angles
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="Z"></param>
    public void ReadAngles()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                rotX = rb.rotation.eulerAngles.x;
                rotY = rb.rotation.eulerAngles.y;
                rotZ = rb.rotation.eulerAngles.z;
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                rotX = rb.rotation.eulerAngles.x;
                rotY = rb.rotation.eulerAngles.y;
                rotZ = rb.rotation.eulerAngles.z;

                // First Player
                rotXSecond = rbSecond.rotation.eulerAngles.x;
                rotYSecond = rbSecond.rotation.eulerAngles.y;
                rotZSecond = rbSecond.rotation.eulerAngles.z;
                break;
        }
        return;
    }

    public void DetectBaseGround()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                isBaseGrounded = baseGrounded.isGrounded;
                break;

            case SingleOrMultiple.MULTIPLE:
                isBaseGrounded = baseGrounded.isGrounded;
                isBaseGroundedSecond = baseGroundedSecond.isGroundedSecond;
                break;
        }
        return;
    }


    /// <summary>
    /// Make a Moving Car on FixedUpdate() - Runs Physics Force
    /// </summary>
    /// <param name="boundary"></param>
    public void Moving()
    {
        ReadAngles();
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                rb.position = new Vector3(
                    0.0f,
                    Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
                    Mathf.Clamp(rb.position.z, boundary.zMin, GetZMax())
                    );
                // For Max Speed in Km/Hr
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed / 3.6f);
                DetectGround();
                GetSpeed();
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                rb.position = new Vector3(
                    0.0f,
                    Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
                    Mathf.Clamp(rb.position.z, boundary.zMin, GetZMax())
                    );
                DetectGround();
                // Second Player
                rbSecond.position = new Vector3(
                    0.0f,
                    Mathf.Clamp(rbSecond.position.y, boundary.yMin, boundary.yMax),
                    Mathf.Clamp(rbSecond.position.z, boundary.zMin, GetZMax())
                    );
                DetectGroundSecond();
                // For Max Speed in Km/Hr
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed / 3.6f);
                rbSecond.velocity = Vector3.ClampMagnitude(rbSecond.velocity, maxSpeedSecond / 3.6f);
                GetSpeed();
                break;
        }
        RotationControlCheck();
        ListenEvent();
        RunGround();
    }

    /// <summary>
    /// To Be Call on Update() Mover. Get Boundary Z Value Updated from GameController
    /// </summary>
    /// <returns>Get boundary.zMax value</returns>
    public float GetZMax()
    {
        return boundary.zMax;
    }

    public void Stop()
    {
        GetSpeed();
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                rb.AddForce(Vector3.zero, ForceMode.Impulse);
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                rb.AddForce(Vector3.zero, ForceMode.Impulse);
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;

                // Second Player
                rbSecond.AddForce(Vector3.zero, ForceMode.Impulse);
                rbSecond.angularVelocity = Vector3.zero;
                rbSecond.velocity = Vector3.zero;
                break;
        }
    }

    void ListenEvent()
    {
        //if (DetectBaseGround())
        //{
        //    Debug.Log("Running On Ground");
        //    BlinkNow();
        //}
        //if (baseObject.transform.position.z > 490)
        //{
        //    BlinkNow();
        //}
        return;
    }

    /// <summary>
    /// This is for Get Speed that returns Double Value
    /// </summary>
    /// <returns>Double Speed value</returns>
    public void GetSpeed()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                Speed = rb.velocity.magnitude * 3.6f;
                break;

            case SingleOrMultiple.MULTIPLE:
                Speed = rb.velocity.magnitude * 3.6f;
                SpeedSecond = rbSecond.velocity.magnitude * 3.6f;
                break;

        }
        return;
    }

    public void DisableGravity()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                rigidBase.useGravity = false;
                rb.useGravity = false;
                break;

            case SingleOrMultiple.MULTIPLE:
                // First PLayer
                rigidBase.useGravity = false;
                rb.useGravity = false;

                // Second Player
                rigidBase.useGravity = false;
                rb.useGravity = false;
                break;
        }
        return;
    }

    public void EnableGravity()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                rigidBase.useGravity = true;
                rb.useGravity = true;
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                rigidBase.useGravity = true;
                rb.useGravity = true;

                // Second Player
                rigidBaseSecond.useGravity = true;
                rbSecond.useGravity = true;
                break;
        }
        return;
    }

    public void RotationControlCheck()
    {
        Quaternion reset = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                if (rb.rotation != reset)
                {
                    rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                if (rb.rotation != reset)
                {
                    rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
                }

                // Second Player
                if (rbSecond.rotation != reset)
                {
                    rbSecond.rotation = Quaternion.Lerp(rbSecond.rotation, reset, Time.deltaTime * turnRate);
                }
                break;
        }
    }

    /// <summary>
    /// This is for enable blinking effects
    /// </summary>
    void BlinkNow()
    {
        resetScript.blinkingAnimate(this, 0.5f, 1.0f);
    }

    /// <summary>
    /// This is for checking whether the object is active in Hierarchy or not. 
    /// If not , It will automatically set it as true in hierarchy
    /// </summary>
    public void checkActive()
    {
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                if (!tyreObject.activeInHierarchy)
                {
                    tyreObject.SetActive(true);
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                if (!tyreObject.activeInHierarchy)
                {
                    tyreObject.SetActive(true);
                }

                // Second Player
                if (!tyreObjectSecond.activeInHierarchy)
                {
                    tyreObjectSecond.SetActive(true);
                }
                break;
        }
        return;
    }

    public void RunGround()
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        switch (playerDouble)
        {
            case SingleOrMultiple.SINGLE:
                if (DetectGround())
                {
                    //Debug.Log("Move " + carObject.name);
                    rb.AddForce(movement * speedForce, ForceMode.Acceleration);
                    rigidBase.mass = 1;
                    return;
                }
                else if (!DetectGround())
                {
                    //Debug.Log("UnMoved " + carObject.name + " ID : " + carObject.GetInstanceID());
                    rigidBase.mass = jumpWeight;
                    rb.AddForce(movement * 0.0f, ForceMode.Acceleration);
                    return;
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                if (DetectGround())
                {
                    //Debug.Log("Move " + carObject.name);
                    rb.AddForce(movement * speedForce, ForceMode.Acceleration);
                    rigidBase.mass = 1;
                    return;
                }
                else if (!DetectGround())
                {
                    //Debug.Log("UnMoved " + carObject.name + " ID : " + carObject.GetInstanceID());
                    rigidBase.mass = jumpWeight;
                    rb.AddForce(movement * 0.0f, ForceMode.Acceleration);
                    return;
                }

                // Second Player
                if (DetectGroundSecond())
                {
                    //Debug.Log("Move " + carObject.name);
                    rbSecond.AddForce(movement * speedForceSecond, ForceMode.Acceleration);
                    rigidBase.mass = 1;
                    return;
                }
                else if (!DetectGroundSecond())
                {
                    //Debug.Log("UnMoved " + carObject.name + " ID : " + carObject.GetInstanceID());
                    rigidBaseSecond.mass = jumpWeightSecond;
                    rbSecond.AddForce(movement * 0.0f, ForceMode.Acceleration);
                    return;
                }
                break;
        }
    }

    /// <summary>
    /// To Detect the Car Grounded Boolean. Must RUNS it on Update()
    /// </summary>
    /// <returns>Boolean Value.</returns>
    public bool DetectGround()
    {
        return isGrounded = detectGround.isGrounded;
    }

    /// <summary>
    /// To Detect the Car Second Grounded Boolean. Must RUNS it on Update()
    /// </summary>
    /// <returns>Boolean Value.</returns>
    public bool DetectGroundSecond()
    {
        return isGroundedSecond = detectGroundSecond.isGroundedSecond;
    }

}
