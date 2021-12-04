using System.Collections;
using UnityEngine;

public class Level2StartCinematic : MonoBehaviour
{
    [SerializeField] GameObject entryPortal;
    [SerializeField] Player Rayman;

    [SerializeField] Camera cinematicCamera;

    private Camera mainCamera;

    private DialogueUI dialogueUI;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("watchedCutscene2") || PlayerPrefs.GetInt("watchedCutscene2") == 0)
        {
            dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
            mainCamera = Camera.main;
            StartCoroutine(Level2StartCinematicCoroutine());
        }
    }

    private IEnumerator Level2StartCinematicCoroutine()
    {
        yield return null;

        SceneController.EnterCinematicMode();
        Rayman.SetControlledByCinematic(true);

        Rayman.gameObject.SetActive(false);

        cinematicCamera.enabled = true;
        mainCamera.enabled = false;

        yield return new WaitForSeconds(0.7f);

        entryPortal.gameObject.SetActive(true);
        yield return StartCoroutine(ScaleLerp(entryPortal.transform, Vector3.zero, entryPortal.transform.localScale, 0.5f));

        Rayman.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.3f);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Here it is, the <b>Dark Void</b>. But I can't see Dark Rayman."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "I wonder if he knows I'm here..."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "Well, seems there is only one way forward. I have to hurry, I can feel myself fading. Let's go!"));

        StartCoroutine(ScaleLerp(entryPortal.transform, entryPortal.transform.localScale, Vector3.zero, 0.5f));

        cinematicCamera.enabled = false;
        mainCamera.enabled = true;
        SceneController.ExitCinematicMode();
        PlayerPrefs.SetInt("watchedCutscene2", 1);

        Destroy(gameObject);
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
}
