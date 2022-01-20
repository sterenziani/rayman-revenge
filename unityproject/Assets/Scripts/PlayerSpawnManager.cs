using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] PlayerColorAndSpawnPosition[] playerColorAndSpawnPositions;

    public MultiplayerData[] players { get; private set; }

    public GameObject enableOnAllSlotsFilled;

    private PlayerInputManager playerInputManager;

    private void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        players = new MultiplayerData[playerInputManager.maxPlayerCount];
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (players[playerInput.playerIndex] != null)
            throw new System.Exception("Rewriting existing player?");

        players[playerInput.playerIndex] = playerInput.gameObject.GetComponent<MultiplayerData>();

        Debug.Log($"Player {playerInput.playerIndex + 1} joined");

        players[playerInput.playerIndex].playerId = playerInput.playerIndex + 1;
        players[playerInput.playerIndex].playerInput = playerInput;

        if (playerColorAndSpawnPositions.Length > playerInput.playerIndex)
        {
            players[playerInput.playerIndex].initialPosition = playerColorAndSpawnPositions[playerInput.playerIndex].SpawnPosition.position;
            players[playerInput.playerIndex].color = playerColorAndSpawnPositions[playerInput.playerIndex].Color;
        }

        playerInput.DeactivateInput();

        GameObject waitingForPlayerDialogue = GameObject.Find($"WaitingForPlayer{playerInput.playerIndex + 1}Dialogue");
        waitingForPlayerDialogue?.SetActive(false);

        if(RemainingEmptyPlaces() == 0)
        {
            foreach(MultiplayerData playerData in players)
            {
                playerData.playerInput.ActivateInput();
            }

            enableOnAllSlotsFilled?.SetActive(true);
        }
    }

    private int RemainingEmptyPlaces()
    {
        int count = 0;
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
                count++;
        }

        return count;
    }

    [System.Serializable]
    class PlayerColorAndSpawnPosition
    {
        public Color Color;
        public Transform SpawnPosition;
    }
}
