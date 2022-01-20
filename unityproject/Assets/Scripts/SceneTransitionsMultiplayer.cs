using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class SceneTransitionsMultiplayer : SceneTransitions
    {
        private DialogueUI dialogueUI;

        private bool gamepadAvailable = true;
        private bool mouseAvailable = true;
        private bool keyboardAvailable = true;

        private PlayerSpawnManager playerSpawnManager;

        private void Awake()
        {
            dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();
            playerSpawnManager = GameObject.FindObjectOfType<PlayerSpawnManager>();

            InputSystem.onDeviceChange += OnInputDeviceConnected;
        }

        private MultiplayerData GetLastManStandingMultiplayerData()
        {
            foreach (MultiplayerData data in playerSpawnManager.players)
            {
                if (data.player != null) {
                    if (data.player.LifePoints > 0 || data.player.hasWon)
                    {
                        return data;
                    }
                }
            }

            return null;
        }

        public override void ReloadScene()
        {
            MultiplayerData winnerData = GetLastManStandingMultiplayerData();
            StartCoroutine(FinishCoroutine(winnerData));
        }

        IEnumerator FinishCoroutine(MultiplayerData winnerData)
        {
            InputSystem.onDeviceChange -= OnInputDeviceConnected;

            string winText;

            if (winnerData != null)
            {
                yield return winnerData.player.Celebrate();
                winText= $"<b>Player {winnerData.playerId}</b> wins!";
            } else
            {
                winText = "It's a tie!";
            }

            StartCoroutine(dialogueUI.ShowTutorialCoroutine(winText + "\nPress <b>SPACE BAR</b> to return to the main menu.", 
                6000, 
                true, () =>
            {
                LoadNextScene();
            }));
        }

        private void OnInputDeviceConnected(InputDevice device, InputDeviceChange change)
        {
            if(device is Gamepad || device is Keyboard || device is Mouse)
            {
                gamepadAvailable = InputSystem.devices.Any(
                    x => x is Gamepad
                    && x.enabled == true
                    && x.added == true);

                mouseAvailable = InputSystem.devices.Any(
                    x => x is Mouse
                    && x.enabled == true
                    && x.added == true);

                keyboardAvailable = InputSystem.devices.Any(
                    x => x is Keyboard
                    && x.enabled == true
                    && x.added == true);
            }
        }
    }
}
