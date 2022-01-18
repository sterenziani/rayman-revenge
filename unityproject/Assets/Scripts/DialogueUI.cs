using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
	[SerializeField] GameObject DialogueBox;
	[SerializeField] GameObject SkipPrompt;
	[SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] Image speakerSprite;
	[SerializeField] Sprite tutorialSprite;
	[SerializeField] Image forwardButtonSprite;
	[SerializeField] TextMeshProUGUI textField;

    [SerializeField] float writingSpeed = 30f;

    private bool durationCountdownStarted = false;
    private bool listeningForNextPress = false;
    private bool nextButtonPressed = false;
    private bool holdingSpeedUpButton = false;

    private MinimapController minimap;
    public bool prevMinimapState = false;

    private void Start()
	{
		forwardButtonSprite.gameObject.SetActive(false);
        minimap = GameObject.Find("Minimap").GetComponent<MinimapController>();
        HideDialog();
    }

    public IEnumerator ShowTutorialCoroutine(string text, int durationInMillis = 0, bool waitForKeyPress = false, Action callback = null)
    {
        speakerName.text = "Murfy";
        speakerSprite.sprite = tutorialSprite;
        speakerSprite.gameObject.SetActive(true);
		forwardButtonSprite.gameObject.SetActive(false);

		yield return StartCoroutine(ShowTextCoroutine(text, durationInMillis, waitForKeyPress, callback));
    }

    public void activateDurationCountdown()
    {
        durationCountdownStarted = true;
    }

    public void ShowTutorial(string text, int durationInMillis = 0, bool isContinuation = false, Action callback = null)
    {
        speakerName.text = "Murfy";
        speakerSprite.sprite = tutorialSprite;
        speakerSprite.gameObject.SetActive(true);
		forwardButtonSprite.gameObject.SetActive(false);
		ShowText(text, durationInMillis, false, callback, isContinuation);
    }

    public void ShowDialogue(Vulnerable speaker, string text, bool isContinuation = false, Action callback = null)
    {
        speakerName.text = speaker.Name;
        speakerSprite.sprite = speaker.sprite;
        speakerSprite.gameObject.SetActive(true);
        ShowText(text, 0, true, callback, isContinuation);
    }

    public IEnumerator ShowDialogueCoroutine(Vulnerable speaker, string text)
    {
        speakerName.text = speaker.Name;
        speakerSprite.sprite = speaker.sprite;
        speakerSprite.gameObject.SetActive(true);
        yield return StartCoroutine(ShowTextCoroutine(text, 0, true, null));
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

    private void ShowText(string text, int durationInMillis = 0, bool waitForKeyPress = false, Action callback = null, bool isContinuation = false)
    {
        if (!isContinuation)
            StopAllCoroutines();

        StartCoroutine(ShowTextCoroutine(text, durationInMillis, waitForKeyPress, callback));
    }

    public IEnumerator ShowTextCoroutine(string text, int durationInMillis = 0, bool waitForKeyPress = false, Action callback = null)
    {
        prevMinimapState = minimap.status;
        minimap.SetMinimapStatus(false);

        durationCountdownStarted = false;
        DialogueBox.SetActive(true);
		if (waitForKeyPress)
			SkipPrompt.SetActive(true);

		List<string> parsedText = ParseText(text);
        textField.text = String.Empty;

        float t = 0;
        int charIndex = 0;
        int lastCharIndex = 0;
        StringBuilder sb = new StringBuilder();

		yield return new WaitForSeconds(0.1f);
		//while (charIndex < parsedText.Count && !Input.GetKey(KeyCode.LeftControl))
        while(charIndex < parsedText.Count && !holdingSpeedUpButton)
        {
            t += Time.deltaTime * writingSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, parsedText.Count);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                sb.Append(parsedText[i]);
            }

            textField.text = sb.ToString();

            lastCharIndex = charIndex;

            yield return null;
        }

        textField.text = text;
		//Write text
		yield return new WaitForSeconds(0.2f);
		if (waitForKeyPress)
		{
			forwardButtonSprite.gameObject.SetActive(true);
            //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl));

            nextButtonPressed = false;
            listeningForNextPress = true;
            yield return new WaitUntil(() => nextButtonPressed || holdingSpeedUpButton);
            listeningForNextPress = false;
            forwardButtonSprite.gameObject.SetActive(false);
			HideDialog();
		}
		else
		{
            while(!durationCountdownStarted)
            {
                yield return null;
            }

            yield return new WaitForSeconds(durationInMillis / 1000f);
			if(text == textField.text)
				HideDialog();
		}

        if(prevMinimapState)
            minimap.SetMinimapStatus(prevMinimapState);

        callback?.Invoke();
	}

    void HideDialog()
    {
        DialogueBox.SetActive(false);
		SkipPrompt.SetActive(false);
	}

    public void OnNextDialog()
    {
        if(listeningForNextPress)
            nextButtonPressed = true;
    }

    public void OnFastForwardDialog()
    {
        holdingSpeedUpButton = !holdingSpeedUpButton;
    }
}
