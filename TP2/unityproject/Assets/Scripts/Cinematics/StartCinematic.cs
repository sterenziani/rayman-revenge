using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCinematic : MonoBehaviour
{
    [SerializeField] Vulnerable Murfy;
    [SerializeField] Player Rayman;

    [SerializeField] DialogueUI dialogueUI;

    private async void OnCollisionEnter(Collision collision)
    {
        SceneController.EnterCinematicMode();

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

        SceneController.ExitCinematicMode();
        Destroy(gameObject);

        /*
        await dialogueUI.ShowDialogue(Murfy, "Well, you've defeated Mr. Dark, Razorbeard and André... so that must mean...");
        await dialogueUI.ShowDialogue(Rayman, "Don't even dare say it, it's not possible!");
        await dialogueUI.ShowDialogue(Murfy, "But you and I know all too well that the demise of your previous enemies means you and you alone are the only creature powerful enough to take the Heart of the World.");
        await dialogueUI.ShowDialogue(Murfy, "Only you could have done it... even if it's another version of you...");
        await dialogueUI.ShowDialogue(Rayman, "You mean... he's back? Dark Rayman's back?");
        await dialogueUI.ShowDialogue(Murfy, "I've suspected something was wrong for a while now... ever since I saw you sneaking into the Fairy Council... except... now we know it wasn't <b>you</b>");
        await dialogueUI.ShowDialogue(Rayman, "So what do we do?");
        await dialogueUI.ShowDialogue(Murfy, "I took us here because");
        */
    }
}
