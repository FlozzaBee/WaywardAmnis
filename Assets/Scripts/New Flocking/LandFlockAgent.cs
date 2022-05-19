using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFlockAgent : MonoBehaviour
{
    public float gravity = 3f;
    public float moveThreshold = 0.1f;
    public float fastSpeed = 10;
    public float slowSpeed = 7;
    private float speed = 10;
    public float turnSmoothTime = 0.25f;

    private bool isInPlayerFlock = false;
    private CharacterController characterController;
    private float vSpeed;
    private bool leftFacing = false;
    private float turnSmoothRef;
    private bool _isWalking = false;
    private bool slowWalking = false;

    

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
        if (isInPlayerFlock == true)
        {
            movementDirection += targetPosition;
            movementDirection -= transform.position;
            if (movementDirection.sqrMagnitude > moveThreshold)
            {
                //Debug.Log(movementDirection.normalized.x);
                return movementDirection.normalized;
            }
            if (movementDirection.sqrMagnitude <= moveThreshold && slowWalking == false && _isWalking)
            {
                speed = slowSpeed;
                slowWalking = true;
                StartCoroutine(SlowWalk(1));
                return movementDirection.normalized;
                
            }
            if (movementDirection.sqrMagnitude <= moveThreshold && slowWalking == true)
            {
                return Vector3.zero;
            }
            else { return Vector3.zero; }
            /*else
            {
                if (!walkWaiting)
                {
                    StartCoroutine(WalkWait(1));
                    walkWaiting = true;
                }
                return Vector3.zero;
            }*/
            
        }//once the player collects the agent, the target is the players position (input in the class)

        else { return movementDirection; }
    }

    //animation
    public void Walking(bool isWalking)
    {
        if (isWalking == true && isInPlayerFlock == true)
        {
            anim.SetBool("isWalking", true);
            _isWalking = isWalking;
        }
        else
        {
            anim.SetBool("isWalking", false);
            _isWalking = isWalking;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerLand" && isInPlayerFlock == false)
        {
            isInPlayerFlock = true;
        }
    }

    IEnumerator SlowWalk(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        speed = fastSpeed;
        slowWalking = false;
        Debug.Log("slowWalking");
    }
}
