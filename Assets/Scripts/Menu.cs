using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Menu : MonoBehaviour
{
    public string SceneName;
    private bool paused = false;
    public VolumeProfile volumeProfile;

    public GameObject pauseMenu;
    //button alpha 
    public CanvasGroup canvas;
    private float alphaRef;
    

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

    public void PauseGame()
    {
        if (!paused)
        {
            Time.timeScale = 0.1f;
            Debug.Log("paused");
            pauseMenu.SetActive(true);
            targetAlpha = 1;
        }
        if (paused)
        {
            Time.timeScale = 1f;
            Debug.Log("unpaused");
            //pauseMenu.SetActive(false);
            StartCoroutine(waitForAlpha(0.2f));
            targetAlpha = 0; 
        }
        paused = !paused;
    }
    
    IEnumerator waitForAlpha(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        pauseMenu.SetActive(false);
    }


    private void Start()
    {
        if (!volumeProfile.TryGet(out depthOfField)) throw new System.NullReferenceException(nameof(depthOfField));
        //assigns depth of field to dof variable
        depthOfField.focalLength.Override(focalLength);
    }
    private void Update()
    {
        if (Mathf.Abs(canvas.alpha - targetAlpha) > 0.01f)
        {
            if (paused)
            {
                currentFocalLength = Mathf.SmoothDamp(currentFocalLength, p_focalLength, ref smoothDampRef, smoothSpeed);
                canvas.alpha = Mathf.SmoothDamp(canvas.alpha, targetAlpha, ref alphaRef, smoothSpeed);
            }
            else
            {
                currentFocalLength = Mathf.SmoothDamp(currentFocalLength, focalLength, ref smoothDampRef, smoothSpeed * 5);
                canvas.alpha = Mathf.SmoothDamp(canvas.alpha, targetAlpha, ref alphaRef, smoothSpeed * 5);
            }

            depthOfField.focalLength.Override(currentFocalLength);
            //Debug.Log("current Focal Length " + currentFocalLength);
        }
    }
}
