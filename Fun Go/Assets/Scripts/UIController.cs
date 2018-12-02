using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private double speedInit;
    private Rect displayPosition;
    [Header("Speed UI Settings")]
    public GUIStyle SpeedUI;

    [Header("Countdown UI Settings")]
    public GUIStyle CountdownUI;
    private float width;
    private float height;
    [HideInInspector]
    public bool enableCount;
    [HideInInspector]
    public string count;

    // Use this for initialization
    void Start()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetSpeed();
        // Get Current UI W & H
        width = Screen.width;
        height = Screen.height;
        SpeedUI.fixedWidth = width;
        SpeedUI.fixedHeight = height;
        CountdownUI.fixedWidth = width;
        CountdownUI.fixedHeight = height;
        StartCoroutine(Count(3));
    }

    void Update()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetSpeed();
    }

    IEnumerator Count(int value)
    {
        for (int i = value; i > 0; i--)
        {
            enableCount = true;
            count = i.ToString();
            yield return new WaitForSeconds(1);
            enableCount = false;
            if(i == 1)
            {
                enableCount = true;
                Debug.Log("Running Break");
                count = "Go";
                yield return new WaitForSeconds(1);
                enableCount = false;
                yield break;
            }
        }
        yield return null;
    }

    void CountDown()
    {
        Rect displayCountdown = new Rect(0 + CountdownUI.contentOffset.x, 0 + CountdownUI.contentOffset.y, 100, 50);
        GUI.TextArea(displayCountdown, count, CountdownUI);
    }

    private void OnGUI()
    {
        displayPosition = new Rect(0 + SpeedUI.contentOffset.x, 0 + SpeedUI.contentOffset.y , 100 , 50);
        GUI.TextArea(displayPosition, speedInit.ToString("F0") + " km/hr", SpeedUI);
        if (enableCount)
        {
            CountDown();
        }
    }
}
