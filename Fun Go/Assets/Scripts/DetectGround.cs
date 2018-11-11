using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour {

    [Tooltip("Assign name of Tag for detect ground")]
    public string[] groundTagName;
    [Tooltip("DON'T TICK THIS. Only tick it if you want it to be in debug mode")]
    public bool isGrounded;

    void Start()
    {
        isGrounded = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach(string groundName in groundTagName)
        {
            if (collision.gameObject.CompareTag(groundName))
            {
                isGrounded = true;
                print("On the ground");
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
                print("In the air");
                return;
            }
        }
    }
}
