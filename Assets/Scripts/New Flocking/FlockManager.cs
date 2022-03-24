using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject player;
    public FlockAgent[] agentScripts;
    public int playerFlockSize = 0;

    [Header("Agent Speed")]
    public float minSpeed;
    public float maxSpeed;

    [HideInInspector]
    public List<GameObject> inPlayerFlock; //uses list instead of array since its easier to add to
    public Transform eventTransform;
    public bool eventInProgress;

    private GameObject[] agentList;
    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        agentList = GameObject.FindGameObjectsWithTag("flockAgent"); //finds all agents and adds them to agentList array
        agentScripts = new FlockAgent[agentList.Length]; //makes new array of FlockAgent scripts with the number of agents as the length
        for (int i = 0; i < agentList.Length; i++) 
        {
            agentScripts[i] = agentList[i].GetComponent<FlockAgent>(); //for each agent, assign its flockingAgent script to the all agents array
            agentScripts[i].speed = AssignSpeed(); //randomly assigns the speed of each agent by accessing its script 
            agentScripts[i].flockManager = this;
        }
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
                agentScripts[i].moveAgent(playerTransform);
            }

            if (eventInProgress == true)
            {
                agentScripts[i].moveAgent(eventTransform);
            } //if event is in progress moves player flock boids towards a different target
        } //calls move agent class on each FlockAgent script
    }

}
