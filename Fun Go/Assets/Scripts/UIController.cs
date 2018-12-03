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

    [HideInInspector]
    public bool counting = false;

    private bool holdCar;

    private GameObject[] allInstantiatePlayer;
    private GameObject[] playerCar;
    private GameObject gameController;
    private int getCount;

    // Use this for initialization
    void Start()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetSpeed();
        getCount = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().countStart;
        // Get Current UI W & H
        width = Screen.width;
        height = Screen.height;
        SpeedUI.fixedWidth = width;
        SpeedUI.fixedHeight = height;
        CountdownUI.fixedWidth = width;
        CountdownUI.fixedHeight = height;
        InitiateCaller();
        StopOrRun(true);
        StartCoroutine(Count(getCount));
    }

    void Update()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetSpeed();
        InitiateCaller();
    }

    public void InitiateCaller()
    {
        allInstantiatePlayer = GameObject.FindGameObjectsWithTag("ClonePlayer");
        playerCar = GameObject.FindGameObjectsWithTag("ParentPlayer");
        gameController = GameObject.FindGameObjectWithTag("GameController");
        return;
    }

    IEnumerator Count(int value)
    {
        for (int i = value; i > 0; i--)
        {
            counting = true;
            StopOrRun(true);
            enableCount = true;
            count = i.ToString();
            yield return new WaitForSeconds(1);
            enableCount = false;
            if(i == 1)
            {
                enableCount = true;
                count = "Go";
                StopOrRun(false);
                yield return new WaitForSeconds(1);
                enableCount = false;
                counting = false;
                yield break;
            }
        }
        yield return null;
    }

    void StopOrRun(bool SetBool)
    {
        foreach(GameObject clonePlayer in allInstantiatePlayer)
        {
            clonePlayer.GetComponent<Mover>().pauseCar = SetBool;
            clonePlayer.GetComponent<Mover>().boundary.zMax = gameController.GetComponent<GameController>().GetAllMapLength();
        }
        foreach (GameObject currentPlayer in playerCar)
        {
            currentPlayer.GetComponent<Mover>().pauseCar = SetBool;
            currentPlayer.GetComponent<Mover>().boundary.zMax = gameController.GetComponent<GameController>().GetAllMapLength();
        }
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
