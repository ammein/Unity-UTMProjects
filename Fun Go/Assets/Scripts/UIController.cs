using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private double speedInit;
    public Text speedText;

	// Use this for initialization
	void Start () {
        speedInit = 0;
        speedText.text = "Speed : " + speedInit;
	}
	
	// Update is called once per frame
	void Update () {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().Speed;
        speedText.text = "Speed : " + speedInit;
    }
}
