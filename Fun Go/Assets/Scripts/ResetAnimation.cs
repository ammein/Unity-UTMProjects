using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateCondition
{
    UPDATEPOS,
    NORMAL
}

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
    private Vector3 currentPos;
    private Quaternion resetRot;
    [HideInInspector]
    public bool conditionPos;
    [HideInInspector]
    public bool gameRunning;

    StateCondition state;

    private void Start()
    {
        state = StateCondition.NORMAL;
    }

    public IEnumerator UpdatePosAnimation(GameObject rb , GameObject baseObject)
    {
        while (gameRunning)
        {
            state = StateCondition.NORMAL;
            if (baseObject.transform.position.z > 50 && baseObject.transform.position.z <= 100)
            {
                Debug.Log("Running After Transform Position");
                state = StateCondition.UPDATEPOS;
            }
            resetRot = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            currentPos = rb.transform.position;
            switch (state)
            {
                case StateCondition.UPDATEPOS:
                    Debug.Log("Running UPDATEPOS");
                    for(int i =0; i <= blinkMax; i++)
                    {
                        rb.transform.position = currentPos;
                        rb.transform.rotation = resetRot;
                        rb.GetComponent<Rigidbody>().AddForce(Vector3.zero, ForceMode.Impulse);
                        rb.SetActive(!rb.activeInHierarchy);
                        yield return new WaitForSeconds(blinkGap);
                    }
                    state = StateCondition.NORMAL;
                    Debug.Log("END FOR LOOP");
                    break;
            }
            yield return null;
        }
    }
}