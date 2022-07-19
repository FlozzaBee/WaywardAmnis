using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public GameObject player;
    public CinemachineVirtualCamera vCam;

    [Header("Zoom in Parameters")]
    public Vector3 startPoint;
    public Vector3 endPoint;

    public float startZoomDistance = 20;
    public float endZoomDistance = 12; 

    [Header("Zoom out parameters")]
    public float zoomOutTarget = 40;
    public float zoomOutTime = 5;
    public float camHeightTarget = 7f;
    private float camZoomSmoothRef;
    private float camXMoveSmoothRef; //used for smoothdamps

    [Header("Modified by EventManager")]
    public bool isZoomingIn = true;
    public bool isZoomingOut = false;


    private CinemachineFramingTransposer transposer;

    private void Start()
    {
        transposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>(); //gets cinemachine virtual camera position perameters 
    }
    private void Update()
    {
        if (isZoomingIn)
        {
            float lerpDistance = endPoint.x - startPoint.x;
            float lerpFraction = (player.transform.position.x - startPoint.x) / lerpDistance;
            float cameraDist = Mathf.Lerp(startZoomDistance, endZoomDistance, lerpFraction); 
            //moves camera closer to the player by a fraction of their progress through the final area
            //Debug.Log(cameraDist);
            transposer.m_CameraDistance = cameraDist; 
        } //Zooms camera into the player character on the final stretch of the game. 

        if (isZoomingOut)
        {
            transposer.m_CameraDistance = Mathf.SmoothDamp(transposer.m_CameraDistance, zoomOutTarget, ref camZoomSmoothRef, zoomOutTime);
            transposer.m_TrackedObjectOffset.x = Mathf.SmoothDamp(transposer.m_TrackedObjectOffset.x, camHeightTarget, ref camXMoveSmoothRef, zoomOutTime);
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
