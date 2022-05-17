using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandFlockManager : MonoBehaviour
{
    public GameObject[] agentList;
    private LandFlockAgent[] agentScripts;

    public Vector3 targetRandomisationMultiplier;
    private Vector3[] randomiseTargetVector;

    public GameObject player;

    public float animSpeedMin = 0.9f;
    public float animSpeedMax = 1.1f;
    private void Awake()
    {
        agentList = GameObject.FindGameObjectsWithTag("LandFlockAgent"); //finds all agents and adds them to agentList array
        agentScripts = new LandFlockAgent[agentList.Length]; //makes new array of FlockAgent scripts with the number of agents as the length
        randomiseTargetVector = new Vector3[agentList.Length];
        for (int i = 0; i < agentList.Length; i++)
        {
            agentScripts[i] = agentList[i].GetComponent<LandFlockAgent>(); //for each agent, assign its flockingAgent script to the all agents array
            //agentScripts[i].speed = AssignSpeed(); //randomly assigns the speed of each agent by accessing its script 
            //agentScripts[i].flockManager = this;
            randomiseTargetVector[i] = Vector3.Scale(Random.insideUnitSphere, targetRandomisationMultiplier); //gives each member of the flock a unique random target vector offset
            Animator anim = agentList[i].GetComponentInChildren<Animator>();
            anim.SetFloat("AnimSpeedModifier", Random.Range(animSpeedMin, animSpeedMax));
            anim.SetBool("isMirror", Random.value > 0.5f);
        }
    }
    private void Update()
    {
        Vector3 playerVector = player.transform.position;

        for (int i = 0; i < agentScripts.Length; i++)
        {
            Vector3 playerVectorRandomised;
            playerVectorRandomised = playerVector + randomiseTargetVector[i];
            agentScripts[i].moveAgent(playerVectorRandomised);
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                agentScripts[i].Walking(true);
            }
            else
            {
                agentScripts[i].Walking(false);
            }
        }
    }
}
