using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public UIManager uiManager;
    public CharacterMovement characterMovement;
    public CinemachineSwitch cinemachinceSwitch;
    public CameraZoom cameraZoom;
    public VFXBehaviour vfxBehaviour;

    private GameObject eventFollowObject;

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
    public float sharkAttackFleeDuration = 1f;

    private bool sharkFollow = false;


    [Header("Air Initialise")]
    public AirFlockManager airFlockManager;
    public ParticleSystem ambientParticles;

    [Header("Tree Barrier")]
    public int TreeTargetFlockSize;
    public Animator treeAnimator;
    public GameObject treeEventTarget;
    public float treeAnimTime;
    public ParticleSystem treeParticles;

    [Header("Water Collision")]
    private float turnVelocity; //smmoothdamp ref
    public float turnTime = 1f;

    [Header("Hawk Event")]
    public GameObject hawk;
    public GameObject hawkBarrier;
    private bool hawkFollow = false;

    [Header("End Event")]
    public LandFlockManager landFlockManager;
    public Animator broadcastAnimator;
    public GameObject[] oceanPlanes;
    [Header("Ending Timings")]
    public float waitForBroadcast = 6;
    public float waitForZoomOut = 2;
    public float waitForThanksCam = 10;
    public float waitForCreditsCam = 5;
    public float waitForTitleCam = 5;

    
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

        if (hawkFollow == true)
        {
            airFlockManager.eventTransform = hawk.transform;
        }

        //debuggies
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            HawkEvent(null);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            float normalizedTime = hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            normalizedTime -= Mathf.Floor(normalizedTime);
            normalizedTime = normalizedTime * hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            normalizedTime = hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - normalizedTime;
            Debug.Log(normalizedTime);
        }*/

    }
    public void DoorOpenAnimation() //animation classes are called in ontriggerenter in the character movement scripts.
    {
        foreach(Animator animator in DoorAnim)
        {
            animator.SetTrigger("IsOpenTrigger"); //sets the IsOpenTrigger in each door animatior
        }
    }
    public void BuildingEvent(Collider trigger)
    {
        if (flockManager.playerFlockSize < uiManager.flockTargetSize)
        {
            uiManager.IndicatorShake();
        }
        else
        {
            flockManager.eventTransform = buildingFallAgentTarget.transform;
            flockManager.eventInProgress = true;
            StartCoroutine(WaitForBuildingFall(buildingAnimTime));
            cinemachinceSwitch.SwitchState("Event1");
            trigger.enabled = false;
        }
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
        cinemachinceSwitch.SwitchState(null);
    }

    public void SharkEvent(Collider trigger) //called through CharacterMovement
    {
        if (flockManager.playerFlockSize < SharkFlockSizeTarget)
        {
            uiManager.IndicatorShake(); //calls the indicator shake animation thorugh the ui manager
            characterMovement.playerControl = false;
            characterMovement.eventDirection = -90;
            StartCoroutine(waitForEventMovement(sharkAttackFleeDuration));
        }

        else
        {
            sharkFollow = true;
            eventFollowObject = shark;
            flockManager.eventInProgress = true;
            flockManager.playerFlockSize = 0;
            shark.GetComponent<Animator>().SetTrigger("SharkFleeTrigger");
            StartCoroutine(WaitForSharkEvent(1));
            StartCoroutine(WaitForSharkBarrier(3));
            cinemachinceSwitch.SwitchState("Event2");
            trigger.enabled = false;
        }
    }

    IEnumerator waitForEventMovement(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        characterMovement.playerControl = true;
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
        uiManager.indicatorOutro();
        cinemachinceSwitch.SwitchState(null);
    }

    public void AirMovementTrigger()
    {
        characterMovement.movementType = 1; //sets movement type to air movement, called in character movement
                                            //(ik bouncing around between character and event controllers is weird, just planning for water>air
                                            //event stuff
        characterMovement.anim.SetBool("Flightcheck", true); //Player Character enables transformation and flying animations, this manager is very based of you
        airFlockManager.enabled = true;
        uiManager.SwitchIndicatorAir();
        uiManager.flockTargetSize = TreeTargetFlockSize;
        ambientParticles.gameObject.SetActive(false);
        flockManager.enabled = false;
    }

    public void TreeBarrierEvent(Collider trigger)
    {
        if (airFlockManager.playerFlockSize < TreeTargetFlockSize)
        {
            uiManager.IndicatorShake();
        }
        else
        {
            airFlockManager.eventTransform = treeEventTarget.transform;
            airFlockManager.eventInProgress = true;
            treeAnimator.SetTrigger("TreeFall");
            cinemachinceSwitch.SwitchState("Event3");
            trigger.enabled = false;
            StartCoroutine(WaitForTreeEvent(2));
            ParticleSystem.EmissionModule emissionModule = treeParticles.emission;
            emissionModule.rateOverTime = 0;
        }    
    }

    IEnumerator WaitForTreeEvent(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        airFlockManager.eventInProgress = false;
        uiManager.flockTargetSize = 20; //20 is temp, replace with next barrier requirement
        cinemachinceSwitch.SwitchState(null);
    }

    public void WaterCollisionEnter()
    {
        
        characterMovement.playerControl = false;
        characterMovement.eventDirection = 90f;
        Debug.Log(characterMovement.eventDirection);
    }

    public void WaterCollisionExit()
    {
        StartCoroutine(waitForEventMovement(0.2f));
    }

    public void HawkEvent(Collider trigger)
    {
        if (airFlockManager.playerFlockSize < uiManager.flockTargetSize)
        {
            uiManager.IndicatorShake();
            characterMovement.playerControl = false;
            characterMovement.eventDirection = 180;
            
            StartCoroutine(waitForEventMovement(sharkAttackFleeDuration));
        }

        else
        {
            hawkFollow = true;
            airFlockManager.eventInProgress = true;
            eventFollowObject = hawk;
            airFlockManager.playerFlockSize = 0;
            hawk.GetComponent<Animator>().SetTrigger("HawkFleeTrigger"); 
            cinemachinceSwitch.SwitchState("Event4"); //no event yet, 
            float normalizedTime = hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            normalizedTime -= Mathf.Floor(normalizedTime);
            normalizedTime = normalizedTime * hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            normalizedTime = hawk.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - normalizedTime;
            StartCoroutine(WaitForHawkBarrier(normalizedTime));
            trigger.enabled = false;
        }
    }



    IEnumerator WaitForHawkBarrier(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        hawkBarrier.SetActive(false);
        uiManager.indicatorOutro();
        cinemachinceSwitch.SwitchState(null);
    }

    public void LandMovementTrigger(Collider krissyStopper)
    {
        krissyStopper.enabled = false;
        characterMovement.movementType = 2; //switches to land movement 
        airFlockManager.enabled = false; //disables birds for performance
        //ui siwtch? might not be needed 
        //characterMovement.anim.SetBool("LandCheck", true); //temp space for corns animation, doesn't do anything yet
        cinemachinceSwitch.SwitchState("PlayerFollow2");
        vfxBehaviour.enableSkyChanger = true;
    }

    public void EndEvent()
    {
        characterMovement.playerControl = false;
        characterMovement.isEnding = true;
        landFlockManager.isEnding = true;
        //transform player to communication mode here
        StartCoroutine(WaitForBroadcast(waitForBroadcast));
        foreach (GameObject plane in oceanPlanes)
        {
            plane.SetActive(false);
        }
        Debug.Log("starting end event");
    }

    IEnumerator WaitForBroadcast(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        broadcastAnimator.SetTrigger("BroadcastTrigger");
        Debug.Log("starting broadcast animation");
        StartCoroutine(WaitForZoomOut(waitForZoomOut));
    }

    IEnumerator WaitForZoomOut(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cameraZoom.ZoomOut();
        StartCoroutine(WaitForCredits(waitForThanksCam));
        Debug.Log("starting zoom out");
    }

    IEnumerator WaitForCredits(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cinemachinceSwitch.SwitchState("End");
        cinemachinceSwitch.SwitchState("End");
        Debug.Log("moving to thanks cam");
        yield return new WaitForSeconds(waitForCreditsCam);
        Debug.Log("moving to credits cam");
        cinemachinceSwitch.SwitchState("End 2");
        cinemachinceSwitch.SwitchState("End 2"); //ive been using unity for like 3 years now and i JUST learned you can put multiple waitforseconds in a coroutine
        //im so mad

        //also the two switchstates is a lazy work around to avoid redoing stuff
        yield return new WaitForSeconds(waitForTitleCam);
        cinemachinceSwitch.SwitchState("End 3");
        cinemachinceSwitch.SwitchState("End 3"); //it crahsed after adding a third yield so im kinda sus
        Debug.Log("moving to title cam");
    }


}


