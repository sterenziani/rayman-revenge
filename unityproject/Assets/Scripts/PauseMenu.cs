using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject PauseMenuUI;
    public AudioClip pauseSound;
    public AudioClip unpauseSound;
    private AudioSource audioSource;

    void Start()
    {
        Cursor.visible = false;
        PauseMenuUI.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPause()
    {
        if (gameIsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        audioSource.PlayOneShot(unpauseSound);

        Cursor.visible = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        audioSource.PlayOneShot(pauseSound);

        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
