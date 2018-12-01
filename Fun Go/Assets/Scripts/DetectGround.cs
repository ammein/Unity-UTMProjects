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
        foreach(string groundName in groundTagName)
        {
            if (collision.gameObject.CompareTag(groundName))
            {
                isGrounded = true;
                print("On the ground : " + collision.gameObject);
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        foreach (string groundName in groundTagName)
        {
            if (collision.gameObject.CompareTag(groundName))
            {
                isGrounded = false;
                print("In the air : " + collision.gameObject);
                return;
            }
        }
    }
}
