using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFlockAgent : MonoBehaviour
{
    public float gravity = 3f;
    public float moveThreshold;
    public float speed = 10;
    public float turnSmoothTime = 0.25f;

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

        Vector3 moveVector = new Vector3(moveDirection.x * Time.deltaTime * speed, vSpeed, moveDirection.z * Time.deltaTime * speed);
        
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

        //animation
        if (moveVector.sqrMagnitude > 0.05f)
        {
            anim.SetBool("isWalking", true);
        }
        if (moveVector.sqrMagnitude < 0.01f)
        {
            anim.SetBool("isWalking", false);
        }
    }
    private Vector3 CalculateDirection(Vector3 targetPosition)
    {
        Vector3 movementDirection;
        movementDirection = Vector3.zero;
        if (isInPlayerFlock == true)
        {
            movementDirection += targetPosition;
            movementDirection -= transform.position;
            if (movementDirection.sqrMagnitude > moveThreshold)
            {
                //Debug.Log(movementDirection.normalized.x);
                return movementDirection.normalized;
            }
            else
            {
                return Vector3.zero; 
            }
            
        }//once the player collects the agent, the target is the players position (input in the class)

        else { return movementDirection; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerLand" && isInPlayerFlock == false)
        {
            isInPlayerFlock = true;
        }
    }
}
