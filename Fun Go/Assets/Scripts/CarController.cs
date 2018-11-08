using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

//[System.Serializable] // Make it enable on unity to serialize
//public class Boundary
//{
//    public float zMin, zMax, yMin, yMax;
//}

//[System.Serializable]
//public class RotationBoundary
//{
//    public float yMin, yMax;
//}

public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Boundary boundary;
    public RotationBoundary rotationBoundary;
    private float currentRotation;
    private Rigidbody[] rb;

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void Start()
    {
        rb = GetComponentsInChildren<Rigidbody>();
    }

    //public void FixedUpdate()
    //{
    //    float motor = maxMotorTorque * Input.GetAxis("Vertical");
    //    float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
    //    moving(rb);

    //    foreach (AxleInfo axleInfo in axleInfos)
    //    {
    //        if (axleInfo.steering)
    //        {
    //            axleInfo.leftWheel.steerAngle = steering;
    //            axleInfo.rightWheel.steerAngle = steering;
    //        }
    //        if (axleInfo.motor)
    //        {
    //            axleInfo.leftWheel.motorTorque = motor;
    //            axleInfo.rightWheel.motorTorque = motor;
    //        }
    //        ApplyLocalPositionToVisuals(axleInfo.leftWheel);
    //        ApplyLocalPositionToVisuals(axleInfo.rightWheel);
    //    }
    //}


    public void moving(Rigidbody[] rb)
    {
        // Move for each components
        foreach (Rigidbody child in rb)
        {
            child.position = new Vector3(
                0.0f,
                Mathf.Clamp(child.position.y, boundary.yMin, boundary.yMax),
                Mathf.Clamp(child.position.z, boundary.zMin, boundary.zMax)
                );

            rotationControl(child);

            if (Input.GetKeyDown("space"))
            {
                transform.Translate(Vector3.up * 260 * Time.deltaTime, Space.World);
            }
        }
    }

    void rotationControl(Rigidbody child)
    {
        currentRotation = Mathf.Clamp(transform.rotation.y, rotationBoundary.yMin, rotationBoundary.yMax);
        if (transform.position.y >= 0)
        {
            transform.rotation = Quaternion.identity * Quaternion.AngleAxis(currentRotation, transform.right);
        }

        //Debug.Log("Current Rotation : " + transform.rotation);
    }
}