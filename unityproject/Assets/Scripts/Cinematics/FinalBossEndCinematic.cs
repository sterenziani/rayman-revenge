using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossEndCinematic : MonoBehaviour
{
    [SerializeField] List<Vulnerable> requireDestroyedBeforeCinematic;

    private DialogueUI dialogueUI;
    [SerializeField] DarkRayman DarkRayman;
    [SerializeField] GameObject heartOfTheWorld;
    [SerializeField] Player Rayman;
    [SerializeField] GameObject exitPortal;

    [SerializeField] AudioClip endingMusic;

    private bool started = false;

    void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!started && requireDestroyedBeforeCinematic.TrueForAll(go => go == null || go.gameObject == null || !go.IsAlive()))
        {
            started = true;
            StartCoroutine(FinishLevel2CinematicCoroutine());
        }
    }

    private IEnumerator FinishLevel2CinematicCoroutine()
    {
        yield return null;

        SceneController.EnterCinematicMode();

        //yield return Rayman.Celebrate();

        //Camera.main.transform.LookAt(entryPortal.transform);
        DarkRayman.GetComponent<Rigidbody>().useGravity = true;
        DarkRayman.manualAnimator.PlayForceContinuous("Idle");

        Rayman.LookAt(DarkRayman.transform);

        var projectiles = GameObject.FindObjectsOfType<Projectile>();
        if (projectiles != null && projectiles.Length > 0)
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.gameObject.SetActive(false);
            }
        }

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(DarkRayman, "I see... only darkness... ahead of me..."));

        DarkRayman.manualAnimator.PlayForceContinuous("Dying");

        yield return new WaitForSeconds(3f);

        Destroy(DarkRayman.gameObject);

        heartOfTheWorld.SetActive(true);

        SceneController.PlayMusic(endingMusic);

        //StartCoroutine(LookAtConstantly(Camera.main.transform, heartOfTheWorld.transform));


        //StartCoroutine(LookAtConstantly(Camera.main.transform, new Vector3(0, 6, 0), 0.1f));
        //Vector3 finalCamPosition = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 4);
        //StartCoroutine(Move(Camera.main.transform, finalCamPosition, 0.15f));
        yield return StartCoroutine(Move(heartOfTheWorld.transform, new Vector3(0, 3, 0), 0.15f));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Wow..."));

        exitPortal.SetActive(true);
        StartCoroutine(ScaleLerp(exitPortal.transform, Vector3.zero, exitPortal.transform.localScale, 0.35f));

        SceneController.ExitCinematicMode();
    }

    private IEnumerator ScaleLerp(Transform transform, Vector3 initialScale, Vector3 endingScale, float speed = 0.2f)
    {
        float startTime = Time.time;

        Vector3 currScale;

        do
        {
            float t = (Time.time - startTime) * speed;
            currScale = Vector3.Lerp(initialScale, endingScale, t);

            transform.localScale = currScale;

            yield return null;
        } while (currScale != endingScale);
    }

    private IEnumerator Move(Transform transform, Vector3 endPosition, float speed = 0.01f)
    {
        float startTime = Time.time;
        Vector3 startPosition = heartOfTheWorld.transform.position;

        Vector3 currPosition;

        do
        {
            float t = (Time.time - startTime) * speed;
            currPosition = Vector3.Lerp(startPosition, endPosition, t);
            transform.position = currPosition;

            yield return null;
        } while (currPosition != endPosition);

        transform.position = endPosition;
    }
}
