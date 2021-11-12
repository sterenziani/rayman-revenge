using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StartCinematic : MonoBehaviour
{
    [SerializeField] Vulnerable Murfy;
    [SerializeField] Player Rayman;

    [SerializeField] Camera cinematicCamera;

    private Camera mainCamera;

    private DialogueUI dialogueUI;

    private void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
        mainCamera = Camera.main;
    }

    private async void OnTriggerEnter(Collider other)
    {
        SceneController.EnterCinematicMode();
        cinematicCamera.enabled = true;
        mainCamera.enabled = false;

        await Task.Delay(1000);

        Murfy.LookAt(Rayman.transform);
        Rayman.LookAt(Murfy.transform);

        await dialogueUI.ShowDialogue(Murfy, "What a calamity! Without its Heart, the Glade of Dreams has started to collapse! It must be returned to it's rightful home quickly, or the entire world will be destroyed!");
        await dialogueUI.ShowDialogue(Rayman, "I know! But where could it be?");
        await dialogueUI.ShowDialogue(Murfy, "That we do not know... but we can find out.");
        await dialogueUI.ShowDialogue(Rayman, "How?");
        await dialogueUI.ShowDialogue(Murfy, "There's an enchanted mirror near here. It should be able to show us the location of the Heart of the World.");
        await dialogueUI.ShowDialogue(Rayman, "Sounds good. But why did you drop me here and not directly in front of the mirror?");
        await dialogueUI.ShowDialogue(Murfy, "Oh... you're not going to like this...");
        await dialogueUI.ShowDialogue(Rayman, "What? What could possibly upset me more than the literal end of the world?");
        await dialogueUI.ShowDialogue(Murfy, "I didn't bring you to the mirror because it's heavely guarded... by Hoodlums.");
        await dialogueUI.ShowDialogue(Rayman, "Hoodlums! You can't be serious! I defeated André at the Tower of the Leptys! How can this be?");
        await dialogueUI.ShowDialogue(Murfy, "I don´t have any answers right now, but the mirror does. Get to it, and it will provide the knowledge we seek.");

        cinematicCamera.enabled = false;
        mainCamera.enabled = true;
        SceneController.ExitCinematicMode();
        Destroy(gameObject);
    }
}
