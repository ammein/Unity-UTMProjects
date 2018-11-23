using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateCondition
{
    UPDATEPOS,
    NORMAL
}
public static class CoroutineExtensions
{
    /// <summary>
    /// Tries to stop a coroutine based on a Coroutine Handle.
    /// will only stop the Coroutine if the handle is not null
    /// </summary>
    /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
    /// <param name="handle">Handle.</param>
    public static MonoBehaviour TryStopCoroutine(this MonoBehaviour script, ref Coroutine handle)
    {
        if (!script) return null;
        if (handle != null) script.StopCoroutine(handle);
        handle = null;
        return script;
    }

    /// <summary>
    /// Starts the coroutine and sets the routine to a Coroutine handle.
    /// </summary>
    /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
    /// <param name="routine">Routine.</param>
    /// <param name="handle">Handle.</param>
    public static MonoBehaviour StartCoroutine(this MonoBehaviour script, IEnumerator routine, ref Coroutine handle)
    {
        if (!script)
        {
            #if UNITY_EDITOR
            Debug.LogWarning("A coroutine cannot run while it is null or being destroyed");
            #endif
            return null;
        }

        if (!script.enabled || !script.gameObject.activeInHierarchy)
        {
            #if UNITY_EDITOR
            Debug.LogWarningFormat(script, "The Script {0} is currently disabled and cannot start coroutines", script);
            #endif
            return script;
        }

        handle = script.StartCoroutine(routine);

        return script;
    }


    /// <summary>
    /// Stops any possible coroutine running on the specified handle and runs a new routine in its place
    /// </summary>
    /// <returns>the Monobehaviour script running the coroutine, allowing chained commands</returns>
    /// <param name="script">Script.</param>
    /// <param name="routine">Routine.</param>
    /// <param name="handle">Handle.</param>
    public static MonoBehaviour RestartCoroutine(this MonoBehaviour script, IEnumerator routine, ref Coroutine handle)
    {
        return script.TryStopCoroutine(ref handle)
            .StartCoroutine(routine, ref handle);
    }
}

public class ResetAnimation : MonoBehaviour {

    [Header("Blink Timing Animations Config :" , order = 0)]
    [Tooltip("Enable / Disable Reset Animation")]
    public bool enableReset = true;
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
    private Coroutine myCoroutine;
    private GameObject rb;
    private GameObject baseObject;

    StateCondition state;

    private void Start()
    {
        state = StateCondition.NORMAL;
    }

    public IEnumerator UpdatePosAnimation()
    {
        while (gameRunning && enableReset)
        {
            rb = gameObject.transform.Find("wheels").gameObject;
            baseObject = gameObject.transform.Find("Base").gameObject;
            state = StateCondition.NORMAL;
            yield return new WaitForSeconds(blinkDelay);
            if (baseObject.transform.position.z > 50 && baseObject.transform.position.z <= 100)
            {
                Debug.Log("Running After Transform Position");
                InvokeRepeating("BlinkNow", blinkDelay, blinkMax);
            }
            if(baseObject.transform.rotation.x < Mover.WrapAngle(-50.0f) && !(baseObject.transform.rotation.x < Mover.WrapAngle(0.0f)))
            {
                Debug.Log("Running Rotation Position");
                InvokeRepeating("BlinkNow", blinkDelay, blinkMax);
            }
            resetRot = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
            currentPos = rb.transform.position;
            yield return null;
        }
    }

    private void BlinkNow()
    {
        rb.transform.position = currentPos;
        rb.transform.rotation = resetRot;
        rb.SetActive(!rb.activeInHierarchy);
    }

}