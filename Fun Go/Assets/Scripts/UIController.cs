using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private double speedInit , speedInitSecond;
    [Header("Coin UI Settings")]
    public GUIStyle CoinUI;

    [Header("Speed UI Settings")]
    public GUIStyle SpeedUI;

    [Header("Countdown UI Settings")]
    public GUIStyle CountdownUI;

    [Header("Countdown UI Settings")]
    public GUIStyle RankUI;

    [Header("Slider UI Player Tracking Settings")]
    [Tooltip("If you want to edit Size of slider , make a padding of a slider each side. And if you wanted to change background , just easily navigate to 'Normal' Collapse Tab")]
    public GUIStyle sliderTracking;
    [SerializeField]
    public Color backgroundSlider;
    [SerializeField]
    public Color thumbSlider;

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
    private UIPlayer uiSliderTracking;
    private UIPlayer uiCoinDisplay;
    private UIPlayer uiRankDisplay;
    private bool splitCam;

    private Boundary getBoundary;

    private float GetZFirst , GetZSecond;

    private int getFirstCoin, getSecondCoin;

    // Use this for initialization
    void Start()
    {
        InitiateCaller();
        GetSpeedValue();
        FetchGameController();
        BoundaryUpdate();
        InstanceUI();
        UpdateCameraSplit();
        StopOrRun(true);
        StartCoroutine(Count(getCount));
    }

    void FetchGameController()
    {
        getCount = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().countStart;
        play = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().play;
    }

    void InstanceUI()
    {
        if(uiPlaySpeed == null && !counting && CheckGameStatus())
            uiPlaySpeed = new UIPlayer(SpeedUI, play);

        if(uiPlayCountdown == null && !counting && CheckGameStatus())
            uiPlayCountdown = new UIPlayer(CountdownUI, play);

        if(uiSliderTracking == null && !counting && CheckGameStatus())
            uiSliderTracking = new UIPlayer(sliderTracking, play, thumbSlider, backgroundSlider);

        if(uiCoinDisplay == null && !counting && CheckGameStatus())
            uiCoinDisplay = new UIPlayer(CoinUI , play);

        if(uiRankDisplay == null && !counting && CheckGameStatus())
            uiRankDisplay = new UIPlayer(RankUI, play);
    }

    void BoundaryUpdate()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                getBoundary = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().boundary;
                GetZFirst = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetZFirstPos();
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                getBoundary = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().boundary;
                GetZFirst = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetZFirstPos();

                // Second Player
                GetZSecond = GameObject.FindGameObjectWithTag("SecondParentPlayer").GetComponent<Mover>().myCar.GetZSecondPos();
                break;

        }
    }

    void UpdateCameraSplit()
    {
        splitCam = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().splitCameraMultiplayer;
    }

    void Update()
    {
        BoundaryUpdate();
        UpdateCameraSplit();
        GetSpeedValue();
        InitiateCaller();
        GetCoinValue();
        if(uiRankDisplay != null && CheckGameStatus())
            uiRankDisplay.NumberPosition();
    }

    private void FixedUpdate()
    {
        if(uiRankDisplay != null && CheckGameStatus())
            uiRankDisplay.NumberPosition();
    }

    void GetSpeedValue()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.Speed;
                break;

            case SingleOrMultiple.MULTIPLE:
                speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.Speed;
                speedInitSecond = GameObject.FindGameObjectWithTag("SecondParentPlayer").GetComponent<Mover>().myCar.Speed;
                break;
        }
    }

    void GetCoinValue()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                getFirstCoin = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetFirstCoin();
                break;

            case SingleOrMultiple.MULTIPLE:
                getFirstCoin = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetFirstCoin();
                getSecondCoin = GameObject.FindGameObjectWithTag("SecondParentPlayer").GetComponent<Mover>().myCar.GetSecondCoin();
                break;
        }
    }

    public void InitiateCaller()
    {
        allInstantiatePlayer = GameObject.FindGameObjectsWithTag("ClonePlayer");

        if(playerCar == null)
            playerCar = GameObject.FindGameObjectWithTag("ParentPlayer");

        if(playerCarSecond == null)
            playerCarSecond = GameObject.FindGameObjectWithTag("SecondParentPlayer");

        if(gameController == null)
            gameController = GameObject.FindGameObjectWithTag("GameController");
        return;
    }

    IEnumerator Count(int value)
    {
        if (CheckGameStatus())
        {
            for (int i = value; i > 0; i--)
            {
                counting = true;
                StopOrRun(true);
                enableCount = true;
                count = i.ToString();
                yield return new WaitForSeconds(1);
                enableCount = false;
                if (i == 1)
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

    public bool CheckGameStatus()
    {
        return gameController.GetComponent<GameController>().sceneGame && gameController.GetComponent<GameController>().async.isDone;
    }

    void CountDown()
    {
        uiPlayCountdown.CountTextArea(count);
        uiPlayCountdown.DisplayArea(splitCam);
    }

    private void OnGUI()
    {
        if(uiSliderTracking != null && CheckGameStatus())
            uiSliderTracking.SliderTracking(GetZFirst, GetZSecond, splitCam, getBoundary);

        if(uiPlaySpeed != null && CheckGameStatus())
        {
            uiPlaySpeed.UpdateSpeed(speedInit, speedInitSecond);
            uiPlaySpeed.DisplayArea(splitCam);
        }

        if (uiCoinDisplay != null && CheckGameStatus())
        {
            uiCoinDisplay.DisplayArea(splitCam);
            uiCoinDisplay.UpdateCoinValue(getFirstCoin, getSecondCoin);
        }

        if(uiRankDisplay != null && CheckGameStatus())
        {
            uiRankDisplay.DisplayRankArea(splitCam);
            uiRankDisplay.DisplayRank();
        }

        if (enableCount)
        {
            CountDown();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("firstPlayer", getFirstCoin);
        PlayerPrefs.SetInt("secondPlayer", getSecondCoin);
    }
}
