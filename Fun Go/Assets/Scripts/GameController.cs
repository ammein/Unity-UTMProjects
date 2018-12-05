using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SingleOrMultiple
{
    SINGLE,
    MULTIPLE
}

[System.Serializable]
public class CameraSettings
{
    public float xMoveCameraMin, xMoveCameraMax, easing, orthoBegin, orthoEnds , rotationX , rotationY, rotationZ;
}

public class GameController : MonoBehaviour {

    [Header("Single Player or Multiplayer")]
    public SingleOrMultiple play;
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

    void Start() {
        spawnPosition = transform.position.z + offsetXMap;
        spawnRotation = transform.rotation;
        StartCoroutine(OutputMap());
        StartCoroutine(CloneObject());
        AllOffset();

        // Make New Camera based on Player Options
        cameraObject = new CameraControl(play, offsetX, offsetY, offsetZ);
        SplitUpdate();
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
        cameraObject.UpdateOnMove(cameraSettings);
        cameraObject.StartMoveCamera();
        cameraObject.FlagCameraUpdate();
        cameraObject.CameraMoveEffect();
    }

    void Update()
    {
        // Update each frame for get All Map Length Value
        GetAllMapLength();

        // To Make it Live Update for all settings
        cameraObject.UpdateOffset(offsetX, offsetY, offsetZ);
        cameraObject.UpdateRotation(cameraSettings.rotationX, cameraSettings.rotationY , cameraSettings.rotationZ);
        SplitUpdate();
        cameraObject.SplitCamera();
        AllOffset();
        DebugCloneJump();
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
            Instantiate(Maps[i], new Vector3(0.0f ,0.0f ,spawnPosition) , spawnRotation);
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
