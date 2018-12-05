using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private double speedInit , speedInitSecond;
    [Header("Speed UI Settings")]
    public GUIStyle SpeedUI;

    [Header("Countdown UI Settings")]
    public GUIStyle CountdownUI;
    [HideInInspector]
    public bool enableCount;
    [HideInInspector]
    public string count;

    [HideInInspector]
    public bool counting = false;

    private bool holdCar;

    private GameObject[] allInstantiatePlayer;
    private GameObject playerCar;
    private GameObject playerCarSecond;
    private GameObject gameController;
    private int getCount;

    private SingleOrMultiple play;

    private UIPlayer uiPlaySpeed;
    private UIPlayer uiPlayCountdown;
    private bool splitCam;

    // Use this for initialization
    void Start()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.Speed;
        getCount = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().countStart;
        play = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().play;
        uiPlaySpeed = new UIPlayer(SpeedUI , play);
        uiPlayCountdown = new UIPlayer(CountdownUI, play);
        UpdateCameraSplit();
        InitiateCaller();
        StopOrRun(true);
        StartCoroutine(Count(getCount));
    }

    void UpdateCameraSplit()
    {
        splitCam = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().splitCameraMultiplayer;
    }

    void Update()
    {
        UpdateCameraSplit();
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.Speed;
        speedInitSecond = GameObject.FindGameObjectWithTag("SecondParentPlayer").GetComponent<Mover>().myCar.Speed;
        InitiateCaller();
    }

    public void InitiateCaller()
    {
        allInstantiatePlayer = GameObject.FindGameObjectsWithTag("ClonePlayer");
        playerCar = GameObject.FindGameObjectWithTag("ParentPlayer");
        playerCarSecond = GameObject.FindGameObjectWithTag("SecondParentPlayer");
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
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                playerCar.GetComponent<Mover>().pauseCar = SetBool;
                playerCar.GetComponent<Mover>().boundary.zMax = gameController.GetComponent<GameController>().GetAllMapLength();
                break;

            case SingleOrMultiple.MULTIPLE:
                playerCar.GetComponent<Mover>().pauseCar = SetBool;
                playerCarSecond.GetComponent<Mover>().pauseCar = SetBool;
                playerCar.GetComponent<Mover>().boundary.zMax = gameController.GetComponent<GameController>().GetAllMapLength();
                playerCarSecond.GetComponent<Mover>().boundary.zMax = gameController.GetComponent<GameController>().GetAllMapLength();
                break;
        }
    }

    void CountDown()
    {
        uiPlayCountdown.CountTextArea(count);
        uiPlayCountdown.DisplayArea(splitCam);
    }

    private void OnGUI()
    {
        uiPlaySpeed.DisplayArea(splitCam);
        uiPlaySpeed.UpdateSpeed(speedInit , speedInitSecond);
        if (enableCount)
        {
            CountDown();
        }
    }
}
