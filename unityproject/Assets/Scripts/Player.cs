using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private Animator _anmCtrl;
	private SpriteRenderer _sprRnd;
	
	public float speed = 5.75f;
	public bool canMove = true;
	public Node startingPosition;
	public Vector2 direction = Vector2.zero;
	private Vector2 nextDirection = Vector2.zero;
	private Node prevNode, currentNode, targetNode;
	private int pelletsConsumed = 0;
	private bool dead = false;
	private KeyCode PACMAN_LEFT_KEY = KeyCode.LeftArrow;
	private KeyCode PACMAN_RIGHT_KEY = KeyCode.RightArrow;
	private KeyCode PACMAN_UP_KEY = KeyCode.UpArrow;
	private KeyCode PACMAN_DOWN_KEY = KeyCode.DownArrow;
	private int PELLET_SCORE = 1;
	private GameBoard gameBoard;
	private GameObject[] ghosts;

	// SONIDOS
	private bool playedFirstMunch = false;
	private AudioSource audioSource;
	public AudioClip munch1;
	public AudioClip munch2;
	public AudioClip deathSound;
	public AudioClip deathQuack;

	// Start is called before the first frame update
	void Start()
	{
		_anmCtrl = GetComponent<Animator>();
		_sprRnd = GetComponent<SpriteRenderer>();
		gameBoard = GameObject.Find("Game").GetComponent<GameBoard>();
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
		audioSource = transform.GetComponent<AudioSource>();
		Restart();
	}

	public void Restart()
	{
		transform.position = startingPosition.transform.position;
		transform.GetComponent<SpriteRenderer>().enabled = true;
		currentNode = startingPosition;
		direction = Vector2.left;
		nextDirection = Vector2.left;
		ChangeDirection(direction);
		canMove = true;
		dead = false;
	}

	// Update is called once per frame
	void Update()
	{
		if(canMove)
		{
			ReadInput();
			UpdateMovement();
			ConsumePellet();
		}
		UpdateSprite();
	}

	void PlayMunchSound()
	{
		if (playedFirstMunch)
			audioSource.PlayOneShot(munch2);
		else
			audioSource.PlayOneShot(munch1);
		playedFirstMunch = !playedFirstMunch;
	}


	void ReadInput()
	{
		if (Input.GetKeyDown(PACMAN_LEFT_KEY))
			ChangeDirection(Vector2.left);
		else if (Input.GetKeyDown(PACMAN_RIGHT_KEY))
			ChangeDirection(Vector2.right);
		else if (Input.GetKeyDown(PACMAN_UP_KEY))
			ChangeDirection(Vector2.up);
		else if (Input.GetKeyDown(PACMAN_DOWN_KEY))
			ChangeDirection(Vector2.down);
	}

	void UpdateMovement()
	{
		if (targetNode != null && targetNode != currentNode)
		{
			// If I turn around, swap origin with target
			if (nextDirection == direction * -1)
			{
				direction *= -1;
				Node tempNode = targetNode;
				targetNode = prevNode;
				prevNode = tempNode;
			}
			// If I passed a node, go back to its position and plan next step
			if (NodeUtilities.OvershotTarget(prevNode.transform.position, transform.localPosition, targetNode.transform.position))
			{
				currentNode = targetNode;
				transform.localPosition = currentNode.transform.position;

				// Check if I'm at a portal
				GameObject otherPortal = NodeUtilities.GetPortal(currentNode.transform.position, gameBoard);
				if (otherPortal != null) {
					transform.localPosition = otherPortal.transform.position;
					currentNode = otherPortal.GetComponent<Node>();
				}

				// If there's a node in the direction I pressed, aim for that. If not, go forward
				Node destinationNode = NodeUtilities.NextNodeInDirection(currentNode, nextDirection);
				if (destinationNode != null)
					direction = nextDirection;
				if (destinationNode == null)
					destinationNode = NodeUtilities.NextNodeInDirection(currentNode, direction);

				// Set the node I decided on as my target
				if (destinationNode != null) {
					targetNode = destinationNode;
					prevNode = currentNode;
					currentNode = null;
				}
				else
					direction = Vector2.zero;
			}
			else
				transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
		}
	}

	void UpdateSprite()
	{
		if(dead)
		{
			_anmCtrl.SetBool("dead", true);
			_sprRnd.flipX = false;
			transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
			return;
		}
		else
			_anmCtrl.SetBool("dead", false);
		if (direction == Vector2.down) {
			_anmCtrl.SetBool("move", true);
			_sprRnd.flipX = true;
			transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
		}
		else if (direction == Vector2.up) {
			_anmCtrl.SetBool("move", true);
			_sprRnd.flipX = false;
			transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
		}
		else if (direction == Vector2.left) {
			_anmCtrl.SetBool("move", true);
			_sprRnd.flipX = true;
			transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
		}
		else if (direction == Vector2.right) {
			_anmCtrl.SetBool("move", true);
			_sprRnd.flipX = false;
			transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
		}
		else {
			_anmCtrl.SetBool("move", false);
		}
	}

	// Eat pellet on current tile
	void ConsumePellet()
	{
		GameObject o = NodeUtilities.GetTileAtPosition(transform.position, gameBoard);
		if (o != null)
		{
			Tile tile = o.GetComponent<Tile>();
			if (tile != null)
			{
				if (!tile.consumed && (tile.isPellet || tile.isSuperPellet))
				{
					o.GetComponent<SpriteRenderer>().enabled = false;
					tile.consumed = true;
					gameBoard.score += PELLET_SCORE;
					pelletsConsumed++;
					PlayMunchSound();
					if (tile.isSuperPellet)
					{
						foreach (GameObject g in ghosts)
						{
							g.GetComponent<Ghost>().StartScaredMode();
						}
					}
				}
			}
		}
	}

	void ChangeDirection(Vector2 d)
	{
		if (d != direction)
			nextDirection = d;
		if (currentNode != null)
		{
			Node destinationNode = NodeUtilities.NextNodeInDirection(currentNode, d);
			if (destinationNode != null) {
				direction = d;
				targetNode = destinationNode;
				prevNode = currentNode;
				currentNode = null;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Ghost ghost = other.GetComponent<Ghost>();
		if (ghost != null)
		{
			ghost.TouchPacman();
		}
	}

	public void Die()
	{
		dead = true;
		StartCoroutine(PlayDeathAudio(1.65f));	// 1.65 secs for deathSound
	}

	IEnumerator PlayDeathAudio(float delay)
	{
		audioSource.PlayOneShot(deathSound);
		yield return new WaitForSeconds(delay); // {delay} secs for deathSound
		audioSource.PlayOneShot(deathQuack);
		yield return new WaitForSeconds(0.19f);	// 0.19 secs between deathQuacks
		audioSource.PlayOneShot(deathQuack);
	}
}