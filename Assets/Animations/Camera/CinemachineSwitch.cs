using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitch : MonoBehaviour
{
    [SerializeField]
    private InputAction action;

    private Animator animator;
    

    private bool playerCamera = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    

    public void SwitchState(string Camera)
    {
        if (playerCamera)
        {
            animator.Play(Camera);
        }
        
        if (playerCamera == false)
        {
            Debug.Log("playercam");
            animator.Play("PlayerFollow");
        }
        playerCamera = !playerCamera;
    }
}
