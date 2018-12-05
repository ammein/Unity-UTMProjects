using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer
{
    private float width;
    private float height;
    private Rect displayPosition , firstPosition , secondPosition;
    private GUIStyle uiSetting;
    private double speedInit , speedInitSecond;
    public string count;
    private SingleOrMultiple play;

    public UIPlayer(GUIStyle UIStyle , SingleOrMultiple playOption)
    {
        play = playOption;
        uiSetting = UIStyle;
        // Get Current UI W & H
        width = Screen.width;
        height = Screen.height;
        uiSetting.fixedWidth = width;
        uiSetting.fixedHeight = height;
    }

    public void DisplayArea(bool split)
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                displayPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                firstPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                break;

            case SingleOrMultiple.MULTIPLE:
                displayPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                if (split)
                {
                    // First Player
                    firstPosition = new Rect((-width / 2) + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                    // Second Player
                    secondPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                    return;
                }
                else
                {
                    // First Player
                    firstPosition = new Rect(0 + uiSetting.contentOffset.x, (-height / 2) + uiSetting.contentOffset.y, 100, 50);
                    // Second Player
                    secondPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + uiSetting.contentOffset.y, 100, 50);
                }
                break;


        }
    }

    public void UpdateSpeed(double speed , double speedSecond)
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                speedInit = speed;
                GUI.TextArea(firstPosition, speedInit.ToString("F0") + " km/hr", uiSetting);
                break;

            case SingleOrMultiple.MULTIPLE:
                // First Player
                speedInit = speed;
                GUI.TextArea(firstPosition, speedInit.ToString("F0") + " km/hr", uiSetting);

                speedInitSecond = speedSecond;
                GUI.TextArea(secondPosition, speedInitSecond.ToString("F0") + " km/hr", uiSetting);
                break;
        }
    }

    public void CountTextArea(string countNow)
    {
        count = countNow;
        GUI.TextArea(displayPosition, count, uiSetting);
    }
}
