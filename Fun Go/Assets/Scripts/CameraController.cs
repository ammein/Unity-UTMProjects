using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Vector3 offset;
    public GameObject player;
    private bool flags;
    public Vector3 distance;
    public float minFov;
    public float maxFov;
    public float sensitivity;
    private float fov;


    // Use this for initialization
    void Start () {
        offset = transform.position - player.transform.position;
	}

    void Update()
    {
        // Camera Zoom on Move
        // Source : https://answers.unity.com/questions/218347/how-do-i-make-the-camera-zoom-in-and-out-with-the.html
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Horizontal") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
        Debug.Log("fov value : " + fov);
    }

    // Update is called once per frame
    void LateUpdate () {
        transform.position = player.transform.position + offset;
    }
}
