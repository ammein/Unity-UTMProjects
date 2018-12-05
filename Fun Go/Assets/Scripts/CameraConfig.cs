using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl
{
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject playerSecond;
    [HideInInspector]
    public Rigidbody playerRigid;
    [HideInInspector]
    public Rigidbody playerRigidSecond;
    private Vector3 offset;
    private Vector3 offsetSecond;
    private bool flagCamera , flagCameraSecond;
    private float playerPosition;
    private float playerPositionSecond;
    private float playerSpeed;
    private float playerSpeedSecond;
    private float cameraSpeed;
    private float cameraSpeedSecond;
    private GameObject gameObject;
    private GameObject gameObjectSecond;
    private float orthoMove;
    private float orthoMoveSecond;
    private float moveCam;
    private float moveCamSecond;

    // Camera
    public Camera cameraSingle;
    public Camera cameraDouble;
    public bool SplitRightOrSplitBottom { get; set; }

    private SingleOrMultiple NumOfPlayer;

    private Quaternion defaultRotation;

    public CameraControl(SingleOrMultiple play , float offsetCamX , float offsetCamY, float offsetCamZ)
    {
        NumOfPlayer = play;
        defaultRotation = new Quaternion(0.0f, -0.7f, 0.0f, 0.7f);
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                gameObject = new GameObject();
                player = GameObject.Find("PlayerCar").transform.Find("Base").gameObject;
                playerRigid = GameObject.Find("PlayerCar").transform.Find("Base").GetComponent<Rigidbody>();
                gameObject.name = "FirstPlayerCamera";
                gameObject.tag = "PrimaryCamera";
                CameraPlayerOne(gameObject);
                offset = new Vector3(player.transform.position.x + offsetCamX, player.transform.position.y + offsetCamY, player.transform.position.z + offsetCamZ) - player.transform.position;
                flagCamera = false;
                break;

            case SingleOrMultiple.MULTIPLE:
                gameObject = new GameObject();
                gameObjectSecond = new GameObject();
                player = GameObject.Find("PlayerCar").transform.Find("Base").gameObject;
                playerRigid = GameObject.Find("PlayerCar").transform.Find("Base").GetComponent<Rigidbody>();
                playerSecond = GameObject.Find("SecondPlayerCar").transform.Find("Base").gameObject;
                playerRigidSecond = GameObject.Find("SecondPlayerCar").transform.Find("Base").GetComponent<Rigidbody>();
                gameObject.name = "FirstPlayerCamera";
                gameObject.tag = "PrimaryCamera";
                CameraPlayerOne(gameObject);
                gameObjectSecond.name = "SecondPlayerCamera";
                gameObjectSecond.tag = "SecondaryCamera";
                CameraPlayerTwo(gameObjectSecond);
                SplitCamera();
                offset = new Vector3(player.transform.position.x + offsetCamX, player.transform.position.y + offsetCamY, player.transform.position.z + offsetCamZ) - player.transform.position;
                offsetSecond = new Vector3(playerSecond.transform.position.x + offsetCamX, playerSecond.transform.position.y + offsetCamY, playerSecond.transform.position.z + offsetCamZ) - playerSecond.transform.position;

                flagCamera = false;
                flagCameraSecond = false;
                break;
        }
    }

    void CameraPlayerOne(GameObject gameObject)
    {
        cameraSingle = gameObject.AddComponent<Camera>();
        gameObject.AddComponent<AudioListener>();
        gameObject.transform.rotation = defaultRotation;
        // Enable Current Camera
        cameraSingle.enabled = true;
        cameraSingle.orthographic = true;
        cameraSingle.orthographicSize = 5;
        cameraSingle.depth = -1;
        cameraSingle.farClipPlane = 1000;
        cameraSingle.nearClipPlane = 0.3f;
    }

    void CameraPlayerTwo(GameObject gameObject)
    {
        cameraDouble = gameObject.AddComponent<Camera>();
        gameObject.AddComponent<AudioListener>();
        gameObject.transform.rotation = defaultRotation;
        // Enable Current Camera
        cameraDouble.enabled = true;
        cameraDouble.orthographic = true;
        cameraDouble.orthographicSize = 5;
        cameraDouble.depth = -1;
        cameraDouble.farClipPlane = 1000;
        cameraDouble.nearClipPlane = 0.3f;
    }

    public void SplitCamera()
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.MULTIPLE:
                if (SplitRightOrSplitBottom)
                {
                    cameraSingle.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
                    cameraDouble.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
                }
                else
                {
                    cameraSingle.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                    cameraDouble.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                }
                break;

            case SingleOrMultiple.SINGLE:
                break;
        }
    }

    public void UpdateOffset(float offsetCamX , float offsetCamY, float offsetCamZ)
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                offset = new Vector3(player.transform.position.x + offsetCamX, player.transform.position.y + offsetCamY, player.transform.position.z + offsetCamZ) - player.transform.position;
                break;
            case SingleOrMultiple.MULTIPLE:
                offset = new Vector3(player.transform.position.x + offsetCamX, player.transform.position.y + offsetCamY, player.transform.position.z + offsetCamZ) - player.transform.position;
                offsetSecond = new Vector3(playerSecond.transform.position.x + offsetCamX, playerSecond.transform.position.y + offsetCamY, playerSecond.transform.position.z + offsetCamZ) - playerSecond.transform.position;
                break;
        }
    }

    public void UpdateRotation(float rotX , float rotY , float rotZ)
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                gameObject.transform.rotation = Quaternion.Euler(rotY, rotX, rotZ) * defaultRotation;
                break;

            case SingleOrMultiple.MULTIPLE:
                gameObject.transform.rotation = Quaternion.Euler(rotY, rotX, rotZ) * defaultRotation;
                gameObjectSecond.transform.rotation = Quaternion.Euler(rotY, rotX, rotZ) * defaultRotation;
                break;
        }
    }

    public void StartMoveCamera()
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                playerPosition = player.transform.position.z;
                playerSpeed = playerRigid.velocity.z;
                cameraSpeed = cameraSingle.velocity.z;
                break;

            case SingleOrMultiple.MULTIPLE:
                playerPosition = player.transform.position.z;
                playerSpeed = playerRigid.velocity.z;
                cameraSpeed = cameraSingle.velocity.z;

                playerPositionSecond = playerSecond.transform.position.z;
                playerSpeedSecond = playerRigidSecond.velocity.z;
                cameraSpeedSecond = cameraDouble.velocity.z;
                break;
        }
    }

    public void FlagCameraUpdate()
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                if (playerRigid.velocity.z > 1)
                {
                    flagCamera = true;
                }
                else if (playerRigid.velocity.z <= 1)
                {
                    flagCamera = false;
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                if (playerRigid.velocity.z > 1)
                {
                    flagCamera = true;
                }
                else if (playerRigid.velocity.z <= 1)
                {
                    flagCamera = false;
                }

                if (playerRigidSecond.velocity.z > 1)
                {
                    flagCamera = true;
                }
                else if (playerRigidSecond.velocity.z <= 1)
                {
                    flagCamera = false;
                }
                break;
        }
    }

    public void UpdateOnMove(CameraSettings cameraSettings)
    {
        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                moveCam += playerSpeed * cameraSettings.easing;
                moveCam = Mathf.Clamp(moveCam, cameraSettings.xMoveCameraMin, cameraSettings.xMoveCameraMax);
                orthoMove += cameraSpeed * 0.1f;
                orthoMove = Mathf.Clamp(orthoMove, cameraSettings.orthoBegin, cameraSettings.orthoEnds);
                break;

            case SingleOrMultiple.MULTIPLE:
                moveCam += playerSpeed * cameraSettings.easing;
                moveCam = Mathf.Clamp(moveCam, cameraSettings.xMoveCameraMin, cameraSettings.xMoveCameraMax);
                orthoMove += cameraSpeed * 0.1f;
                orthoMove = Mathf.Clamp(orthoMove, cameraSettings.orthoBegin, cameraSettings.orthoEnds);

                moveCamSecond += playerSpeedSecond * cameraSettings.easing;
                moveCamSecond = Mathf.Clamp(moveCamSecond, cameraSettings.xMoveCameraMin, cameraSettings.xMoveCameraMax);
                orthoMoveSecond += cameraSpeedSecond * 0.1f;
                orthoMoveSecond = Mathf.Clamp(orthoMoveSecond, cameraSettings.orthoBegin, cameraSettings.orthoEnds);
                break;
        }
    }

    public void CameraMoveEffect()
    {

        switch (NumOfPlayer)
        {
            case SingleOrMultiple.SINGLE:
                if (flagCamera)
                {
                    cameraSingle.orthographicSize = orthoMove;
                    gameObject.transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
                    //Debug.Log("Enable Flag Camera");
                }
                else
                {
                    cameraSingle.orthographicSize = orthoMove;
                    gameObject.transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
                    //Debug.Log("Disable Flag Camera");
                }


                if (Input.GetKeyDown("space"))
                {
                    cameraSingle.orthographicSize = orthoMove++;
                }
                break;

            case SingleOrMultiple.MULTIPLE:
                if (flagCamera)
                {
                    cameraSingle.orthographicSize = orthoMove;
                    gameObject.transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
                    //Debug.Log("Enable Flag Camera");
                }
                else
                {
                    cameraSingle.orthographicSize = orthoMove;
                    gameObject.transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
                    //Debug.Log("Disable Flag Camera");
                }

                if (Input.GetKeyDown("space"))
                {
                    cameraSingle.orthographicSize = orthoMove++;
                }

                if (flagCameraSecond)
                {
                    cameraDouble.orthographicSize = orthoMoveSecond;
                    gameObjectSecond.transform.position = playerSecond.transform.position + new Vector3(0.0f, 0.0f, moveCamSecond) + offset;
                    //Debug.Log("Enable Flag Camera");
                }
                else
                {
                    cameraDouble.orthographicSize = orthoMoveSecond;
                    gameObjectSecond.transform.position = playerSecond.transform.position + new Vector3(0.0f, 0.0f, moveCamSecond) + offset;
                    //Debug.Log("Disable Flag Camera");
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    cameraDouble.orthographicSize = orthoMoveSecond++;
                }
                break;
        }
    }
}
