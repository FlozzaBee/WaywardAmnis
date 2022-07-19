using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;


public class Menu : MonoBehaviour
{
    public string SceneName;
    private bool paused = false;
    public VolumeProfile volumeProfile;

    public GameObject pauseMenu;
    //button alpha 
    public CanvasGroup canvas;
    private float alphaRef;

    //button alpha
    public CanvasGroup indicator;

    //controller fix
    public EventSystem eventSystem;
    

    private UnityEngine.Rendering.Universal.DepthOfField depthOfField;
    //depth of field perameter variable

    [Header("Depth Of Field")]
    public float smoothSpeed = 0.05f;
    private float smoothDampRef;
    private float currentFocalLength = 1;
    [Header("unpaused")]
    public float focalLength = 1;
    [Header("paused")]
    public float p_focalLength = 20;

    private float targetAlpha = 0;
    private bool doneUnpause = true; 

 
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Called");
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1; //resets time scale since its lowered after pausing
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0); //returns to title scene
        Time.timeScale = 1; //resets time scale 
    }
    //these methods are called via button On Clicks

    public void PauseGame()
    {
        if (!paused)
        {
            Time.timeScale = 0.1f;
            Debug.Log("paused");
            pauseMenu.SetActive(true);
            targetAlpha = 1; //enables menu ui and slows time
            paused = !paused;
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject); //sets which menu item is selected, for better controller support
            return;
        }
        if (paused && doneUnpause)
        {
            Time.timeScale = 1f; //resumes time scale to normal
            Debug.Log("unpaused");
            StartCoroutine(waitForAlpha(0.2f)); //slight delay prevents issues from quick pause inputs
            targetAlpha = 0;
            doneUnpause = false;
            paused = !paused;
        }
        
    }
    
    IEnumerator waitForAlpha(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        pauseMenu.SetActive(false);
        doneUnpause = true; //disables the ui after its faded to 0% opacity and allows pausing again
        
    }


    private void Start()
    {
        if (!volumeProfile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));
        //assigns depth of field to dof variable
        depthOfField.focalLength.Override(focalLength);
    }
    private void Update()
    {
        if (Mathf.Abs(canvas.alpha - targetAlpha) > 0.01f) //only calculates new alpha when above above threshold difference from target alpha
        {
            if (paused)
            {
                currentFocalLength = Mathf.SmoothDamp(currentFocalLength, p_focalLength, ref smoothDampRef, smoothSpeed); //calculates depth of field change to smoothly blur background 
                canvas.alpha = Mathf.SmoothDamp(canvas.alpha, targetAlpha, ref alphaRef, smoothSpeed); //calculates alpha to smoothly fade in menu
                
            }
            else
            {
                currentFocalLength = Mathf.SmoothDamp(currentFocalLength, focalLength, ref smoothDampRef, smoothSpeed * 5); //calculates depth of field change to smoothly un-blur background 
                canvas.alpha = Mathf.SmoothDamp(canvas.alpha, targetAlpha, ref alphaRef, smoothSpeed * 5); //calculates alpha to smoothly fade out menu. 
                //the smoothSpeed is increased to compensate for the timescale change when paused
            }
            indicator.alpha = 1 - canvas.alpha;
            depthOfField.focalLength.Override(currentFocalLength); //applies alpha and depth of field calculations
            //Debug.Log("current Focal Length " + currentFocalLength);
        }
    }
}
