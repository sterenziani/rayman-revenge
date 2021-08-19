using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 30;
    private static int boardHeight = 33;
	public int totalPellets = 0;
	public int score = 0;
    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    void Start()
    {
		// Load all interactive tiles into the array and place them on the board
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
			if (o.name != "Nodes" && o.name != "Non-Nodes" && o.name != "Maze" && o.name != "Pellets" && o.name != "PathTiles"
				 && o.tag != "Player" && o.tag != "Enemy" && o.tag != "Maze" && o.tag != "MainCamera" && o.tag != "GhostHomeNode")
			{
				if(o.GetComponent<Tile>() != null)
				{
					if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet)
						totalPellets++;
				}
				board[(int)pos.x, (int)pos.y] = o;
			}
        }
	}
	
    void Update()
    {
        
    }
}
