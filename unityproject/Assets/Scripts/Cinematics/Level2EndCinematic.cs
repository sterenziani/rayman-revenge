using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2EndCinematic : MonoBehaviour
{
    [SerializeField] List<Vulnerable> requireDestroyedBeforeCinematic;

    private DialogueUI dialogueUI;
    [SerializeField] GameObject entryPortal;
    [SerializeField] Player Rayman;

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
        PlayerPrefs.GetInt("watchedCutscene2", 0);

        yield return Rayman.Celebrate();

        Camera.main.transform.LookAt(entryPortal.transform);
        Rayman.LookAt(entryPortal.transform);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "This portal... I feel... cold..."));
        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "I haven't felt like this since..."));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(Rayman, "<b>He's</b> there, that much I know. I hope I'm ready. I have to be, or else all will be lost."));

        SceneTransitions sceneTransitions = FindObjectOfType<SceneTransitions>();
        sceneTransitions.LoadNextScene();
    }
}
