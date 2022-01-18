using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class SceneTransitionsMultiplayer : SceneTransitions
    {
        [SerializeField] Player player1;
        [SerializeField] Player player2;

        private DialogueUI dialogueUI;

        private bool gamepadAvailable = true;
        private bool mouseAvailable = true;
        private bool keyboardAvailable = true;

        private void Awake()
        {
            dialogueUI = GameObject.Find("Dialogue UI").GetComponent<DialogueUI>();

            InputSystem.onDeviceChange += OnInputDeviceConnected;
        }

        public override void ReloadScene()
        {
            Player winner = player1.LifePoints <= 0 ? player2 : player1;
            StartCoroutine(FinishCoroutine(winner));
        }

        IEnumerator FinishCoroutine(Player winner)
        {
            InputSystem.onDeviceChange -= OnInputDeviceConnected;

            yield return winner.Celebrate();

            string playerText = winner == player1 ? "Player 1" : "Player 2";

            string winText = $"<b>{playerText}</b> wins!\nPress <b>SPACEBAR</b> to return to the main menu.";

            StartCoroutine(dialogueUI.ShowTutorialCoroutine(winText, 6000, true, () =>
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
