using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Rigidbody car;
    private float speed;
    public Text speedText;

	// Use this for initialization
	void Start () {
        car = car.GetComponent<Rigidbody>();
        speed = 0;
        speedText.text = "Speed : " + speed.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        speed = car.velocity.z;
        UpdateSpeed();
	}

    void UpdateSpeed()
    {
        speedText.text = "Speed : " + speed;
    }
}
