using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;
	public Node[] secretNeighbors;
    public Vector2[] validDirections;
	public bool isGhostArea;

	void Start()
    {
        validDirections = new Vector2[neighbors.Length + secretNeighbors.Length];
        for(int i=0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            Vector2 tempVector = neighbor.transform.localPosition - transform.localPosition;
            validDirections[i] = tempVector.normalized;
        }
		for (int i = 0; i < secretNeighbors.Length; i++)
		{
			Node neighbor = secretNeighbors[i];
			Vector2 tempVector = neighbor.transform.localPosition - transform.localPosition;
			validDirections[neighbors.Length + i] = tempVector.normalized;
		}
	}

	public Node[] GetAllNeighbors()
	{
		Node[] allNeighbors = new Node[neighbors.Length + secretNeighbors.Length];
		for (int i = 0; i < neighbors.Length; i++)
		{
			allNeighbors[i] = neighbors[i];
		}
		for (int i = 0; i < secretNeighbors.Length; i++)
		{
			allNeighbors[neighbors.Length + i] = secretNeighbors[i];
		}
		return allNeighbors;
	}
}
