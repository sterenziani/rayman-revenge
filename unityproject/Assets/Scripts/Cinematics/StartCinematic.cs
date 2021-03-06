using System.Collections;
using UnityEngine;
public class StartCinematic : MonoBehaviour
{
    [SerializeField] Vulnerable Murfy;
    [SerializeField] Player Rayman;

    [SerializeField] Transform MurfyLookAt;

    [SerializeField] Camera cinematicCamera;

    private Camera mainCamera;

    private DialogueUI dialogueUI;

    private void Start()
    {
        if(!PlayerPrefs.HasKey("watchedCutscene1") || PlayerPrefs.GetInt("watchedCutscene1") == 0)
        {
            dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
            mainCamera = Camera.main;
            StartCoroutine(CinematicCoroutine());
        }
    }

    private IEnumerator CinematicCoroutine()
    {
        yield return null;

        SceneController.EnterCinematicMode();
        cinematicCamera.enabled = true;
        mainCamera.enabled = false;
        Rayman.ToggleHelicopter(true);

        yield return new WaitUntil(() => Rayman.IsGrounded());

		Rayman.ToggleHelicopter(false);
		//Murfy.LookAt(Rayman.transform);
        Rayman.LookAt(MurfyLookAt);

		yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
			Murfy,
			"What a calamity! With the <b>Heart of the World</b> gone, the Glade of Dreams has started to collapse!"));

		yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
			Murfy,
			"It must be returned to its rightful home quickly, or we will all soon be destroyed!"));

		yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Rayman,
            "I know! But where could it be?"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Murfy,
            "That we do not know... but we can find out."));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Rayman,
            "How?"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Murfy,
            "There's an <b>enchanted mirror</b> nearby, that's why I brought us here. It should be able to show us the location of the Heart of the World!"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Rayman,
            "Sounds good. But why did you drop me here and not directly in front of the mirror?"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Murfy,
            "Oh... You're not going to like this..."));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Rayman,
            "What?! What could possibly upset me more than the literal end of the world?!"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Murfy,
            "I didn't bring you to the mirror because it's heavely guarded... by <b>Hoodlums</b>."));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Rayman,
            "Hoodlums?! You can't be serious! I defeated Andr? at the Tower of the Leptys! How can this be?!"));

        yield return StartCoroutine(dialogueUI.ShowDialogueCoroutine(
            Murfy,
            "I don?t have any answers right now, but the mirror does! Get to it, and it will provide the knowledge we seek!"));

        cinematicCamera.enabled = false;
        mainCamera.enabled = true;
		Rayman.transform.rotation = Quaternion.Euler(0, 0, 0);
		Rayman.transform.position = new Vector3(0, 1, 0);
		SceneController.ExitCinematicMode();
        PlayerPrefs.SetInt("watchedCutscene1", 1);
        Destroy(gameObject);

    }
}
