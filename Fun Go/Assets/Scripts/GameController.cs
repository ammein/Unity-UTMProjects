using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

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

public class GameController : MonoBehaviour {

    public static GameController control;

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
    [Header("Get Second Car Object")]
    public GameObject secondCar;
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

    private float spawnPosition;
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

    private int firstCoin;
    private int secondCoin;

    private bool sceneGame = false;


    // TODO : Load Coin with scene changes
    //void Awake()
    //{
    //    // To make it persist data without deleting it
    //    // Source : https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data
    //    if (control == null)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //        control = this;
    //    }
    //    else if (control != this)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    void Start() {
        CurrentActiveScene();
    }

    void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    void CurrentActiveScene()
    {
        currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Game")
        {
            // Initialize and start all
            spawnPosition = transform.position.z + offsetXMap;
            spawnRotation = transform.rotation;
            StartCoroutine(OutputMap());
            StartCoroutine(CloneObject());
            AllOffset();

            // Make New Camera based on Player Options
            cameraObject = new CameraControl(play, offsetX, offsetY, offsetZ);
            SplitUpdate();

            // TODO : Persist and Load Coin
//            firstCoin = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetFirstCoin();
//            secondCoin = GameObject.FindGameObjectWithTag("SecondParentPlayer").GetComponent<Mover>().myCar.GetSecondCoin();
//#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
//            if (firstCoin != null || secondCoin != null)
//#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
//            {
//                GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.SetCoin(firstCoin, secondCoin);
//            }
            sceneGame = true;
            homeButton = GameObject.FindGameObjectWithTag("HomeButton").GetComponent<Button>();
            return;
        }
        else
        {
            singlePlayerButton = GameObject.FindGameObjectWithTag("SinglePlayerButton").GetComponent<Button>();
            multiPlayerButton = GameObject.FindGameObjectWithTag("MultiPlayerButton").GetComponent<Button>();
            sceneGame = false;
            GetSecondCoin();
            GetFirstCoin();
            return;
        }
    }


    void OnGUI()
    {
        if (!sceneGame)
        {
            if (singlePlayerButton != null)
            {
                singlePlayerButton.onClick.AddListener(delegate
                {
                    LoadScene("Game");
                });
            }
            return;
        }
        else
        {
            if (homeButton != null)
            {
                homeButton.onClick.AddListener(delegate
                {
                    LoadScene("MainMenu");
                });
            }
            return;
        }
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

    void Update()
    {
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
        for (int i = 0; i < Maps.Length; i++)
        {
            // Random Selected Map
            int random = Random.Range(0, Maps.Length);
            // Clone The Map in total of maps
            Instantiate(Maps[random], new Vector3(0.0f ,0.0f ,spawnPosition) , spawnRotation);
            spawnPosition += Maps[i].transform.GetChild(0).transform.position.z;
            spawnRotation *= Maps[i].transform.GetChild(0).transform.rotation;
            yield return new WaitForSeconds(1);
            if (i == (Maps.Length - 1))
            {
                yield break;
            }
        }
        yield return null;
    }

    public float GetAllMapLength()
    {
        return spawnPosition;
    }

    IEnumerator CloneObject()
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
        yield return null;
    }
}
