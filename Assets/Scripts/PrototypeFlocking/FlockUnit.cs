using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockUnit : MonoBehaviour
{
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp; //how close to the cohesion vector we can get in a frame, lower is closer, essentially turn speed

    private List<FlockUnit> cohesionNeighbours = new List<FlockUnit>();
    private Flock assignedFlock;
    private Vector3 currentVelocity;
    private float speed;
    private Vector3 startPos;

    public Transform myTransform { get; set; }
    public GameObject player;
    public float randomDirectionMultiplier;
    public GameObject childObject;
    

    private void Start()
    {
        myTransform = transform;
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = myTransform.position;
    }

    public void AssignFlock(Flock flock)
    {
        assignedFlock = flock;
    }

    public void InitialiseSpeed(float speed)
    {
        this.speed = speed; 
    }

    public void MoveUnit()
    {
        //findNeighbours();
        Vector3 cohesionVector = CalculateCohesionVector(); //runs calculatecohesionvector class
        Vector3 moveVector = Vector3.SmoothDamp(myTransform.forward, cohesionVector, ref currentVelocity, smoothDamp); //calculates direction from current to target position, and smooths it
        moveVector = moveVector.normalized * speed;
        myTransform.forward = moveVector; 
        myTransform.position += moveVector * Time.deltaTime;  //moves the unit and its rotation
    }

    /*
    private void findNeighbours()
    {
        cohesionNeighbours.Clear();
        var allUnits = assignedFlock.allUnits;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentUnit = allUnits[i];
            if (currentUnit != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentUnit.transform.position - transform.position);
                if(currentNeighbourDistanceSqr <= assignedFlock.cohesionDistance * assignedFlock.cohesionDistance)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
            }
        }
    }
    */




    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            cohesionNeighbours.Clear();
            var allUnits = assignedFlock.allUnits;
            for (int i = 0; i < allUnits.Length; i++)
            {
                var currentUnit = allUnits[i];
                if (currentUnit != this)
                {
                    cohesionNeighbours.Add(currentUnit);
                }
            }
            childObject.GetComponent<Rainbow>().enabled = true;
            
        }
    }



    private Vector3 CalculateCohesionVector()
    {
        Vector3 cohesionVector = Vector3.zero;
        //int neighboursInFOV = 0;
        if (cohesionNeighbours.Count == 0)
        {
            cohesionVector += startPos;
            cohesionVector -= myTransform.position; //player position - current position = the direction you wanna go to reach player
            cohesionVector = Vector3.Normalize(cohesionVector);
            cohesionVector += Random.insideUnitSphere * randomDirectionMultiplier;
            return cohesionVector;
        }
        /*for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }
        if (neighboursInFOV == 0)
            return cohesionVector;
        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;  */
        cohesionVector += player.transform.position;
        cohesionVector -= myTransform.position; //player position - current position = the direction you wanna go to reach player
        cohesionVector = Vector3.Normalize(cohesionVector); 
        cohesionVector += Random.insideUnitSphere * randomDirectionMultiplier; //applies a bit of randomisation to the target direction
        return cohesionVector;
        
    }

    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }

}
