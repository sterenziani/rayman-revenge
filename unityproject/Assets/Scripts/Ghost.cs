using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	private float speed;
	public float regularSpeed = 5.6f;
	public float scaredSpeed = 2.5f;
	private Animator _anmCtrl;
	private SpriteRenderer _sprRnd;
	private GameBoard gameBoard;
	private AudioSource audioSource;

	public enum Mode { CHASE, SCATTER, SCARED, EATEN };
	private Mode prevMode;
	private Mode currentMode = Mode.CHASE;
	public enum GhostType { BLINKY, PINKY, INKY, CLYDE };
	public GhostType type = GhostType.BLINKY;
	private GameObject[] ghosts;

	public bool canMove = true;
	public bool isInGhostCage = true;
	private bool scaredWhite = false;       // Used to check what stage of color blinking it's in
	public int baseReleaseWait = 0;			// Time the ghost must remain in cage in Level 1
	private float ghostReleaseTimer = 0;    // Timer to check when it's time to release ghost from cage

	public Node spawnNode;					// Node the ghost starts from when reset
	public Node retreatNode;				// Node the ghost retreats to when eaten
	public Node homeNode;					// Node the ghost gravitates towards when in SCATTER mode
	public Node prevNode, currentNode, targetNode;
	private Vector2 direction = Vector2.zero;
	private Vector2 nextDirection = Vector2.zero;
	private GameObject pacman;
	private GameObject blinky;

	private int modeChangeIndex = 0;
	private static int CHASE_MODE_LENGTH = 20;


	private float modeChangeTimer = 0;		// Timer to check when it's time to change modes
	private float scaredModeTimer = 0;		// Timer to check how long they�ve been in scared mode
	private float whiteModeTimer = 0;       // Timer to check when it's time to blink different a color

	// These will be set when a level starts
	private static Mode[] modeOrder;
	private static int[] modeDurations;
	private int scaredModeDuration;
	private int releaseWait;

	public RuntimeAnimatorController regularAnimatorController;
	public RuntimeAnimatorController scaredAnimatorController;
	public RuntimeAnimatorController whiteAnimatorController;
	public RuntimeAnimatorController eyesAnimatorController;
	
	void Start()
    {
		_anmCtrl = GetComponent<Animator>();
		_sprRnd = GetComponent<SpriteRenderer>();
		gameBoard = GameObject.Find("Game").GetComponent<GameBoard>();
		audioSource = GameObject.Find("Game").transform.GetComponent<AudioSource>();
		pacman = GameObject.FindGameObjectWithTag("Player");
		blinky = GameObject.Find("Ghost - Blinky");
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
		currentNode = spawnNode;
		InitializeGhost();
	}

	public void moveToSpawn()
	{
		transform.position = spawnNode.transform.position;
		currentMode = Mode.CHASE;
	}

	public void Restart()
	{
		transform.position = spawnNode.transform.position;
		currentNode = spawnNode;
		ghostReleaseTimer = 0;
		modeChangeIndex = 0;
		isInGhostCage = (type != GhostType.BLINKY);
		InitializeGhost();
		currentMode = Mode.CHASE;
		gameBoard.PlayNormalAlarm();
		canMove = true;
		transform.GetComponent<SpriteRenderer>().enabled = true;
	}

	void InitializeGhost()
	{
		SetDifficultyForLevel(GameBoard.level);
		speed = regularSpeed;
		if (isInGhostCage)
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
		ghostReleaseTimer = 0;
		modeChangeTimer = 0;
		scaredModeTimer = 0;
		whiteModeTimer = 0;
		currentMode = prevMode;
	}

    // Update is called once per frame
    void Update()
    {
		if(canMove)
		{
			UpdateMode();
			UpdateMovement();
			TryToExitCage();
		}
		UpdateSprite();
	}

	void ChangeMode(Mode m)
	{
		if(m == Mode.SCARED && currentMode != Mode.EATEN)
		{
			speed = scaredSpeed;
			if(currentMode != Mode.SCARED)
				prevMode = currentMode;
		}
		else if (m == Mode.EATEN)
		{
			speed = regularSpeed * 3;
		}
		else
			speed = regularSpeed;
		if (currentMode != Mode.EATEN || m != Mode.SCARED)
			currentMode = m;
	}

	public void StartScaredMode()
	{
		gameBoard.PlayScaredAlarm();
		GameBoard.ghostEatenScore = 200;
		scaredModeTimer = 0;
		if(!isInGhostCage)
			TurnAround();
		ChangeMode(Mode.SCARED);
	}

	void ExitScaredMode()
	{
		gameBoard.PlayNormalAlarm();
		scaredModeTimer = 0;
		ChangeMode(prevMode);
	}

	void StartEatenMode()
	{
		ChangeMode(Mode.EATEN);
		GameBoard.score += GameBoard.ghostEatenScore;
		gameBoard.StartEaten(this.GetComponent<Ghost>());
		GameBoard.ghostEatenScore *= 2;
	}

	void ExitEatenMode()
	{
		bool lastGhost = true;
		foreach (GameObject g in ghosts)
		{
			if (g.GetComponent<Ghost>().currentMode == Mode.SCARED)
			{
				lastGhost = false;
				break;
			}
		}
		if(lastGhost)
		{
			gameBoard.PlayNormalAlarm();
		}
		currentNode = retreatNode;
		isInGhostCage = true;
		ChangeMode(prevMode);
		InitializeGhost();
		releaseWait = 2;
	}

	void SetDifficultyForLevel(int level)
	{
		float[] speeds = {5.6f, 5.9f, 6.2f, 6.5f * level/4};
		float[] scaredSpeeds = { 2.7f, 3.0f, 3.3f, 3.5f * level/4};
		int[] scaredModeDurations = { 10, 9, 7, Mathf.Max(3, 5 * level/4)};
		Mode[][] order = {
							new Mode[]{ Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE },
							new Mode[]{ Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE },
							new Mode[]{ Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE, Mode.SCATTER, Mode.CHASE },
							new Mode[]{ Mode.CHASE}
						};
		int[][] durations = {
								new int[]{ 7, 7, CHASE_MODE_LENGTH, 7, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH, 7, CHASE_MODE_LENGTH },
								new int[]{ 7, 5, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH },
								new int[]{ 7, 4, CHASE_MODE_LENGTH, 4, CHASE_MODE_LENGTH, 5, CHASE_MODE_LENGTH, 4, CHASE_MODE_LENGTH },
								new int[]{ CHASE_MODE_LENGTH }
							};
		if (level > speeds.Length)
			level = speeds.Length;
		regularSpeed = speeds[level-1];
		scaredSpeed = scaredSpeeds[level-1];
		modeOrder = order[level-1];
		modeDurations = durations[level-1];
		scaredModeDuration = scaredModeDurations[level-1];
		releaseWait = Mathf.Max(0, baseReleaseWait - level);
	}

	public void TouchPacman()
	{
		if (currentMode == Mode.SCARED)
			StartEatenMode();
		else if (currentMode != Mode.EATEN)
		{
			gameBoard.StartDeath();
		}
	}

	void UpdateMode()
	{
		if (currentMode == Mode.EATEN)
		{
			// If I passed the spawnNode, that means I'm stuck and should reset currentNode
			if (prevNode == retreatNode)
				ExitEatenMode();
		}
		else if (currentMode == Mode.SCARED)
		{
			scaredModeTimer += Time.deltaTime;
			if (scaredModeTimer > scaredModeDuration)
			{
				ExitScaredMode();
			}
			else if (scaredModeTimer > (scaredModeDuration-3))
			{
				whiteModeTimer += Time.deltaTime;
				if (whiteModeTimer >= 0.1f)
				{
					scaredWhite = !scaredWhite;
					whiteModeTimer = 0f;
				}
			}
			else
				scaredWhite = false;
		}
		else if (currentMode != Mode.EATEN)
		{
			modeChangeTimer += Time.deltaTime;
			if (modeChangeTimer > modeDurations[modeChangeIndex])
			{
				// Go to next mode if available
				if(modeChangeIndex < modeOrder.Length - 1)
					modeChangeIndex++;
				ChangeMode(modeOrder[modeChangeIndex]);
				modeChangeTimer = 0;
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
				if (otherPortal != null)
				{
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
		if(currentMode == Mode.SCARED)
		{
			if(scaredWhite)
				_anmCtrl.runtimeAnimatorController = whiteAnimatorController;
			else
				_anmCtrl.runtimeAnimatorController = scaredAnimatorController;
		}
		else if(currentMode == Mode.EATEN)
		{
			_anmCtrl.runtimeAnimatorController = eyesAnimatorController;
		}
		else
		{
			_anmCtrl.runtimeAnimatorController = regularAnimatorController;
		}
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
			case Mode.EATEN:
				targetTile = retreatNode.transform.position;
				break;
			case Mode.SCARED:
				int r = Random.Range(1, 4);
				switch(r)
				{
					case 1:
						targetTile = GetBlinkyTargetTile();
						break;
					case 2:
						targetTile = GetPinkyTargetTile();
						break;
					case 3:
						targetTile = GetInkyTargetTile();
						break;
					case 4:
						targetTile = GetClydeTargetTile();
						break;
				}
				break;
		}
		Node destinationNode = null;
		int destinationCounter = 0;
		Node[] foundDestinations = new Node[4];
		Vector2[] foundDestinationsDirection = new Vector2[4];

		Node[] neighbors;
		if (currentMode == Mode.EATEN)
			neighbors = currentNode.GetAllNeighbors();
		else
			neighbors = currentNode.neighbors;

		// Add possible directions, excluding going backwards
		for (int i=0; i < neighbors.Length; i++)
		{
			if(currentNode.validDirections[i] != direction*-1){
				foundDestinations[destinationCounter] = neighbors[i];
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
		Vector2 blinkyTile = transform.position;
		Vector2 pacmanTile = transform.position;
		GameObject tile = NodeUtilities.GetTileAtPosition(blinky.transform.position, gameBoard);
		if(tile != null)
			blinkyTile = tile.transform.position;
		tile = NodeUtilities.GetTileAtPosition(pacman.transform.position, gameBoard);
		if(tile != null)
			pacmanTile = tile.transform.position;
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

	void TurnAround()
	{
		// If prevNode has no other neighbors, the ghost will get stuck as they can't go backwards
		if(prevNode.neighbors.Length > 1)
		{
			Node temp = prevNode;
			prevNode = targetNode;
			targetNode = temp;
			direction = -direction;
		}
	}
}
