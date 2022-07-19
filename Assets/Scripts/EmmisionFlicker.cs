using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmisionFlicker : MonoBehaviour
{
    //disables and enables emission for a flickering light effect 
    public Material material;
    public float minOnTime = 0.1f;
    public float maxOnTime = 1f;
    public float minOffTime = 0.1f;
    public float maxOffTime = 2f;

    private void Start()
    {
        StartCoroutine(FlickerOn(Random.Range(minOnTime, maxOnTime)));
    }

    IEnumerator FlickerOn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        material.EnableKeyword("_EMISSION");
        StartCoroutine(FlickerOff(Random.Range(minOffTime, maxOffTime)));
    }

    IEnumerator FlickerOff(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        material.DisableKeyword("_EMISSION");
        StartCoroutine(FlickerOn(Random.Range(minOnTime, maxOnTime)));
    }
}
