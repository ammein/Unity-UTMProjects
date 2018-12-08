using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;

public class UIPlayer
{
    private float width;
    private float height;
    private Rect displayPosition , firstPosition , secondPosition;
    private GUIStyle uiSetting;
    private double speedInit , speedInitSecond;
    private int firstCoin, secondCoin;
    public string count;
    private SingleOrMultiple play;

    private GUIStyle sliderThumb;
    private GUIStyle sliderBackground;
    private Texture2D whitePixel;
    private Texture2D blackPixel;

    private Color thumb;
    private Color background;

    public int firstPlayerPosition , secondPlayerPosition;

    private GameObject[] allPlayers;

    private int rankFirst , rankSecond;

    // Methods to find layer
    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {
        GameObject[] goArray = Object.FindObjectsOfType<GameObject>();
        List<GameObject> goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

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

    public UIPlayer(GUIStyle UIStyle , SingleOrMultiple playOption, Color thumbColor , Color backgroundColor)
    {
        play = playOption;
        uiSetting = UIStyle;
        // Get Current UI W & H
        width = Screen.width;
        height = Screen.height;
        uiSetting.fixedWidth = width;
        uiSetting.fixedHeight = height;

        thumb = thumbColor;
        whitePixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        whitePixel.SetPixel(0, 0, new Color(thumb.r , thumb.g , thumb.b));
        whitePixel.Apply();

        background = backgroundColor;
        blackPixel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        blackPixel.SetPixel(0, 0, new Color(background.r , background.g , background.b));
        blackPixel.Apply();

        sliderBackground = new GUIStyle();
        sliderBackground.padding = new RectOffset(uiSetting.padding.left, uiSetting.padding.right, uiSetting.padding.top, uiSetting.padding.bottom);
        sliderBackground.normal.background = blackPixel;
        sliderBackground.hover.background = blackPixel;
        sliderBackground.active.background = blackPixel;
        sliderBackground.focused.background = blackPixel;

        sliderThumb = new GUIStyle();
        sliderThumb.stretchHeight = true;
        sliderThumb.fixedWidth = 20f;
        sliderThumb.normal.background = whitePixel;
        sliderThumb.hover.background = whitePixel;
        sliderThumb.active.background = whitePixel;
        sliderThumb.focused.background = whitePixel;
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
                    secondPosition = new Rect(0, 0 + uiSetting.contentOffset.y, 100, 50);
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

    public void UpdateCoinValue(int first , int second)
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                firstCoin = first;
                GUI.TextArea(firstPosition, "Coin : " + first.ToString(), uiSetting);
                break;

            case SingleOrMultiple.MULTIPLE:
                firstCoin = first;
                GUI.TextArea(firstPosition, "Coin : " + first.ToString(), uiSetting);
                secondCoin = second;
                GUI.TextArea(secondPosition, "Coin : " + first.ToString(), uiSetting);
                break;
        }
    }

    void DisplayCoin()
    {

    }

    public void SliderTracking(float zFirstPos , float zSecondPos , bool split , Boundary boundary)
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                // First Player
                firstPosition = new Rect((-width / 2) + uiSetting.contentOffset.x, 0 + 18.0f, Screen.width / 1.25f, 30);
                zFirstPos = GUI.HorizontalSlider(firstPosition, zFirstPos, 0, boundary.zMax , sliderBackground, sliderThumb);
                break;

            case SingleOrMultiple.MULTIPLE:
                if (split)
                {
                    // First Player
                    firstPosition = new Rect((-width / 2) + uiSetting.contentOffset.x, 0 + 18.0f, Screen.width / 2.8f, 30);
                    zFirstPos = GUI.HorizontalSlider(firstPosition, zFirstPos, 0, boundary.zMax, sliderBackground, sliderThumb);
                    // Second Player
                    secondPosition = new Rect(0 + uiSetting.contentOffset.x, 0 + 18.0f, Screen.width / 2.8f, 30);
                    zSecondPos = GUI.HorizontalSlider(secondPosition, zSecondPos, 0, boundary.zMax, sliderBackground, sliderThumb);
                    return;
                }
                else
                {
                    // First Player
                    firstPosition = new Rect(20, (-height / 2) + uiSetting.contentOffset.y, Screen.width / 1.25f, 30);
                    zFirstPos = GUI.HorizontalSlider(firstPosition, zFirstPos, 0, boundary.zMax, sliderBackground, sliderThumb);

                    // Second Player
                    secondPosition = new Rect(20, 0 + uiSetting.contentOffset.y, Screen.width / 1.25f, 30);
                    zSecondPos = GUI.HorizontalSlider(secondPosition, zSecondPos, 0, boundary.zMax, sliderBackground, sliderThumb);
                }
                break;
        }
    }

    public void NumberPosition()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                allPlayers = UIPlayer.FindGameObjectsWithLayer(9);
                rankFirst = 1;
                for(int r = 1; r < allPlayers.Length; r++)
                {
                    if(DistanceFromStart("ParentPlayer") < DistanceFromStart(allPlayers[r - 1].name))
                    {
                        rankFirst++;
                    }
                }
                 break;

            case SingleOrMultiple.MULTIPLE:
                allPlayers = UIPlayer.FindGameObjectsWithLayer(9);
                rankFirst = 1;
                for (int r = 1; r < allPlayers.Length; r++)
                {
                    Debug.Log("All Players = " + allPlayers[r]);
                    if (DistanceFromStart("ParentPlayer") < DistanceFromStart(allPlayers[r - 1].name))
                    {
                        rankFirst++;
                    }

                    if (DistanceFromStart("SecondParentPlayer") < DistanceFromStart(allPlayers[r - 1].name))
                    {
                        rankSecond++;
                    }
                }
                break;
        }
    }

    public void DisplayRank()
    {
        switch (play)
        {
            case SingleOrMultiple.SINGLE:
                GUI.TextArea(firstPosition, rankFirst.ToString() + " / " + allPlayers.Length.ToString(), uiSetting);
                break;

            case SingleOrMultiple.MULTIPLE:
                GUI.TextArea(firstPosition,rankFirst.ToString() + " / " + allPlayers.Length.ToString(), uiSetting);
                GUI.TextArea(secondPosition,rankSecond.ToString() + " / " + allPlayers.Length.ToString(), uiSetting);
                break;
        }
    }

    private float DistanceFromStart(string playerIndex)
    {
        return GameObject.FindGameObjectWithTag(playerIndex).transform.position.z - 0;
    }

    public void CountTextArea(string countNow)
    {
        count = countNow;
        GUI.TextArea(displayPosition, count, uiSetting);
    }
}
