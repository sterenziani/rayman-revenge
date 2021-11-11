using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] GameObject DialogueBox;
    [SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] Image speakerSprite;
    [SerializeField] Sprite tutorialSprite;
    [SerializeField] TextMeshProUGUI textField;

    [SerializeField] float writingSpeed = 30f;

    private float latestStartTime;

    private void Start()
    {
        HideDialog();
    }

    public async Task ShowTutorial(string text, int? durationInMillis = null)
    {
        speakerName.text = "Murfy";
        speakerSprite.sprite = tutorialSprite;
        speakerSprite.gameObject.SetActive(true);

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
        float startTime = Time.realtimeSinceStartup;
        latestStartTime = startTime;

        DialogueBox.SetActive(true);

        await WriteText(text, startTime);

        if (startTime != latestStartTime)
            return;

        if (durationInMillis == null)
            await WaitForKeyPress();
        else
            await Task.Delay(durationInMillis.Value);

        if (startTime == latestStartTime)
            HideDialog();
    }

    private async Task WriteText(string text, float startTime)
    {
        textField.text = String.Empty;
        int speedMultiplier = 1;
        int lastCharIndex = -1;
        int openingTags = 0;
        int closingSlashes = 0;
        int closingTags = 0;
        bool readingTags = false;

        float t = 0;
        int charIndex = 0;

        while(charIndex < text.Length)
        {
            t += Time.deltaTime * writingSpeed * speedMultiplier;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, text.Length);
            if (lastCharIndex != charIndex && charIndex < text.Length)
            {
                lastCharIndex = charIndex;

                if (text[charIndex] == '<')
                {
                    openingTags++;
                    readingTags = true;
                }
                if (text[charIndex] == '/' && openingTags == closingTags + 1)
                    closingSlashes++;
                if (text[charIndex] == '>')
                {
                    closingTags++;
                    if (openingTags == closingTags && openingTags == 2*closingSlashes)
                        readingTags = false;
                }
                if (!readingTags)
                {
                    int lastIndex = charIndex + 1;
                    if (lastIndex < text.Length)
                        lastIndex = charIndex + 1;
                    textField.text = text.Substring(0, lastIndex);
                    speedMultiplier = 1;
                }
                else
                    speedMultiplier = 3;
            }

            if (startTime != latestStartTime)
                return;

            await Task.Yield();

            if (startTime != latestStartTime)
                return;
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
