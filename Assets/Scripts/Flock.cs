using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("spawn variables")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed vars")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [Header("Detection distances")]
    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    

    public float cohesionDistance { get { return _cohesionDistance; } }

    public FlockUnit[] allUnits;

    public Color GizmoColour;

    private void Start()
    {
        GenerateUnits();
    }

    private void Update()
    {
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit(); 
        }
    }

    private void GenerateUnits()
    {
        allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = Random.insideUnitSphere;
            randomVector = Vector3.Scale(randomVector, spawnBounds);
            var spawnPosition = transform.position + randomVector;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].AssignFlock(this);
            allUnits[i].InitialiseSpeed(Random.Range(minSpeed, maxSpeed));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColour;
        Gizmos.DrawWireCube(transform.position, spawnBounds * 2);
    }
}
