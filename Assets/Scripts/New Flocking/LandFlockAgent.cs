using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFlockAgent : MonoBehaviour
{
    public float gravity = 3f;
    public float moveThreshold = 0.1f;
    public float accelThreshold = 2f;
    public float fastSpeed = 10;
    public float slowSpeed = 7;
    public float speed;
    public float turnSmoothTime = 0.25f;

    public LandFlockManager landFlockManager;

    private bool isInPlayerFlock = false;
    private CharacterController characterController;
    private float vSpeed;
    private bool leftFacing = false;
    private float turnSmoothRef;

    private Animator anim;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        speed = slowSpeed;
    }

    public void moveAgent(Vector3 targetPosition)
    {
        Vector3 moveDirection = CalculateDirection(targetPosition);
        //calc gravity
        if (characterController.isGrounded == true)
        {
            vSpeed = 0;
        }
        vSpeed -= gravity * Time.deltaTime;

        Vector3 moveVector = new Vector3(moveDirection.x * Time.deltaTime * speed, vSpeed * Time.deltaTime, moveDirection.z * Time.deltaTime * speed);
        
        characterController.Move(moveVector);

        //turning
        if (moveDirection.x < 0)
        {
            leftFacing = true;
        }
        if (moveDirection.x > 0)
        {
            leftFacing = false;
        }
        float angle = transform.rotation.eulerAngles.y;
        if (leftFacing == true)
        {
            angle = Mathf.SmoothDampAngle(angle, 180, ref turnSmoothRef, turnSmoothTime);
        }
        if (leftFacing == false)
        {
            angle = Mathf.SmoothDampAngle(angle, 0, ref turnSmoothRef, turnSmoothTime);
        }
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    private Vector3 CalculateDirection(Vector3 targetPosition)
    {
        Vector3 movementDirection;
        movementDirection = Vector3.zero;
        if (isInPlayerFlock)
        {
            movementDirection += targetPosition;
            movementDirection -= transform.position;
            movementDirection.y = 0;
            if (movementDirection.sqrMagnitude > accelThreshold)
            {
                speed = fastSpeed;
                StartCoroutine(GottaGoFast(1));
            }
            
            if (movementDirection.sqrMagnitude < moveThreshold)
            {
                Walking(false);
                speed = slowSpeed;
                return Vector3.zero;
            }
            else
            {
                Walking(true);
                return movementDirection.normalized;
            }
        }
        else
        {
            Walking(false);
            return Vector3.zero;
        }
    }

    IEnumerator GottaGoFast(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        speed = slowSpeed;
    }

    //animation
    private void Walking(bool isWalking)
    {
        if (isWalking == true && isInPlayerFlock == true)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerLand" && isInPlayerFlock == false)
        {
            landFlockManager.DoHaptics();
            isInPlayerFlock = true;
            Debug.Log("dunna haptic");
        }
    }
}
