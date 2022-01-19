using System.Collections;
using UnityEngine;
public class MultiplayerStartCinematic : MonoBehaviour
{
    private DialogueUI dialogueUI;

    [SerializeField] Camera player1Camera;
    [SerializeField] Camera player2Camera;
    [SerializeField] Camera cinematicCamera;

    private void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
        StartCoroutine(CinematicCoroutine());
    }

    private IEnumerator CinematicCoroutine()
    {
        yield return null;

        SceneController.EnterCinematicMode();

        player1Camera.enabled = false;
        player2Camera.enabled = false;

        cinematicCamera.enabled = true;

        StartCoroutine(dialogueUI.ShowTutorialCoroutine(
            "Only one Rayman can be the True Rayman! Who of you will be?\nTime to prove it in this <s>Bomberman clone</s> absolutely original game!"
            , 6000, true, () => {
                StartCoroutine(dialogueUI.ShowTutorialCoroutine(
                    "3..."
                    , 1000, false, () =>
                    {
                        StartCoroutine(dialogueUI.ShowTutorialCoroutine(
                            "2..."
                            , 1000, false, () =>
                            {
                                StartCoroutine(dialogueUI.ShowTutorialCoroutine(
                                    "1..."
                                    , 1000, false, () =>
                                    {
                                        player1Camera.enabled = true;
                                        player2Camera.enabled = true;

                                        cinematicCamera.enabled = false;

                                        SceneController.ExitCinematicMode();
                                        Destroy(gameObject);
                                    }));
                            }));
                    }));
            }));
    }
}
