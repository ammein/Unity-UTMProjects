using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public float speed , maxSpeed , speedAccelerate , jumpWeight , turnRate , jumpForce , rotX , rotY , rotZ;
    public GameObject gameObject , baseObject , tyreObject;
    private Rigidbody rb , rigidBase;
    private double Speed;
    public ResetAnimation resetScript;
    public bool isGrounded , isBaseGrounded;
    public DetectGround baseGrounded;
    public DetectGround detectGround;
    private float timer = 0;
    private float countdown = 0;

    public Car(GameObject myGameObject ,CarConfigurations carConfig)
    {
        speed = carConfig.speedForce;
        speedAccelerate = carConfig.speedAccelerate;
        maxSpeed = carConfig.maxSpeed;
        gameObject = myGameObject;
        jumpWeight = carConfig.jumpWeight;
        turnRate = carConfig.turnRate;
        jumpForce = carConfig.jumpForce;
        Speed = 0;
    }


    /// <summary>
    /// Init this Car on Start()
    /// </summary>
    public void InitStart()
    {
        baseObject = gameObject.transform.Find("Base").gameObject;
        tyreObject = gameObject.transform.Find("wheels").gameObject;
        rb = tyreObject.GetComponent<Rigidbody>();
        rigidBase = gameObject.transform.Find("Base").gameObject.GetComponent<Rigidbody>();
        resetScript = gameObject.GetComponent<ResetAnimation>();
        detectGround = gameObject.transform.Find("wheels").GetComponent<DetectGround>();
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
    }

    /// <summary>
    /// To Stick the Base Position with Wheels
    /// </summary>
    public void StickBase()
    {
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
    }

    /// <summary>
    /// Physic Jump
    /// </summary>
    public void JumpNow()
    {
        rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
    }


    /// <summary>
    /// For Read Rigidbody Angles
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <param name="Z"></param>
    public void ReadAngles()
    {
        rotX = rb.rotation.eulerAngles.x;
        rotY = rb.rotation.eulerAngles.y;
        rotZ = rb.rotation.eulerAngles.z;
        return;
    }

    public bool DetectBaseGround()
    {
        Debug.Log("Base touch the ground ? " + isBaseGrounded);
        return isBaseGrounded = baseGrounded.isGrounded;
    }


    /// <summary>
    /// Make a Moving Car on FixedUpdate() - Runs Physics Force
    /// </summary>
    /// <param name="boundary"></param>
    public void Moving(Boundary boundary)
    {
        ReadAngles();
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        isGrounded = detectGround.isGrounded;
        DetectBaseGround();

        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );
        RotationControlCheck();
        ListenEvent();
        // For Max Speed in Km/Hr
        Speed = rb.velocity.magnitude * 3.6f;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed / 3.6f);
        if (isGrounded)
        {
            Debug.Log("Move");
            rb.AddForce(movement * speed, ForceMode.Acceleration);
            rigidBase.mass = 1;
            return;
        }
        else if (!isGrounded)
        {
            Debug.Log("UnMoved");
            rigidBase.mass = jumpWeight;
            rb.AddForce(movement * 0.0f, ForceMode.Acceleration);
            return;
        }
    }

    public void Stop()
    {
        rb.AddForce(Vector3.zero, ForceMode.Impulse);
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
    }

    void ListenEvent()
    {
        //if (DetectBaseGround())
        //{
        //    Debug.Log("Running On Ground");
        //    BlinkNow();
        //}
        if (baseObject.transform.position.z > 490)
        {
            BlinkNow();
        }
        return;
    }

    /// <summary>
    /// This is for Get Speed that returns Double Value
    /// </summary>
    /// <returns>Double Speed value</returns>
    public double GetSpeed()
    {
        return Speed;
    }

    public void DisableGravity()
    {
        rb.useGravity = false;
        return;
    }

    public void RotationControlCheck()
    {
        //Debug.LogWarning("Running Rotation : " + Mover.WrapAngle(rotX).ToString("F0"));
        Quaternion reset = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        if (rb.rotation != reset)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
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
        if (!tyreObject.activeInHierarchy)
        {
            tyreObject.SetActive(true);
        }
    }

    /// <summary>
    /// To Detect the Car Grounded Boolean. Must RUNS it on Update()
    /// </summary>
    /// <returns>Boolean Value.</returns>
    public bool DetectGround()
    {
        return isGrounded;
    }

}
