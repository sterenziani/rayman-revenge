using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public async Task ShowTutorial(string text, int durationInMillis = 0)
    {
        speakerName.text = "Murfy";
        speakerSprite.sprite = tutorialSprite;
        speakerSprite.gameObject.SetActive(true);

        await ShowText(text, durationInMillis);
    }

    public async Task ShowDialogue(Vulnerable speaker, string text)
    {
        speakerName.text = speaker.Name;
        speakerSprite.sprite = speaker.sprite;
        speakerSprite.gameObject.SetActive(true);

        await ShowText(text, 0, true);
    }

    private List<string> ParseText(string text)
    {
        List<string> result = new List<string>();

        HashSet<string> currentTags = new HashSet<string>();
        bool tagClosing = false;
        bool tagOpened = false;
        string currentTag = "";

        foreach(char c in text)
        {
            if(c == '<')
            {
                tagOpened = true;
            } else if(c == '>')
            {
                if (!tagClosing)
                    currentTags.Add(currentTag);
                else
                    currentTags.Remove(currentTag);

                currentTag = "";
                tagOpened = false;
                tagClosing = false;
            }
            else if(c == '/' && tagOpened)
            {
                tagClosing = true;
            }
            else
            {
                if(tagOpened)
                {
                    currentTag += c;
                } else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string tag in currentTags)
                    {
                        sb.Append($"<{tag}>");
                    }

                    sb.Append(c);

                    foreach (string tag in currentTags)
                    {
                        sb.Append($"</{tag}>");
                    }

                    result.Add(sb.ToString());
                }
            }
        }

        return result;
    }

    private async Task ShowText(string text, int durationInMillis = 0, bool waitForKeyPress = false)
    {
        float startTime = Time.realtimeSinceStartup;
        latestStartTime = startTime;

        DialogueBox.SetActive(true);

        await WriteText(text, startTime);

        if (startTime != latestStartTime)
            return;

		if (waitForKeyPress)
		{
			await WaitForKeyPress();
			if (startTime == latestStartTime)
				HideDialog();
		}
		else
		{
			StartCoroutine(HideDialogAfter(text, durationInMillis));
		}
	}

	IEnumerator HideDialogAfter(string desiredText, int ms)
	{
		yield return new WaitForSeconds(ms / 1000f);
		if(textField.text == desiredText)
			HideDialog();
	}

	private async Task WriteText(string text, float startTime)
    {
        List<string> parsedText = ParseText(text);

        textField.text = String.Empty;

        float t = 0;
        int charIndex = 0;
        int lastCharIndex = 0;
        StringBuilder sb = new StringBuilder();

        while (charIndex < parsedText.Count && !Input.GetKeyDown(KeyCode.LeftControl))
        {
            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, parsedText.Count);

            for(int i = lastCharIndex; i < charIndex; i++)
            {
                sb.Append(parsedText[i]);
            }

            textField.text = sb.ToString();

            lastCharIndex = charIndex;

            if (startTime != latestStartTime)
                return;

            await Task.Yield();
        }

        textField.text = text;
    }

    private async Task WaitForKeyPress()
    {
        while (PauseMenu.gameIsPaused || (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.LeftControl)))
        {
            await Task.Yield();
        }
    }

    void HideDialog()
    {
        DialogueBox.SetActive(false);
    }
}
