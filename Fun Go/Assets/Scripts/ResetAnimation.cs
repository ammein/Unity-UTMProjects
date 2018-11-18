using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimation : MonoBehaviour {

    [Header("Blink Timing Animations Config :" , order = 0)]
    public float blinkGap;
    public int blinkMax;
    [HideInInspector]
    public int blinkCount;

    public IEnumerator UpdatePosAnimation(GameObject rb)
    {
        yield return new WaitForSeconds(0);
        while (blinkCount <= blinkMax)
        {
            rb.SetActive(!rb.activeInHierarchy);
            blinkCount += 1;
            yield return new WaitForSeconds(blinkGap);
            if (blinkCount == blinkMax)
            {
                blinkCount = 0;
                break;
            }
        }
    }

    
}
