using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimCon : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("a") | Input.GetKey("w") | Input.GetKey("s") | Input.GetKey("d"))
        {
            anim.SetBool("PlayerSmoving", true);
        }
        else
        {
            anim.SetBool("PlayerSmoving", false);
        }
    }
}
