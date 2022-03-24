using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    

    
    public float smoothDampVelocity = 1;
    public float randomnessMultiplier;

    [HideInInspector]
    public float speed;
    public FlockManager flockManager;
 
    private bool isInPlayerFlock = false;
    private Vector3 startPosition;
    private Vector3 currentVelocity;
    private float inFlockSpeedModifier = 1;
    


    private void Start()
    {
        startPosition = transform.position;
        
    }

    /*public void AssignFlock(FlockManager flock)
    {
        flockManager = flock; 
    } //assigns the flock manager to this agent, called in flock manager  */

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isInPlayerFlock == false) //if the boid collides with a hitbox tagged player, not currently in the flock,
        {
            isInPlayerFlock = true; 
            GetComponentInChildren<Rainbow>().enabled = true;  //enable rainbow script
            inFlockSpeedModifier = 3; //increases speed when in players flock
            if (!flockManager.inPlayerFlock.Contains(this.gameObject) )
            {
                flockManager.inPlayerFlock.Add(this.gameObject); //if the current boid is not in the player flock list, add it.
            }
            flockManager.playerFlockSize = flockManager.inPlayerFlock.Count;
        }
    }

    private Vector3 CalculateMovementDirection(Vector3 TargetPosition) //vector for the direction of the agents target
    {
        Vector3 movementDirection;
        movementDirection = Vector3.zero; //movement vector = target position - current position 
        if (isInPlayerFlock == false)
        {
            movementDirection += startPosition;
            movementDirection -= transform.position;
            //Debug.Log(movementDirection);
            movementDirection += Random.insideUnitSphere * randomnessMultiplier;
            return movementDirection.normalized;

        } //if the player hasnt collected them yet the target position is where they spawned
        if (isInPlayerFlock == true)
        {
            movementDirection += TargetPosition;
            movementDirection -= transform.position;
            //Debug.Log(movementDirection);
            movementDirection += Random.insideUnitSphere * randomnessMultiplier;
            return movementDirection.normalized;
        }//once the player collects the agent, the target is the players position (input in the class)
        else
        {
            return movementDirection;
        }
    }

    public void moveAgent(Transform TargetPosition)
    {
        Vector3 moveDirection = CalculateMovementDirection(TargetPosition.position); //calls the calculatemovementdirection funtion
        Vector3 moveVector = Vector3.SmoothDamp(transform.forward, moveDirection, ref currentVelocity, smoothDampVelocity); //gradually changes movevector to be closer to moveDirection
        moveVector = moveVector.normalized * speed * inFlockSpeedModifier; //normalizes the moveVector and applies speed multiplier
        transform.forward = moveVector; //changes the agents rotation to face its target
        transform.position += moveVector * Time.deltaTime; //moves the agent in the direction its facing
    }


}
