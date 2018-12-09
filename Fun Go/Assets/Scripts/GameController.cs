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
public class CloneAccessoriesController
{
    [Header("Enable/Disable All Clone Accesories : ")]
    public bool enableClone = true;
    [Space(10.0f , order = 0)]
    [Header("Clone Type : ")]
    public bool body;
    public bool tyre;
    public bool rear;
    public bool roof;
}

[System.Serializable]
public class Accessories
{
    [Header("Upload Your Accessories Here")]
    public GameObject[] roof;
    public GameObject[] body;
    public GameObject[] tyre;
    public GameObject[] rear;

    [Header("Enable/Disable Random Accessories")]
    public CloneAccessoriesController clone;
}

[System.Serializable]
public class PlayerPreferences
{
    public int firstCoinPlayer, secondCoinPlayer;
}

public class GameController : MonoBehaviour {

    public static GameController control;

    [Header("Coins Player")]
    public PlayerPreferences playerPreferences;

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
    [Range(1.0f, 10.0f)]
    public int numberSpawn;

    [Header("Get All Map Prefabs")]
    public GameObject[] Maps;

    [Header("All Accessories Prefabs")]
    public Accessories accessory;

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
    [HideInInspector]
    public int countScene;

    public AsyncOperation async;

    private bool sceneGame = false;
    [HideInInspector]
    public bool loadingScene = true;
    [Header("Loading Text UI Object")]
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

        loadingText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log("On Enable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        Debug.Log("On Scene Loaded " + scene.name);
        loadingScene = true;
        Debug.Log(mode);
    }


    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        if (sceneGame)
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
        if (sceneGame)
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
            playerPreferences.firstCoinPlayer = PlayerPrefs.GetInt("firstPlayer");
            playerPreferences.secondCoinPlayer = PlayerPrefs.GetInt("secondPlayer");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // Source : http://myriadgamesstudio.com/how-to-use-the-unity-scenemanager/
    IEnumerator AsyncLoadScene(string scene)
    {
        //// deactivate scene
        //async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        //async.allowSceneActivation = false;
        //loadingScene = true;
        //while (!async.isDone)
        //{
        //    Debug.Log("Build Index = " + SceneManager.GetActiveScene().buildIndex);
        //    countScene = SceneManager.sceneCount;
        //    Debug.Log((double)async.progress);
        //    if(async.progress >= 0.9f)
        //    {
        //        if(countScene == 1)
        //        {
        //            async.allowSceneActivation = true;
        //        }
        //        Scene nextScene = SceneManager.GetSceneByName(scene);
        //        SceneManager.SetActiveScene(nextScene);
        //    }
        //    yield return null;
        //}

        // deactivate scene
        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        // this value stops the scene from displaying when it's finished loading
        loadingScene = true;
        while (!async.isDone)
        {
            countScene = SceneManager.sceneCount;
            loadingText.text = async.progress.ToString();
            Debug.Log("Count Scene = " + countScene);
            if (countScene == 1)
            {
                loadingScene = false;
            }
        }
        yield return null;
    }

    IEnumerator SyncLoadScene(string scene)
    {
        GameObject allObject = GameObject.Find("Canvas");
        allObject.FindObject("TipsTrick").SetActive(true);
        loadingText.gameObject.SetActive(true);
        loadingScene = false;
        yield return new WaitForSeconds(0.5f);
        loadingText.text = "Loading .";
        yield return new WaitForSeconds(0.5f);
        loadingText.text = "Loading . .";
        yield return new WaitForSeconds(0.5f);
        // load the scene asynchronously
        SceneManager.LoadScene(scene);
        // Return IEnumerator to null
        yield return null;
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
                StartCoroutine(SyncLoadScene("Game"));

            });

            multiPlayerButton.onClick.AddListener(delegate
            {
                StartPlay(SingleOrMultiple.MULTIPLE);
                StartCoroutine(SyncLoadScene("Game"));
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
        return playerPreferences.firstCoinPlayer;
    }

    public int GetSecondCoin()
    {
        return playerPreferences.secondCoinPlayer;
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
            if (currentScene.name == "Game")
            {
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
