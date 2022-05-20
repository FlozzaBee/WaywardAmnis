using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXBehaviour : MonoBehaviour
{
    [Header("Fog")]
    public Color startColour;
    public Color EndColour;
    public Material m;
    public float maxDepth;
    public float minDepth;
    public GameObject player;

    [Header("SkyBox")]
    public Material skybox;
    public float startBlend = 0.29f;
    public float endBlend = 1f;
    public Color startTint;
    public Color endTint;
    public float transitionTime = 30f;

    private float colourLerp = 0;
    private float currentTransition;
    private float transitionRef;

    [Header("Altered by EventManager")]
    public bool enableFogChanger = true;
    public bool enableSkyChanger = false;

    private Color currentColour;

    private void Start()
    {
        currentTransition = startBlend;
        skybox.SetFloat("_CubemapTransition", startBlend);
        skybox.SetColor("_TintColor", startTint);
    }
    void Update()
    {
        if (enableFogChanger)
        {
            float zFraction;
            zFraction = player.transform.position.y / minDepth;
            currentColour = Color.Lerp(startColour, EndColour, zFraction);
            m.SetColor("Color_B270E39F", currentColour);
            //Debug.Log("Colour " + currentColour);
            //Debug.Log("player " + player.transform.position.z + " min depth " + minDepth);
            //Debug.Log("zFraction " + zFraction);
        }

        if (enableSkyChanger)
        {
            currentTransition = Mathf.SmoothDamp(currentTransition, endBlend, ref transitionRef, transitionTime);
            if (currentTransition - startBlend != 0)
            {
                colourLerp = (currentTransition - startBlend) / (1 - startBlend);
            }
            currentColour = Color.Lerp(startTint, endTint, colourLerp);
            skybox.SetFloat("_CubemapTransition", currentTransition);
            skybox.SetColor("_TintColor", currentColour);
           // Debug.Log(lerpinit);
        }

    }
}
