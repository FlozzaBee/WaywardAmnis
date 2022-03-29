using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject player;
    public FlockAgent[] agentScripts;
    public int playerFlockSize = 0;

    [Header("Agent Settings")]
    public float minSpeed;
    public float maxSpeed;
    public float targetRandomisationMultiplier = 1;

    [HideInInspector]
    public List<GameObject> inPlayerFlock; //uses list instead of array since its easier to add to
    public Transform eventTransform;
    public bool eventInProgress;

    private GameObject[] agentList;
    private int playerFlockSizePrevious; //used to track when player flock size increases
    private CharacterMovement characterMovement; //used to call haptics 

    private Vector3[] randomiseTargetVector;
    private Vector3 playerTransformRandomised;
    


    // Start is called before the first frame update
    void Start()
    {
        agentList = GameObject.FindGameObjectsWithTag("flockAgent"); //finds all agents and adds them to agentList array
        agentScripts = new FlockAgent[agentList.Length]; //makes new array of FlockAgent scripts with the number of agents as the length
        randomiseTargetVector = new Vector3[agentList.Length];
        for (int i = 0; i < agentList.Length; i++) 
        {
            agentScripts[i] = agentList[i].GetComponent<FlockAgent>(); //for each agent, assign its flockingAgent script to the all agents array
            agentScripts[i].speed = AssignSpeed(); //randomly assigns the speed of each agent by accessing its script 
            agentScripts[i].flockManager = this;
            randomiseTargetVector[i] = Random.insideUnitSphere * targetRandomisationMultiplier; //gives each member of the flock a unique random target vector offset
        }

        characterMovement = player.GetComponent<CharacterMovement>();

    }

    private float AssignSpeed()
    {
        float speed;
        speed = Random.Range(minSpeed, maxSpeed);
        return speed; //picks random speed between range and returns it
    }

    private void Update()
    {
        Transform playerTransform = player.transform;

        for (int i = 0; i < agentScripts.Length; i++)
        {
            if (eventInProgress == false)
            {
                playerTransformRandomised = playerTransform.position + randomiseTargetVector[i];
                agentScripts[i].moveAgent(playerTransformRandomised);
            }

            if (eventInProgress == true)
            {
                agentScripts[i].moveAgent(eventTransform.position);
            } //if event is in progress moves player flock boids towards a different target
        } //calls move agent class on each FlockAgent script

        if (playerFlockSize > playerFlockSizePrevious)
        {
            playerFlockSizePrevious = playerFlockSize;
            characterMovement.ControllerRumbleLight();
        }
    }

}
