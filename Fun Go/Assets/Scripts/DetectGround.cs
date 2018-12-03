using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour {
    [Header("Name Your Ground for Collision Detect (On Ground/In The Air)")]
    [Tooltip("Assign name of Tag for detect ground")]
    public string[] groundTagName;
    [HideInInspector] // Hides var belows
    public bool isGrounded;

    void Start()
    {
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision Enter Running");
        foreach(string groundName in groundTagName)
        {
            if (collision.gameObject.CompareTag(groundName))
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Let Clone Player Jump
        foreach(string groundName in groundTagName)
        {
            if(gameObject.CompareTag("ClonePlayer") && collision.gameObject.CompareTag(groundName))
            {
                Debug.LogWarning("Collision Stay Clone Player" + groundName);
                gameObject.GetComponent<Mover>().myCar.JumpNow();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Collision Exit Running");
        foreach (string groundName in groundTagName)
        {
            if (collision.gameObject.CompareTag(groundName))
            {
                isGrounded = false;
                //print("In the air : " + collision.gameObject.tag + " with " + groundName);
            }
        }
    }
}
