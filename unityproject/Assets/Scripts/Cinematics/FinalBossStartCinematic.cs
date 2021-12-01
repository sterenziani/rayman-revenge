using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossStartCinematic : MonoBehaviour
{
    [SerializeField] Player Rayman;
    [SerializeField] DarkRayman DarkRayman;

    [SerializeField] Camera cinematicCamera;

    [SerializeField] AudioClip darkRaymanRevealAudioClip;

    [SerializeField] AudioClip preludeMusic;
    [SerializeField] AudioClip battleMusic;

    private Camera mainCamera;

    private DialogueUI dialogueUI;

    private void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
        mainCamera = Camera.main;

        StartCoroutine(CinematicCoroutine());
    }

    private IEnumerator CinematicCoroutine()
    {
        yield return null;

        SceneController.EnterCinematicMode();

        SceneController.PlayMusic(preludeMusic);

        DarkRayman.transform.position = new Vector3(0, -0.5f, 0);
        //DarkRayman.GetComponent<ManualAnimator>()?.PlayForceContinuous("Idle");

        cinematicCamera.enabled = true;
        mainCamera.enabled = false;

        DarkRayman.SetControlledByCinematic(true);
        DarkRayman.LookAt(Rayman.transform);
        Rayman.LookAt(DarkRayman.transform);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "At last... Here you are. I've been waiting for you."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "You were expecting me?"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "Of course I was! I know better than anyone that you will never give up. After all, <b>I am you</b>."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "You are most definitly <b>not</b> me! I would never do something like this."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "You never had to. You've never truly been in chains..."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "...And you don't know the first thing about what someone who has is willing to do to break them."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Is that what this is all about?"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "I was created by Mr. Dark with the explicit purpose of destroying Rayman."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "But he's gone, and this is your choice now!"));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "I know it is. I chose... Not to be a shadow anymore. This is my ultimate freedom."));


        //DarkRayman.GetComponent<ManualAnimator>()?.PlayForceContinuous("Floating");
        yield return StartCoroutine(Move(new Vector3(0, 0.5f, 0), 0.45f));

        DarkRayman.transform.position = new Vector3(0, 0.5f, 0);
        DarkRayman.initialPosition = DarkRayman.transform.position;

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "<b>Prepare to die!</b>"));

        cinematicCamera.enabled = false;
        mainCamera.enabled = true;

        SceneController.PlayMusic(battleMusic);

        SceneController.ExitCinematicMode();

        Destroy(gameObject);
    }

    private IEnumerator Move(Vector3 endPosition, float speed = 0.01f)
    {
        float startTime = Time.time;
        Vector3 startPosition = DarkRayman.transform.position;

        Vector3 currPosition;

        do
        {
            float t = (Time.time - startTime) * speed;
            currPosition = Vector3.Lerp(startPosition, endPosition, t);
            DarkRayman.transform.position = currPosition;

            yield return null;
        } while (currPosition != endPosition);

        DarkRayman.transform.position = endPosition;
    }
}
