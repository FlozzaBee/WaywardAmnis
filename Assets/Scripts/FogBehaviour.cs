using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogBehaviour : MonoBehaviour
{
    public Color startColour;
    public Color EndColour;
    public Material m;
    public float maxDepth;
    public float minDepth;
    public GameObject player;

    private Color currentColour;
    void Update()
    {
        float zFraction;
        zFraction = player.transform.position.y / minDepth;
        currentColour = Color.Lerp(startColour, EndColour, zFraction);
        m.SetColor("Color_B270E39F", currentColour);
        //Debug.Log("Colour " + currentColour);
        //Debug.Log("player " + player.transform.position.z + " min depth " + minDepth);
        //Debug.Log("zFraction " + zFraction);
    }
}
