using UnityEngine;

public class TutorialShower : MonoBehaviour
{
    private DialogueUI dialogueUI;

    [SerializeField] string tutorialText;
    [SerializeField] int durationInMillis;
    [SerializeField] bool waitForKeyPress = false;
    [SerializeField] bool destroyAfterShow = false;
	
    void Start()
    {
        dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            StartCoroutine(dialogueUI.ShowTutorialCoroutine(tutorialText, durationInMillis, waitForKeyPress, () =>
            {
                if (other.gameObject.GetComponent<Player>() != null && this != null && destroyAfterShow && this.gameObject != null)
                    Destroy(this.gameObject);
            }));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        dialogueUI.activateDurationCountdown();
        if(destroyAfterShow)
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
