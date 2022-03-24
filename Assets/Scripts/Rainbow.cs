using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainbow : MonoBehaviour
{
    private Material[] materialsArray;
    private float currentHue;

    public float shiftSpeed;
    public int[] rainbowMaterials;
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
            materialsArray[rainbowMaterials[i]].color = Color.HSVToRGB(currentHue, 1, 1);
        }
        if (currentHue >= 1)
        {
            currentHue = 0;
        }
    }
}
