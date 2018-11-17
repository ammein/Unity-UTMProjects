using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimation : MonoBehaviour {

    [Header("Blink Timing Animations Config :" , order = 0)]
    public float blinkGap;
    public int blinkMax;

    public IEnumerator UpdatePosAnimation(GameObject rb)
    {
        for(int i = 0; i < blinkMax; i++)
        {
            rb.SetActive(!rb.activeInHierarchy);
            yield return new WaitForSeconds(blinkGap);
            if(i == blinkMax)
                break;
        }
    }

    
}
