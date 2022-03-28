using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//required to use ui elements

public class UIManager : MonoBehaviour
{
    public Image swarmSizeIndicator;
    public FlockManager flockManager; //needed to check flock size
    public float flockTargetSize; //current target number of flock units to progress

    [Header("InitialAnimation")]
    public Animator IndicatorAnimator;

    [Header("IndicatorSmoothing")]
    public float smoothTime;
    private float indicatorVelocityRef;

    private float indicatorAmount; //how full to make the radial indicator, 0 - 1
    private bool indicatorInitialized = false; //bool for whether the ui intro animation has activated so it doesnt play twice
    private bool indicatorFilled = false; //another bool for the animator



    private void Update()
    {
        if (flockManager.playerFlockSize == 1 && indicatorInitialized == false)
        {
            IndicatorInitialize();
        }

        if (flockManager.playerFlockSize > 1)
        {
            indicatorAmount = flockManager.playerFlockSize / flockTargetSize;
            //swarmSizeIndicator.fillAmount = indicatorAmount;
        }

        if (flockManager.playerFlockSize == flockTargetSize && indicatorFilled == false)
        {
            indicatorFilled = true;
            IndicatorAnimator.SetTrigger("IndicatorFilled");
        }

        if (flockManager.playerFlockSize != flockTargetSize)
        {
            indicatorFilled = false; //resets the indicator toggle 
        }
        swarmSizeIndicator.fillAmount = Mathf.SmoothDamp(swarmSizeIndicator.fillAmount, indicatorAmount, ref indicatorVelocityRef, smoothTime);
    }

    void IndicatorInitialize()
    {
        indicatorInitialized = true;
        IndicatorAnimator.SetTrigger("IndicatorInitialize");
        StartCoroutine(waitForInitialize(1f));
    }

    IEnumerator waitForInitialize(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        indicatorAmount = flockManager.playerFlockSize / flockTargetSize;
    }

    public void IndicatorShake()
    {
        
        IndicatorAnimator.SetTrigger("IndicatorShake");
    }
}
