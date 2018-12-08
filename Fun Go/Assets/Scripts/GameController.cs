using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using ExtensionMethods;

public enum SingleOrMultiple
{
    SINGLE,
    MULTIPLE
}

[System.Serializable]
public class CameraSettings
{
    public float xMoveCameraMin, xMoveCameraMax, easing, orthoBegin, orthoEnds , orthoBeginSplit , orthoEndsSplit , rotationX , rotationY, rotationZ;
}

[System.Serializable]
public class CoinPlayer
{
    public int firstPlayer, secondPlayer;
}

public class GameController : MonoBehaviour {

    public static GameController control;

    [Header("Coins Player")]
    public CoinPlayer coin;

    [Header("Single Player or Multiplayer")]
    public SingleOrMultiple play;
    private Button singlePlayerButton;
    private Button multiPlayerButton;
    private Button homeButton;
    [Header("Horizontal or Vertical")]
    public bool splitCameraMultiplayer;
    [Header("Get Clone Car Object")]
    public GameObject car;
    [Header("Debug for Making All Clone Jump")]
    public bool cloneJump = false;
    [HideInInspector]
    public GameObject spawn;
    [HideInInspector]
    public GameObject obj;
    [Header("Spawn Settings")]
    [Range(0.1f , 1.0f)]
    public float delaySpawnCar;
    [Range(1.0f, 5.0f)]
    public int numberSpawn;

    [Header("Get All Map Prefabs")]
    public GameObject[] Maps;

    [Header("Stop Before Finish Line Offset")]
    public float finishLine;

    private float[] mapLength;

    private float spawnPosition = 0;
    private Quaternion spawnRotation;
    [Header("Offset Map Position on First Spawn")]
    [Range(0, -10)]
    public float offsetXMap;
    [Header("Countdown Settings")]
    [Range(0, 5)]
    public int countStart;

    [Header("Offset Camera From Player Position")]
    public float offsetCamX;
    public float offsetCamY;
    public float offsetCamZ;

    [Header("Camera Settings")]
    public CameraSettings cameraSettings;
    [HideInInspector]
    public CameraControl cameraObject;

    private float offsetX;
    private float offsetY;
    private float offsetZ;
    private bool updateCloneJump;

    private Scene currentScene;

    public AsyncOperation async;

    public int firstCoin;
    public int secondCoin;

    private bool sceneGame = false;

    public bool loadingScene = false;

    public Text loadingText;


    // TODO : Load Coin with scene changes
    void Awake()
    {
        // To make it persist data without deleting it
        // Source : https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
    }


    // Run Start Here
    void Start()
    {
        spawnPosition = transform.position.z + offsetXMap;
        spawnRotation = transform.rotation;
        CurrentActiveScene();
        InitCoroutine();
    }


    void LateUpdate()
    {
        // Everything for all camera to move
        // Only move when all physics calculation rendered
        if (sceneGame && !loadingScene)
        {
            cameraObject.UpdateOnMove(cameraSettings);
            cameraObject.StartMoveCamera();
            cameraObject.FlagCameraUpdate();
            cameraObject.CameraMoveEffect();
            return;
        }
    }

    // Update Here
    void Update()
    {
        GetScene();
        CurrentActiveScene();
        UpdateLoadingScene();
        if (sceneGame && !loadingScene)
        {
            // Update each frame for get All Map Length Value
            GetAllMapLength();
            // To Make it Live Update for all settings
            cameraObject.UpdateOffset(offsetX, offsetY, offsetZ);
            cameraObject.UpdateRotation(cameraSettings.rotationX, cameraSettings.rotationY, cameraSettings.rotationZ);
            SplitUpdate();
            cameraObject.SplitCamera();
            AllOffset();
            DebugCloneJump();
            return;
        }

        // Send Value to disk. This only for coin update value
        if (!PlayerPrefs.HasKey("firstPlayer") && !PlayerPrefs.HasKey("secondPlayer"))
        {
            PlayerPrefs.SetInt("firstPlayer", 0);
            PlayerPrefs.SetInt("secondPlayer", 0);
        }
        else
        {
            firstCoin = PlayerPrefs.GetInt("firstPlayer");
            secondCoin = PlayerPrefs.GetInt("secondPlayer");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Source : http://myriadgamesstudio.com/how-to-use-the-unity-scenemanager/
    IEnumerator LoadScene(int scene)
    {
        GameObject allObject = GameObject.Find("Canvas");
        allObject.FindObject("TipsTrick").SetActive(true);
        yield return new WaitForSeconds(1);
        // load the scene asynchronously
        SceneManager.LoadScene(scene);
        // Return IEnumerator to null
        yield return null;
        // deactivate scene
        //async.allowSceneActivation = false;
        // Flag the loading scene to be tag onto other conditions
        //loadingScene = true;
        //while (!async.isDone)
        //{
        //if (loadingText != null)
        //{
        //    loadingText.text = "Loading progress: " + (async.progress * 100) + "%";
        //}

        //if ((double)async.progress >= 0.9f)
        //{
        //    Debug.Log("Progress : " + (double)async.progress);
        //    async.allowSceneActivation = true;
        //    if (loadingText != null)
        //    {
        //        loadingText.text = "Game Loaded";
        //    }
        //    yield return new WaitForSeconds(1);
        //    Destroy(loadingText);
        //    loadingScene = false;
        //    // Activate back the scene
        //    Debug.Log("Allow Scene Activate " + async.allowSceneActivation);
        //    Debug.Log("Async Done ? " + async.isDone);
        //}

        //    if((double)async.progress >= 0.9f)
        //    {
        //        loadingScene = false;
        //    }
        //    yield return null;
        //}
    }

    public Scene GetScene()
    {
        return currentScene = SceneManager.GetActiveScene();
    }

    public bool UpdateLoadingScene()
    {
        return loadingScene;
    }

    void CurrentActiveScene()
    {
        if(currentScene.name == "Game")
        {
            Debug.Log("Running Game");
            // Initialize and start all
            AllOffset();
            // Make New Camera based on Player Options
            SplitUpdate();
            sceneGame = true;
            homeButton = GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>();
            return;
        }
        if(currentScene.name == "MainMenu")
        {
            singlePlayerButton = GameObject.FindGameObjectWithTag("SinglePlayerButton").GetComponent<Button>();
            multiPlayerButton = GameObject.FindGameObjectWithTag("MultiPlayerButton").GetComponent<Button>();
            singlePlayerButton.onClick.AddListener(delegate
            {
                StartPlay(SingleOrMultiple.SINGLE);
                StartCoroutine(LoadScene(1));

            });

            multiPlayerButton.onClick.AddListener(delegate
            {
                StartPlay(SingleOrMultiple.MULTIPLE);
                StartCoroutine(LoadScene(1));
            });
            sceneGame = false;
            GetSecondCoin();
            GetFirstCoin();
            return;
        }
    }

    void InitCoroutine()
    {
        StartCoroutine(OutputMap());
        StartCoroutine(CloneObject());
        StartCoroutine(StartCamera());
    }

    void StartPlay(SingleOrMultiple playType)
    {
        play = playType;
    }

    public int GetFirstCoin()
    {
        return firstCoin;
    }

    public int GetSecondCoin()
    {
        return secondCoin;
    }

    void AllOffset()
    {
        offsetX = offsetCamX;
        offsetY = offsetCamY;
        offsetZ = offsetCamZ;
        return;
    }

    void SplitUpdate()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                cameraObject.SplitRightOrSplitBottom = false;
                break;

            case SingleOrMultiple.MULTIPLE:
                cameraObject.SplitRightOrSplitBottom = splitCameraMultiplayer;
                break;
        }
    }

    // This triggers when a new value comes in. OnChange Event Using Class Get
    // Source : https://answers.unity.com/questions/1354785/call-a-function-when-a-bool-changes-value.html
    public bool EnableCloneJump
    {
        get
        {
            return updateCloneJump;
        }

        set
        {
            if (value == updateCloneJump)
                return;

            updateCloneJump = value;
            if (updateCloneJump)
            {
                // Debug Purpose Only
                GameObject[] allClone = GameObject.FindGameObjectsWithTag("ClonePlayer");
                foreach (GameObject clone in allClone)
                {
                    clone.GetComponent<Mover>().myCar.cloneFlag = value;
                }
            }
        }
    }

    /// <summary>
    /// Debug Purpose Only
    /// </summary>
    void DebugCloneJump()
    {
        EnableCloneJump = cloneJump;
    }

    IEnumerator OutputMap()
    {
        while (true)
        {
            if (currentScene.name == "Game")
            {
                Debug.Log("Running Map");
                for (int i = 0; i < Maps.Length; i++)
                {
                    // Random Selected Map
                    int random = Random.Range(0, Maps.Length);
                    // Clone The Map in total of maps
                    Instantiate(Maps[random], new Vector3(0.0f, 0.0f, spawnPosition), spawnRotation);
                    spawnPosition += Maps[random].transform.GetChild(0).transform.position.z;
                    spawnRotation *= Maps[random].transform.GetChild(0).transform.rotation;
                    yield return new WaitForSeconds(1);
                    if (i == (Maps.Length - 1))
                    {
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator StartCamera()
    {
        while (true)
        {
            Debug.Log("Running Coroutine Camera : " + loadingScene);
            if (currentScene.name == "Game")
            {
                Debug.Log("Pass through scene = " + currentScene.name);
                cameraObject = new CameraControl(play, offsetX, offsetY, offsetZ);
                yield break;
            }
            yield return null;
        }
    }

    public float GetAllMapLength()
    {
        return spawnPosition;
    }

    IEnumerator CloneObject()
    {
        Debug.Log("Running Clone Car");
        while (true)
        {
            if (currentScene.name == "Game")
            {
                spawn = GameObject.Find("SpawnPlayer").gameObject;
                for (int i = 0; i < numberSpawn; i++)
                {
                    yield return new WaitForSeconds(delaySpawnCar);
                    obj = Instantiate(car, spawn.transform.position, spawn.transform.rotation);
                    if (i == (numberSpawn - 1))
                    {
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
}
