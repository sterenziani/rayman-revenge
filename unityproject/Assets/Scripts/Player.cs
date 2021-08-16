using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 4f;
    private Animator _anmCtrl;
    private SpriteRenderer _sprRnd;

    private Node prevNode, currentNode, targetNode;
	private int pelletsConsumed = 0;
    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection = Vector2.zero;
    private KeyCode PACMAN_LEFT_KEY = KeyCode.LeftArrow;
    private KeyCode PACMAN_RIGHT_KEY = KeyCode.RightArrow;
    private KeyCode PACMAN_UP_KEY = KeyCode.UpArrow;
    private KeyCode PACMAN_DOWN_KEY = KeyCode.DownArrow;
	private int PELLET_SCORE = 1;

    // Start is called before the first frame update
    void Start()
    {
		speed = 5f;
        _anmCtrl = GetComponent<Animator>();
        _sprRnd = GetComponent<SpriteRenderer>();
        Node node = GetNodeAtPosition(transform.localPosition);
		if (node != null)
			currentNode = node;
		direction = Vector2.left;
		ChangeDirection(direction);
    }

    // Update is called once per frame
    void Update()
    {
		Debug.Log("SCORE: " +GameObject.Find("Game").GetComponent<GameBoard>().score);
        ReadInput();
        UpdateMovement();
		UpdateSprite();
		ConsumePellet();
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
		if(targetNode != null && targetNode != currentNode)
		{
			// If I turn around, switch origin and target
			if(nextDirection == direction * -1)
			{
				direction *= -1;
				Node tempNode = targetNode;
				targetNode = prevNode;
				prevNode = tempNode;
			}
			// If I passed a node, go back and plan next step
			if(OvershotTarget())
			{
				currentNode = targetNode;
				transform.localPosition = currentNode.transform.position;

				// Check if I'm at a portal
				GameObject otherPortal = GetPortal(currentNode.transform.position);
				if(otherPortal != null){
					transform.localPosition = otherPortal.transform.position;
					currentNode = otherPortal.GetComponent<Node>();
				}
				
				// If there's a node in the direction I pressed, aim for that. If not, go forward
				Node destinationNode = NextNodeInDirection(nextDirection);
				if(destinationNode != null)
					direction = nextDirection;
				if(destinationNode == null)
					destinationNode = NextNodeInDirection(direction);
				
				// Set the node I decided on as my target
				if(destinationNode != null){
					targetNode = destinationNode;
					prevNode = currentNode;
					currentNode = null;
				}
				else
					direction = Vector2.zero;
			}
			else
				transform.localPosition += (Vector3)(direction * speed)*Time.deltaTime;
		}
    }

	void UpdateSprite()
	{
	    if(direction == Vector2.down){
            _anmCtrl.SetBool("move", true);
            _sprRnd.flipX = true;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
		}
        else if(direction == Vector2.up){
            _anmCtrl.SetBool("move", true);
            _sprRnd.flipX = false;
            transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        else if(direction == Vector2.left){
            _anmCtrl.SetBool("move", true);
            _sprRnd.flipX = true;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
        else if(direction == Vector2.right){
            _anmCtrl.SetBool("move", true);
            _sprRnd.flipX = false;
            transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
        }
		else{
			_anmCtrl.SetBool("move", false);
		}
	}

	void ConsumePellet()
	{
		GameObject o = GetTileAtPosition(transform.position);
		if(o != null)
		{
			Tile tile = o.GetComponent<Tile>();
			if(tile != null)
			{
				if(!tile.consumed && (tile.isPellet || tile.isSuperPellet))
				{
					o.GetComponent<SpriteRenderer>().enabled = false;
					tile.consumed = true;
					GameObject.Find("Game").GetComponent<GameBoard>().score += PELLET_SCORE;
					pelletsConsumed++;
				}
			}
		}
	}

    Node GetNodeAtPosition(Vector2 position)
    {
        GameObject tile = GetTileAtPosition(position);
        if(tile != null)
            return tile.GetComponent<Node>();
        return null;
    }

    Node NextNodeInDirection(Vector2 d)
    {
        Node destinationNode = null;
		if(currentNode != null)
		{
			for(int i=0; i < currentNode.neighbors.Length; i++)
			{
				if(currentNode.validDirections[i] == d){
					destinationNode = currentNode.neighbors[i];
					break;
				}
			}
		}
        return destinationNode;
    }

    void ChangeDirection(Vector2 d)
    {
        if(d != direction)
			nextDirection = d;
		if(currentNode != null)
		{
			Node destinationNode = NextNodeInDirection(d);
			if(destinationNode != null){
				direction = d;
				targetNode = destinationNode;
				prevNode = currentNode;
				currentNode = null;
			}
		}
    }

	// Returns true if we passed the target we were trying to reach
	bool OvershotTarget()
	{
		float nodeToTarget = DistanceFromNode(targetNode.transform.position);
		float nodeToSelf = DistanceFromNode(transform.localPosition);
		return nodeToSelf > nodeToTarget;
	}

	float DistanceFromNode(Vector2 targetPosition)
	{
		Vector2 vec = targetPosition - (Vector2)prevNode.transform.position;
		return vec.sqrMagnitude;
	}

	GameObject GetPortal(Vector2 position)
	{
		GameObject tile = GetTileAtPosition(position);
		if(tile != null)
		{
			if(tile.GetComponent<Tile>() != null && tile.GetComponent<Tile>().isPortal){
				GameObject otherPortal = tile.GetComponent<Tile>().portalPair;
				return otherPortal;
			}
		}
		return null;
	}

	GameObject GetTileAtPosition(Vector2 position)
	{
		int tileX = Mathf.RoundToInt(position.x);
		int tileY = Mathf.RoundToInt(position.y);
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX, tileY];
		return tile;
	}
}