using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public float speed , maxSpeed , speedAccelerate , jumpWeight , turnRate , jumpForce;
    public GameObject gameObject , baseObject , tyreObject;
    private Rigidbody rb , rigidBase;
    private double Speed;
    public ResetAnimation resetScript;
    bool animateNow = false;
    public bool isGrounded;
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
        StickBase();
    }

    /// <summary>
    /// To Stick the Base Position with Wheels
    /// </summary>
    public void StickBase()
    {
        baseObject.transform.position = tyreObject.transform.position;
        baseObject.transform.rotation = tyreObject.transform.rotation;
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

    /// <summary>
    /// Physic Jump
    /// </summary>
    public void JumpNow()
    {
        rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
    }


    /// <summary>
    /// Make a Moving Car on FixedUpdate() - Runs Physics Force
    /// </summary>
    /// <param name="boundary"></param>
    public void Moving(Boundary boundary)
    {
        Vector3 movement = new Vector3(0.0f, 0.0f, speedAccelerate);
        isGrounded = detectGround.isGrounded;
        Speed = rb.velocity.magnitude * 3.6;

        rb.position = new Vector3(
            0.0f,
            Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
            );
        RotationControlCheck();
        ListenEvent();
        // For Max Speed in Km/Hr
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed / 3.6f);
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

    void ListenEvent()
    {
        if (baseObject.transform.position.z > 50 && baseObject.transform.position.z <= 100)
        {
            BlinkNow(2.0f);
        }
        if (baseObject.transform.position.z > 490)
        {
            BlinkNow(2.0f);
        }
    }

    bool CountDown(float limit)
    {
        countdown += Time.deltaTime;
        Debug.Log("Countdown : " + countdown.ToString("F0"));
        if(limit < countdown)
        {
            countdown = 0;
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// This is for Get Speed that returns Double Value
    /// </summary>
    /// <returns>Double Speed value</returns>
    public double GetSpeed()
    {
        return Speed;
    }

    void RotationControlCheck()
    {
        //Debug.Log("RB Rotation" + rb.rotation);
        //Debug.Log("Transform Rotation" + rb.transform.rotation);
        Quaternion reset = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
        if (rb.rotation != reset)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, reset, Time.deltaTime * turnRate);
        }
    }

    /// <summary>
    /// This is for enable blinking effects
    /// </summary>
    void BlinkNow(float limitDuration)
    {
        if (CountDown(limitDuration))
        {
            resetScript.blinkingAnimate(tyreObject, 0.5f, 1.0f);
            return;
        }
        else
        {
            return;
        }
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
