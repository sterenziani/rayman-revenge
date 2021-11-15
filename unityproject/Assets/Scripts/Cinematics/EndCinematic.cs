using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCinematic : MonoBehaviour
{
    [SerializeField] Vulnerable Murfy;
    [SerializeField] Player Rayman;
    [SerializeField] DarkRayman DarkRayman;
    [SerializeField] Renderer PortalSkyboxRenderer;

    [SerializeField] GameObject RaymanWayPoint;

    [SerializeField] Camera cinematicCamera;

    [SerializeField] AudioClip darkRaymanRevealAudioClip;

    private Camera mainCamera;

    private DialogueUI dialogueUI;

    private void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
        mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
            StartCoroutine(EndCinematicCoroutine());
    }

    private IEnumerator EndCinematicCoroutine()
    {
        SceneController.EnterCinematicMode();
        cinematicCamera.enabled = true;
        mainCamera.enabled = false;

        DarkRayman.SetControlledByCinematic(true);
        DarkRayman.gameObject.SetActive(true);

        Rayman.transform.position = RaymanWayPoint.transform.position;

        Murfy.LookAt(Rayman.transform);
        Rayman.LookAt(Murfy.transform);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "Here it is! The magic mirror. Maybe we'll finally get out answers."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Yes, this situation needs a quick fix, I can feel my energy fading away. How do we use it?"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "Just look into it and think of the Heart of the World. It's position, and hopefully it's current owner, should be reflected."));

        Rayman.LookAt(PortalSkyboxRenderer.transform);
        yield return new WaitForSeconds(0.8f);

        yield return StartCoroutine(FadePortalIn());

        DarkRayman.gameObject.GetComponent<AudioSource>()?.PlayOneShot(darkRaymanRevealAudioClip);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "But that's... That's <b>Dark Rayman</b>, the evil clone of you Mr. Dark created all those years ago!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "It can't be! How did he survive Mr. Dark's defeat?"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "I don't know, I wasn't even part of the franchise back then!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Well, no matter he's behind this, I have to get the Heart back and restore balance before it's too late!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "That's right, you have no choice but to face him. And that place he's in... that's the Dark Void, the empty space between worlds."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Yes, I've been told about it. It's supposed to be deadly, but I'll have to take my chances. I'll use the mirror's magic to go there and stop him!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Murfy, "I won't be able to come with you, since I'm not strong enough, but I know you can save the world. After all, you've done it before!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Let's do this then!"));

        cinematicCamera.enabled = false;
        mainCamera.enabled = true;
        SceneController.ExitCinematicMode();

        SceneTransitions sceneTransitions = FindObjectOfType<SceneTransitions>();
        sceneTransitions.LoadNextScene();
    }

    private IEnumerator FadePortalIn()
    {
        float startTime = Time.time;
        float speed = 0.5f;
        Color startColor = PortalSkyboxRenderer.material.color;
        Color endColor = new Color(
            startColor.r,
            startColor.g,
            startColor.b,
            0.4f);

        Color currColor;

        do
        {
            float t = (Time.time - startTime) * speed;
            currColor = Color.Lerp(startColor, endColor, t);
            PortalSkyboxRenderer.material.color = currColor;

            yield return null;
        } while (currColor != endColor);
    }
}
