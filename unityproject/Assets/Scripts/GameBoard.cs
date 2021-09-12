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
	private AudioSource audioSource;
	public AudioClip backgroundAudioNormal;
	public AudioClip backgroundAudioScared;
	private GameObject pacman;
	private GameObject[] ghosts;

	private bool dying = false;

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
		audioSource = transform.GetComponent<AudioSource>();
	}


	public void Restart()
	{
		lives--;
		dying = false;
		pacman.transform.GetComponent<Player>().Restart();
		foreach (GameObject g in ghosts)
			g.GetComponent<Ghost>().Restart();
	}

	public void StartDeath()
	{
		if(!dying)
		{
			dying = true;
			pacman.transform.GetComponent<Player>().canMove = false;
			foreach (GameObject g in ghosts)
				g.GetComponent<Ghost>().canMove = false;
			pacman.transform.GetComponent<Animator>().enabled = false;
			audioSource.Stop();
			StartCoroutine(ProcessDeathAfter(2));		// Freeze for 2 seconds after touched
		}
	}

	IEnumerator ProcessDeathAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach (GameObject g in ghosts)
			g.GetComponent<SpriteRenderer>().enabled = false;
		StartCoroutine(ProcessDeathAnimation(2.3f));	// 2.3 = Time that death animation takes
	}

	IEnumerator ProcessDeathAnimation(float delay)
	{
		pacman.transform.GetComponent<Animator>().enabled = true;
		pacman.transform.GetComponent<Player>().Die();
		yield return new WaitForSeconds(delay);
		StartCoroutine(ProcessRestart(2));				// Freeze for 2 seconds after death
	}
	
	IEnumerator ProcessRestart(float delay)
	{
		pacman.GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds(delay);
		Restart();
	}

	public void PlayNormalAlarm()
	{
		audioSource.clip = backgroundAudioNormal;
		audioSource.Play();
	}

	public void PlayScaredAlarm()
	{
		audioSource.clip = backgroundAudioScared;
		audioSource.Play();
	}
}
