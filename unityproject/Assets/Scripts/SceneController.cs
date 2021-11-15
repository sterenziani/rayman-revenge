using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    private static Vulnerable[] vulnerables;
    public static TutorialShower[] tutorials;
	private static GameObject hud;

    public static void HideHUD()
	{
		hud = GameObject.Find("Play UI");
		if (hud != null)
            hud.SetActive(false);
    }

    public static void ShowHUD()
    {
        if(hud != null)
            hud.SetActive(true);
    }

    public static void EnterCinematicMode()
    {
        vulnerables = FindObjectsOfType<Vulnerable>();
        tutorials = FindObjectsOfType<TutorialShower>();

		foreach (Vulnerable v in vulnerables)
            v.SetControlledByCinematic(true);
        foreach (TutorialShower t in tutorials)
            t.gameObject.SetActive(false);
        HideHUD();
    }

    public static void ExitCinematicMode()
    {
        foreach (Vulnerable v in vulnerables)
            if (v != null && v.gameObject != null)
                v.SetControlledByCinematic(false);
        foreach (TutorialShower t in tutorials)
            if(t != null && t.gameObject != null)
                t.gameObject.SetActive(true);
		ShowHUD();
    }



    public static void StopMusic()
    {
        GameObject soundtrack = GameObject.Find("Sound");
        AudioSource soundtrackSource = null;
        if (soundtrack != null)
        {
            soundtrackSource = soundtrack.GetComponent<AudioSource>();
            if (soundtrackSource != null)
                soundtrackSource.Stop();
        }
    }

    public static void PauseMusic()
    {
        GameObject soundtrack = GameObject.Find("Sound");
        AudioSource soundtrackSource = null;
        if (soundtrack != null)
        {
            soundtrackSource = soundtrack.GetComponent<AudioSource>();
            if (soundtrackSource != null)
                soundtrackSource.Pause();
        }
    }

    public static void ResumeMusic()
    {
        GameObject soundtrack = GameObject.Find("Sound");
        AudioSource soundtrackSource = null;
        if (soundtrack != null)
        {
            soundtrackSource = soundtrack.GetComponent<AudioSource>();
            if (soundtrackSource != null)
                soundtrackSource.UnPause();
        }
    }

    public static void RestartMusic()
    {
        GameObject soundtrack = GameObject.Find("Sound");
        AudioSource soundtrackSource = null;
        if (soundtrack != null)
        {
            soundtrackSource = soundtrack.GetComponent<AudioSource>();
            if (soundtrackSource != null)
                soundtrackSource.Play();
        }
    }

    public static void PlayMusic(AudioClip musicToPlay)
    {
        GameObject soundtrack = GameObject.Find("Sound");
        AudioSource soundtrackSource = null;
        if (soundtrack != null)
        {
            soundtrackSource = soundtrack.GetComponent<AudioSource>();
            if (soundtrackSource != null)
            {
                soundtrackSource.clip = musicToPlay;
                soundtrackSource.Play();
            }
        }
    }
}
