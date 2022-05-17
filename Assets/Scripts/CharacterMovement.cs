using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public EventManager eventManager;
    public FlockManager flockManager;
    public UIManager uiManager;
    public Animator anim;

    
    

    public int movementType = 0; //0 = water, 1 = air, 2 = land //public for debug only

    [Header("Ocean Movement Settings")]
    public float speed = 12f;
    public float turnSmoothTime = 0.1f;
    public float waterSpeedSmoothTime = 0.25f;
    private float waterSpeedCurrent;
    private float waterSpeedRef;

    [Header("Air Movement Settings")]
    public float airSpeed = 24;
    public float airIdleSpeed = 3;
    public float airSpeedSmoothTime = 0.1f; //time to accelerate to target speed
    public float airIdleAngleSmoothTime = 0.5f; //time to rotate to idle angle from movement angle
    public float airIdleAnimMultiplier =0.5f;
    public float airWallCollisionDist = 4; //used for raycast wall detection distance
    private float airCurrentSpeed;
    private float airSpeedRef; //used for smoothdamp ref
    private float airTargetAngle;
    private bool isIdleTurning = false;
    private LayerMask layerMask;

    [Header("Land Movement Settings")]
    public float landSpeed = 10f;
    public float landSpeedSmoothTime = 0.25f;
    public float gravity = 9.81f;
    public float landTurnSmoothTime = 0.2f;
    private float landSpeedCurrent;
    private float landSpeedRef;
    private float verSpeed;
    private bool leftFacing;
    private float landSmoothTurnRef;
    private bool zLeveled = false;
    private float landZTurnRef;


    [Header("HapticSettings")]
    [Range(0, 1)]
    public float lightRumbleStrength;
    public float lightVibrateDuration;

    [Header("MiscDebug")]
    [Range(5,120)]
    public int targetFrameRate = 60;

    [Header("ControllerDebug")]
    public bool controllerDebug = false;
    [Range(0, 1)]
    public float vibrationLowFreq = 0;
    [Range(0, 1)]
    public float vibrationHighFreq = 0;

    

    private float turnSmoothVelocity;
    private float turnSmoothVelocity1;
    private float marvinStopper;
    private bool controllerEnabled = false;

    

    //eventManager related
    [HideInInspector]
    public bool playerControl = true;
    [HideInInspector]
    public float eventDirection;

    

    private void Start()
    {
        QualitySettings.vSyncCount = 0; //for testing framerate dependency, remove before ship

        marvinStopper = transform.position.z;

        /*for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            Debug.Log(InputSystem.devices[i]);

        }*/
        Debug.Log(Gamepad.all.Count);
        
        if (Gamepad.all.Count > 0)
        {
            controllerEnabled = true; 
        }//checks for controllers and sets bool to true if found

        layerMask = LayerMask.GetMask("Wall");
    }

    

    void Update()
    {
        Application.targetFrameRate = targetFrameRate; //for testing framerate dependency, remove before ship

        Vector3 direction = Vector3.zero;
        if (playerControl == true) // player control is disabled by some events
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); //gets horizontal input (a,d)
            float vertical = Input.GetAxisRaw("Vertical"); //gets vert input (w,s)
            direction = new Vector3(horizontal, vertical, 0f).normalized; //cretes vector3 with the combined movement inputs, normalised so diagonals are the same speed
            //Debug.Log("Input " + direction);
            
        }

        else
        {
            float angle = transform.eulerAngles.z;
            angle = Mathf.SmoothDampAngle(angle, eventDirection, ref turnSmoothVelocity, 0.15f);
            Vector3 eventVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            controller.Move(eventVector * speed * Time.deltaTime);
            //while player control disabled, direction is controlled by the event manager
        }
        if (movementType == 0 && playerControl == true)
        {
            PlayerMovement(direction);
        }
        if (movementType == 1 && playerControl == true)
        {
            PlayerMovementAir(direction);
        }
        if (movementType == 2 && playerControl == true)
        {
            PlayerMovementLand(direction);
        }

        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
            Debug.Log("Quit Called");
        }//quits program on esc (and r2 for some reason?)

        if (Input.GetButtonDown("Reset"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }//reloads scene on R 

        if (controllerDebug == true)
        {
            Gamepad.current.SetMotorSpeeds(vibrationLowFreq, vibrationHighFreq);
        }
        
        
    }

    private void LateUpdate()
    {
        //z axis constraining aka marvin stopper 
        transform.position = new Vector3(transform.position.x, transform.position.y, marvinStopper);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DoorTrigger")
        {
            eventManager.DoorOpenAnimation();
        }

        if (other.tag == "BuildingFallTrigger" )
        {
            eventManager.BuildingEvent(other);
        }

        if (other.tag == "SharkTrigger")
        {
            eventManager.SharkEvent(other); 
        }//calls events in event manager when touches triggers

        if (other.tag == "AirMovementTrigger")
        {
            eventManager.AirMovementTrigger();
        }//switches to air movement

        if (other.tag == "TreeTrigger")
        {
            eventManager.TreeBarrierEvent(other);
        }

        if (other.tag == "WaterTrigger")
        {
            eventManager.WaterCollisionEnter();
        }

        if (other.tag == "HawkTrigger")
        {
            eventManager.HawkEvent(other);
        }

        if (other.tag == "LandMovementTrigger")
        {
            eventManager.LandMovementTrigger();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WaterTrigger")
        {
            eventManager.WaterCollisionExit();
        }
    }


    //haptic controller 

    private void PlayerMovement(Vector3 direction)//called in update
    {
        float targetSpeed;
        float angle;
        if (direction.sqrMagnitude >= 0.1f)
        {
            targetSpeed = speed;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //sets targetAngle to the direction of travel (in degrees)
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smoothly moves from current to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle); //applies smoothed 
            anim.SetBool("PlayerSmoving", true); //Moving animation enabled
        }
        else
        {
            angle = transform.rotation.eulerAngles.z;
            targetSpeed = 0;
            direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0); //calculates what direction ur facing
            anim.SetBool("PlayerSmoving", false); //Idle animation enabled
        }
        waterSpeedCurrent = Mathf.SmoothDamp(waterSpeedCurrent, targetSpeed, ref waterSpeedRef, waterSpeedSmoothTime);
        controller.Move(direction * waterSpeedCurrent * Time.deltaTime);
        //smoothhh yeah
        //Debug.Log(waterSpeedCurrent);
    }

    private void PlayerMovementAir(Vector3 direction)
    {
        float targetSpeed;
        
        float angle = transform.rotation.eulerAngles.z;


        if (direction.sqrMagnitude >= 0.1f)
        {
            airTargetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //sets targetAngle to the direction of travel (in degrees)
            angle = Mathf.SmoothDampAngle(angle, airTargetAngle, ref turnSmoothVelocity, turnSmoothTime); //smoothly moves from current to target angle    

            targetSpeed = airSpeed; //assigns target speed to in motion speed

            anim.SetFloat("AnimSpeedMultiplier", 1); //resets anim speed mult to 1 after idle.
             anim.SetBool("PlayerSmoving", true); //Moving animation enabled
        }
        else
        {
            targetSpeed = airIdleSpeed; //asigns target to idle speed
            if (isIdleTurning == false)
            {
                airTargetAngle = 0;

                if ((angle >= 90 && angle <= 270)) //if between 90 & 270 degrees rotation (aka looking left)
                {
                    airTargetAngle = 180;
                } //im stupid 
            }//only sets target angle outside of idle turn time
            angle = Mathf.SmoothDampAngle(angle, airTargetAngle, ref turnSmoothVelocity1, airIdleAngleSmoothTime);
            direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0);

            anim.SetFloat("AnimSpeedMultiplier", airIdleAnimMultiplier); // slows corns movement animation by multiplier while idle
            anim.SetBool("PlayerSmoving", false); //Idle animation enabled 

            //raycast stuff to make the player turn if they idle hit a wall
            RaycastHit hitRight;
            if (Physics.Raycast(transform.position, new Vector3(1, 0, 0) * 100, out hitRight, airWallCollisionDist, layerMask) && isIdleTurning == false)
            {
                isIdleTurning = true;
                airTargetAngle = 180;
                StartCoroutine(WaitForIdleTurn(1.5f));
                //Debug.Log("right ray " + hitRight.collider.gameObject);
                //Debug.Log("right ray " + hitRight.distance);
                Debug.DrawRay(transform.position, (new Vector3(1, 0, 0) * airWallCollisionDist), Color.magenta, 1);
            }

            RaycastHit hitLeft;
            if (Physics.Raycast(transform.position, new Vector3(-1, 0, 0) * 100, out hitLeft, airWallCollisionDist, layerMask) && isIdleTurning == false)
            {
                isIdleTurning = true;
                airTargetAngle = 0;
                StartCoroutine(WaitForIdleTurn(1.5f));
                //Debug.Log("left ray " + hitLeft.collider.gameObject);
                //Debug.Log("left ray " + hitLeft.distance);
                Debug.DrawRay(transform.position, new Vector3(-1, 0, 0) * airWallCollisionDist, Color.cyan, 1);
            }
        }

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        airCurrentSpeed = Mathf.SmoothDamp(airCurrentSpeed, targetSpeed, ref airSpeedRef, airSpeedSmoothTime);
        controller.Move(direction * airCurrentSpeed * Time.deltaTime);
    }

    private void PlayerMovementLand(Vector3 direction)
    {
        Vector3 directionLand;
        float targetSpeed;
        if (direction.sqrMagnitude >= 0.1f)
        {
            targetSpeed = speed;
            anim.SetBool("PlayerSmoving", true); //Moving animation enabled
        }
        else
        {
            targetSpeed = 0;
            anim.SetBool("PlayerSmoving", false); //Idle animation enabled
        }
        landSpeedCurrent = Mathf.SmoothDamp(landSpeedCurrent, targetSpeed, ref landSpeedRef, landSpeedCurrent);

        if (controller.isGrounded == true)
        {
            verSpeed = 0;
        }
        verSpeed -= gravity * Time.deltaTime;
        directionLand = new Vector3(direction.x * landSpeedCurrent * Time.deltaTime, verSpeed, 0);

        controller.Move(directionLand);

        //Direction switching
        if (direction.x < 0)
        {
            leftFacing = true;
        }
        if (direction.x > 0)
        {
            leftFacing = false;
        }
        float angle = transform.rotation.eulerAngles.y;
        if (leftFacing == true)
        {
            angle = Mathf.SmoothDampAngle(angle, 180, ref landSmoothTurnRef, landTurnSmoothTime);
        }
        if (leftFacing == false)
        {
            angle = Mathf.SmoothDampAngle(angle, 0, ref landSmoothTurnRef, landTurnSmoothTime);
        }
        //transform.rotation = Quaternion.Euler(0, angle, 0);

        //initial z axis leveling rotation
        if (zLeveled == false)
        {
            float angleZ = transform.rotation.eulerAngles.z;
            angleZ = Mathf.SmoothDampAngle(angleZ, 0, ref landZTurnRef, 0.25f);
            transform.rotation = Quaternion.Euler(0, angle, angleZ);
            if (angleZ == 0)
            {
                zLeveled = true;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void ControllerRumbleLight()
    {
        if (controllerEnabled == true) //checks if controller is connected, otherwise causes crashes when haptics are called and no controller is plugged in. 
        {
            /*
            Gamepad.current.SetMotorSpeeds(lightRumbleStrength, lightRumbleStrength);
            Debug.Log("haptics started");
            StartCoroutine(lightVibrationDuration(lightVibrateDuration));
            */

            foreach (Gamepad gamepad in Gamepad.all)
            {
                gamepad.SetMotorSpeeds(lightRumbleStrength, lightRumbleStrength);
                StartCoroutine(lightVibrationDuration(lightVibrateDuration));
            }
        }
    }

    IEnumerator WaitForIdleTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isIdleTurning = false;
    }

    IEnumerator lightVibrationDuration(float vibrateDuration)
    {
        yield return new WaitForSeconds(vibrateDuration);
        foreach (Gamepad gamepad in Gamepad.all)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
        
        /*
        Gamepad.current.SetMotorSpeeds(0, 0);
        //Gamepad.current.ResetHaptics();
        Debug.Log("haptics stopped");
        */
    }
}
