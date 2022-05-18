using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public GameObject player;
    [Header("Zoom in Parameters")]
    public Vector3 startPoint;
    public Vector3 endPoint;

    public float startZoomDistance = 20;
    public float endZoomDistance = 12; 

    public CinemachineVirtualCamera vCam;

    private CinemachineFramingTransposer transposer;

    [Header("Zoom out parameters")]
    public float zoomOutTarget = 40;
    public float zoomOutTime = 5;
    private float camZoomSmoothRef;

    [Header("Modified by EventManager")]
    public bool isZoomingIn = true;
    public bool isZoomingOut = false;

    //Zooms camera into the player character on the final stretch of the game. 

    private void Start()
    {
        transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    private void Update()
    {
        if (isZoomingIn)
        {
            float lerpDistance = endPoint.x - startPoint.x;
            float lerpFraction = (player.transform.position.x - startPoint.x) / lerpDistance;
            float cameraDist = Mathf.Lerp(startZoomDistance, endZoomDistance, lerpFraction);
            //Debug.Log(cameraDist);
            transposer.m_CameraDistance = cameraDist;
        } //zooms in with progression betwee target points.
        
        if (isZoomingOut)
        {
            transposer.m_CameraDistance = Mathf.SmoothDamp(transposer.m_CameraDistance, zoomOutTarget, ref camZoomSmoothRef, zoomOutTime);
        }//smoothly moves camera to zoomed out target
    }

    public void ZoomOut()
    {
        isZoomingIn = false;
        isZoomingOut = true;
    } // switches some bools so the zoom out code runs instead of the zoom in. called in event mananager

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(startPoint, 1);
        Gizmos.DrawSphere(endPoint, 1);
    } // displays the target points in the editor
}
