using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car
{
    public float speed;
    public float maxSpeed;
    public GameObject gameObject;
    public GameObject baseObject;
    public GameObject tyreObject;
    private Rigidbody rb;
    private Rigidbody rigidBase;
    private double Speed;
    public float speedAccelerate;
    public float jumpWeight;
    public float turnRate;
    public float jumpForce;
    public ResetAnimation resetScript;
    bool animateNow = false;
    public bool isGrounded;
    private DetectGround detectGround;
    private float timer = 0;

    public Car(GameObject myGameObject , float mySpeed , float myMaxSpeed , float accelerate , float jump , float weight , float turn)
    {
        speed = mySpeed;
        speedAccelerate = accelerate;
        maxSpeed = myMaxSpeed;
        gameObject = myGameObject;
        jumpWeight = weight;
        turnRate = turn;
        jumpForce = jump;
        Speed = 0;
    }

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

    public void jumpCode()
    {
        rb.AddForce(Vector3.up * jumpForce * Input.GetAxis("Jump"), ForceMode.Impulse);
    }

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

    public double GetSpeed()
    {
        return Speed;
    }

    void ListenEvent()
    {
        if (baseObject.transform.position.z > 50 && baseObject.transform.position.z <= 100)
        {
            BlinkNow(2.0f);
        }
        if(baseObject.transform.position.z > 490)
        {
            BlinkNow(2.0f);
        }
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

    void BlinkNow(float duration)
    {
        timer += Time.deltaTime;
        if(timer >= duration)
        {
            Debug.Log("Running Close Blink" + timer);
            checkActive();
            timer = 0;
        }
        if(timer <= duration)
        {
            Debug.Log("Running Open Blink" + timer);
            resetScript.blinkingAnimate(tyreObject, 0.5f, 1.0f);
        }
    }

    public void checkActive()
    {
        if (!tyreObject.activeInHierarchy)
        {
            tyreObject.SetActive(true);
        }
    }

    public bool DetectGround()
    {
        return isGrounded;
    }

}
