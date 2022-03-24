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
    public int hammerheadFlockSize = 18;
    public Animator hammerheadAnimator; //i was working on this its not finished yet dont forget 
     
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("animStarting?");
            foreach (Animator animator in BuildingAnim)
            {
                animator.SetBool("IsFallTrigger", true);
            }
        }
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
        uiManager.flockTargetSize = hammerheadFlockSize;
    }

    public void HammerheadAnimation()
    {
        
    }
}


