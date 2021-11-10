using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] GameObject DialogueBox;
    [SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] Image speakerSprite;
    [SerializeField] TextMeshProUGUI textField;

    [SerializeField] float writingSpeed = 30f;

    private void Start()
    {
        HideDialog();
    }

    public async Task ShowTutorial(string text, int? durationInMillis = null)
    {
        speakerName.text = "Suggestion";
        speakerSprite.gameObject.SetActive(false);

        await ShowText(text, durationInMillis);
    }

    public async Task ShowDialogue(Vulnerable speaker, string text, int? durationInMillis = null)
    {
        speakerName.text = speaker.Name;
        speakerSprite.sprite = speaker.sprite;
        speakerSprite.gameObject.SetActive(true);

        await ShowText(text, durationInMillis);
    }

    private async Task ShowText(string text, int? durationInMillis = null)
    {
        DialogueBox.SetActive(true);

        await WriteText(text);

        if (durationInMillis == null)
            await WaitForKeyPress();
        else
            await Task.Delay(durationInMillis.Value);

        HideDialog();
    }

    private async Task WriteText(string text)
    {
        textField.text = String.Empty;

        float t = 0;
        int charIndex = 0;

        while(charIndex < text.Length)
        {
            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);

            textField.text = text.Substring(0, charIndex);

            await Task.Yield();
        }

        textField.text = text;
    }

    private async Task WaitForKeyPress()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            await Task.Yield();
        }
    }

    void HideDialog()
    {
        DialogueBox.SetActive(false);
    }
}
