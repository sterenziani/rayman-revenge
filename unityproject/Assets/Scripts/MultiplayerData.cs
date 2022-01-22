using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class MultiplayerData : MonoBehaviour
{
    public int playerId;
    public Vector3 initialPosition;
    public PlayerInput playerInput;
    public Color color;

    public Player player { get; private set; }
    public CameraController cameraController { get; private set; }
    public PlayerHudController playerHudController { get; private set; }

    private PauseMenu pauseMenu;
    private DialogueUI dialogueUI;
    private MinimapController minimapController;

    void Start()
    {
        player = gameObject.GetComponentInChildren<Player>();
        cameraController = gameObject.GetComponentInChildren<CameraController>();

        if(playerId > 1)
        {
            AudioListener audioListener = cameraController.gameObject.GetComponent<AudioListener>();
            if(audioListener != null)
            {
                DestroyImmediate(audioListener);
            }
        }

        pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        dialogueUI = GameObject.FindObjectOfType<DialogueUI>();
        minimapController = GameObject.FindObjectOfType<MinimapController>();

        player.gameObject.transform.position = initialPosition;

        playerHudController = GameObject.Find($"Player {playerId} Health Bar")?.GetComponent<PlayerHudController>();
        playerHudController?.SetTrackingVulnerable(player);

        player.SetOverlayColor(color);
    }

    void OnAttack()
    {
        player?.OnAttack();
    }

    void OnMovement(InputValue value)
    {
        player?.OnMovement(value);
    }

    void OnJump()
    {
        player?.OnJump();
    }

    void OnCameraMovement(InputValue value)
    {
        cameraController?.OnCameraMovement(value);
    }

    void OnCameraZoomOut()
    {
        cameraController.OnCameraZoomOut();
    }

    void OnCameraZoomIn()
    {
        cameraController.OnCameraZoomIn();
    }

    void OnCameraZoomInMouseScroll()
    {
        OnCameraZoomInMouseScroll();
    }

    void OnPause()
    {
        pauseMenu?.OnPause();
    }

    void OnNextDialog()
    {
        dialogueUI?.OnNextDialog();
    }

    void OnFastForwardDialog()
    {
        dialogueUI?.OnFastForwardDialog();
    }

    void OnMinimapToggle()
    {
        minimapController?.OnMinimapToggle();
    }
}
