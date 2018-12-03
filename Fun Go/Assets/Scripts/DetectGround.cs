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
        //Debug.Log("Clone Player ? " + gameObject.transform.parent.tag);
        //Debug.Log("Collision Clone Player ? " + collision.gameObject.tag);
        // Let Clone Player Jump If detect "Hill"
        if (gameObject.transform.parent.tag == "ClonePlayer" && collision.gameObject.CompareTag("Hill"))
        {
            Debug.LogWarning("Collision Enter Clone Player Hill");
            gameObject.transform.parent.GetComponent<Mover>().myCar.cloneFlag = true;
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
