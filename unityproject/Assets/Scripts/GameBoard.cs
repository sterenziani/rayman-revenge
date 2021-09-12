using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
	public int boardWidth = 30;
	public int boardHeight = 33;
	private int totalPellets = 0;
	public int lives = 3;
	public int score = 0;
	public GameObject[,] board;
	public AudioClip backgroundAudioNormal;
	public AudioClip backgroundAudioScared;
	private GameObject pacman;
	private GameObject[] ghosts;

	void Start()
    {
		totalPellets = 0;
		board = new GameObject[boardWidth, boardHeight];
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
			if(o.name != "Pellets")
			{
				totalPellets++;
			}
        }
		pacman = pacman = GameObject.FindGameObjectWithTag("Player");
		ghosts = GameObject.FindGameObjectsWithTag("Enemy");
	}


	public void Restart()
	{
		lives--;
		pacman.transform.GetComponent<Player>().Restart();
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().Restart();
	}

	void Update()
    {
        
    }
}
