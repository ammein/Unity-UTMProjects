using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CameraSettings
{
    public float xMoveCameraMin, xMoveCameraMax, easing, orthoBegin, orthoEnds;
}

public class CameraController : MonoBehaviour {
    private Vector3 offset;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Rigidbody playerRigid;
    private bool flags;
    public Vector3 distance;
    public float minFov;
    public float maxFov;
    public float sensitivity;
    private float fov;
    private float playerPosition;
    private float playerSpeed;
    private float cameraSpeed;
    public float onMoveOffset;
    private float moveCam;
    public CameraSettings cameraSettings;
    private bool flagCamera;
    private float orthoMove;


    // Use this for initialization
    void Start () {
        player = GameObject.Find("PlayerCar").transform.Find("Base").gameObject;
        playerRigid = GameObject.Find("PlayerCar").transform.Find("Base").GetComponent<Rigidbody>();
        offset = transform.position - player.transform.position;
        flagCamera = false;
	}

    // Unlock This Event if you want projection zoom in & out
    //void Update()
    //{
    //    // Camera Zoom on Move (Projection)
    //    // Source : https://answers.unity.com/questions/218347/how-do-i-make-the-camera-zoom-in-and-out-with-the.html
    //    float fov = Camera.main.fieldOfView;
    //    fov += Input.GetAxis("Horizontal") * sensitivity;
    //    fov = Mathf.Clamp(fov, minFov, maxFov);
    //    Camera.main.fieldOfView = fov;
    //    //Debug.Log("fov value : " + fov);
    //}

    // Update is called once per frame
    void LateUpdate () {
        playerPosition = player.transform.position.z;
        playerSpeed = playerRigid.velocity.z;
        cameraSpeed = Camera.main.velocity.z;
        moveCam += playerSpeed * cameraSettings.easing;
        moveCam = Mathf.Clamp(moveCam, cameraSettings.xMoveCameraMin, cameraSettings.xMoveCameraMax);
        orthoMove += cameraSpeed * 0.1f;
        orthoMove = Mathf.Clamp(orthoMove, cameraSettings.orthoBegin, cameraSettings.orthoEnds);
        if (playerRigid.velocity.z > 1)
        {
            flagCamera = true;
        }
        else if (playerRigid.velocity.z <= 1)
        {
            flagCamera = false;
        }
        onMove();
        //Debug.Log("Speed Value : " + moveCam);
    }

    void onMove()
    {
        if (flagCamera)
        {
            Camera.main.orthographicSize = orthoMove;
            transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
            //Debug.Log("Enable Flag Camera");
        }
        else
        {
            Camera.main.orthographicSize = orthoMove;
            transform.position = player.transform.position + new Vector3(0.0f, 0.0f, moveCam) + offset;
            //Debug.Log("Disable Flag Camera");
        }

        if (Input.GetKeyDown("space"))
        {
            Camera.main.orthographicSize = orthoMove++;
        }
    }

    void perspectiveMove()
    {
        transform.position = player.transform.position + offset;
        if (playerSpeed > 1)
        {
            transform.position = (player.transform.position + new Vector3(moveCam, 0.0f, 0.0f)) + offset;
        }
        else if (playerSpeed <= 0)
        {
            transform.position = (player.transform.position + new Vector3(moveCam, 0.0f, 0.0f)) + offset;
        }
    }
}
