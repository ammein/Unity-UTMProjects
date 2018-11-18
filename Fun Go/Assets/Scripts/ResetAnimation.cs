using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimation : MonoBehaviour {

    [Header("Blink Timing Animations Config :" , order = 0)]
    public float blinkGap;
    public int blinkMax;
    [HideInInspector]
    public int blinkCount;
    private bool exitLoop;

    public IEnumerator UpdatePosAnimation(GameObject rb , GameObject baseObject)
    {
        while (true)
        {
            exitLoop = false;
            if (baseObject.transform.position.z > 50 && baseObject.transform.position.z < 200)
            {
                for (int i = 0; i <= blinkMax; i++)
                {
                    rb.SetActive(!rb.activeInHierarchy);
                    blinkCount++;
                    yield return new WaitForSeconds(blinkGap);
                    if(blinkCount == blinkMax)
                    {
                        exitLoop = true;
                        yield break;
                    }
                }
            }
            if (exitLoop)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }
    }
}