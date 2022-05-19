using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingBirdController : MonoBehaviour
{
    public float minAnimMod = 0.8f;
    public float maxAnimMod = 1.2f;
    public float rainbowSpeedMin = 0.8f;
    public float rainbowSpeedMax = 1.2f;
    public float minSpinnerMod = 0.7f;
    public float maxSpinnerMod = 1.3f;

    [Header("Debuggy")]
    public GameObject[] agentArray;
    public GameObject[] spinnerArray;

    
    void Start()
    {
        agentArray = GameObject.FindGameObjectsWithTag("EndingBird");
        for (int i = 0; i < agentArray.Length; i++)
        {
            agentArray[i].GetComponent<Animator>().SetFloat("AnimSpeedModifier", Random.Range(minAnimMod, maxAnimMod));
            agentArray[i].GetComponentInChildren<Rainbow>().shiftSpeed = Random.Range(rainbowSpeedMin, rainbowSpeedMax);
        }

        spinnerArray = GameObject.FindGameObjectsWithTag("EndingBirdSpinner");
        for (int i = 0; i < spinnerArray.Length; i++)
        {
            spinnerArray[i].GetComponent<Animator>().SetFloat("AnimSpeedModifier", Random.Range(minSpinnerMod, maxSpinnerMod));
        }
    }

}
