using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTagTransform : MonoBehaviour {

    private GameObject positionTag;

    private Vector3 currentPos;
    private Quaternion currentRot;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    [Header("Number Tag Position Settings")]
    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    private float offsetFromPlayer;
    [Header("Offset Tag")]
    [Range(10.0f , -10.0f)]
    public float offset;

    // Use this for initialization
    void Start () {
        positionTag = gameObject;
        posOffset = positionTag.transform.position;
        offsetFromPlayer = transform.position.y - currentPos.y;
        UpdatePos();
        //StartCoroutine(TransformMovement());
    }

    void UpdatePos()
    {
        currentPos = positionTag.transform.parent.position;
        //currentRot = Quaternion.Euler(0.0f, positionTag.transform.parent.rotation.y, 0.0f);
    }

    public float UpdateOffset()
    {
        return offset;
    }

    void UpdateAllPos()
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos + currentPos - new Vector3(0.0f, offsetFromPlayer - offset, 0.0f);
        return;
    }

    // Source Transform Movement 
    // Source : http://www.donovankeith.com/2016/05/making-objects-float-up-down-in-unity/
    void FixedUpdate () {
        // Float up/down with a Sin()
        UpdateOffset();
        UpdatePos();
        UpdateAllPos();
    }
}
