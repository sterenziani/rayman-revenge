using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	public float speed = 4.9f;
	private Animator _anmCtrl;
	private SpriteRenderer _sprRnd;
	private GameBoard gameBoard;

	public enum Mode { CHASE, SCATTER, SCARED, EATEN };
	private Mode currentMode = Mode.CHASE;
	public enum GhostType { BLINKY, PINKY, INKY, CLYDE };
	public GhostType type = GhostType.BLINKY;

	public bool isInGhostCage = true;
	public int releaseWait = 0;
	private float ghostReleaseTimer = 0;

	public Node spawnNode;
	public Node homeNode;
	private Node prevNode, currentNode, targetNode;
	private Vector2 direction = Vector2.zero;
	private Vector2 nextDirection = Vector2.zero;
	private GameObject pacman;
	private GameObject blinky;

	private float modeChangeTimer = 0;
	private int modeChangeIndex = 0;
	private static int CHASE_MODE_LENGTH = 20;
	private static Mode[] modeOrder = { Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE};
	private static int[] modeTimers = { 7, 7, CHASE_MODE_LENGTH, 7, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH, 7, CHASE_MODE_LENGTH };

	// Start is called before the first frame update
	void Start()
    {
		_anmCtrl = GetComponent<Animator>();
		_sprRnd = GetComponent<SpriteRenderer>();
		gameBoard = GameObject.Find("Game").GetComponent<GameBoard>();
		pacman = GameObject.FindGameObjectWithTag("Player");
		blinky = GameObject.Find("Ghost - Blinky");
		Node node = NodeUtilities.GetNodeAtPosition(transform.localPosition, gameBoard);
		if (node != null)
			currentNode = node;
		if(isInGhostCage)
		{
			direction = currentNode.validDirections[0];
			targetNode = currentNode.neighbors[0];
		}
		else
		{
			direction = Vector2.left;
			targetNode = ChooseNextNode();
		}
		prevNode = currentNode;
	}

    // Update is called once per frame
    void Update()
    {
		UpdateMode();
		UpdateMovement();
		TryToExitCage();
		UpdateSprite();
	}

	void ChangeMode(Mode m)
	{
		currentMode = m;
	}

	void UpdateMode()
	{
		if(currentMode != Mode.SCARED)
		{
			modeChangeTimer += Time.deltaTime;
			if (modeChangeTimer > modeTimers[modeChangeIndex])
			{
				// Go to next mode if available
				if(modeChangeIndex < modeOrder.Length - 1)
					modeChangeIndex++;
				ChangeMode(modeOrder[modeChangeIndex]);
				modeChangeTimer = 0;
				Debug.Log("(" +modeChangeIndex +") Changing to " + modeOrder[modeChangeIndex] + " mode!");
			}
		}
	}

	void UpdateMovement()
	{
		if (targetNode != null && targetNode != currentNode && !isInGhostCage)
		{
			// If I passed a node, go back and plan next step
			if (NodeUtilities.OvershotTarget(prevNode.transform.position, transform.localPosition, targetNode.transform.position))
			{
				currentNode = targetNode;
				transform.localPosition = currentNode.transform.position;

				// Check if I'm at a portal
				GameObject otherPortal = NodeUtilities.GetPortal(currentNode.transform.position, gameBoard);
				if (otherPortal != null){
					transform.localPosition = otherPortal.transform.position;
					currentNode = otherPortal.GetComponent<Node>();
				}
				// Set next target
				targetNode = ChooseNextNode();
				prevNode = currentNode;
				currentNode = null;
			}
			else
				transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
		}
	}

	void UpdateSprite()
	{
		_anmCtrl.SetInteger("directionX", (int)direction.x);
		_anmCtrl.SetInteger("directionY", (int)direction.y);
		if (direction == Vector2.left)
			_sprRnd.flipX = true;
		else
			_sprRnd.flipX = false;
	}

	Node ChooseNextNode()
	{
		Vector2 targetTile = Vector2.zero;
		switch(currentMode)
		{
			case Mode.CHASE:
				targetTile = GetTargetTile();
				break;
			case Mode.SCATTER:
				targetTile = homeNode.transform.position;
				break;

		}
		Node destinationNode = null;
		int destinationCounter = 0;
		Node[] foundDestinations = new Node[4];
		Vector2[] foundDestinationsDirection = new Vector2[4];
		// Add possible directions, excluding going backwards
		for(int i=0; i < currentNode.neighbors.Length; i++)
		{
			if(currentNode.validDirections[i] != direction*-1){
				foundDestinations[destinationCounter] = currentNode.neighbors[i];
				foundDestinationsDirection[destinationCounter] = currentNode.validDirections[i];
				destinationCounter++;
			}
		}
		// If I can move, go towards my targetTile
		if (foundDestinations.Length > 0)
		{
			float minDistance = 9999f;
			for(int i=0; i < foundDestinations.Length; i++)
			{
				if(foundDestinationsDirection[i] != Vector2.zero){
					float distance = NodeUtilities.DistanceBetween(foundDestinations[i].transform.position, targetTile);
					if(distance < minDistance){
						minDistance = distance;
						destinationNode = foundDestinations[i];
						direction = foundDestinationsDirection[i];
					}
				}
			}
		}
		return destinationNode;
	}

	Vector2 GetTargetTile()
	{
		Vector2 targetTile = Vector2.zero;
		switch (type)
		{
			case GhostType.BLINKY:
				targetTile = GetBlinkyTargetTile();
				break;
			case GhostType.PINKY:
				targetTile = GetPinkyTargetTile();
				break;
			case GhostType.INKY:
				targetTile = GetInkyTargetTile();
				break;
			case GhostType.CLYDE:
				targetTile = GetClydeTargetTile();
				break;
		}
		return targetTile;
	}

	// Aim for the node that takes me closer to PacMan
	Vector2 GetBlinkyTargetTile()
	{
		return NodeUtilities.GetTileAtPosition(pacman.transform.position, gameBoard).transform.position;
	}

	// Aim two tiles ahead of PacMan
	Vector2 GetPinkyTargetTile()
	{
		Vector2 pacmanTile = NodeUtilities.GetTileAtPosition(pacman.transform.position, gameBoard).transform.position;
		return pacmanTile + (2 * pacman.GetComponent<Player>().direction);
	}

	// Aim to double the distance between Pinky's target and Blinky
	Vector2 GetInkyTargetTile()
	{
		Vector2 blinkyTile = NodeUtilities.GetTileAtPosition(blinky.transform.position, gameBoard).transform.position;
		Vector2 pacmanTile = NodeUtilities.GetTileAtPosition(pacman.transform.position, gameBoard).transform.position;
		Vector2 targetTile = pacmanTile + (2 * pacman.GetComponent<Player>().direction);
		float distance = 2 * NodeUtilities.DistanceBetween(blinkyTile, targetTile);

		targetTile = new Vector2(blinkyTile.x + distance, blinkyTile.y + distance);
		return targetTile;
	}

	// Like Blinky, but if he's 8 tiles or less away from his home node, then target his home node
	Vector2 GetClydeTargetTile()
	{
		float distance = NodeUtilities.DistanceBetween(transform.localPosition, pacman.transform.position);
		Vector2 targetTile = Vector2.zero;
		if(distance > 8)
			targetTile = NodeUtilities.GetTileAtPosition(pacman.transform.position, gameBoard).transform.position;
		else
			targetTile = homeNode.transform.position;
		return targetTile;
	}

	void TryToExitCage()
	{
		ghostReleaseTimer += Time.deltaTime;
		if (releaseWait < ghostReleaseTimer)
			isInGhostCage = false;
	}
}
