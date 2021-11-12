using UnityEngine;

public class TutorialShower : MonoBehaviour
{
    private DialogueUI dialogueUI;

    [SerializeField] string tutorialText;
    [SerializeField] int durationInMillis;
    [SerializeField] bool waitForKeyPress = false;
    [SerializeField] bool destroyAfterShow = false;

    // Start is called before the first frame update
    void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
    }

    private async void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            if(waitForKeyPress)
                await dialogueUI.ShowTutorial(tutorialText);
            else
                await dialogueUI.ShowTutorial(tutorialText, durationInMillis);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() != null && this != null && destroyAfterShow && this.gameObject != null)
            Destroy(this.gameObject);
    }
}
