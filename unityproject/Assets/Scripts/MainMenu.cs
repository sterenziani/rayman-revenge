using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("watchedCutscene1", 0);
        PlayerPrefs.SetInt("watchedCutscene2", 0);
        PlayerPrefs.SetInt("watchedCutscene3", 0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
