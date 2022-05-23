using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPlus : MonoBehaviour
{
    private Material[] materialsArray;
    private float currentHue;

    public float shiftSpeed;
    public int[] rainbowMaterials;

    [Range(0,1)] 
    public float saturation = 1;
    [Range(0, 1)] 
    public float value = 1;
    // Start is called before the first frame update
    void Start()
    {
        materialsArray = GetComponent<Renderer>().materials;
        //Debug.Log("matarray " + materialsArray[0]);
    }

    // Update is called once per frame
    void Update()
    {
        currentHue += shiftSpeed * Time.deltaTime;
        for (int i = 0; i < rainbowMaterials.Length; i++)
        {
            materialsArray[rainbowMaterials[i]].color = Color.HSVToRGB(currentHue, saturation, value);
        }
        if (currentHue >= 1)
        {
            currentHue = 0;
        }
    }
}
