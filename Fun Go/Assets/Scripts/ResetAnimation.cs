using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimation : MonoBehaviour {

    [Header("Blink Timing Animations Config :" , order = 0)]
    [Tooltip("Time Between Blinks (Float)")]
    public float blinkGap;
    [Tooltip("Number of Blink Maximum (Integer)")]
    public int blinkMax;
    [Tooltip("To delay before trigger blink (Float)")]
    public float blinkDelay;
    [HideInInspector]
    public int blinkCount;
    private bool exitLoop;
    private Vector3 currentPos;
    private Quaternion resetRot;
    [HideInInspector]
    public bool conditionPos;
    [HideInInspector]
    public bool gameRunning;

    public IEnumerator UpdatePosAnimation(GameObject rb , GameObject baseObject)
    {
        gameRunning = true;
        while (gameRunning)
        {
            resetRot = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            exitLoop = false;
            if (conditionPos)
            {
                currentPos = rb.transform.position;
                rb.GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Impulse);
                rb.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                rb.GetComponent<Rigidbody>().velocity = Vector3.zero;
                if (currentPos != null)
                {
                    rb.transform.position = currentPos;
                    rb.transform.rotation = resetRot;
                }
                yield return new WaitForSeconds(blinkDelay);
                for (int i = 0; i <= blinkMax; i++)
                {
                    rb.SetActive(!rb.activeInHierarchy);
                    blinkCount++;
                    yield return new WaitForSeconds(blinkGap);
                    if (blinkCount == blinkMax)
                    {
                        exitLoop = true;
                        gameRunning = false;
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
}