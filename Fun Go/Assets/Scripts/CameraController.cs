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
    public GameObject player;
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
        playerPosition = player.transform.position.x;
        playerSpeed = player.GetComponent<Rigidbody>().velocity.x;
        cameraSpeed = Camera.main.velocity.x;
        moveCam += playerPosition * cameraSettings.easing;
        moveCam = Mathf.Clamp(moveCam, cameraSettings.xMoveCameraMin, cameraSettings.xMoveCameraMax);
        orthoMove += cameraSpeed * 0.1f;
        orthoMove = Mathf.Clamp(orthoMove, cameraSettings.orthoBegin, cameraSettings.orthoEnds);
        //transform.position = player.transform.position + offset;
        //if(playerSpeed > 1)
        //{
        //    transform.position = (player.transform.position + new Vector3(moveCam, 0.0f, 0.0f)) + offset;
        //}
        //else if(playerSpeed <= 0)
        //{
        //    transform.position = (player.transform.position + new Vector3(moveCam, 0.0f, 0.0f)) + offset;
        //}
        if (playerSpeed >= moveCam)
        {
            flagCamera = true;
        }
        else if (playerSpeed <= moveCam)
        {
            flagCamera = false;
        }
        onMove();
        //Debug.Log("MoveCamera Value : " + moveCam);
        //Debug.Log("Test" + (Camera.main.transform.position.x + Camera.main.velocity.x));
    }

    void onMove()
    {
        if (flagCamera)
        {
            Camera.main.orthographicSize = orthoMove;
            transform.position = player.transform.position + new Vector3(moveCam, 0.0f, 0.0f) + offset;
            //Debug.Log("Enable Flag Camera");
        }
        else
        {
            Camera.main.orthographicSize = orthoMove;
            transform.position = player.transform.position + new Vector3(moveCam - moveCam, 0.0f, 0.0f) + offset;
            //Debug.Log("Disable Flag Camera");
        }
    }
}
