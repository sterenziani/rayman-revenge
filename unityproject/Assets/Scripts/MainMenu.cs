using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject errorPopup;

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

    public void PlayMultiplayer()
    {
        IEnumerable<InputDevice> gamepads = InputSystem.devices.Where(
            x => x is Gamepad
            && x.enabled == true
            && x.added == true);

        if(!gamepads.Any())
        {
            this.gameObject.SetActive(false);
            errorPopup.SetActive(true);
        } else
        {
            SceneManager.LoadScene("MultiplayerMinigame");
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
