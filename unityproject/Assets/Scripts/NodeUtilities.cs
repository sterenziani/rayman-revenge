using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUtilities : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public static float DistanceBetween(Vector2 posA, Vector2 posB)
	{
		Vector2 vec = posB - posA;
		return Mathf.Sqrt(vec.sqrMagnitude);
	}

	public static Node GetNodeAtPosition(Vector2 position, GameBoard gameBoard)
	{
		GameObject tile = GetTileAtPosition(position, gameBoard);
		if (tile != null)
			return tile.GetComponent<Node>();
		return null;
	}

	public static GameObject GetTileAtPosition(Vector2 position, GameBoard gameBoard)
	{
		int tileX = Mathf.RoundToInt(position.x);
		int tileY = Mathf.RoundToInt(position.y);
		GameObject tile = gameBoard.board[tileX, tileY];
		return tile;
	}

	// Returns the first node reachable from currentNode by going in direction d
	public static Node NextNodeInDirection(Node currentNode, Vector2 d)
	{
		Node destinationNode = null;
		if (currentNode != null)
		{
			for (int i = 0; i < currentNode.neighbors.Length; i++)
			{
				if (currentNode.validDirections[i] == d){
					destinationNode = currentNode.neighbors[i];
					break;
				}
			}
		}
		return destinationNode;
	}

	// Returns the portal if present on that position
	public static GameObject GetPortal(Vector2 position, GameBoard gameBoard)
	{
		GameObject tile = GetTileAtPosition(position, gameBoard);
		if (tile != null)
		{
			if (tile.GetComponent<Tile>() != null && tile.GetComponent<Tile>().isPortal){
				GameObject otherPortal = tile.GetComponent<Tile>().portalPair;
				return otherPortal;
			}
		}
		return null;
	}

	// Returns true if we passed the target we were trying to reach
	public static bool OvershotTarget(Vector2 prevNodePosition, Vector2 currentPosition, Vector2 targetNodePosition)
	{
		float prevNodeToTarget = DistanceBetween(prevNodePosition, targetNodePosition);
		float prevNodeToSelf = DistanceBetween(prevNodePosition, currentPosition);
		return prevNodeToSelf > prevNodeToTarget;
	}
}
