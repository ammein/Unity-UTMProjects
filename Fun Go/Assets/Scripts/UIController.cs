using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    private double speedInit;
    private Rect displayPosition;
    [Header("Make Your UI Settings Here :")]
    public GUIStyle GUISettings;
    private float width;
    private float height;

    // Use this for initialization
    void Start()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().Speed;
        // Get Current UI W & H
        width = Screen.width;
        height = Screen.height;
        GUISettings.fixedWidth = width;
        GUISettings.fixedHeight = height;
    }

    void Update()
    {
        speedInit = GameObject.FindGameObjectWithTag("ParentPlayer").GetComponent<Mover>().myCar.GetSpeed();
    }

    private void OnGUI()
    {
        displayPosition = new Rect(0 + GUISettings.contentOffset.x, 0 + GUISettings.contentOffset.y , 100 , 50);
        GUI.TextArea(displayPosition, speedInit.ToString("F2") + " km/hr", GUISettings);
    }
}
