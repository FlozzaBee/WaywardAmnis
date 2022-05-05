using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//required to use ui elements

public class UIManager : MonoBehaviour
{
    public Image swarmSizeIndicator;
    public Image airSwarmIndicator;
    public Image landSwarmIndicator;
    public FlockManager flockManager; //needed to check flock size
    public AirFlockManager airFlockManager;
    //public LandFlockManager
    public int flockType = 0; //0 = fish, 1 = birds, 2 = goat
    private int playerFlockSize;
    public float flockTargetSize; //current target number of flock units to progress

    [Header("Animation")]
    public Animator seaIndicatorAnimator;
    public Animator airIndicatorAnimator;
    public Animator landIndicatorAnimator;
    private Animator currentIndicatorAnimator;

    [Header("Indicator Smoothing")]
    public float smoothTime;
    private float indicatorVelocityRef;

    private float indicatorAmount; //how full to make the radial indicator, 0 - 1
    private bool indicatorInitialized = false; //bool for whether the ui intro animation has activated so it doesnt play twice
    private bool indicatorFilled = false; //another bool for the animator

    private void Start()
    {
        currentIndicatorAnimator = seaIndicatorAnimator;
    }

    private void Update()
    {
        if (flockType == 0)
        {
            playerFlockSize = flockManager.playerFlockSize;
        }
        if (flockType == 1)
        {
            playerFlockSize = airFlockManager.playerFlockSize;
        }
        /*if (flockType == 2)
        {
            playerFlockSize = landFlockManager.playerFlockSize;
        }*/

        if (playerFlockSize == 1 && indicatorInitialized == false)
        {
            IndicatorInitialize();
        }// plays initialization anim when you pick up first fish

        if (playerFlockSize > 1)
        {
            indicatorAmount = playerFlockSize / flockTargetSize;
            //swarmSizeIndicator.fillAmount = indicatorAmount;
        }

        if (playerFlockSize == flockTargetSize && indicatorFilled == false)
        {
            indicatorFilled = true;
            IndicatorFilled();
        }

        if (playerFlockSize != flockTargetSize)
        {
            indicatorFilled = false; //resets the indicator toggle 
        }
        swarmSizeIndicator.fillAmount = Mathf.SmoothDamp(swarmSizeIndicator.fillAmount, indicatorAmount, ref indicatorVelocityRef, smoothTime);
    }

    void IndicatorInitialize()
    {
        indicatorInitialized = true;
        currentIndicatorAnimator.SetTrigger("IndicatorInitialize");
        StartCoroutine(waitForInitialize(1f));
    }

    IEnumerator waitForInitialize(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        indicatorAmount = playerFlockSize / flockTargetSize;
    }

    public void IndicatorFilled()
    {
        currentIndicatorAnimator.SetTrigger("IndicatorFilled");
    }

    public void IndicatorShake()
    {
        currentIndicatorAnimator.SetTrigger("IndicatorShake");
    }

    public void indicatorOutro()
    {
        currentIndicatorAnimator.SetTrigger("IndicatorOutro");
    }

    public void SwitchIndicatorAir()
    {
        flockType = 1;
        indicatorAmount = 0;
        airSwarmIndicator.enabled = true;
        swarmSizeIndicator = airSwarmIndicator;
        currentIndicatorAnimator = airIndicatorAnimator;
        indicatorInitialized = false;
    }
}
