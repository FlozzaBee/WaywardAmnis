using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 1f;
    public float turnSmoothTime = 0.1f;
    public EventManager eventManager;
    public FlockManager flockManager;
    public Animator anim;

    [Header("HapticSettings")]
    [Range(0, 1)]
    public float lightRumbleStrength;
    public float lightVibrateDuration;

    [Header("ControllerDebug")]
    public bool controllerDebug = false;
    [Range(0, 1)]
    public float vibrationLowFreq = 0;
    [Range(0, 1)]
    public float vibrationHighFreq = 0;

    private float turnSmoothVelocity;
    private float marvinStopper;
    private bool controllerEnabled = false;


    private void Start()
    {
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
    }

    
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); //gets horizontal input (a,d)
        float vertical = Input.GetAxisRaw("Vertical"); //gets vert input (w,s)
        Vector3 direction = new Vector3(horizontal, vertical, 0f).normalized; //cretes vector3 with the combined movement inputs, normalised so diagonals are the same speed

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //sets targetAngle to the direction of travel (in degrees)
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smoothly moves from current to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle); //applies smoothed rotation
            controller.Move(direction * speed * Time.deltaTime); //applies movement
            anim.SetBool("PlayerSmoving", true); //Moving animation enabled
        }
        else
        {
         anim.SetBool("PlayerSmoving", false);   
        }

        

        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
            Debug.Log("Quit Called");
        }

        if (controllerDebug == true)
        {
            Gamepad.current.SetMotorSpeeds(vibrationLowFreq, vibrationHighFreq);
        }
    }

    private void LateUpdate()
    {
        //z axis constraining
        transform.position = new Vector3(transform.position.x, transform.position.y, marvinStopper);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DoorTrigger")
        {
            eventManager.DoorOpenAnimation();
        }

        if (other.tag == "BuildingFallTrigger" && flockManager.playerFlockSize > eventManager.buildingAnimFlockSize)
        {
            eventManager.BuildingFallAnimation();
        }

        if (other.tag == "SharkTrigger")
        {
            eventManager.SharkEvent(); //calls SharkEvent when touches the shark trigger
        }
    }

    //haptic controller 

    

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
