using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject PauseMenuUI;
    public AudioClip pauseSound;
    public AudioClip unpauseSound;
    private AudioSource audioSource;

    void Start()
    {
        PauseMenuUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (gameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        audioSource.PlayOneShot(unpauseSound);
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        audioSource.PlayOneShot(pauseSound);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}