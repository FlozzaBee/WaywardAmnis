using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public UIManager uiManager;

    [Header("Building fall event")]
    public FlockManager flockManager;
    public Animator[] BuildingAnim;
    public GameObject buildingFallAgentTarget;
    public int buildingAnimFlockSize = 7;
    public float buildingAnimTime = 2;

    [Header("Door open event")]
    public Animator[] DoorAnim;

    [Header("Hammerhead pass event")]
    public int SharkFlockSizeTarget = 18;
    public Animator hammerheadAnimator; //i was working on this its not finished yet dont forget 
    public GameObject shark;
    public GameObject sharkBarrier;
    public Animation sharkFleeAnim;

    private bool sharkFollow = false; 
     
    
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("animStarting?");
            foreach (Animator animator in BuildingAnim)
            {
                animator.SetBool("IsFallTrigger", true);
            }
        }*/

        if (sharkFollow == true)
        {
            flockManager.eventTransform = shark.transform;
            Debug.Log("shark event should be happening");
        } //tells the boids where the shark is for shark event. 

    }
    public void DoorOpenAnimation() //animation classes are called in ontriggerenter in the character movement scripts.
    {
        foreach(Animator animator in DoorAnim)
        {
            animator.SetTrigger("IsOpenTrigger"); //sets the IsOpenTrigger in each door animatior
        }
    }

    public void BuildingFallAnimation()
    {
        flockManager.eventTransform = buildingFallAgentTarget.transform;
        flockManager.eventInProgress = true;
        StartCoroutine(WaitForBuildingFall(buildingAnimTime)); 
    }

    IEnumerator WaitForBuildingFall(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        flockManager.eventInProgress = false;
        foreach (Animator animator in BuildingAnim)
        {
            animator.SetBool("IsFallTrigger", true); //sets the IsFallTriggre on the building animators after a certain amount of seconds
        }
        uiManager.flockTargetSize = SharkFlockSizeTarget;
    }

    public void SharkEvent() //called through CharacterMovement
    {
        if (flockManager.playerFlockSize < SharkFlockSizeTarget)
        {
            uiManager.IndicatorShake(); //calls the indicator shake animation thorugh the ui manager
        }

        else
        {
            sharkFollow = true;
            flockManager.eventInProgress = true;
            flockManager.playerFlockSize = 0;
            shark.GetComponent<Animator>().SetTrigger("SharkFleeTrigger");
            StartCoroutine(WaitForSharkEvent(1));
            StartCoroutine(WaitForSharkBarrier(3));
        }
    }

    IEnumerator WaitForSharkEvent(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        shark.GetComponent<Animator>().SetFloat("SpinSpeedMultiplier", 2.5f);
    }

    IEnumerator WaitForSharkBarrier(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        sharkBarrier.SetActive(false);
    }
}


