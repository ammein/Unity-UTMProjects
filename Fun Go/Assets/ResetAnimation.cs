using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimation : MonoBehaviour {

    private GameObject rootCar;
    [Header("Blink Timing Animations Config :" , order = 0)]
    public float blinkGap;
    public int blinkMax;
    public float blinkDuration;

    // Use this for initialization
    void Start()
    {
        rootCar = GetComponent<GameObject>();    
    }

    public IEnumerator UpdatePosAnimation(GameObject baseObject , Rigidbody rb)
    {
        while (true)
        {
            for(int i = 0; i < blinkMax; i++)
            {
                rootCar.SetActive(!rootCar.activeInHierarchy);
                if (i ==  blinkMax)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(blinkGap);
        }
    }

    
}
